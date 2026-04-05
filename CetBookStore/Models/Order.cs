using System.ComponentModel.DataAnnotations;

namespace CetBookStore.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public Decimal TotalPrice { get; set;  }

        public virtual List<OrderItem>? OrderItems { get; set; }
    }
}
