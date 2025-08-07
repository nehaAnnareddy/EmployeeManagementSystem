using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeApi
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAll()
        {
            return Ok(_context.Employees.ToList());
        }

        // GET: api/EmployeeApi/{id}
        [HttpGet("{id}")]
        public ActionResult<Employee> Get(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // POST: api/EmployeeApi
        [HttpPost]
        public ActionResult<Employee> Post(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
        }

        // PUT: api/EmployeeApi/{id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, Employee employee)
        {
            if (id != employee.Id)
                return BadRequest();

            var existing = _context.Employees.Find(id);
            if (existing == null)
                return NotFound();

            existing.Name = employee.Name;
            existing.Email = employee.Email;
            existing.DateOfBirth = employee.DateOfBirth;
            existing.Department = employee.Department;

            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/EmployeeApi/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
