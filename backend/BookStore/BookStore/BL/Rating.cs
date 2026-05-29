namespace BookStore.BL
{
    public class Rating
    {
        int userID;
        int bookID;
        int ratingStars; // number of stars
        string description;
        DateTime dateCreated;
        string header;

        

        public Rating(int userID, int bookID, int ratingStars, string description, DateTime dateCreated, string header)
        {
            this.UserID = userID;
            this.BookID = bookID;
            this.RatingStars = ratingStars;
            this.Description = description;
            DateCreated = dateCreated;
            this.Header = header;
        }

        public Rating() { }


        public static int InsertUserReview(Rating rating)
        {
            DBservices dbs = new DBservices();
            return dbs.InsertUserReview(rating);
        }
        static public int GetUserRatingReview(int userID, int ratingID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUserRatingReview(userID, ratingID);
        }

        static public int RateReview(int userID, int ratingID, int score)
        {
            DBservices dbs = new DBservices();
            return dbs.RateReview(userID, ratingID, score);
        }

        static public Object GetTopFiveRatingsReviews(int userID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetTopFiveRatingsReviews(userID);
        }

        static public Object GetTopFiveRatingsReviewsForBook(int userID, int bookID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetTopFiveRatingsReviewsForBook(userID, bookID);
        }

        static public Object GetUsersBookReview(int userID, int bookID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUsersBookReview(userID, bookID);
        }

        static public Object GetUsersAllReviews(int userID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUsersAllReviews(userID);
        }

        static public Object GetRatingWrittenDataForSentimentAnalysis(int bookID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetRatingWrittenDataForSentimentAnalysis(bookID);
        }

        static public float GetAverageRatingForBook(int BookID)
        {
            DBservices dbs = new DBservices ();
            return dbs.GetAverageRatingForBook(BookID);
        }

        public int UserID { get => userID; set => userID = value; }
        public int BookID { get => bookID; set => bookID = value; }
        public int RatingStars { get => ratingStars; set => ratingStars = value; }
        public string Description { get => description; set => description = value; }
        public DateTime DateCreated { get => dateCreated; set => dateCreated = value; }
        public string Header { get => header; set => header = value; }
    }
}
