using BookStore.BL;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static System.Reflection.Metadata.BlobBuilder;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        // GET: api/<BookController>
        [HttpGet("GetAllSitewideBooks")]
        public Object GetAllDbBooksAdHoc()
        {
            return Book.GetAllDbBooksAdHoc();
        }


        [HttpGet("GetBooksByTitle")]
        public Object GetBooksByTitle(string query)
        {
            //Console.WriteLine(query);
            return Book.GetBooksByTitle(query);
        }

        [HttpGet("GetAverageRatingForBook")]
        public float GetAverageRatingForBook(int BookID)
        {
            return Book.GetAverageRatingForBook(BookID);
        }


        [HttpGet("GetBooksByWords")]
        public Object GetBooksByWords(string query)
        {
            //Console.WriteLine(query);
            return Book.GetBooksByWords(query);
        }

        [HttpGet("GetBookQuestion_WhichDatePublished")]
        public Object GetBookQuestion_WhichDatePublished()
        {
            return Book.GetBookQuestion_WhichDatePublished();
        }

        [HttpGet("GetBookQuestion_WhichPublisher")]
        public Object GetBookQuestion_WhichPublisher()
        {
            return Book.GetBookQuestion_WhichPublisher();
        }
        
        [HttpGet("GetBookQuestion_WhichPageCount")]
        public Object GetBookQuestion_WhichPageCount()
        {
            return Book.GetBookQuestion_WhichPageCount();
        }

        [HttpGet("GetBookQuestion_WhichAuthor")]
        public Object GetBookQuestion_WhichAuthor()
        {
            return Book.GetBookQuestion_WhichAuthor();
        }

        [HttpGet("GetAuthorQuestion_WhichAuthorImage")]
        public Object GetAuthorQuestion_WhichAuthorImage()
        {
            return Book.GetAuthorQuestion_WhichAuthorImage();
        }

        [HttpGet("bookPage")]
        public Object GetAllBookInfo(int id)
        {   
            return Book.GetAllBookInfo(id);
        }

        [HttpGet("GetFlashSaleBooks")]
        public Object GetFlashSaleBooks()
        {
            return Book.GetFlashSaleBooks();
        }

        [HttpPost("CheckIfBookIsInDB")]
        public IActionResult CheckIfBookIsInDB([FromBody] JsonElement data)
        {
            int userID = data.GetProperty("userID").GetInt32();
            if (!BookStoreUser.IsUserAdmin(userID))
                return Forbid();
            string title = data.GetProperty("title").GetString();
            string isbn_13 = data.GetProperty("isbn13").GetString();
            string isbn_10 = data.GetProperty("isbn10").GetString();
            return Ok(Book.CheckIfBookIsInDB(title, isbn_10, isbn_13));
        }

      


        // GET api/<BookController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("GetBookDataForAdminTable")]
        public IActionResult GetBookDataForAdminTable(int userID)
        {
            if (!BookStoreUser.IsUserAdmin(userID))
                return Forbid();
            return Ok(Book.GetBookDataForAdminTable());
        }


        [HttpGet("GetTop10UserOwnedBooks")]
        public Object GetTop10UserOwnedBooks()
        {
            return Book.GetTop10UserOwnedBooks();
        }

        // POST api/<BookController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("AddNewBookToDB")]
        public IActionResult AddNewBookToDB([FromBody] JsonElement data)
        {
            int userID = data.GetProperty("userID").GetInt32();
            if (!BookStoreUser.IsUserAdmin(userID))
                return Forbid();
            // Use TryGetProperty to safely access properties that may not exist or be null
            string title = data.TryGetProperty("title", out var titleElement) ? titleElement.GetString() : null;
            string subtitle = data.TryGetProperty("subtitle", out var subtitleElement) ? subtitleElement.GetString() : null;
            string description = data.TryGetProperty("description", out var descriptionElement) ? descriptionElement.GetString() : null;
            string publisher = data.TryGetProperty("publisher", out var publisherElement) ? publisherElement.GetString() : null;
            DateTime publishedDate = data.TryGetProperty("publishedDate", out var publishedDateElement) && publishedDateElement.TryGetDateTime(out var dt)? dt: DateTime.MinValue;
            string printType = data.TryGetProperty("printType", out var printTypeElement) ? printTypeElement.GetString() : null;
            int pageCount = data.TryGetProperty("pageCount", out var pageCountElement) ? pageCountElement.GetInt32() : 0;
            string previewLink = data.TryGetProperty("previewLink", out var previewLinkElement) ? previewLinkElement.GetString() : null;
            string maturityRating = data.TryGetProperty("maturityRating", out var maturityRatingElement) ? maturityRatingElement.GetString() : null;
            string language = data.TryGetProperty("language", out var languageElement) ? languageElement.GetString() : null;
            string infoLink = data.TryGetProperty("infoLink", out var infoLinkElement) ? infoLinkElement.GetString() : null;
            string ISBN_10 = data.TryGetProperty("ISBN_10", out var ISBN_10Element) ? ISBN_10Element.GetString() ?? "irrelevant" : "irrelevant";
            string ISBN_13 = data.TryGetProperty("ISBN_13", out var ISBN_13Element) ? ISBN_13Element.GetString() ?? "irrelevant" : "irrelevant";
            string smallThumbnail = data.TryGetProperty("smallThumbnail", out var smallThumbnailElement) ? smallThumbnailElement.GetString() : null;
            string thumbnail = data.TryGetProperty("thumbnail", out var thumbnailElement) ? thumbnailElement.GetString() : "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg";
            string canonicalVolumeLink = data.TryGetProperty("canonicalVolumeLink", out var canonicalVolumeLinkElement) ? canonicalVolumeLinkElement.GetString() : null;
            string selfLink = data.TryGetProperty("selfLink", out var selfLinkElement) ? selfLinkElement.GetString() : null;
            bool isEbook = data.TryGetProperty("isEbook", out var isEbookElement) && isEbookElement.GetBoolean();
            string downloadLink = data.TryGetProperty("downloadLink", out var downloadLinkElement) ? downloadLinkElement.GetString() : null;
            double price = data.TryGetProperty("price", out var priceElement) ? priceElement.GetDouble() : 0.0;

            return Ok(Book.AddNewBookToDB(title, subtitle, description, publisher, publishedDate, printType, pageCount, previewLink, maturityRating, language, infoLink, ISBN_10, ISBN_13, smallThumbnail, thumbnail, canonicalVolumeLink, selfLink, isEbook, downloadLink, price));
        }

        [HttpPost("InsertNewCategoryAndConnectToBook")]
        public IActionResult InsertNewCategoryAndConnectToBook([FromBody] JsonElement data)
        {
            int userID = data.GetProperty("userID").GetInt32();
            if (!BookStoreUser.IsUserAdmin(userID))
                return Forbid();
            string CategoryName = data.GetProperty("categoryName").ToString();
            int BookID = data.GetProperty("bookID").GetInt32();
            return Ok(Book.InsertNewCategoryAndConnectToBook(CategoryName, BookID));
        }

            // PUT api/<BookController>/5
            [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
