using Ef6CoreForPosgreSQL.Models;
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
    public class RepeatableExpenseSupervisorCronJob : CronJobService
    {
        private readonly ILogger <RepeatableExpenseSupervisorCronJob> _logger;

        IServiceProvider _serviceProvider;
        public RepeatableExpenseSupervisorCronJob(IScheduleConfig<RepeatableExpenseSupervisorCronJob> config, ILogger<RepeatableExpenseSupervisorCronJob> logger, IServiceProvider serviceProvider)
         : base(config.CronExpression, config.TimeZoneInfo)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Repeatable Expense Supervisor started");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJobRESC is working.");
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ExpenseAppContext>();

            var repeatableExpenses = context.RepeatableExpenses.ToList();
            repeatableExpenses.ForEach(re => re.daysToTrigger = re.daysToTrigger - 1);
            context.SaveChanges();

            var expensesToRestart = context.RepeatableExpenses.Where(c => c.daysToTrigger == 0).ToList();
            expensesToRestart.ForEach(re =>
            {
                var user = context.Users.Where(u => u.Id == re.User.Id).FirstOrDefault();
                context.CurrentExpenses.Add(new CurrentExpense
                {
                    Date = DateTime.Now,
                    ExpenseData = re.ExpenseData,
                    User = user
                });

                re.daysToTrigger = re.timeToRepeatInDays;
            });
            context.SaveChanges();


            var expensesToRemove = context.RepeatableExpenses.Where(c => c.ExpiryDate.HasValue && DateTime.Compare(DateTime.Now, c.ExpiryDate.GetValueOrDefault()) > 0);
            context.RepeatableExpenses.RemoveRange(expensesToRemove);
            context.SaveChanges();

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 3 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
