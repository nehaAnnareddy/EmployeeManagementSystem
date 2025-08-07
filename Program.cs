using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EmployeeDB"));


var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!dbContext.Employees.Any())
        {
            // Seed 100 employees using Bogus
            var faker = new Bogus.Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.Email, f => f.Internet.Email())
                .RuleFor(e => e.DateOfBirth, f => f.Date.Past(30, DateTime.Today.AddYears(-20)))
                .RuleFor(e => e.Department, f => f.Commerce.Department());

            dbContext.Employees.AddRange(faker.Generate(100));
            dbContext.SaveChanges();
        }
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}"
);


app.Run();
