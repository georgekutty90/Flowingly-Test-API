using Flowingly.API.Common;
using Flowingly.API.LogicLayer;
using Flowingly.API.LogicLayer.Interface;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Allow your frontend origin
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddControllers();

// Register TextParserImplementation for ITextParserImplementation
builder.Services.AddScoped<ITextParserImplementation, TextParserImplementation>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApplicationKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "ApplicationKey", // your header name
        Type = SecuritySchemeType.ApiKey,
        Description = "123456789"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApplicationKey" // Same as above
                }
            },
            new string[] {}
        }
    });
});
var app = builder.Build();
app.UseCors();
// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();