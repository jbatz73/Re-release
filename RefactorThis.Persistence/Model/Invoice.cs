using System.Collections.Generic;
using RefactorThis.Persistence.Enum;
using RefactorThis.Persistence.Repository;

namespace RefactorThis.Persistence.Model
{
    public class Invoice
    {
        public InvoiceType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public List<Payment> Payments { get; set; }

    }
}