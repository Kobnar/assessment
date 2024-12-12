using System.Text;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// See: https://www.mongodb.com/docs/manual/tutorial/install-mongodb-on-os-x/
// See: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-9.0&tabs=visual-studio

// Configure MongoDB
builder.Services.Configure<AssessmentDatabaseSettings>(builder.Configuration.GetSection("AssessmentDatabase"));
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<AssessmentDatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString); // Create the MongoDB client
});
builder.Services.AddSingleton<AUsersService>();

// Configure JWT Authentication
builder.Services.Configure<AJwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<AJwtService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

// Add services to the container.
builder.Services.AddControllers(); // Add support for controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Check MongoDB connection at startup
var mongoClient = app.Services.GetRequiredService<IMongoClient>();
try
{
    // Try a simple command to check if the connection works
    var databases = mongoClient.ListDatabases(); // This will throw if the connection fails
    Console.WriteLine("MongoDB connection successful.");
}
catch (Exception ex)
{
    // If the connection fails, log the error
    Console.WriteLine($"MongoDB connection failed: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

app.Run();
