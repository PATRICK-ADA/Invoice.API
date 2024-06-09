namespace InvoiceService.API.InvoiceService.Domain.Entities
{
    
    public class GenerateInvoiceModel
    { 
        public string UserName { get; set; }
        public List<string> Cars { get; set; } = new();
        public decimal Amount { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
