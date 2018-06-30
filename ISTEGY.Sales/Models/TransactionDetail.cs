using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISTEGY.Sales.Models
{
    public class TransactionDetail
    {
        [Key, ForeignKey("Transaction"), Column(Order = 0)]
        public int TranId { get; set; }
        public virtual Transaction Transaction { get; set; }

        [Key, ForeignKey("Product"), Column(Order = 1)]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
    }
}