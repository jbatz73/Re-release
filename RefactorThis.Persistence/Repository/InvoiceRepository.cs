using RefactorThis.Persistence.Model;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Repository
{
    public class InvoiceRepository: IInvoiceRepository
    {
        private Invoice _invoice;
        public InvoiceRepository(Invoice invoice)
        {
            _invoice = invoice;
        }

        public Invoice GetInvoice(string reference)
        {
            //Extract data here from dbcontext
            return _invoice;
        }

        public void SaveInvoice(Invoice invoice)
        {
            //Save data here through dbcontext
        }

        public void AddInvoice(Invoice invoice)
        {
            _invoice = invoice;
        }
    }
}