using InvoiceService.API.InvoiceService.Core.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceService.API.NotificationService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
       private readonly IInvoiceService _invoiceService;


        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        
        [HttpGet("Retrieve-Highest_Bidder")]
       public async Task<IActionResult> GetBidderInvoiceAsync()
       {

            var result = await _invoiceService.GetBidderInvoiceAsync();
            return Ok(result);

        }
    }
}
