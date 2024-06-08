using InvoiceService.API.InvoiceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;



namespace RoomService.Infrastructure.Data
{
    public class AppDbContext : DbContext 
    {
        public DbSet<BidderModel> Invoices { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public AppDbContext()
        {

        }
        
    }
}