using BookStore.BL;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IConfiguration _config;

        public RatingController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("GetSentimentAnalysis")]
        public async Task<IActionResult> GetSentimentAnalysis([FromBody] JsonElement body)
        {
            string text = body.GetProperty("text").GetString();
            string apiKey = _config["HuggingFace:ApiKey"];

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = JsonSerializer.Serialize(new { inputs = text });
            var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://api-inference.huggingface.co/models/lxyuan/distilbert-base-multilingual-cased-sentiments-student",
                content);

            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        // GET: api/<RatingController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("GetUserRatingReview")]
        public int GetUserRatingReview(int userID, int ratingID)
        {
            return Rating.GetUserRatingReview(userID, ratingID);
        }

        [HttpGet("GetRatingWrittenDataForSentimentAnalysis")]
        public Object GetRatingWrittenDataForSentimentAnalysis(int bookID)
        {
            return Rating.GetRatingWrittenDataForSentimentAnalysis(bookID);
        }

        [HttpGet("GetAverageRatingForBook")]
        public float GetAverageRatingForBook(int BookID)
        {
            return Rating.GetAverageRatingForBook(BookID);
        }


        [HttpGet("GetUsersBookReview")]
        public Object GetUsersBookReview(int userID, int bookID)
        {
            return Rating.GetUsersBookReview(userID, bookID);
        }

        [HttpGet("GetUsersAllReviews")]
        public Object GetUsersAllReviews(int userID)
        {
            //Console.WriteLine(userID);
            return Rating.GetUsersAllReviews(userID);
        }

        [HttpGet("GetTopFiveRatingsReviews")]
        public Object GetTopFiveRatingsReviews(int userID)
        {
            ////Console.WriteLine(userID);
            return Rating.GetTopFiveRatingsReviews(userID);
        }

        [HttpGet("GetTopFiveRatingsReviewsForBook")]
        public Object GetTopFiveRatingsReviewsForBook(int userID, int bookID)
        {
            ////Console.WriteLine(userID);
            ////Console.WriteLine(bookID);
            return Rating.GetTopFiveRatingsReviewsForBook(userID, bookID);
        }

        [HttpPost("RateReview")]
        public int RateReview([FromBody] JsonElement data)
        {
            int userID = data.GetProperty("userID").GetInt32();
            int ratingID = data.GetProperty("ratingID").GetInt32();
            int score = data.GetProperty("score").GetInt32();
            return Rating.RateReview(userID, ratingID, score);
        }

        // POST api/<RatingController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("InsertUserReview")]
        public int InsertUserReview([FromBody] JsonElement rating)
        {


            int userId = rating.GetProperty("userID").GetInt32();
            int bookID = rating.GetProperty("bookID").GetInt32();
            int ratingStars = rating.GetProperty("ratingStars").GetInt32();
            string description = rating.GetProperty("description").GetString();
            DateTime date = rating.GetProperty("dateCreated").GetDateTime();
            string header = rating.GetProperty("header").GetString();

            Rating ratingAssembled = new Rating(userId, bookID, ratingStars, description, date, header);

            return Rating.InsertUserReview(ratingAssembled);
        }


        // PUT api/<RatingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RatingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
