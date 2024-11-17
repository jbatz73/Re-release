using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Repository;
using RefactorThis.Domain.Service;
using RefactorThis.Persistence.Enum;

namespace RefactorThis.Domain.Tests
{
	
    [TestFixture]
	public class InvoicePaymentProcessorTests
	{
		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference( )
		{
            Invoice invoice = null;
            var invRepo = new InvoiceRepository(invoice);
			
			var paymentProcessor = new InvoiceService(invRepo);
			var payment = new Payment( );
			var failureMessage = "";

			try
			{
				var result = paymentProcessor.ProcessPayment(payment, PaymentProcessType.NoInvFndPayment);
			}
			catch ( InvalidOperationException e )
			{
				failureMessage = e.Message;
			}

			Assert.AreEqual( "There is no invoice matching this payment", failureMessage );
		}

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            
            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = null
            };

            var repo = new InvoiceRepository(invoice);

            //repo.AddInvoice(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment();

            var result = paymentProcessor.ProcessPayment(payment, PaymentProcessType.NoPymntNeeded);

            Assert.AreEqual("No payment needed", result);
        }


        [Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 10,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 10
					}
				}
			};
            
			var repo = new InvoiceRepository(invoice);

           // repo.AddInvoice(invoice);

			var paymentProcessor = new InvoiceService(repo);

			var payment = new Payment( );

			var result = paymentProcessor.ProcessPayment(payment, PaymentProcessType.InvFullyPaid);

			Assert.AreEqual("Invoice was already fully paid", result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};
            var repo = new InvoiceRepository(invoice);
            //repo.AddInvoice(invoice);

			var paymentProcessor = new InvoiceService(repo);

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment( payment, PaymentProcessType.PartPayExistsAmtPdExceedAmtDue);

			Assert.AreEqual("The payment is greater than the partial amount remaining", result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var invoice = new Invoice()
			{
				Amount = 5,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};

            var repo = new InvoiceRepository(invoice);

            //repo.AddInvoice(invoice);

			var paymentProcessor = new InvoiceService(repo);

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment(payment, PaymentProcessType.NoPartPayExistAmtPdExcdInvAmt);

			Assert.AreEqual("The payment is greater than the invoice amount", result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};

            var repo = new InvoiceRepository(invoice);

            //repo.AddInvoice(invoice);

			var paymentProcessor = new InvoiceService(repo);

			var payment = new Payment( )
			{
				Amount = 5
			};

			var result = paymentProcessor.ProcessPayment(payment, PaymentProcessType.PartPayExistAmtPdEqlAmtDue);

			Assert.AreEqual("Final partial payment received, invoice is now fully paid", result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount( )
		{
			
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( ) { new Payment( ) { Amount = 10 } }
			};

            var repo = new InvoiceRepository(invoice);

            //repo.AddInvoice(invoice);

			var paymentProcessor = new InvoiceService(repo);

			var payment = new Payment( )
			{
				Amount = 10
			};

			var result = paymentProcessor.ProcessPayment(payment, PaymentProcessType.NoPartPayExistAmtPdEqlInvAmt);

			Assert.AreEqual("Invoice was already fully paid", result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};

            var repo = new InvoiceRepository(invoice);

            //repo.AddInvoice(invoice);

			var paymentProcessor = new InvoiceService(repo);

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment( payment, PaymentProcessType.PartPayExistAmtPdIsLessAmtDue);

			Assert.AreEqual("Another partial payment received, still not fully paid", result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
		
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
            var repo = new InvoiceRepository(invoice);

            //repo.AddInvoice(invoice);

			var paymentProcessor = new InvoiceService(repo);

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment(payment, PaymentProcessType.NoPartPayExistAmtPdIsLessInvAmt);

			Assert.AreEqual("Invoice is now partially paid", result);
		}
	}
}