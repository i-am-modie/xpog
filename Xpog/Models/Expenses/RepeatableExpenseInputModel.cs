using System;
using System.ComponentModel.DataAnnotations;

namespace Xpog.Models.Expenses
{
    public class RepeatableExpenseInputModel : BaseExpenseInputModel
    {
        [Required]
        [Range(0, 365)]
        public int timeToRepeatInDays { get; set; }
        public DateTime FirstOccurence { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
