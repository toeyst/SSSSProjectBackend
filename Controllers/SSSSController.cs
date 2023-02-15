using Microsoft.AspNetCore.Mvc;
using SSSSProject.Models;
using SSSSProject.Data;
using Service;
namespace SSSSProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SSSSController : ControllerBase
    {
        private readonly IMasterService masterService;
        public SSSSController(IMasterService masterService)
        {
            this.masterService = masterService;
        }
        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetDetail()
        {
            try
            {
                var detail = await masterService.GetProductDetail();
                return Ok(detail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}