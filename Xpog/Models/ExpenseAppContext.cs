using Microsoft.EntityFrameworkCore;
using Xpog.Enitities.Expenses;
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
        public DbSet<ExpenseData> ExpenseDatas { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CurrentExpense> CurrentExpenses { get; set; }
        public DbSet<MonthlyExpense> MonthlyExpenses { get; set; }
        public DbSet<RepeatableExpense> RepeatableExpenses { get; set; }
    }
}

