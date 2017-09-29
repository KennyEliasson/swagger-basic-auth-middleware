public class SecureSwaggerMiddleware
    {
        private static readonly List<string> WhiteListedExtensions = new List<string>() {".css", ".js", ".png", ".gif", ".jpg", ".jpeg"};
        private readonly RequestDelegate _next;

        public SecureSwaggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger logger, IOptions<SwaggerAuthConfiguration> config)
        {
            if (!config.Value.Enabled)
            {
                await _next.Invoke(context);
                return;
            }

            var uriComponent = context.Request.Path.ToUriComponent();
            
            if (!uriComponent.Contains("swagger")) {
                await _next.Invoke(context);
                return;
            }
            
            if (WhiteListedExtensions.Any(extension => uriComponent.EndsWith(extension))) {
                await _next.Invoke(context);
                return;
            }
            
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                //Extract credentials
                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                var seperatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                if (username == config.Value.Username && password == config.Value.Password)
                {
                    await _next.Invoke(context);
                    return;
                }
            }
            
            // Authorization failed
            context.Response.Headers.Add("WWW-Authenticate", "Basic");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
