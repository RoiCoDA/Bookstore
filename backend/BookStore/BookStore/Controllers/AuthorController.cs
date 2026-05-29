using BookStore.BL;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        // GET: api/<AuthorController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("authorPage")]
        public Object GetAuthorAndBooksByAuthorAdHoc(int id)
        {
            //Console.WriteLine(id);
            return Author.GetAuthorAndBooksByAuthorAdHoc(id);
        }

        [HttpGet("GetAuthorDataForAdminTable")]
        public IActionResult GetAuthorDataForAdminTable(int userID)
        {
            if (!BookStoreUser.IsUserAdmin(userID))
                return Forbid();
            return Ok(Author.GetAuthorDataForAdminTable());
        }

        [HttpGet("GetAllForumPosts")]
        public Object GetAllForumPosts(int AuthorID)
        {
            return Author.GetAllForumPosts(AuthorID);
        }

        [HttpGet("GetAllWritersDisplayAdHoc")]
        public Object GetAllWritersDisplayAdHoc()
        {
            return Author.GetAllWritersDisplayAdHoc();
        }

        [HttpGet("GetAuthorsByName")]
        public Object GetAuthorsByName(string query)
        {
            return Author.GetAuthorsByName(query);
        }

        [HttpGet("Load100ChatMessages")]
        public Object Load100ChatMessages(int AuthorID, int Offset)
        {
            return Author.Load100ChatMessages(AuthorID, Offset);
        }

        [HttpGet("LoadLastChatMessage")]
        public Object LoadLastChatMessage(int AuthorID)
        {
            return Author.LoadLastChatMessage(AuthorID);
        }

        [HttpGet("GetChatMessageByChatMessagIDAndAuthorID")]
        public Object GetChatMessageByChatMessagIDAndAuthorID(int AuthorID, int MessageID)
        {
            return Author.GetChatMessageByChatMessagIDAndAuthorID(AuthorID, MessageID);
        }




        // POST api/<AuthorController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("SubmitForumPost")]
        public bool SubmitForumPost([FromBody] JsonElement data)
        {
            int AuthorID = data.GetProperty("authorID").GetInt32();
            int UserID = data.GetProperty("userID").GetInt32();
            string Header = data.GetProperty("header").GetString();
            string Content = data.GetProperty("content").GetString();
            int ResponseToPostID = data.GetProperty("responseToPostID").GetInt32();
            return Author.SubmitForumPost(AuthorID, UserID, Header, Content, ResponseToPostID);
        }


        [HttpPost("InsertNewAuthorAndConnectToBook")]
        public IActionResult InsertNewAuthorAndConnectToBook([FromBody] JsonElement data)
        {
            int userID = data.GetProperty("userID").GetInt32();
            if (!BookStoreUser.IsUserAdmin(userID))
                return Forbid();
            string authorName = data.TryGetProperty("authorName", out var authorNameElement) ? authorNameElement.GetString() : null;
            string summary = data.TryGetProperty("authorSummary", out var summaryElement) ? summaryElement.GetString() : null;
            string image = data.TryGetProperty("authorImage", out var imageElement) ? imageElement.GetString() : null;
            string link = data.TryGetProperty("authorLink", out var linkElement) ? linkElement.GetString() : null;
            int bookID = data.GetProperty("bookID").GetInt32();

            return Ok(Author.InsertNewAuthorAndConnectToBook(authorName, summary, image, link, bookID));
        }





        [HttpPut("DoesAuthorExistInDB")]
        public bool DoesAuthorExistInDB(string AuthorName)
        {
            return Author.DoesAuthorExistInDB(AuthorName);
        }



        // PUT api/<AuthorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
