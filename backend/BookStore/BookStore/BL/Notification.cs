namespace BookStore.BL
{
    public class Notification
    {
        string userID;
        string tradeID;
        string description;
        DateTime dateCreated;
        Boolean notificationRead;
        string notificationType;

        public Notification(string userID, string tradeID, string description, DateTime dateCreated, bool notificationRead, string notificationType)
        {
            this.UserID = userID;
            this.TradeID = tradeID;
            this.Description = description;
            this.DateCreated = dateCreated;
            this.NotificationRead = notificationRead;
            this.NotificationType = notificationType;
        }

        public Notification() { }

        static public Object GetAllUserNotifications(int userID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetAllUserNotifications(userID);
        }

        static public bool MarkNotificationAsRead(int NotificationID)
        {
            DBservices dbs = new DBservices();
            return dbs.MarkNotificationAsRead(NotificationID);
        }

        static public bool DeleteNotification(int NotificationID)
        {
            DBservices dbs = new DBservices();
            return dbs.DeleteNotification(NotificationID);
        }


        public string UserID { get => userID; set => userID = value; }
        public string TradeID { get => tradeID; set => tradeID = value; }
        public string Description { get => description; set => description = value; }
        public DateTime DateCreated { get => dateCreated; set => dateCreated = value; }
        public bool NotificationRead { get => notificationRead; set => notificationRead = value; }
        public string NotificationType { get => notificationType; set => notificationType = value; }
    }
}
