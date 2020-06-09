using System.ComponentModel.DataAnnotations;

namespace Xpog.Models
{
    public class ExpenseModel
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Amount { get; set; }
    }
}
