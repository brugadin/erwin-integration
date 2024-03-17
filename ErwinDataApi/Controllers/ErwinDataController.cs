using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using ErwinDataExtractorLib;

namespace ErwinDataApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErwinDataController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                // Check if the request contains multipart/form-data.
                if (HttpContext.Request.Form.Files.Any())
                {
                    var file = HttpContext.Request.Form.Files[0];

                    // Ensure the file is not empty
                    if (file.Length > 0)
                    {
                        // Define the path where the file will be saved
                        // Use a unique name for each file to avoid overwriting
                        var filePath = Path.Combine(@"C:\Users\gabri\Code\erwinFiles", Guid.NewGuid().ToString() + ".erwin");

                        // Create a new file stream to copy the uploaded file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // Now that the file is saved, extract data from it
                        var entitiesString = ErwinDataExtractor.ExtractErwinDataToJson(filePath);

                        // Perform any additional operations with entitiesString...

                        return Ok(entitiesString);
                    }
                    else
                    {
                        return BadRequest("Empty file.");
                    }
                }
                else
                {
                    return BadRequest("No file uploaded.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
