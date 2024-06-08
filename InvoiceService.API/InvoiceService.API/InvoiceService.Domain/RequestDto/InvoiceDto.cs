

namespace InvoiceService.API.InvoiceService.Domain.RequestDto
{
    public class InvoiceDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = null!;
        public List<string> Cars { get; set; } = null!;
        public decimal AmountPaid { get; set; }

    }
}
