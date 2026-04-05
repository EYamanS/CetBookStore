using System.ComponentModel.DataAnnotations;

namespace CetBookStore.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int BookId { get; set; }

        [Range(1, 100)]
        public int  Quantity { get; set; }

        public Decimal Price { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Book? Book { get; set; }
    }
}
