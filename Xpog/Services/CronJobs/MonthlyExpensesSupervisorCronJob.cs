using Ef6CoreForPosgreSQL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xpog.Enitities.Expenses;

namespace Xpog.Services.CronJobs
{
    public class MonthlyExpensesSupervisorCronJob : CronJobService
    {
        private readonly ILogger<MonthlyExpensesSupervisorCronJob> _logger;

        IServiceProvider _serviceProvider;
        public MonthlyExpensesSupervisorCronJob(IScheduleConfig<MonthlyExpensesSupervisorCronJob> config, ILogger<MonthlyExpensesSupervisorCronJob> logger, IServiceProvider serviceProvider)
         : base(config.CronExpression, config.TimeZoneInfo)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Monthly Expense Supervisor: started");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Monthly Expense Supervisor: working");
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ExpenseAppContext>();

            _logger.LogInformation("Monthly Expense Supervisor: adding triggered monhly ones");
            var currentDate = DateTime.Now;
            var currentDay = currentDate.Day;
            var countOfDaysInCurrentMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            IQueryable<MonthlyExpense> query = context.MonthlyExpenses.Include(c => c.ExpenseData).Include(c => c.User);

            if (currentDay == countOfDaysInCurrentMonth)
            {
                query = query.Where(me => me.TriggeringDateOfMonth <= currentDay);
            }
            else
            {
                query = query.Where(me => me.TriggeringDateOfMonth == currentDay);
            }

            var expensesToTrigger = query.ToList();
            expensesToTrigger.ForEach(ett =>
            {
                var user = context.Users.Where(u => u.Id == ett.User.Id).FirstOrDefault();
                context.CurrentExpenses.Add(new CurrentExpense
                {
                    Date = currentDate,
                    ExpenseData = ett.ExpenseData,
                    User = user
                });
            });

            context.SaveChanges();

            _logger.LogInformation("Monthly Expense Supervisor: removing expired ones");
            var expensesToRemove = context.MonthlyExpenses.Where(c => c.ExpiryDate.HasValue && DateTime.Compare(DateTime.Now, c.ExpiryDate.Value) >= 0).ToList();
            context.MonthlyExpenses.RemoveRange(expensesToRemove);
            context.SaveChanges();

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Monthly Expense Supervisor:  stopping.");
            return base.StopAsync(cancellationToken);
        }
    }



}
