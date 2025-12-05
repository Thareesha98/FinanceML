using System;
using System.Collections.Generic; // Added for IReadOnlyList<double>

namespace ShapeCalculator.Interfaces
{
    // 1. Interface for core geometric calculations (Contract)
    public interface IGeometricShape
    {
        double CalculateArea();
        double CalculatePerimeter();
        string GetName();
    }

    // 2. Interface for display/output logic (Contract)
    public interface IDisplayable
    {
        void DisplayInfo();
    }
}

namespace ShapeCalculator.Shapes
{
    using ShapeCalculator.Interfaces;

    // Base functionality for displaying information
    public static class ShapeDisplay
    {
        public static void Display(IGeometricShape shape)
        {
            Console.WriteLine($"\n--- {shape.GetName()} ---");
            // The display logic is consistent across all shapes
            Console.WriteLine($"Area: {shape.CalculateArea():F2}");
            Console.WriteLine($"Perimeter/Circumference: {shape.CalculatePerimeter():F2}");
        }
    }

    // 3. Sealed Class: Circle (Implements Contracts)
    // Sealed prevents further inheritance, promoting a simpler hierarchy.
    public sealed class Circle : IGeometricShape, IDisplayable
    {
        public double Radius { get; }

        public Circle(double radius)
        {
            if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be positive.");
            Radius = radius;
        }

        public double CalculateArea() => Math.PI * Radius * Radius;
        public double CalculatePerimeter() => 2 * Math.PI * Radius;
        public string GetName() => "Circle";

        public void DisplayInfo() => ShapeDisplay.Display(this);
    }

    // 4. Sealed Class: Rectangle (Implements Contracts)
    public sealed class Rectangle : IGeometricShape, IDisplayable
    {
        public double Width { get; }
        public double Height { get; }

        public Rectangle(double width, double height)
        {
            if (width <= 0 || height <= 0) throw new ArgumentException("Dimensions must be positive.");
            Width = width;
            Height = height;
        }

        public double CalculateArea() => Width * Height;
        public double CalculatePerimeter() => 2 * (Width + Height);
        public string GetName() => "Rectangle";

        public void DisplayInfo() => ShapeDisplay.Display(this);
    }

    // 5. Sealed Class: Triangle (Implements Contracts)
    public sealed class Triangle : IGeometricShape, IDisplayable
    {
        public double SideA { get; }
        public double SideB { get; }
        public double SideC { get; }

        public Triangle(double a, double b, double c)
        {
            if (a + b <= c || a + c <= b || b + c <= a)
                throw new ArgumentException("Invalid side lengths for a triangle (Violates Triangle Inequality).");

            SideA = a;
            SideB = b;
            SideC = c;
        }

        public double CalculateArea()
        {
            // Heron's Formula
            double s = CalculatePerimeter() / 2;
            return Math.Sqrt(s * (s - SideA) * (s - SideB) * (s - SideC));
        }

        public double CalculatePerimeter() => SideA + SideB + SideC;
        public string GetName() => "Triangle";

        public void DisplayInfo() => ShapeDisplay.Display(this);
    }
}
