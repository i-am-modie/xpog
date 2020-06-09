using Microsoft.EntityFrameworkCore;
using Xpog.Entities;

namespace Xpog.Models
{
    public class ExpenseContext: DbContext
    {
        public ExpenseContext(DbContextOptions<ExpenseContext> options) : base(options)
        {}

        public DbSet<Expense> ExpenseItems { get; set; }
    }
}
