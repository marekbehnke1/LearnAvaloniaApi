using Microsoft.EntityFrameworkCore;
using LearnAvaloniaApi.Data;
using LearnAvaloniaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Adds scope for the password service
builder.Services.AddScoped<IPasswordService, PasswordService>();
// Adds scope for the JWT service
builder.Services.AddScoped<IJwtService, JwtService>();

// Adds the middleware authentication service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        var secretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new KeyNotFoundException("JWT Key not set");
        var key = Encoding.ASCII.GetBytes(secretKey);
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];

        // These options are the same as the ones we configured when creating the JWT token in JWT service
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = issuer,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Adding the dbcontext service
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

var app = builder.Build();

// Ensures that the database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// This forces the connection to use https
app.UseHttpsRedirection();

// Enables auth
app.UseAuthentication();
app.UseAuthorization();

// Maps the API controller routes
app.MapControllers();

// Starts the app
app.Run();
