using Microsoft.EntityFrameworkCore;
using LearnAvaloniaApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


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
app.UseAuthorization();

// Maps the API controller routes
app.MapControllers();

// Starts the app
app.Run();
