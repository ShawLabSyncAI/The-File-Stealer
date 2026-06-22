
using Microsoft.OpenApi.Models;

namespace CDL_File_Stealer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string basePath = builder.Configuration["FileApi:BaseFolder"] ?? throw new InvalidOperationException("FileApi:BaseFolder is not configured.");
            string apiToken = builder.Configuration["FileApi:ApiToken"] ?? throw new InvalidOperationException("FileApi:ApiToken is not configured.");

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Adds an "Authorize" button in Swagger UI so Try-it-out calls send the X-Api-Token header.
                options.AddSecurityDefinition("ApiToken", new OpenApiSecurityScheme
                {
                    Name = "X-Api-Token",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Permanent API token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiToken" }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddSingleton(new FileStore(basePath));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<TokenAuthMiddleware>(apiToken);

            app.MapControllers();

            app.Run();
        }
    }
}
