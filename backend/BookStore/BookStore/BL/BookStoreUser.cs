using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Threading.Tasks;

namespace BookStore.BL
{
    public class BookStoreUser
    {
        string userName;
        string userPassword;
        string userEmail;
        DateTime dateCreated;
        string image;
        Boolean isActive;
        Boolean isAdmin;
        string signature;

        private readonly IHubContext<NotificationHub> _hubContext;

        public BookStoreUser(string userName, string userPassword, string userEmail, DateTime dateCreated, string image, bool isActive, bool isAdmin, string signature, IHubContext<NotificationHub> hubContext)
        {
            this.UserName = userName;
            this.UserPassword = userPassword;
            this.UserEmail = userEmail;
            this.DateCreated = dateCreated;
            this.Image = image;
            this.IsActive = isActive;
            this.IsAdmin = isAdmin;
            this.Signature = signature;
            this._hubContext = hubContext;
        }

        public BookStoreUser(IHubContext<NotificationHub> hubContext)
        {
            this._hubContext = hubContext;
        }

        public bool CreateNewUser()
        {
            DBservices dbs = new DBservices();
            return dbs.CreateNewUser(this);
        }

        static public Object GetUserData(string userEmail)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUserData(userEmail);
        }

        static public Object getUserDataByID(int UserID)
        {
            DBservices dbs = new DBservices();
            return dbs.getUserDataByID(UserID);
        }

        static public bool DoesBookUserConnectionExist(int UserID, int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.DoesBookUserConnectionExist(UserID, BookID);
        }

        static public bool AddNewUserBookRelation(int UserID, int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.AddNewUserBookRelation(UserID, BookID);
        }

        static public int ToggleBookUserToWishlist(int UserID, int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.ToggleBookUserToWishlist(UserID, BookID);
        }

        static public bool UserBookApplyOwnership(int UserID, int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.UserBookApplyOwnership(UserID, BookID);
        }

        static public Object GetUserOwnedBooksAhHoc(int UserID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUserOwnedBooksAdHoc(UserID);
        }

        static public Object GetUserReadBooksAhHoc(int UserID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUserReadBooksAhHoc(UserID);
        }

        static public bool MarkBookAsRead(int UserID, int BookID, bool readStatus)
        {
            DBservices dbs = new DBservices();
            return dbs.MarkBookAsRead(UserID, BookID, readStatus);
        }

        static public Object GetUserTradedBooksAdHoc(int UserID)
        {
            DBservices dbs = new DBservices ();
            return dbs.GetUserTradedBooksAdHoc(UserID);
        }

        static public Object GetUserWishlistedBooksAdHoc(int UserID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUserWishlistedBooksAdHoc(UserID);
        }

        static public bool UserBuyBook(int UserID, int BookID)
        {
            DBservices dbs = new DBservices ();
            return dbs.UserBuyBook(UserID, BookID);
        }
        static public Object GetAllTradableBooks(int UserID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetAllTradableBooks(UserID);
        }

        static public Object GetUserDataForAdminTable()
        {
            DBservices dbs = new DBservices();
            return dbs.GetUserDataForAdminTable();
        }

        static public bool ChangeUserImage(int userId, string userImage)
        {
            DBservices dbs = new DBservices();
            return dbs.ChangeUserImage(userId, userImage);
        }

        static public bool ChangeUserSignature(int userId, string userSignature)
        {
            DBservices dbs = new DBservices();
            return dbs.ChangeUserSignature(userId, userSignature);
        }

        static public bool ChangeUserPassword(int UserID, string UserPassword)
        {
            DBservices dbs = new DBservices();
            return dbs.ChangeUserPassword(UserID, UserPassword);
        }
        static public int CheckUserConnectionToBookForTrade(int UserID, int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.CheckUserConnectionToBookForTrade(UserID, BookID);
        }
        static public int CreateBookTradesHistoryEntry(int BookID, int UserIDInitiator, int UserIDRecipient)
        {
            DBservices dbs = new DBservices();
            return dbs.CreateBookTradesHistoryEntry( BookID, UserIDInitiator, UserIDRecipient);

        }

        static public bool DenyDuplicateTradeRequests(int BookID, int UserIDInitiator, int UserIDRecipient)
        {
            DBservices dbs = new DBservices();
            return dbs.DenyDuplicateTradeRequests(BookID, UserIDInitiator, UserIDRecipient);
        }
        public async Task<int> SendTradeNotificationToOfferingUser(int TradeID, int NotificationRecipient)
        {
            DBservices dbs = new DBservices();

            int NotifID = dbs.SendTradeNotificationToOfferingUser(TradeID, NotificationRecipient);
            if (NotifID != -1)
            {
                var NotificationCheckMessage = new
                {
                    @event = "Notification",
                    data = new { message = "Check Notifications", UserID = NotificationRecipient }
                };

                // Send the WebSocket message
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", NotificationCheckMessage);
            }
            return NotifID;
        }


        //static public bool TradeConclusionNotification(int TradeID, int NotificationRecipient)
        public async Task<bool> TradeConclusionNotification(int TradeID)
        {
            //DBservices dbs = new DBservices();
            //return dbs.TradeConclusionNotification(TradeID);

            DBservices dbs = new DBservices();

            var result = dbs.TradeConclusionNotification(TradeID);

            bool isSuccess = result.isSuccess;
            int NotificationRecipient = result.TradeInitiator;
            if (isSuccess == true)
            {
                var NotificationCheckMessage = new
                {
                    @event = "Notification",
                    data = new { message = "Check Notifications", UserID = NotificationRecipient }
                };

                // Send the WebSocket message
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", NotificationCheckMessage);
            }
            return isSuccess;
        }

        static public bool TradeOfferAccepted(int TradeID)
        {
            DBservices dbs = new DBservices();
            return dbs.TradeOfferAccepted(TradeID);
        }

        static public Object GetBookIDAndInitiatorIDFromBookTradesHistoryEntry(int TradeID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetBookIDAndInitiatorIDFromBookTradesHistoryEntry(TradeID);
        }

        static public bool CheckIfUserOwnsBook(int UserID, int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.CheckIfUserOwnsBook(UserID, BookID);
        }

        static public bool TradeOfferRejected(int TradeID)
        {
            DBservices dbs = new DBservices();
            return dbs.TradeOfferRejected(TradeID);
        }

        public async Task<bool> SendForumMentionNotification(int AuthorID, int UserID, int ResponseToPostID)
        {
            DBservices dbs = new DBservices();
            var result = dbs.SendForumMentionNotification(AuthorID, UserID, ResponseToPostID);


            bool isSuccess = result.isSuccess;
            int? NotificationRecipient = result.originalPostUserID;
            if (isSuccess == true)
            {
                if (NotificationRecipient != null)
                {
                    var NotificationCheckMessage = new
                    {
                        @event = "Notification",
                        data = new { message = "Check Notifications", UserID = NotificationRecipient }
                    };

                    // Send the WebSocket message
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", NotificationCheckMessage);
                }
            }
            //Console.WriteLine("result from user" + result);
            //Console.WriteLine("isSuccess from user" + isSuccess);
            return isSuccess;
        }

        static public bool SendBookBoughtNotification(int BookID, int UserID)
        {
            DBservices dbs = new DBservices();
            return dbs.SendBookBoughtNotification(BookID, UserID);  
        }

        static public bool RecordBookPurchaseInHistory(int UserID, int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.RecordBookPurchaseInHistory(UserID, BookID);
        }

        static public bool IsUserAdmin(int userID)
        {
            DBservices dbs = new DBservices();
            return dbs.IsUserAdmin(userID);
        }

        static public bool DoesUserHaveUnreadNotifications(int UserID)
        {
            DBservices dbs = new DBservices();
            return dbs.DoesUserHaveUnreadNotifications(UserID);
        }

        static public bool UpdateUserScore(int UserID, int UserScore)
        {
            DBservices dbs = new DBservices();
            return dbs.UpdateUserScore(UserID, UserScore);
        }

        static public Object GetTop10UsersByScore()
        {
            DBservices dbs = new DBservices();
            return dbs.GetTop10UsersByScore();
        }

        public async Task<Object> SendNewChatMessage(int UserID, int AuthorID, string MessageContent)
        {
            DBservices dbs = new DBservices();
            var result = dbs.SendNewChatMessage(UserID, AuthorID, MessageContent);

            dynamic message = result;
            var ChatMessage = new
            {
                @event = "ChatMessage",
                data = new { message = message.NewMessageID, authorID = AuthorID }
            };
            //Console.WriteLine("result: " + result);
            //Console.WriteLine("message: " + message);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", ChatMessage);
            return result;
        }

        public BookStoreUser() { }

        public string UserName { get => userName; set => userName = value; }
        public string UserPassword { get => userPassword; set => userPassword = value; }
        public string UserEmail { get => userEmail; set => userEmail = value; }
        public DateTime DateCreated { get => dateCreated; set => dateCreated = value; }
        public string Image { get => image; set => image = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }
        public string Signature { get => signature; set => signature = value; }
    }

}
