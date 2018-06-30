using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISTEGY.Sales.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, StringLength(30, MinimumLength = 5)]
        public string Title { get; set; }
        public virtual List<Inventory> Inventories { get; set; }
        public virtual List<TransactionDetail> TransactionDetails { get; set; }
    }
}