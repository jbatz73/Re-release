using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Enum
{
    public enum PaymentProcessType
    {
        [Description("No Invoice For Payment")]
        NoInvFndPayment = 1,
        [Description("No Payment Needed")]
        NoPymntNeeded = 2,
        [Description("Invoice Already Fully Paid")]
        InvFullyPaid = 3,
        [Description("Partial Payment Exists And Amount Paid Exceeds Amount Due")]
        PartPayExistsAmtPdExceedAmtDue = 4,
        [Description("No Partial Payment Exists And Amount Paid Exceeds Invoice Amount")]
        NoPartPayExistAmtPdExcdInvAmt = 5,
        [Description("Partial Payment Exists And Amount Paid Equals Amount Due")]
        PartPayExistAmtPdEqlAmtDue = 6,
        [Description("No Partial Payment Exists And Amount Paid Equals Invoice Amount")]
        NoPartPayExistAmtPdEqlInvAmt = 7,
        [Description("Partial Payment Exists And Amount Paid Is Less Than Amount Due")]
        PartPayExistAmtPdIsLessAmtDue = 8,
        [Description("No Partial Payment Exists And Amount Paid Is Less Than Invoice Amount")]
        NoPartPayExistAmtPdIsLessInvAmt = 9
    }
}
