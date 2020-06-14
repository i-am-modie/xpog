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
using Microsoft.AspNetCore.Authorization;
using Xpog.Models.Expenses;
using Xpog.Entities;

namespace Xpog.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CurrentExpensesController : ControllerBase
    {
        private readonly ExpenseAppContext _context;

        public CurrentExpensesController(ExpenseAppContext context)
        {
            _context = context;
        }

        private System.Linq.IQueryable<CurrentExpense> UserCurrentExpensesQueryBuilder (int uid)
        {
            return _context.CurrentExpenses.Include(c => c.ExpenseData).Where(c => c.User.Id == uid);
        }

        private int GetUserId() {
            var principal = HttpContext.User;
            var idString = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (idString == null || idString == "")
            {
                throw new Exception("unknown user id: try to relogin");
            }

            var id = int.Parse(idString);
            return id;
        }

        // GET: api/CurrentExpenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrentExpense>>> GetCurrentExpenses(DateTime? startDate, DateTime? endDate)
        {
            var id = GetUserId();
            var query = this.UserCurrentExpensesQueryBuilder(id);
            if(startDate.HasValue)
            {
                var date = startDate.GetValueOrDefault();
                query = query.Where(c => DateTime.Compare(date, c.Date) <= 0);
            }
            if(endDate.HasValue)
            {
                var date = endDate.GetValueOrDefault();
                query = query.Where(c => DateTime.Compare(date, c.Date) >= 0);
            }
            Console.WriteLine(query.ToString());
            return await query.ToListAsync();
        }


        // GET: api/CurrentExpenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrentExpense>> GetCurrentExpense(int id)
        {
            var uid = GetUserId();
            var currentExpense = await this.UserCurrentExpensesQueryBuilder(uid).Where(c => c.Id == id).FirstOrDefaultAsync();
            
            if (currentExpense == null)
            {
                return NotFound();
            }

            return currentExpense;
        }

        // PUT: api/CurrentExpenses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurrentExpense(int id, CurrentExpenseInputModel currentExpenseInput)
        {
            var uid = GetUserId();
            var expenseToModify = await this.UserCurrentExpensesQueryBuilder(uid).Where(c => c.Id == id).FirstOrDefaultAsync();

            if (expenseToModify == null)
            {
                throw new Exception("You don't own any current expense with that id");
            }

            expenseToModify.Date = currentExpenseInput.Date;
            expenseToModify.ExpenseData = new ExpenseData
            {
                Amount = currentExpenseInput.Amount,
                Description = currentExpenseInput.Description
            };


            _context.Entry(expenseToModify).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrentExpenseExists(id))
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

        // POST: api/CurrentExpenses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CurrentExpense>> PostCurrentExpense(CurrentExpenseInputModel currentExpenseInput)
        {
            var user = await  _context.Users.FindAsync(GetUserId());
            var currentExpense = new CurrentExpense
            {
                Date = currentExpenseInput.Date,
                User = user,
                ExpenseData = new ExpenseData
                {
                    Amount = currentExpenseInput.Amount,
                    Description = currentExpenseInput.Description
                }
            };

            _context.CurrentExpenses.Add(currentExpense);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCurrentExpense", new { id = currentExpense.Id }, currentExpense);
        }

        // DELETE: api/CurrentExpenses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CurrentExpense>> DeleteCurrentExpense(int id)
        {

            var uid = GetUserId();
            var currentExpense = await _context.CurrentExpenses.Where(c => c.User.Id == uid && c.Id == id).FirstOrDefaultAsync();
            if (currentExpense == null)
            {
                return NotFound();
            }

            _context.CurrentExpenses.Remove(currentExpense);
            await _context.SaveChangesAsync();

            return currentExpense;
        }

        private bool CurrentExpenseExists(int id)
        {
            return _context.CurrentExpenses.Any(e => e.Id == id);
        }
    }
}
