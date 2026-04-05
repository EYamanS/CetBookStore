using System.ComponentModel.DataAnnotations;

namespace CetBookStore.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public int BookId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public virtual Book? Book { get; set; }

    }
}
