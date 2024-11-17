using RefactorThis.Persistence.Enum;
using RefactorThis.Persistence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    public interface IInvoiceService
    {
        string ProcessPayment(Payment payment, PaymentProcessType paymentProcessType);
    }
}
