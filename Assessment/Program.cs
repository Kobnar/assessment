using System.Text;
using Assessment.Authentication;
using Assessment.Models;
using Assessment.Services;
using Assessment.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// See: https://www.mongodb.com/docs/manual/tutorial/install-mongodb-on-os-x/
// See: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-9.0&tabs=visual-studio

// Configure MongoDB
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString); // Create the MongoDB client
});
builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<IProfilesService, ProfilesService>();

// Configure JWT Authentication
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Authentication"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"])),
            RoleClaimType = "groups",
        };
    });

// Add custom access control policy requirements
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminScopePolicy", policy =>
        policy.Requirements.Add(new ScopeRequirement("admin")));
});

// Add custom access control policy handlers
builder.Services.AddSingleton<IAuthorizationHandler, ScopeRequirementHandler>();

// Add services to the container.
builder.Services.AddControllers(); // Add support for controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

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
