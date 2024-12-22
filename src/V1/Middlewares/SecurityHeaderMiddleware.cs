namespace V1.Middlewares
{
    /// <summary>
    /// Middleware to set security headers in HTTP response to enhance application security.
    /// The middleware adds the following security headers:
    /// - X-Content-Type-Options: Prevents browsers from interpreting files as a different MIME type.
    /// - X-XSS-Protection: Enables cross-site scripting (XSS) protection in modern browsers.
    /// - Content-Security-Policy: Restricts the sources from which content can be loaded to improve security.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public class SecurityHeaderMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// Invokes the middleware to add security headers to the HTTP response.
        /// </summary>
        /// <param name="context">The HTTP context for the request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            // Add 'X-Content-Type-Options' header to prevent MIME type sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Add 'X-XSS-Protection' header to enable XSS protection in modern browsers
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Add 'Content-Security-Policy' header to restrict sources of content for enhanced security
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");

            // Pass control to the next middleware in the pipeline
            await _next(context);
        }
    }
}
