using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISTEGY.Sales.Models
{
    public class Transaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VoucherSerial { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType Type { get; set; }

        [ForeignKey("Store")]
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }

        public virtual List<TransactionDetail> Details { get; set; }
    }
}