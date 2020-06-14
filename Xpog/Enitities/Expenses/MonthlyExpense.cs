using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Xpog.Enitities.Expenses
{
    public class MonthlyExpense : BaseExpense
    {
       [Range(1,31)]
       public int TriggeringDateOfMonth { get; set; }
       public DateTime? ExpiryDate { get; set; }
    }
}
