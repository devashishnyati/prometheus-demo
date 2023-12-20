using System.Diagnostics;
using System.Text;

namespace PrometheusDemo.Extensions
{
    public class RequestResponseLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Capture the response
            var originalBodyStream = context.Response.Body;
            try
            {
                // Set up distributed trace ID
                var traceId = Activity.Current?.Id ?? "null";

                // Log the request details
                await LogRequest(context.Request, traceId);

              

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    // Log the response
                    await LogResponseAsync(context.Request, context.Response, traceId, responseBody);

                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);

                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred in the RequestResponseLoggingMiddleware.");
            }
        }

        private async Task LogRequest(HttpRequest request, string traceId)
        {
            // Log request details
            var requestBody = await FormatRequestBody(request);
            _logger.LogInformation($"TraceID: {traceId} | Method: {request.Method} | Path: {request.Path} | Host: {request.Host} | " +
                $"Scheme: {request.Scheme} | Protocol: {request.Protocol} | IP: {request.HttpContext.Connection.RemoteIpAddress} | " +
                $"Request Query: {request.QueryString.ToString()} | Request Body: {requestBody}");
        }

        private async Task LogResponseAsync(HttpRequest request, HttpResponse response, string traceId, MemoryStream responseBody)
        {
            // Log response details
            var requestBody = await FormatRequestBody(request);

            responseBody.Seek(0, SeekOrigin.Begin);
            string responseContent = await new StreamReader(responseBody).ReadToEndAsync();

            _logger.LogInformation($"TraceID: {traceId} | Method: {request.Method} | Path: {request.Path} | Host: {request.Host} | " +
                $"Scheme: {request.Scheme} | Protocol: {request.Protocol} | IP: {request.HttpContext.Connection.RemoteIpAddress} | " +
                $"Request Query: {request.QueryString.ToString()} | Request Body: {requestBody} | Status Code: {response.StatusCode}" +
                $" | Response Body: {responseContent}");
        }

        private async Task<string> FormatRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBodyAsString = Encoding.UTF8.GetString(buffer);
            return requestBodyAsString;
        }

    }
}
