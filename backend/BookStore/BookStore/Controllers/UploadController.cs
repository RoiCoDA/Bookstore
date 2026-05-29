using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        // GET: api/<UploadController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UploadController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        // POST api/<UploadController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files)
        {
            List<string> imageLinks = new List<string>();

            // !! This folder needs to be created on the server side
            string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "uploadedFiles");

            
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    string originalFileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                    string fileExtension = Path.GetExtension(formFile.FileName);
                    string filePath = Path.Combine(path, formFile.FileName);
                    int counter = 1;

                    // Check if file exists and add a counter to the filename if it does
                    while (System.IO.File.Exists(filePath))
                    {
                        string newFileName = $"{originalFileName}_{counter}{fileExtension}";
                        filePath = Path.Combine(path, newFileName);
                        counter++;
                    }

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    imageLinks.Add(Path.GetFileName(filePath));
                }
            }

            return Ok(imageLinks);
        }


        // PUT api/<UploadController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UploadController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
