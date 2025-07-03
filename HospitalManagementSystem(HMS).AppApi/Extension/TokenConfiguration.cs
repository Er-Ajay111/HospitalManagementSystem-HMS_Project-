using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HospitalManagementSystem_HMS_.AppApi.Extension
{
    public static class TokenConfiguration
    {
        public static WebApplicationBuilder AddTokenConfiguration(this WebApplicationBuilder app)
        {
            app.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    Description = "Enter the Bearer Authorization string as follows:'Bearer Genrated-JWT-Token'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] {}
                    }
                });
            });
            var settings = app.Configuration.GetSection("ApiSettings");
            var secretKey = settings.GetValue<string>("SecretKey");
            var issuer = settings.GetValue<string>("Issuer");
            var audience = settings.GetValue<string>("Audience");
            var key = Encoding.UTF8.GetBytes(secretKey);
            app.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = audience
                };
            });
            return app;
        }
    }
}
