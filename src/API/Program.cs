using System.Text;
using API.Abstractions.Helpers.Auth.Authenticators;
using API.Abstractions.Services;
using API.Extensions;
using API.Helpers.Auth;
using API.Helpers.Auth.Authenticators;
using API.Helpers.Auth.Token;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(JwtConfiguration.SectionName));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme."
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
            Array.Empty<string>()
        }
    });
});
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtConfig:AccessTokenSecret"))),
        ValidIssuer = builder.Configuration.GetValue<string>("JwtConfig:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("JwtConfig:Audience"),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    });

#region Service

builder.Services.AddSingleton<AccessTokenHelper>();
builder.Services.AddSingleton<RefreshTokenHelper>();
builder.Services.AddScoped<IAuthenticator, JwtAuthenticator>();
builder.Services.AddScoped<IUserService, UserService>();

#endregion

var app = builder.Build();

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

#region Custom

app.UseInvalidTokenHandleMiddleware();

app.UseExceptionHandleMiddleware();

#endregion

app.MapControllers();

#endregion

app.Run();
