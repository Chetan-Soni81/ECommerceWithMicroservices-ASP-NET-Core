namespace ApiGateway.Presentation.Middleware
{
    public class AttachSigatureToRequest(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Logic to attach signature to the request
            context.Request.Headers["Api-Gateway"] = "Signed";
            await next(context);
        }
    }
}
