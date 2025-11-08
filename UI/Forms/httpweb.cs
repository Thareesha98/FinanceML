//
// File 2: Simple HTTP Web Server
// Demonstrates: Networking (HttpListener), async/await, Task, and simple routing.
//
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    /// <summary>
    /// A simple, asynchronous HTTP server using HttpListener.
    /// This server is multi-threaded by default thanks to the thread pool
    /// handling async I/O.
    /// </summary>
    public class BasicHttpServer
    {
        private readonly HttpListener _listener;
        private readonly string _prefix;
        private bool _isRunning = false;

        /// <summary>
        /// Initializes a new instance of the BasicHttpServer class.
        /// </summary>
        /// <param name="prefix">
        /// The prefix to listen on (e.g., "http://localhost:8080/").
        /// Must end with a forward slash.
        /// </param>
        public BasicHttpServer(string prefix)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is not supported on this platform.");
            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Prefix cannot be empty.", nameof(prefix));
            }

            _prefix = prefix;
            _listener = new HttpListener();
            _listener.Prefixes.Add(_prefix);
        }

        /// <summary>
        /// Starts the web server and begins listening for incoming requests.
        /// This method runs indefinitely until Stop() is called.
        /// </summary>
        public async Task StartAsync()
        {
            if (_isRunning)
            {
                return; // Server is already running
            }

            try
            {
                _listener.Start();
                _isRunning = true;
                Log($"Server started. Listening on {_prefix}", ConsoleColor.Green);

                // The main server loop.
                while (_isRunning)
                {
                    try
                    {
                        // Asynchronously wait for a new request.
                        HttpListenerContext context = await _listener.GetContextAsync();

                        // Fire and forget: Process the request on a thread pool thread
                        // without awaiting it. This allows the loop to immediately
                        // go back to waiting for the next request.
                        _ = ProcessRequestAsync(context);
                    }
                    catch (HttpListenerException ex)
                    {
                        // This exception can occur when Stop() is called.
                        if (_isRunning)
                        {
                            Log($"HttpListenerException: {ex.Message}", ConsoleColor.Red);
                        }
                        // If _isRunning is false, we're stopping, so just break.
                        else
                        {
                            break;
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // This occurs if the listener is closed.
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log($"Unexpected error in server loop: {ex.Message}", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to start server: {ex.Message}", ConsoleColor.Red);
                _isRunning = false;
            }
            finally
            {
                if (_isRunning)
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Stops the web server.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            _isRunning = false;
            _listener.Stop();
            ((IDisposable)_listener).Dispose();
            Log("Server stopped.", ConsoleColor.Yellow);
        }

        /// <summary>
        /// Handles an individual incoming HTTP request.
        /// </summary>
        /// <param name="context">The HttpListenerContext for the request.</param>
        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string urlPath = request.Url.AbsolutePath;
            string method = request.HttpMethod;

            Log($"Request: {method} {urlPath} from {request.RemoteEndPoint}");

            try
            {
                string responseString = "";
                string contentType = "text/html";
                int statusCode = 200;

                // Simple routing logic
                switch (urlPath.ToLower())
                {
                    case "/":
                        responseString = await HandleHomepage();
                        break;

                    case "/about":
                        responseString = await HandleAboutPage();
                        break;

                    case "/api/data":
                        // Example of handling a different method (POST)
                        if (method == "POST")
                        {
                            responseString = await HandlePostData(request);
                        }
                        else
                        {
                            responseString = await HandleApiGetData();
                        }
                        contentType = "application/json";
                        break;

                    default:
                        responseString = await Handle404NotFound();
                        statusCode = 404;
                        break;
                }

                // Send the response back to the client
                await SendResponse(response, responseString, contentType, statusCode);
            }
            catch (Exception ex)
            {
                Log($"Error processing request {urlPath}: {ex.Message}", ConsoleColor.Red);
                try
                {
                    // Attempt to send a 500 Internal Server Error response
                    string errorResponse = "<html><body><h1>500 Internal Server Error</h1></body></html>";
                    await SendResponse(response, errorResponse, "text/html", 500);
                }
                catch (Exception finalEx)
                {
                    // If sending the error fails, just log it.
                    Log($"Failed to send 500 error response: {finalEx.Message}", ConsoleColor.Red);
                }
            }
            finally
            {
                // Always close the response stream.
                response.Close();
            }
        }

        /// <summary>
        /// A helper method to send a response.
        /// </summary>
        private async Task SendResponse(HttpListenerResponse response, string content, string contentType, int statusCode)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentType = contentType;
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = statusCode;

            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        #region Page Handlers

        /// <summary>
        /// Handles the homepage request.
        /// </summary>
        private Task<string> HandleHomepage()
        {
            string html = @"
                <html>
                    <head><title>Welcome!</title></head>
                    <body style='font-family: sans-serif; text-align: center;'>
                        <h1>Welcome to the Simple HTTP Server</h1>
                        <p>This is the homepage, served from C#.</p>
                        <p>Try these links:</p>
                        <ul>
                            <li><a href='/'>Home</a></li>
                            <li><a href='/about'>About</a></li>
                            <li><a href='/api/data'>API Data (JSON)</a></li>
                            <li><a href='/idontexist'>Non-existent Page (404)</a></li>
                        </ul>
                    </body>
                </html>";
            return Task.FromResult(html);
        }

        /// <summary>
        /// Handles the /about page request.
        /// </summary>
        private Task<string> HandleAboutPage()
        {
            string html = @"
                <html>
                    <head><title>About</title></head>
                    <body style='font-family: sans-serif;'>
                        <h1>About This Server</h1>
                        <p>This is a lightweight, asynchronous web server written in C#
                           using the <strong>HttpListener</strong> class.</p>
                        <p><a href='/'>Back to Home</a></p>
                    </body>
                </html>";
            return Task.FromResult(html);
        }

        /// <summary>
        /// Handles the /api/data GET request (returns JSON).
        /// </summary>
        private Task<string> HandleApiGetData()
        {
            // Using a simple string for JSON, but in a real app,
            // you would serialize an object.
            // e.g., using System.Text.Json.JsonSerializer
            string json = $@"
                {{
                    ""serverTime"": ""{DateTime.Now:O}"",
                    ""message"": ""Hello from the API!"",
                    ""data"": {{
                        ""id"": 123,
                        ""value"": ""Some data""
                    }}
                }}";
            return Task.FromResult(json);
        }

        /// <summary>
        /// Handles the /api/data POST request (reads and echoes body).
        /// </summary>
        private async Task<string> HandlePostData(HttpListenerRequest request)
        {
            string requestBody;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            string json = $@"
                {{
                    ""status"": ""success"",
                    ""message"": ""Received your POST data."",
                    ""echo"": ""{requestBody.Replace("\"", "\\\"")}""
                }}";
            return json;
        }

        /// <summary>
        /// Handles any request not matched by the router (404).
        /// </summary>
        private Task<string> Handle404NotFound()
        {
            string html = @"
                <html>
                    <head><title>404 Not Found</title></head>
                    <body style='font-family: sans-serif;'>
                        <h1>404 - Page Not Found</h1>
                        <p>Sorry, the resource you requested could not be found.</p>
                        <p><a href='/'>Back to Home</a></p>
                    </body>
                </html>";
            return Task.FromResult(html);
        }

        #endregion

        /// <summary>
        /// Thread-safe logging helper.
        /// </summary>
        private static readonly object _logLock = new object();
        private void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            lock (_logLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
                Console.ResetColor();
            }
        }
    }

    /// <summary>
    /// Main program class to run the server.
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Note: On Windows, you may need to run this as Administrator
            // or run the following command in an admin shell:
            // netsh http add urlacl url=http://+:8080/ user=EVERYONE
            string url = "http://localhost:8080/";
            if (args.Length > 0)
            {
                url = args[0];
            }

            var server = new BasicHttpServer(url);
            
            Console.Title = "Simple HTTP Server";
            Console.WriteLine("======= Simple HTTP Server =======");
            Console.WriteLine("Starting server...");
            
            // Start the server in a non-blocking way
            Task serverTask = server.StartAsync();

            Console.WriteLine("\nServer is running.");
            Console.WriteLine($"Try visiting {url} in your browser.");
            Console.WriteLine("Try other paths like /about or /api/data");
            Console.WriteLine("\nPress 'Q' to stop the server...");

            // Keep the console app alive until 'Q' is pressed
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Q)
                    {
                        break;
                    }
                }
                await Task.Delay(100);
            }

            Console.WriteLine("\nStopping server...");
            server.Stop();
            await serverTask; // Wait for the server task to complete shutdown
            Console.WriteLine("Server shut down complete.");
        }
    }
}
