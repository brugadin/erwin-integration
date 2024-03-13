using Microsoft.AspNetCore.Mvc;
using ErwinDataExtractorLib;

namespace ErwinDataApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErwinDataController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Specify the path to your Erwin file or accept it as a parameter
                string erwinFilePath = @"C:\Users\gabri\Downloads\test1.erwin";
                var entitiesString = ErwinDataExtractor.ExtractErwinDataToJson(erwinFilePath);
                return Ok(entitiesString);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
