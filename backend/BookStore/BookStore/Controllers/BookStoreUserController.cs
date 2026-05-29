using BookStore.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookStoreUserController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public BookStoreUserController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // GET: api/<BookStoreUserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BookStoreUserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        [HttpGet("CheckIfUserOwnsBook")]
        public bool CheckIfUserOwnsBook(int UserID, int BookID)
        {
            //Console.WriteLine("From CheckIfUserOwnsBook");
            //Console.WriteLine(UserID);
            //Console.WriteLine(BookID);
            return BookStoreUser.CheckIfUserOwnsBook(UserID, BookID);
        }



        [HttpGet("GetAllTradableBooks")]

        public Object GetAllTradableBooks(int UserID)
        {
            return BookStoreUser.GetAllTradableBooks(UserID);
        }

        [HttpGet("getUserData")]
        public Object GetUserData(string userEmail)
        {
            return BookStoreUser.GetUserData(userEmail);
        }

        [HttpGet("getUserDataByID")]
        public Object getUserDataByID(int UserID)
        {
            return BookStoreUser.getUserDataByID(UserID);
        }

        [HttpGet("DoesBookUserConnectionExist")]
        public Boolean DoesBookUserConnectionExist(int UserID, int BookID)
        {
            if (UserID == 0)
            {
                return false;
            }
            return BookStoreUser.DoesBookUserConnectionExist(UserID, BookID);
        }

        [HttpGet("GetUserOwnedBooksAhHoc")]
        public Object GetUserOwnedBooksAhHoc(int UserID)
        {
            return BookStoreUser.GetUserOwnedBooksAhHoc(UserID);
        }

        [HttpGet("GetUserReadBooksAhHoc")]
        public Object GetUserReadBooksAhHoc(int UserID)
        {
            return BookStoreUser.GetUserReadBooksAhHoc(UserID);
        }

        [HttpGet("GetUserTradedBooksAdHoc")]
        public Object GetUserTradedBooksAdHoc(int UserID)
        {
            return BookStoreUser.GetUserTradedBooksAdHoc(UserID);
        }

        [HttpGet("GetUserWishlistedBooksAdHoc")]
        public Object GetUserWishlistedBooksAdHoc(int UserID)
        {
            return BookStoreUser.GetUserWishlistedBooksAdHoc(UserID);
        }

        [HttpGet("GetUserDataForAdminTable")]
        public IActionResult GetUserDataForAdminTable(int userID)
        {
            if (!BookStoreUser.IsUserAdmin(userID))
                return Forbid();
            return Ok(BookStoreUser.GetUserDataForAdminTable());
        }


        [HttpGet("CheckUserConnectionToBookForTrade")]

        public int CheckUserConnectionToBookForTrade(int UserID, int BookID)
        {
            return BookStoreUser.CheckUserConnectionToBookForTrade(UserID, BookID);
        }


        [HttpGet("DenyDuplicateTradeRequests")]
        public bool DenyDuplicateTradeRequests(int BookID, int UserIDInitiator, int UserIDRecipient)
        {
            return BookStoreUser.DenyDuplicateTradeRequests(BookID, UserIDInitiator, UserIDRecipient);  
        }

        [HttpGet("GetBookIDAndInitiatorIDFromBookTradesHistoryEntry")]
        public Object GetBookIDAndInitiatorIDFromBookTradesHistoryEntry(int TradeID)
        {
            return BookStoreUser.GetBookIDAndInitiatorIDFromBookTradesHistoryEntry(TradeID);
        }

        [HttpGet("DoesUserHaveUnreadNotifications")]
        public bool DoesUserHaveUnreadNotifications(int UserID)
        {
            return BookStoreUser.DoesUserHaveUnreadNotifications(UserID);
        }


        [HttpGet("GetTop10UsersByScore")]
        public Object GetTop10UsersByScore()
        {
            return BookStoreUser.GetTop10UsersByScore();
        }


        [HttpPost("SendTradeNotificationToOfferingUser")]
        public Task<int> SendTradeNotificationToOfferingUser([FromBody] JsonElement data)
        {
            int TradeID = data.GetProperty("TradeID").GetInt32();
            int NotificationRecipient = data.GetProperty("NotifRecipient").GetInt32();
            BookStoreUser tempUser = new BookStoreUser(_hubContext);

            return tempUser.SendTradeNotificationToOfferingUser(TradeID, NotificationRecipient);
        }

        [HttpPost("CreateBookTradesHistoryEntry")] ///////////////////////////////////
        public int CreateBookTradesHistoryEntry([FromBody] JsonElement data)
        {
            int BookID = data.GetProperty("BookID").GetInt32();
            int UserIDInitiator = data.GetProperty("UserIDInitiator").GetInt32();
            int UserIDRecipient = data.GetProperty("UserIDRecipient").GetInt32();
            return BookStoreUser.CreateBookTradesHistoryEntry(BookID, UserIDInitiator, UserIDRecipient);
        }

        [HttpPost("TradeConclusionNotification")]
        public Task<bool> TradeConclusionNotification(int TradeID)
        {
            //return BookStoreUser.TradeConclusionNotification(TradeID);

            BookStoreUser tempUser = new BookStoreUser(_hubContext);

            return tempUser.TradeConclusionNotification(TradeID);
        }

        [HttpPost("AddNewUserBookRelation")]
        public bool AddNewUserBookRelation([FromBody] JsonElement data)
        {
            //Console.WriteLine(data);
            int userId = data.GetProperty("UserID").GetInt32();
            int bookId = data.GetProperty("BookID").GetInt32();
            return BookStoreUser.AddNewUserBookRelation(userId, bookId);
        }

        [HttpPost("ToggleBookUserToWishlist")]
        public int ToggleBookUserToWishlist([FromBody] JsonElement data)
        {
            int userId = data.GetProperty("UserID").GetInt32();
            int bookId = data.GetProperty("BookID").GetInt32();
            return BookStoreUser.ToggleBookUserToWishlist(userId, bookId);
        }

        [HttpPost("RecordBookPurchaseInHistory")]
        public bool RecordBookPurchaseInHistory([FromBody] JsonElement data)
        {
            int userId = data.GetProperty("UserID").GetInt32();
            int bookId = data.GetProperty("BookID").GetInt32();
            return BookStoreUser.RecordBookPurchaseInHistory(userId, bookId);
        }

        [HttpPost("UserBookApplyOwnership")]
        public bool UserBookApplyOwnership(int UserID, int BookID)
        {
            return BookStoreUser.UserBookApplyOwnership(UserID, BookID);
        }


        [HttpPut("ChangeUserImage")]
        public bool ChangeUserImage([FromBody] JsonElement data)
        {
            //Console.WriteLine(data);
            int userId = data.GetProperty("UserID").GetInt32();
            string userImage = data.GetProperty("UserImage").GetString();
            return BookStoreUser.ChangeUserImage(userId, userImage);
        }

        [HttpPut("ChangeUserSignature")]
        public bool ChangeUserSignature([FromBody] JsonElement data)
        {
            //Console.WriteLine(data);
            int userId = data.GetProperty("UserID").GetInt32();
            string userSignature = data.GetProperty("UserSignature").GetString();
            return BookStoreUser.ChangeUserSignature(userId, userSignature);
        }

        [HttpPost("SendNewChatMessage")]
        public Object SendNewChatMessage([FromBody] JsonElement data)
        {
            int UserID = data.GetProperty("userID").GetInt32();
            int AuthorID = data.GetProperty("authorID").GetInt32();
            string MessageContent = data.GetProperty("MessageContent").GetString();
            BookStoreUser tempUser = new BookStoreUser(_hubContext);
            return tempUser.SendNewChatMessage(UserID, AuthorID, MessageContent);
        }


        // POST api/<BookStoreUserController>
        [HttpPost("registerUser")]
        public bool Post([FromBody] BookStoreUser user)
        {
            return user.CreateNewUser();
        }

        [HttpPost("SendForumMentionNotification")]
        public Task<bool> SendForumMentionNotification([FromBody] JsonElement data)
        {
            int AuthorID = data.GetProperty("authorID").GetInt32();
            int UserID = data.GetProperty("userID").GetInt32();
            int ResponseToPostID =  data.GetProperty("responseToPostID").GetInt32();

            BookStoreUser tempUser= new BookStoreUser(_hubContext);

            return tempUser.SendForumMentionNotification(AuthorID, UserID, ResponseToPostID);
        }

        [HttpPost("SendBookBoughtNotification")]
        public bool SendBookBoughtNotification([FromBody] JsonElement data)
        {
            int BookID = data.GetProperty("BookID").GetInt32();
            int UserID = data.GetProperty("UserID").GetInt32();
            return BookStoreUser.SendBookBoughtNotification(BookID,UserID);
        }




        [HttpPut("ChangeUserPassword")]
        public bool ChangeUserPassword([FromBody] JsonElement data)
        {
            //Console.WriteLine(data);
            int userId = data.GetProperty("UserID").GetInt32();
            string userPassword = data.GetProperty("UserPassword").GetString();

            //Console.WriteLine(userId);
            //Console.WriteLine(userPassword);
            return BookStoreUser.ChangeUserPassword(userId, userPassword);
        }



        [HttpPut("TradeOfferAccepted")]
        public bool TradeOfferAccepted(int TradeID)
        {
            return BookStoreUser.TradeOfferAccepted(TradeID);
        }

        [HttpPut("TradeOfferRejected")]
        public bool TradeOfferRejected(int TradeID)
        {
            return BookStoreUser.TradeOfferRejected(TradeID);
        }


        [HttpPut("UpdateUserScore")]
        public bool UpdateUserScore([FromBody] JsonElement data)
        {
            int UserID = data.GetProperty("UserID").GetInt32();
            int UserScore = data.GetProperty("UserScore").GetInt32();
            //Console.WriteLine(UserID);
            //Console.WriteLine(UserScore);
            return BookStoreUser.UpdateUserScore(UserID, UserScore);
        }


        // PUT api/<BookStoreUserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpPut("MarkBookAsRead")]
        public bool MarkBookAsRead([FromBody] JsonElement data)
        {
            int userId = data.GetProperty("UserID").GetInt32();
            int bookId = data.GetProperty("BookID").GetInt32();
            bool isRead = data.GetProperty("IsRead").GetBoolean();
            //Console.WriteLine(userId + " " + bookId + " " + isRead );
            return BookStoreUser.MarkBookAsRead(userId, bookId, isRead);
        }

        // int UserID, int BookID, int readStatus

        [HttpPut("UserBuyBook")]
        public bool UserBuyBook([FromBody] JsonElement data)
        {
            //Console.WriteLine(data);
            int userId = data.GetProperty("UserID").GetInt32();
            int bookId = data.GetProperty("BookID").GetInt32();
            
            return BookStoreUser.UserBuyBook(userId, bookId);
        }

        // DELETE api/<BookStoreUserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
