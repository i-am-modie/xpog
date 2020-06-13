using Microsoft.EntityFrameworkCore;
using Xpog.Entities;

namespace Ef6CoreForPosgreSQL.Models
{
    public class ExpenseAppContext: DbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseIdentityColumns();
        }

        public ExpenseAppContext(DbContextOptions<ExpenseAppContext> options) : base(options) {} 
        public DbSet<Expense> ExpenseItems { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

