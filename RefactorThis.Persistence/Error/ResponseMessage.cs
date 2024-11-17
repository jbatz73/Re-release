using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Error
{
    public class ResponseMessage
    {
        public const string MessageNoPaymentNeeded = "No payment needed";
        public const string MessageInvoiceNoFound = "There is no invoice matching this payment";
        public const string MessageInvoiceInvalidState = "The invoice is in an invalid state, it has an amount of 0 and it has payments";
        public const string MessageWasFullyPaid = "Invoice was already fully paid";
        public const string MessageNowFullyPaid = "Invoice is now fully paid";
        public const string MessageNowPartialPaid = "Invoice is now partially paid";
        public const string MessagePaymentExceed = "The payment is greater than the partial amount remaining";
        public const string MessageNoPartialPaymentExceed = "The payment is greater than the invoice amount";
        public const string MessageFinalPayReceved = "Final partial payment received, invoice is now fully paid";
        public const string MessagePaymentReceivedUnpaid = "Another partial payment received, still not fully paid";
    }
}
