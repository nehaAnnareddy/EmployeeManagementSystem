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
    public class EmployeeApiControllerTests
    {
        private AppDbContext _context;
        private EmployeeApiController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _context.Employees.Add(new Employee
            {
                Name = "Test Employee",
                Email = "test@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Department = "HR"
            });

            _context.SaveChanges();

            _controller = new EmployeeApiController(_context);
        }

        [Test]
        public void GetAll_ReturnsAllEmployees()
        {
            var result = _controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<Employee>>(okResult.Value);
            Assert.AreEqual(1, ((List<Employee>)okResult.Value).Count);
        }

        [Test]
        public void Get_ExistingId_ReturnsEmployee()
        {
            var emp = _context.Employees.First();
            var result = _controller.Get(emp.Id);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<Employee>(okResult.Value);
        }

        [Test]
        public void Get_NonExistingId_ReturnsNotFound()
        {
            var result = _controller.Get(9999);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public void Post_ValidEmployee_AddsEmployee()
        {
            var newEmp = new Employee
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                DateOfBirth = new DateTime(1995, 5, 5),
                Department = "IT"
            };

            var result = _controller.Post(newEmp);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOf<Employee>(createdResult.Value);

            var allEmps = _context.Employees.ToList();
            Assert.AreEqual(2, allEmps.Count);
        }

        [Test]
        public void Put_ExistingId_UpdatesEmployee()
        {
            var existing = _context.Employees.First();
            existing.Name = "Updated Name";

            var result = _controller.Put(existing.Id, existing);
            Assert.IsInstanceOf<NoContentResult>(result);

            var updated = _context.Employees.Find(existing.Id);
            Assert.AreEqual("Updated Name", updated.Name);
        }

        [Test]
        public void Put_NonMatchingId_ReturnsBadRequest()
        {
            var existing = _context.Employees.First();
            var result = _controller.Put(existing.Id + 1, existing);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void Delete_ExistingId_DeletesEmployee()
        {
            var emp = _context.Employees.First();
            var result = _controller.Delete(emp.Id);
            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.IsNull(_context.Employees.Find(emp.Id));
        }

        [Test]
        public void Delete_NonExistingId_ReturnsNotFound()
        {
            var result = _controller.Delete(9999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
