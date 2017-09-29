# swagger-basic-auth-middleware
.NET Core MVC Middleware for securing the .json files from unauthorized access with Basic Auth.

# What to do
* Setup Swagger as per usual
* Add configuration to appsettings.json or other store
* In Startup.cs add services.Configure<SwaggerAuthConfiguration>(Configuration.GetSection("<YOUR SECTION>")); or similiar.
* In Configure() add .UseMiddleware<SecureSwaggerMiddleware>() before adding Swagger.  
  
