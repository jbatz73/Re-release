using System;
using System.Linq;
using System.Threading.Tasks;
using RefactorThis.Persistence.Error;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Repository;
using RefactorThis.Persistence.Enum;

namespace RefactorThis.Domain.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;
        private Invoice invoice;

        public InvoiceService(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment, PaymentProcessType paymentProcessType)
        {
            this.invoice = _invoiceRepository.GetInvoice(payment.Reference);
            var responseMessage = string.Empty;

            switch (paymentProcessType)
            {
                case PaymentProcessType.NoInvFndPayment:
                    responseMessage = this.NoInvoiceFoundForPaymentReference();
                    break;

                case PaymentProcessType.NoPymntNeeded:
                    responseMessage = this.NoPaymentNeeded();
                    break;

                case PaymentProcessType.InvFullyPaid:
                    responseMessage = this.InvoiceAlreadyFullyPaid();
                    break;

                case PaymentProcessType.PartPayExistsAmtPdExceedAmtDue:
                    responseMessage = this.PartialPaymentExistsAndAmountPaidExceedsAmountDue(payment);
                    break;

                case PaymentProcessType.NoPartPayExistAmtPdExcdInvAmt:
                    responseMessage = this.NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount(payment);
                    break;

                case PaymentProcessType.PartPayExistAmtPdEqlAmtDue:
                    responseMessage = this.PartialPaymentExistsAndAmountPaidEqualsAmountDue(payment);
                    break;

                case PaymentProcessType.NoPartPayExistAmtPdEqlInvAmt:
                    responseMessage = this.NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount(payment);
                    break;

                case PaymentProcessType.PartPayExistAmtPdIsLessAmtDue:
                    responseMessage = this.PartialPaymentExistsAndAmountPaidIsLessThanAmountDue(payment);
                    break;

                case PaymentProcessType.NoPartPayExistAmtPdIsLessInvAmt:
                    responseMessage = this.NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount(payment);
                    break;
            }

            return responseMessage;
        }

        private string NoInvoiceFoundForPaymentReference()
        {
            var message = string.Empty;

            if (this.invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;


            return message;
        }

        private string NoPaymentNeeded()
        {
            var message = string.Empty;

            if (this.invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;
            else
            {
                if (invoice.Amount == 0)
                {
                    if (invoice.Payments == null || !invoice.Payments.Any())
                    {
                        message = ResponseMessage.MessageNoPaymentNeeded;
                    }
                    else
                    {
                        message = ResponseMessage.MessageInvoiceInvalidState;
                    }
                }
            }

            return message;
        }

        private string InvoiceAlreadyFullyPaid()
        {
            var message = string.Empty;

            if (this.invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;
            else
            {
                if (invoice.Payments != null && invoice.Payments.Any())
                {
                    if (invoice.Payments.Sum(x => x.Amount) != 0 && invoice.Amount == invoice.Payments.Sum(x => x.Amount))
                    {
                        message = ResponseMessage.MessageWasFullyPaid;
                    }
                }
            }
            return message;
        }

        private string PartialPaymentExistsAndAmountPaidExceedsAmountDue(Payment payment)
        {
            var message = string.Empty;

            if (this.invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;
            else
            {
                if (invoice.Payments != null && invoice.Payments.Any())
                {
                    if (invoice.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid))
                    {
                        message = ResponseMessage.MessagePaymentExceed;
                    }
                }
            }
            return message;
        }

        private string NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount(Payment payment)
        {
            var message = string.Empty;

            if (this.invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;
            else
            {
                if (invoice.Payments == null || !invoice.Payments.Any())
                {
                    if (payment.Amount > invoice.Amount)
                    {
                        message = ResponseMessage.MessageNoPartialPaymentExceed;
                    }
                }
            }

            return message;
        }

        private string PartialPaymentExistsAndAmountPaidEqualsAmountDue(Payment payment)
        {
            var message = string.Empty;

            if (this.invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;
            else
            {
                if ((invoice.Payments != null && invoice.Payments.Any()) && ((invoice.Amount - invoice.AmountPaid) == payment.Amount))
                {
                    switch (invoice.Type)
                    {
                        case InvoiceType.Standard:
                            invoice.AmountPaid += payment.Amount;
                            invoice.Payments.Add(payment);
                            message = ResponseMessage.MessageFinalPayReceved;
                            break;
                        case InvoiceType.Commercial:
                            invoice.AmountPaid += payment.Amount;
                            invoice.TaxAmount += payment.Amount * 0.14m;
                            invoice.Payments.Add(payment);
                            message = ResponseMessage.MessageFinalPayReceved;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _invoiceRepository.SaveInvoice(invoice);

                }
            }
            return message;
        }

        private string NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount(Payment payment)
        {
            var message = string.Empty;

            if (invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;
            else
            {
                if (invoice.Payments == null || !invoice.Payments.Any())
                {
                    if (invoice.Amount == payment.Amount)
                    {
                        switch (invoice.Type)
                        {
                            case InvoiceType.Standard:
                                invoice.AmountPaid = payment.Amount;
                                invoice.TaxAmount = payment.Amount * 0.14m;
                                invoice.Payments.Add(payment);
                                message = ResponseMessage.MessageNowFullyPaid;
                                break;
                            case InvoiceType.Commercial:
                                invoice.AmountPaid = payment.Amount;
                                invoice.TaxAmount = payment.Amount * 0.14m;
                                invoice.Payments.Add(payment);
                                message = ResponseMessage.MessageNowFullyPaid;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _invoiceRepository.SaveInvoice(invoice);
                    }
                }
            }

            return message;
        }

        private string PartialPaymentExistsAndAmountPaidIsLessThanAmountDue(Payment payment)
        {
            var message = string.Empty;

            if (invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;
            else
            {
                if ((invoice.Payments != null && invoice.Payments.Any()) && (payment.Amount < (invoice.Amount - invoice.AmountPaid)))
                {
                    switch (invoice.Type)
                    {
                        case InvoiceType.Standard:
                            invoice.AmountPaid += payment.Amount;
                            invoice.Payments.Add(payment);
                            message = ResponseMessage.MessagePaymentReceivedUnpaid;
                            break;
                        case InvoiceType.Commercial:
                            invoice.AmountPaid += payment.Amount;
                            invoice.TaxAmount += payment.Amount * 0.14m;
                            invoice.Payments.Add(payment);
                            message = ResponseMessage.MessagePaymentReceivedUnpaid;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _invoiceRepository.SaveInvoice(invoice);
                }
            }

            return message;
        }

        private string NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount(Payment payment)
        {
            var message = string.Empty;

            if (invoice == null)
                message = ResponseMessage.MessageInvoiceNoFound;
            else
            {
                if ((invoice.Payments == null || !invoice.Payments.Any()) && (payment.Amount < invoice.Amount))
                {
                    switch (invoice.Type)
                    {
                        case InvoiceType.Standard:
                            invoice.AmountPaid = payment.Amount;
                            invoice.TaxAmount = payment.Amount * 0.14m;
                            invoice.Payments.Add(payment);
                            message = ResponseMessage.MessageNowPartialPaid;
                            break;
                        case InvoiceType.Commercial:
                            invoice.AmountPaid = payment.Amount;
                            invoice.TaxAmount = payment.Amount * 0.14m;
                            invoice.Payments.Add(payment);
                            message = ResponseMessage.MessageNowPartialPaid;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _invoiceRepository.SaveInvoice(invoice);
                }
            }

            return message;
        }



    }
}