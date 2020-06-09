using System.ComponentModel.DataAnnotations;

namespace Xpog.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Amount { get; set; }
    }
}
