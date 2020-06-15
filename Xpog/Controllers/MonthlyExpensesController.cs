using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ef6CoreForPosgreSQL.Models;
using Xpog.Enitities.Expenses;
using System.Security.Claims;
using Xpog.Models.Expenses;
using Xpog.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Xpog.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MonthlyExpensesController : ControllerBase
    {
        private readonly ExpenseAppContext _context;

        public MonthlyExpensesController(ExpenseAppContext context)
        {
            _context = context;
        }

        private System.Linq.IQueryable<MonthlyExpense> UserMonthlyExpensesQueryBuilder(int uid)
        {
            return _context.MonthlyExpenses.Include(c => c.ExpenseData).Where(c => c.User.Id == uid);
        }

        private int GetUserId()
        {
            var principal = HttpContext.User;
            var idString = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (idString == null || idString == "")
            {
                throw new Exception("unknown user id: try to relogin");
            }

            var id = int.Parse(idString);
            return id;
        }

        // GET: api/MonthlyExpenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MonthlyExpense>>> GetMonthlyExpenses()
        {
            return await this.UserMonthlyExpensesQueryBuilder(GetUserId()).ToListAsync();
        }

        // GET: api/MonthlyExpenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MonthlyExpense>> GetMonthlyExpense(int id)
        {
            var uid = GetUserId();
            var monthlyExpense = await this.UserMonthlyExpensesQueryBuilder(uid).Where(c => c.Id == id).FirstOrDefaultAsync();

            if (monthlyExpense == null)
            {
                return NotFound();
            }

            return monthlyExpense;
        }

        // PUT: api/MonthlyExpenses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMonthlyExpense(int id, MonthlyExpenseInputModel monthlyExpenseInput)
        {
            var uid = GetUserId();
            var expenseToModify = await this.UserMonthlyExpensesQueryBuilder(uid).Where(c => c.Id == id).FirstOrDefaultAsync();

            if (expenseToModify == null)
            {
                throw new Exception("You don't own any current expense with that id");
            }

            expenseToModify.ExpiryDate = monthlyExpenseInput.ExpiryDate;
            expenseToModify.TriggeringDateOfMonth = monthlyExpenseInput.TriggeringDayOfMonth;
            expenseToModify.ExpenseData = new ExpenseData
            {
                Amount = monthlyExpenseInput.Amount,
                Description = monthlyExpenseInput.Description
            };

            _context.Entry(expenseToModify).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonthlyExpenseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MonthlyExpenses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MonthlyExpense>> PostMonthlyExpense(MonthlyExpenseInputModel monthlyExpenseInput)
        {

            var user = await _context.Users.FindAsync(GetUserId());
            var expenseToAdd = new MonthlyExpense
            {
                ExpiryDate = monthlyExpenseInput.ExpiryDate,
                TriggeringDateOfMonth = monthlyExpenseInput.TriggeringDayOfMonth,
                ExpenseData = new ExpenseData
                {
                    Amount = monthlyExpenseInput.Amount,
                    Description = monthlyExpenseInput.Description
                },
                User = user
            };
            _context.MonthlyExpenses.Add(expenseToAdd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMonthlyExpense", new { id = expenseToAdd.Id }, expenseToAdd);
        }

        // DELETE: api/MonthlyExpenses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MonthlyExpense>> DeleteMonthlyExpense(int id)
        {
            var monthlyExpense = await this.UserMonthlyExpensesQueryBuilder(GetUserId()).Where(c => c.Id == id).FirstOrDefaultAsync();

            if (monthlyExpense == null)
            {
                return NotFound();
            }

            _context.MonthlyExpenses.Remove(monthlyExpense);
            await _context.SaveChangesAsync();

            return monthlyExpense;
        }

        private bool MonthlyExpenseExists(int id)
        {
            return _context.MonthlyExpenses.Any(e => e.Id == id);
        }
    }
}
