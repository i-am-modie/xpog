using Microsoft.EntityFrameworkCore;

namespace Xpog.Models
{
    public class ExpenseContext: DbContext
    {
        public ExpenseContext(DbContextOptions<ExpenseContext> options) : base(options)
        {}

        public DbSet<ExpenseItem> ExpenseItems { get; set; }
    }
}
