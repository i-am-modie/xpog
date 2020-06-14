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

namespace Xpog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepeatableExpensesController : ControllerBase
    {
        private readonly ExpenseAppContext _context;

        private System.Linq.IQueryable<RepeatableExpense> UserRepeatableExpensesQueryBuilder(int uid)
        {
            return _context.RepeatableExpenses.Include(c => c.ExpenseData).Where(c => c.User.Id == uid);
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

        public RepeatableExpensesController(ExpenseAppContext context)
        {
            _context = context;
        }

        // GET: api/RepeatableExpenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepeatableExpense>>> GetRepeatableExpenses()
        {
            var id = GetUserId();
            return await this.UserRepeatableExpensesQueryBuilder(id).ToListAsync();
        }

        // GET: api/RepeatableExpenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RepeatableExpense>> GetRepeatableExpense(int id)
        {

            var uid = GetUserId();
            var repeatableExpense = await this.UserRepeatableExpensesQueryBuilder(uid).Where(c => c.Id == id).FirstOrDefaultAsync();

            if (repeatableExpense == null)
            {
                return NotFound();
            }

            return repeatableExpense;
        }

        // PUT: api/RepeatableExpenses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepeatableExpense(int id, RepeatableExpenseInputModel repeatableExpenseInput)
        {
            var uid = GetUserId();
            var expenseToModify = await this.UserRepeatableExpensesQueryBuilder(uid).Where(c => c.Id == id).FirstOrDefaultAsync();

            if (expenseToModify == null)
            {
                throw new Exception("You don't own any current expense with that id");
            }

            //     public int timeToRepeatInDays { get; set; }
            //  public DateTime FirstOccurence { get; set; }
            // public DateTime? ExpiryDate { get; set; }

            expenseToModify.ExpiryDate = repeatableExpenseInput.ExpiryDate;
            expenseToModify.daysToTrigger = repeatableExpenseInput.FirstOccurence.HasValue ? (repeatableExpenseInput.FirstOccurence.GetValueOrDefault() - DateTime.Now).Days : repeatableExpenseInput.TimeToRepeatInDays;
            expenseToModify.timeToRepeatInDays = repeatableExpenseInput.TimeToRepeatInDays;
            expenseToModify.ExpenseData = new ExpenseData
            {
                Amount = repeatableExpenseInput.Amount,
                Description = repeatableExpenseInput.Description
            };


            _context.Entry(expenseToModify).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepeatableExpenseExists(id))
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

        // POST: api/RepeatableExpenses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RepeatableExpense>> PostRepeatableExpense(RepeatableExpenseInputModel repeatableExpenseInput)
        {
            var user = await _context.Users.FindAsync(GetUserId());
            var repeatableExpense = new RepeatableExpense
            {
                ExpiryDate = repeatableExpenseInput.ExpiryDate,
                daysToTrigger = repeatableExpenseInput.FirstOccurence.HasValue ? (repeatableExpenseInput.FirstOccurence.GetValueOrDefault() - DateTime.Now).Days : repeatableExpenseInput.TimeToRepeatInDays,
                timeToRepeatInDays = repeatableExpenseInput.TimeToRepeatInDays,
                ExpenseData = new ExpenseData
                {
                    Amount = repeatableExpenseInput.Amount,
                    Description = repeatableExpenseInput.Description
                }
            };

            _context.RepeatableExpenses.Add(repeatableExpense);
            await _context.SaveChangesAsync();

            if (!repeatableExpenseInput.FirstOccurence.HasValue)
            {
                var currentExpense = new CurrentExpense
                {
                    Date = DateTime.Now,
                    User = user,
                    ExpenseData = repeatableExpense.ExpenseData
                };
                _context.CurrentExpenses.Add(currentExpense);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetRepeatableExpense", new { id = repeatableExpense.Id }, repeatableExpense);
        }

        // DELETE: api/RepeatableExpenses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RepeatableExpense>> DeleteRepeatableExpense(int id)
        {
            var uid = GetUserId();
            var expenseToDelete = await this.UserRepeatableExpensesQueryBuilder(uid).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (expenseToDelete == null)
            {
                return NotFound();
            }

            _context.RepeatableExpenses.Remove(expenseToDelete);
            await _context.SaveChangesAsync();

            return expenseToDelete;
        }

        private bool RepeatableExpenseExists(int id)
        {
            return _context.RepeatableExpenses.Any(e => e.Id == id);
        }
    }
}
