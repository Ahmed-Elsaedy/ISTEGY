using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ISTEGY.Sales.Models
{
    public class Store
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, StringLength(30, MinimumLength = 5)]
        public string Title { get; set; }
        public virtual List<Inventory> Inventories { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}