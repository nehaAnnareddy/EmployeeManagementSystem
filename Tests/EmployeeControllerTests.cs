using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Controllers;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EmployeeManagementSystem.Tests
{
    public class EmployeeControllerTests
    {
        private AppDbContext _context;
        private EmployeeController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new EmployeeController(_context);
        }

        [Test]
        public void Index_ReturnsViewWithAllEmployees()
        {
            _context.Employees.AddRange(new List<Employee>
            {
                new Employee { Name = "Alice", Email = "alice@test.com", Department = "IT", DateOfBirth = DateTime.Today },
                new Employee { Name = "Bob", Email = "bob@test.com", Department = "HR", DateOfBirth = DateTime.Today }
            });
            _context.SaveChanges();

            var result = _controller.Index(null, null) as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as List<Employee>;
            Assert.AreEqual(2, model.Count);
        }

        [Test]
        public void Index_SearchByName_ReturnsFilteredResults()
        {
            _context.Employees.AddRange(new List<Employee>
            {
                new Employee { Name = "Alice", Email = "alice@test.com", Department = "IT", DateOfBirth = DateTime.Today },
                new Employee { Name = "Bob", Email = "bob@test.com", Department = "HR", DateOfBirth = DateTime.Today }
            });
            _context.SaveChanges();

            var result = _controller.Index("Name", "Ali") as ViewResult;
            var model = result.Model as List<Employee>;
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Alice", model[0].Name);
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            var result = _controller.Create() as ViewResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void Create_Post_ValidEmployee_RedirectsToIndex()
        {
            var employee = new Employee
            {
                Name = "New",
                Email = "new@test.com",
                Department = "IT",
                DateOfBirth = DateTime.Today
            };

            var result = _controller.Create(employee) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(1, _context.Employees.Count());
        }

        [Test]
        public void Create_Post_InvalidModel_ReturnsView()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = _controller.Create(new Employee()) as ViewResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void Edit_Get_ValidId_ReturnsViewWithEmployee()
        {
            var emp = new Employee { Name = "Edit", Email = "edit@test.com", Department = "HR", DateOfBirth = DateTime.Today };
            _context.Employees.Add(emp);
            _context.SaveChanges();

            var result = _controller.Edit(emp.Id) as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as Employee;
            Assert.AreEqual(emp.Name, model.Name);
        }

        [Test]
        public void Edit_Get_InvalidId_ReturnsNotFound()
        {
            var result = _controller.Edit(9999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Edit_Post_ValidUpdate_RedirectsToIndex()
        {
            var emp = new Employee { Name = "Old", Email = "old@test.com", Department = "IT", DateOfBirth = DateTime.Today };
            _context.Employees.Add(emp);
            _context.SaveChanges();

            var updated = new Employee { Id = emp.Id, Name = "New", Email = "new@test.com", Department = "HR", DateOfBirth = DateTime.Today };

            var result = _controller.Edit(emp.Id, updated) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("New", _context.Employees.Find(emp.Id)?.Name);
        }

        [Test]
        public void Edit_Post_InvalidModel_ReturnsView()
        {
            var emp = new Employee { Name = "Old", Email = "old@test.com", Department = "IT", DateOfBirth = DateTime.Today };
            _context.Employees.Add(emp);
            _context.SaveChanges();

            _controller.ModelState.AddModelError("Name", "Required");

            var result = _controller.Edit(emp.Id, emp) as ViewResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void Delete_Get_ValidId_ReturnsView()
        {
            var emp = new Employee { Name = "Del", Email = "del@test.com", Department = "Admin", DateOfBirth = DateTime.Today };
            _context.Employees.Add(emp);
            _context.SaveChanges();

            var result = _controller.Delete(emp.Id) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(emp.Id, ((Employee)result.Model).Id);
        }

        [Test]
        public void Delete_Get_InvalidId_ReturnsNotFound()
        {
            var result = _controller.Delete(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void DeleteConfirmed_ExistingId_RemovesEmployee()
        {
            var emp = new Employee { Name = "Del", Email = "del@test.com", Department = "Admin", DateOfBirth = DateTime.Today };
            _context.Employees.Add(emp);
            _context.SaveChanges();

            var result = _controller.DeleteConfirmed(emp.Id) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.IsNull(_context.Employees.Find(emp.Id));
        }

        [Test]
        public void DeleteConfirmed_NonExistingId_ReturnsNotFound()
        {
            var result = _controller.DeleteConfirmed(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
