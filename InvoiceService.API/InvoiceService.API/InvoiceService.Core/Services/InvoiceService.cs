
using InvoiceService.API.InvoiceService.Core.Abstraction;
using BidService.API.BidService.Core.ApiResponse;
using InvoiceService.API.InvoiceService.Domain.RequestDto;

namespace InvoiceService.Core.Services
{

    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
       
        public InvoiceService(IInvoiceRepository bidRepository)
        {
        
            _invoiceRepository = bidRepository;
           
        }

        public async Task<ApiResponse<object>> GetBidderInvoiceAsync()
        {

            var result = await _invoiceRepository.GetBidderInvoiceAsync();
            return result;  

        }

      
    }
}
