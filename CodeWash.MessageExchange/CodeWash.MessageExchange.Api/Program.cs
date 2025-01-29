using CodeWash.MessageExchange.Api.Hubs;
using CodeWash.MessageExchange.Api.Midllewares;
using CodeWash.MessageExchange.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(builder.Configuration);

builder.Services.AddCors(setup =>
{
    setup.AddDefaultPolicy(config => config
        .WithOrigins("https://localhost:7255")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(origin => true));
});

// NOTE: Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Missing JWT Key"))),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"]
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/messageHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddDataAccessServices();

WebApplication app = builder.Build();

app.UseCors();

app.UseMiddleware<ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<MessageHub>("/messageHub");

app.Run();
