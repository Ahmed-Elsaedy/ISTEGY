using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISTEGY.Sales.Models
{
    public class Inventory
    {
        [Key, ForeignKey("Store"), Column(Order = 0)]
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }

        [Key, ForeignKey("Product"), Column(Order = 1)]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }
    }
}