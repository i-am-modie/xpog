using Ef6CoreForPosgreSQL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xpog.Enitities.Expenses;

namespace Xpog.Services.CronJobs
{
    public class RepeatableExpensesSupervisorCronJob : CronJobService
    {
        private readonly ILogger <RepeatableExpensesSupervisorCronJob> _logger;

        IServiceProvider _serviceProvider;
        public RepeatableExpensesSupervisorCronJob(IScheduleConfig<RepeatableExpensesSupervisorCronJob> config, ILogger<RepeatableExpensesSupervisorCronJob> logger, IServiceProvider serviceProvider)
         : base(config.CronExpression, config.TimeZoneInfo)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Repeatable Expense Supervisor: started");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Repeatable Expense Supervisor: working");
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ExpenseAppContext>();
           

            _logger.LogInformation("Repeatable Expense Supervisor: decrementing trigger days");
            var repeatableExpenses = context.RepeatableExpenses.ToList();
            repeatableExpenses.ForEach(re =>
            {
                var daysToTrigger = re.daysToTrigger;
                re.daysToTrigger = daysToTrigger - 1;
            }    );

            context.SaveChanges();

            _logger.LogInformation("Repeatable Expense Supervisor: restarting trigger days");
            var expensesToRestart = context.RepeatableExpenses.Include(c => c.ExpenseData).Include(c => c.User).Where(c => c.daysToTrigger <= 0).ToList();
            
            expensesToRestart.ForEach(re =>
            {
                var expense = re.ExpenseData;
                var user = re.User;
                re.daysToTrigger = re.timeToRepeatInDays;
                context.CurrentExpenses.Add(new CurrentExpense
                {
                    Date = DateTime.Now,
                    ExpenseData = expense,
                    User = user
                });

    
                context.SaveChanges();
            });
           

            _logger.LogInformation("Repeatable Expense Supervisor: removing expired ones");
            var expensesToRemove = context.RepeatableExpenses.Where(c => c.ExpiryDate.HasValue && DateTime.Compare(DateTime.Now, c.ExpiryDate.Value) >= 0).ToList();
            context.RepeatableExpenses.RemoveRange(expensesToRemove);
            context.SaveChanges();

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Repeatable Expense Supervisor: stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
