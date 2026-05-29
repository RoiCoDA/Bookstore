namespace BookStore.BL
{
    public class Book
    {
        string title;
        string subtitle;
        string description;
        string publisher;
        DateTime publishedDate;
        string printType;
        int pageCount;
        string previewLink;
        string maturityRating;
        string language;
        string infoLink;
        string isbn10;
        string isbn13;
        string smallThumbnail;
        string thumbnail;
        string canonicalVolumeLink;
        string selfLink;
        Boolean isEbook;
        string downloadLink;
        float price;
        int bookID;
        Boolean isActive;
        

        public Book() { }

        public Book(string title, string subtitle, string description, string publisher, DateTime publishedDate, string printType, int pageCount, string previewLink, string maturityRating, string language, string infoLink, string isbn10, string isbn13, string smallThumbnail, string thumbnail, string canonicalVolumeLink, string selfLink, bool isEbook, string downloadLink, float price, int bookID, bool isActive)
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.Publisher = publisher;
            this.PublishedDate = publishedDate;
            this.PrintType = printType;
            this.PageCount = pageCount;
            this.PreviewLink = previewLink;
            this.MaturityRating = maturityRating;
            this.Language = language;
            this.InfoLink = infoLink;
            this.Isbn10 = isbn10;
            this.Isbn13 = isbn13;
            this.SmallThumbnail = smallThumbnail;
            this.Thumbnail = thumbnail;
            this.CanonicalVolumeLink = canonicalVolumeLink;
            this.SelfLink = selfLink;
            this.IsEbook = isEbook;
            this.DownloadLink = downloadLink;
            this.Price = price;
            this.BookID = 0;
            this.IsActive = true;
        }


        static public Object GetAllDbBooksAdHoc()
        {
            DBservices dbs = new DBservices();
            return dbs.GetAllDbBooksAdHoc();
        }

        static public Object GetAllBookInfo(int id)
        {
            DBservices dbs = new DBservices();
            return dbs.GetAllBookInfo(id);
        }

        static public Object GetBookDataForAdminTable()
        {
            DBservices dbs = new DBservices();
            return dbs.GetBookDataForAdminTable();
        }

       

        static public Object GetBooksByTitle(string query)
        {
            DBservices dbs = new DBservices();
            return dbs.GetBooksByTitle(query);
        }

        static public Object GetBooksByWords(string query)
        {
            DBservices dbs = new DBservices();
            return dbs.GetBooksByWords(query);
        }

        static public bool CheckIfBookIsInDB(string title, string isbn_10, string isbn_13)
        {
            DBservices dbs = new DBservices();
            return dbs.CheckIfBookIsInDB(title, isbn_10, isbn_13);
        }

     

        static public Object GetBookQuestion_WhichDatePublished()
        {
            DBservices dbs = new DBservices();
            return dbs.GetBookQuestion_WhichDatePublished();
        }
        static public Object GetBookQuestion_WhichPublisher() 
        { 
            DBservices dbs = new DBservices();
            return dbs.GetBookQuestion_WhichPublisher();
        }
        static public Object GetBookQuestion_WhichPageCount() 
        {
            DBservices dbs = new DBservices();
            return dbs.GetBookQuestion_WhichPageCount();
        }

        static public Object GetBookQuestion_WhichAuthor() 
        {
            DBservices dbs = new DBservices();
            return dbs.GetBookQuestion_WhichAuthor();
        }

        static public Object GetAuthorQuestion_WhichAuthorImage() 
        {
            DBservices dbs = new DBservices();
            return dbs.GetAuthorQuestion_WhichAuthorImage();
        }

        static public float GetAverageRatingForBook(int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetAverageRatingForBook(BookID);
        }

        static public int AddNewBookToDB( string title,string subtitle,string description,string publisher,DateTime publishedDate,string printType,int pageCount,string previewLink,string maturityRating,string language,string infoLink,string ISBN_10,string ISBN_13,string smallThumbnail,string thumbnail,string canonicalVolumeLink,string selfLink,bool isEbook,string downloadLink,double price)
        {
            DBservices dbs = new DBservices ();
            return dbs.AddNewBookToDB(title, subtitle, description, publisher, publishedDate, printType, pageCount, previewLink, maturityRating, language, infoLink, ISBN_10, ISBN_13, smallThumbnail, thumbnail, canonicalVolumeLink, selfLink, isEbook, downloadLink, price);
        }

        static public bool InsertNewCategoryAndConnectToBook(string CategoryName, int BookID)
        {
            DBservices dbs = new DBservices();
            return dbs.InsertNewCategoryAndConnectToBook(CategoryName, BookID);   
        }

        static public Object GetFlashSaleBooks()
        {
            DBservices dbs = new DBservices();
            return dbs.GetFlashSaleBooks();
        }
        static public Object GetTop10UserOwnedBooks()
        {
            DBservices dbs = new DBservices();
            return dbs.GetTop10UserOwnedBooks();
        }

        public string Title { get => title; set => title = value; }
        public string Subtitle { get => subtitle; set => subtitle = value; }
        public string Description { get => description; set => description = value; }
        public string Publisher { get => publisher; set => publisher = value; }
        public DateTime PublishedDate { get => publishedDate; set => publishedDate = value; }
        public string PrintType { get => printType; set => printType = value; }
        public int PageCount { get => pageCount; set => pageCount = value; }
        public string PreviewLink { get => previewLink; set => previewLink = value; }
        public string MaturityRating { get => maturityRating; set => maturityRating = value; }
        public string Language { get => language; set => language = value; }
        public string InfoLink { get => infoLink; set => infoLink = value; }
        public string Isbn10 { get => isbn10; set => isbn10 = value; }
        public string Isbn13 { get => isbn13; set => isbn13 = value; }
        public string SmallThumbnail { get => smallThumbnail; set => smallThumbnail = value; }
        public string Thumbnail { get => thumbnail; set => thumbnail = value; }
        public string CanonicalVolumeLink { get => canonicalVolumeLink; set => canonicalVolumeLink = value; }
        public string SelfLink { get => selfLink; set => selfLink = value; }
        public bool IsEbook { get => isEbook; set => isEbook = value; }
        public string DownloadLink { get => downloadLink; set => downloadLink = value; }
        public float Price { get => price; set => price = value; }
        public int BookID { get => bookID; set => bookID = value; }
        public bool IsActive { get => isActive; set => isActive = value; }

        
    }
}
