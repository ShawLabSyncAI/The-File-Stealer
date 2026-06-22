namespace CDL_File_Stealer
{
    //Very basic auth class.
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiToken;

        public TokenAuthMiddleware(RequestDelegate next, string apiToken)
        {
            _next = next;
            _apiToken = apiToken;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Headers["X-Api-Token"];
            if (token != _apiToken)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or missing token.");
                return;
            }

            await _next(context);
        }
    }
}
