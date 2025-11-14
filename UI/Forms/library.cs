namespace ECommerce.Services.Implementations
{
    using ECommerce.Services.Interfaces;
    using ECommerce.Models;
    using ECommerce.DTOs;
    using ECommerce.Data;
    using ECommerce.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    
    // Placeholder classes for dependencies (Data Repository, Payment Gateway, Email Sender)
    // In a real application, these would be injected services.
    public interface IRepository<T> { Task<T> GetByIdAsync(int id); Task<int> SaveAsync(T entity); Task<bool> UpdateAsync(T entity); }
    public interface IPaymentGateway { Task<(bool Success, string TransactionId)> ChargeCustomerAsync(decimal amount, string token); }
    public interface IEmailSender { Task SendEmailAsync(string recipient, string subject, string body); }
    public interface IInventoryService { Task<bool> ReserveStockAsync(IEnumerable<(int ProductId, int Quantity)> items); Task<bool> ReleaseStockAsync(IEnumerable<(int ProductId, int Quantity)> items); }

    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IEmailSender _emailSender;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<OrderProcessingService> _logger;
        private const decimal TAX_RATE = 0.08m;

        public OrderProcessingService(
            IRepository<Order> orderRepository,
            IRepository<Product> productRepository,
            IPaymentGateway paymentGateway,
            IEmailSender emailSender,
            IInventoryService inventoryService,
            ILogger<OrderProcessingService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderResultDto> CreateOrderAsync(OrderRequestDto orderDto)
        {
            _logger.LogInformation($"Attempting to create order for customer {orderDto.CustomerId}.");
            
            // 1. Validation and Price Calculation
            if (orderDto.LineItems == null || !orderDto.LineItems.Any())
            {
                return new OrderResultDto { Success = false, Message = "Order must contain line items." };
            }

            var (calculatedOrder, isValid) = await CalculateOrderTotalsAsync(orderDto);

            if (!isValid)
            {
                return new OrderResultDto { Success = false, Message = "One or more products in the order are invalid or out of stock." };
            }

            // 2. Inventory Reservation (Transactional/Pre-check)
            var reservationItems = calculatedOrder.LineItems.Select(li => (li.ProductId, li.Quantity));
            if (!await _inventoryService.ReserveStockAsync(reservationItems))
            {
                 return new OrderResultDto { Success = false, Message = "Failed to reserve stock. Inventory update failed." };
            }

            // 3. Database Persistence (Initial Status: Pending Payment)
            calculatedOrder.Status = OrderStatus.PendingPayment;
            calculatedOrder.OrderDate = DateTime.UtcNow;
            int orderId = await _orderRepository.SaveAsync(calculatedOrder);
            
            if (orderId <= 0)
            {
                // Must release reserved stock if DB save fails
                await _inventoryService.ReleaseStockAsync(reservationItems); 
                return new OrderResultDto { Success = false, Message = "Database error during order creation." };
            }

            _logger.LogInformation($"Order {orderId} created successfully with status {calculatedOrder.Status}.");
            
            // Note: Payment and final inventory update are handled in subsequent methods for separation of concerns.
            return new OrderResultDto
            {
                Success = true,
                OrderId = orderId,
                TotalAmount = calculatedOrder.TotalAmount,
                Message = "Order created. Proceed to payment."
            };
        }

        private async Task<(Order Order, bool IsValid)> CalculateOrderTotalsAsync(OrderRequestDto dto)
        {
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                ShippingAddress = dto.ShippingAddress,
                LineItems = new List<OrderItem>()
            };
            
            decimal subtotal = 0;
            var productIds = dto.LineItems.Select(li => li.ProductId).Distinct();
            
            // Assuming a method to batch retrieve products by IDs exists in the repository
            var products = new Dictionary<int, Product>(); // Placeholder
            
            foreach (var item in dto.LineItems)
            {
                // In a real scenario, this would check stock availability and price validity
                Product product = await _productRepository.GetByIdAsync(item.ProductId);

                if (product == null || product.Stock < item.Quantity)
                {
                    _logger.LogWarning($"Product ID {item.ProductId} invalid or insufficient stock.");
                    return (null, false);
                }

                order.LineItems.Add(new OrderItem 
                { 
                    ProductId = item.ProductId, 
                    Quantity = item.Quantity, 
                    UnitPrice = product.Price 
                });
                subtotal += product.Price * item.Quantity;
            }

            order.SubTotal = subtotal;
            order.TaxAmount = subtotal * TAX_RATE;
            order.TotalAmount = order.SubTotal + order.TaxAmount;

            return (order, true);
        }

        public async Task<bool> ProcessPaymentAsync(int orderId)
        {
            Order order = await GetOrderDetailsAsync(orderId);

            if (order == null || order.Status != OrderStatus.PendingPayment)
            {
                _logger.LogWarning($"Payment skipped. Order {orderId} not found or status is not PendingPayment.");
                return false;
            }
            
            // Assume we retrieve the payment token linked to the CustomerId or OrderId
            string paymentToken = "CUST_PAYMENT_TOKEN_" + order.CustomerId; 
            
            var (success, transactionId) = await _paymentGateway.ChargeCustomerAsync(order.TotalAmount, paymentToken);

            if (success)
            {
                order.Status = OrderStatus.Processing;
                order.TransactionId = transactionId;
                await _orderRepository.UpdateAsync(order);
                _logger.LogInformation($"Payment successful for order {orderId}. Transaction: {transactionId}.");
                return true;
            }
            else
            {
                order.Status = OrderStatus.PaymentFailed;
                await _orderRepository.UpdateAsync(order);
                // Note: Inventory should be released if payment fails
                await _inventoryService.ReleaseStockAsync(order.LineItems.Select(li => (li.ProductId, li.Quantity)));
                _logger.LogError($"Payment failed for order {orderId}. Stock released.");
                return false;
            }
        }

        public async Task<bool> UpdateInventoryAsync(int orderId)
        {
            // Inventory updates are now part of the payment process for atomicity
            _logger.LogWarning("UpdateInventoryAsync is deprecated. Inventory should be updated or reserved during Create/Payment.");
            return true; 
        }

        public async Task SendConfirmationEmailAsync(int orderId)
        {
            Order order = await GetOrderDetailsAsync(orderId);
            
            if (order == null || order.Status != OrderStatus.Processing)
            {
                _logger.LogWarning($"Confirmation email skipped for order {orderId}. Order not ready or failed.");
                return;
            }

            string recipient = "customer_" + order.CustomerId + "@example.com";
            string subject = $"Order Confirmation #{orderId}";
            string body = $"Thank you for your order! Your total was {order.TotalAmount:C}. Details: ...";

            await _emailSender.SendEmailAsync(recipient, subject, body);
            
            _logger.LogInformation($"Confirmation email sent for order {orderId}.");
        }

        public async Task<Order> GetOrderDetailsAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }
        
        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            // Placeholder: In a real repo, this would query by status.
            return new List<Order>(); 
        }
    }
}
