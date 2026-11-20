
// Provides ASP.NET Core API validation configuration.
using Microsoft.AspNetCore.Mvc;

// Provides EF Core and SQL Server database functionality.
using Microsoft.EntityFrameworkCore;

// Imports the AppDbContext class.
using StaffDb.Data;

// Creates the application builder.
// Loads configuration, services and environment information.
var builder = WebApplication.CreateBuilder(args);


// Registers controller support in the dependency injection container.
// Allows the application to handle API controllers.
//
// No direct SQL equivalent.
builder.Services.AddControllers();


// Registers AppDbContext with dependency injection.
//
// UseSqlServer configures EF Core to use SQL Server.
// GetConnectionString reads the DefaultConnection from appsettings.json.
//
// SQL Server connection configuration.
// No direct SQL command.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


// Configures the automatic response when model validation fails.
//
// No direct SQL equivalent.
// This controls HTTP responses, not database operations.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Customizes the response returned when model validation fails.
    options.InvalidModelStateResponseFactory = context =>
    {
        // Gets all request properties that contain validation errors.
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)

            // Converts validation errors into a dictionary.
            .ToDictionary(
                entry => entry.Key,

                // Gets the error messages for each property.
                entry => entry.Value!
                    .Errors

                    // Uses a default message if an error message is empty.
                    .Select(error =>
                        string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Invalid value."
                            : error.ErrorMessage)

                    // Converts error messages into an array.
                    .ToArray());

        // Returns HTTP 400 Bad Request with a custom JSON response.
        return new BadRequestObjectResult(new
        {
            message = "Validation failed.",
            errors
        });
    };
});


// Registers services required for API endpoint discovery.
// No direct SQL equivalent.
builder.Services.AddEndpointsApiExplorer();


// Registers Swagger generation services.
// No direct SQL equivalent.
builder.Services.AddSwaggerGen();


// Configures Cross-Origin Resource Sharing (CORS).
// Allows approved frontend origins to call this API.
// No direct SQL equivalent.
builder.Services.AddCors(options =>
{
    // Creates a CORS policy named AllowLocalhost.
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy

            // Allows origins that pass the IsLocalhostOrigin method.
            .SetIsOriginAllowed(IsLocalhostOrigin)

            // Allows any HTTP request header.
            .AllowAnyHeader()

            // Allows any HTTP method: GET, POST, PUT, DELETE, etc.
            .AllowAnyMethod();
    });
});


// Builds the application using the registered services.
var app = builder.Build();


// Enables Swagger middleware only in the Development environment.
if (app.Environment.IsDevelopment())
{
    // Generates the Swagger JSON document.
    app.UseSwagger();

    // Displays the Swagger user interface.
    app.UseSwaggerUI();
}


// Applies the AllowLocalhost CORS policy to incoming requests.
app.UseCors("AllowLocalhost");


// Enables authorization middleware.
// Does not enforce authentication by itself.
app.UseAuthorization();


// Maps controller routes so HTTP requests can reach controller actions.
app.MapControllers();


// Starts the web application and listens for HTTP requests.
app.Run();


// Checks whether an origin is a valid absolute URI.
static bool IsLocalhostOrigin(string origin)
{
    // Attempts to parse the supplied origin into a URI.
    // Returns false if the origin is invalid.
    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
    {
        return false;
    }

    // Allows only origins whose hostname is localhost.
    // Comparison ignores uppercase/lowercase differences.
    return uri.Host.Equals(
        "localhost",
        StringComparison.OrdinalIgnoreCase);
}
