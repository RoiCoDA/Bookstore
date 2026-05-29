// Supposed book from Google API that also contains author and categories which we will later split

namespace BookStore.BL
{
    public class BookHelper
    {
        string[] authors;
        string[] categories;
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
        string isnb13;
        string smallThumbnail;
        string thumbnail;
        string canonicalVolumeLink;
        string selfLink;
        Boolean isEbook;
        string downloadLink;
        float price;

        public BookHelper(string[] authors, string[] categories, string title, string subtitle, string description, string publisher, DateTime publishedDate, string printType, int pageCount, string previewLink, string maturityRating, string language, string infoLink, string isbn10, string isnb13, string smallThumbnail, string thumbnail, string canonicalVolumeLink, string selfLink, bool isEbook, string downloadLink, float price)
        {
            this.Authors = authors;
            this.Categories = categories;
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
            this.Isnb13 = isnb13;
            this.SmallThumbnail = smallThumbnail;
            this.Thumbnail = thumbnail;
            this.CanonicalVolumeLink = canonicalVolumeLink;
            this.SelfLink = selfLink;
            this.IsEbook = isEbook;
            this.DownloadLink = downloadLink;
            this.Price = price;
        }

        public BookHelper() { }

        public string[] Authors { get => authors; set => authors = value; }
        public string[] Categories { get => categories; set => categories = value; }
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
        public string Isnb13 { get => isnb13; set => isnb13 = value; }
        public string SmallThumbnail { get => smallThumbnail; set => smallThumbnail = value; }
        public string Thumbnail { get => thumbnail; set => thumbnail = value; }
        public string CanonicalVolumeLink { get => canonicalVolumeLink; set => canonicalVolumeLink = value; }
        public string SelfLink { get => selfLink; set => selfLink = value; }
        public bool IsEbook { get => isEbook; set => isEbook = value; }
        public string DownloadLink { get => downloadLink; set => downloadLink = value; }
        public float Price { get => price; set => price = value; }
    }
}
