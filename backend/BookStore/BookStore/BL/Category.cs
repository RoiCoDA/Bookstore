namespace BookStore.BL
{
    public class Category
    {
        string CategoryName;

        public Category(string categoryName)
        {
            CategoryName1 = categoryName;
        }

        public Category() { }

        public string CategoryName1 { get => CategoryName; set => CategoryName = value; }
    }
}
