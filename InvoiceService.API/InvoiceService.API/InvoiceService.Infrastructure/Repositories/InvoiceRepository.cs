using BidService.API.BidService.Core.ApiResponse;
using InvoiceService.API.InvoiceService.Core.Abstraction;
using InvoiceService.API.InvoiceService.Domain.RequestDto;
using Microsoft.EntityFrameworkCore;
using RoomService.Infrastructure.Data;


namespace Invoice.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        
        private readonly AppDbContext _context;
        public InvoiceRepository(AppDbContext context)
        {
           
            _context = context;

        }

        public async Task<ApiResponse<object>> GetBidderInvoiceAsync()
        {

            var maxAmountPaid = await _context.Invoices.MaxAsync(b => b.AmountPaid);
            var highestBidder = await _context.Invoices
                .Where(b => b.AmountPaid == maxAmountPaid)
                .FirstOrDefaultAsync();     

            return new SuccessApiResponse<InvoiceDto>("Retrieved the details of the Highest bidder from Kafka successfully", highestBidder);
        }
    }
}
