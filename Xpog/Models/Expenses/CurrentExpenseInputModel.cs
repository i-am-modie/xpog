using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Xpog.Models.Expenses
{
    public class CurrentExpenseInputModel : BaseExpenseInputModel
    {
        [Required]
        public DateTime Date { get; set; }
    }
}
