using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Xpog.Models.Expenses
{
    public class MonthlyExpenseInputModel : BaseExpenseInputModel
    {
        [Required]
        [Range(1, 31)]
        public int TriggeringDateOfMonth { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
