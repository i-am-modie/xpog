using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Xpog.Enitities.Expenses
{
    public class RepeatableExpense : BaseExpense
    {
        [Range(0,365)]
        public int timeToRepeatInDays { get; set; }
        public int daysToTrigger { get; set; }
        public DateTime? ExpiryDate { get; set; }


    }
}
