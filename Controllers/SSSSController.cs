using Microsoft.AspNetCore.Mvc;
using SSSSProject.Models;
using SSSSProject.Data;
using Service;
using Microsoft.AspNetCore.Cors;


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
        [HttpPost("GetDetail")]

        [EnableCors("CorsPolicy")]
        public async Task<IActionResult> GetDetail(string input)
        {
            try
            {
                var detail = await masterService.GetProductDetail(input);
                Response.Headers.Add("Access-Control-Allow-Origin", "*");
                return Ok(detail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}