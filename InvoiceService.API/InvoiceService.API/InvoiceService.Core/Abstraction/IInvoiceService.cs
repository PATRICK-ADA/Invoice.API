using BidService.API.BidService.Core.ApiResponse;


namespace InvoiceService.API.InvoiceService.Core.Abstraction
{
    public interface IInvoiceService
    {
        Task<ApiResponse<object>> GetBidderInvoiceAsync();


    }
}
