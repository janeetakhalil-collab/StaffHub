
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffDb.Data;
using StaffDb.Filters;
using StaffDb.Middlewares;
using StaffDb.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register FluentValidation validators.
builder.Services.AddValidatorsFromAssemblyContaining<
    DepartmentRequestValidator>();

// Register validation filter.
builder.Services.AddScoped<ValidationFilter>();

// Register Entity Framework Core with SQL Server.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Customize automatic model validation error responses.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!
                    .Errors
                    .Select(error =>
                        string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Invalid value."
                            : error.ErrorMessage)
                    .ToArray());

        return new BadRequestObjectResult(new
        {
            message = "Validation failed.",
            errors
        });
    };
});

// Register Swagger services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS policy.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy
            .SetIsOriginAllowed(IsLocalhostOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Handle unexpected exceptions across the downstream pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

// Log HTTP requests, response status codes and execution time.
app.UseMiddleware<RequestLoggingMiddleware>();

// Enable Swagger only in development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enable endpoint routing.
app.UseRouting();

// Apply the configured CORS policy.
app.UseCors("AllowLocalhost");

// Enable authorization middleware.
app.UseAuthorization();

// Map controller endpoints.
app.MapControllers();

app.Run();

// Check whether the request origin is localhost.
static bool IsLocalhostOrigin(string origin)
{
    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
    {
        return false;
    }

    return uri.Host.Equals(
        "localhost",
        StringComparison.OrdinalIgnoreCase);
}
