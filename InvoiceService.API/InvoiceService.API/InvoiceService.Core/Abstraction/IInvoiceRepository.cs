using BidService.API.BidService.Core.ApiResponse;

namespace InvoiceService.API.InvoiceService.Core.Abstraction
{
    public interface IInvoiceRepository
    {
        Task<ApiResponse<object>> GetBidderInvoiceAsync();




    }
}
