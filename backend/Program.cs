using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedRolesAndAdminAsync(context);
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();

async Task SeedRolesAndAdminAsync(ApplicationDbContext context)
{
    await context.Database.MigrateAsync();

    if (!await context.Roles.AnyAsync(r => r.Name == "Admin"))
        context.Roles.Add(new Role { Name = "Admin" });

    if (!await context.Roles.AnyAsync(r => r.Name == "Manager"))
        context.Roles.Add(new Role { Name = "Manager" });

    if (!await context.Roles.AnyAsync(r => r.Name == "User"))
        context.Roles.Add(new Role { Name = "User" });

// FOR SAVING CHANGES ON ROLES
    await context.SaveChangesAsync();

    var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

    if (adminRole != null)
    {
        if (!await context.Users.AnyAsync(u => u.Email == "alexanderivanferraz@gmail.com"))
        {
            var adminUser = new User
            {
                Email = "alexanderivanferraz@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1t5At35t"),
                RoleId = adminRole.Id,
                IsActive = true
            };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}