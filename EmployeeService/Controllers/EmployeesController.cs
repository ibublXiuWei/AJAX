using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Models;
using EmployeeService.DTO;
using Microsoft.AspNetCore.Cors;

namespace EmployeeService.Controllers
{
    [EnableCors("AllowAny")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly northwindContext _context;

        public EmployeesController(northwindContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<IEnumerable<EmployeeDTO>> GetEmployees()
        {
            return _context.Employees.Select(emp=>new EmployeeDTO { 
                EmployeeId=emp.EmployeeId,
                FirstName=emp.FirstName,
                LastName=emp.LastName,
                Title=emp.Title
            });
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<EmployeeDTO> GetEmployees(int id)
        {
            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return null;
            }
            EmployeeDTO EmpDTO = new EmployeeDTO { 
                EmployeeId= employees.EmployeeId,
                FirstName=employees.FirstName,
                LastName=employees.LastName,
                Title=employees.Title
            };
            return EmpDTO;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutEmployees(int id, EmployeeDTO empDTO)
        {
            if (id != empDTO.EmployeeId)
            {
                return "欲修改的員工記錄不正確";
            }
            Employees emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                return "欲修改的員工記錄不存在";
            }
            emp.FirstName = empDTO.FirstName;
            emp.LastName = empDTO.LastName;
            emp.Title = empDTO.Title;
            _context.Entry(emp).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeesExists(id))
                {
                    return "欲修改的員工記錄不存在";
                }
                else
                {
                    throw;
                }
            }
            return "修改成功!";
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<Employees> PostEmployees(Employees employees)
        //{
        //    _context.Employees.Add(employees);
        //    await _context.SaveChangesAsync();
        //    return employees;
        //}
        [HttpPost]
        public async Task<string> PostEmployees(EmployeeDTO empDTO)
        {
            Employees emp = new Employees
            {
                FirstName=empDTO.FirstName,
                LastName=empDTO.LastName,
                Title = empDTO.Title
            };
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();
            return "新增成功!";
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<string> DeleteEmployees(int id)
        {
            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return "查無此員工!";
            }
            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();
            return "刪除成功!";
        }

        private bool EmployeesExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        [HttpPost("Filter")] //Uri=>api/Employees/Filter
        public async Task<IEnumerable<EmployeeDTO>>FilterEmployee([FromBody]EmployeeDTO employee)
        {

            return _context.Employees
                .Where(emp=>emp.FirstName.Contains(employee.FirstName) || emp.LastName.Contains(employee.FirstName) || emp.Title.Contains(employee.FirstName))
                .Select(emp=>new EmployeeDTO
            {
                EmployeeId = emp.EmployeeId,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Title = emp.Title
            });
        }
    }
}
