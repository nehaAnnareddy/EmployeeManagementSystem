using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System.Linq;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Employee
        public IActionResult Index(string searchField, string searchTerm)
        {
            var employees = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                switch (searchField)
                {
                    case "Name":
                        employees = employees.Where(e => e.Name.ToLower().Contains(searchTerm.ToLower()));
                        break;
                    case "Email":
                        employees = employees.Where(e => e.Email.ToLower().Contains(searchTerm.ToLower()));
                        break;
                    case "Department":
                        employees = employees.Where(e => e.Department.ToLower().Contains(searchTerm.ToLower()));
                        break;
                }

            }

            return View(employees.ToList());
        }

        
        // GET: /Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index)); 
            }
            return View(employee);
        }
        // GET: /Employee/Edit/{id}
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: /Employee/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee updatedEmployee)
        {
            if (id != updatedEmployee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingEmployee = _context.Employees.FirstOrDefault(e => e.Id == id);
                if (existingEmployee == null)
                {
                    return NotFound();
                }
                
                existingEmployee.Name = updatedEmployee.Name;
                existingEmployee.Email = updatedEmployee.Email;
                existingEmployee.DateOfBirth = updatedEmployee.DateOfBirth;
                existingEmployee.Department = updatedEmployee.Department;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(updatedEmployee);
        }
        // GET: /Employee/Delete/{id}
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee); 
        }

        // POST: /Employee/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}