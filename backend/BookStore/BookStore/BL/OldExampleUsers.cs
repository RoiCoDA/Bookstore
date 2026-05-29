//using EX1.BL;
//using System.Net;

//namespace Server_Side.BL
//{
//    public class Users
//    {
//        int id;
//        string name;
//        string email;
//        string password;
//        bool isAdmin;
//        bool isActive;


//        public Users(int id, string name, string email, string password, bool isAdmin, bool isActive)
//        {
//            this.Id = id;
//            this.Name = name;
//            this.Email = email;
//            this.Password = password;
//            this.IsAdmin = isAdmin;
//            this.IsActive = isActive;
//        }

//        public Users(Users user)
//        {
//            Id = user.Id;
//            Name = user.Name;
//            Email = user.Email;
//            Password = user.Password;
//            IsAdmin = user.IsAdmin;
//            IsActive = user.IsActive;
//        }

//        public Users() { }

//        public static List<Users> Read()
//        {
//            DBservices dbs = new DBservices();
//            return dbs.ReadUsers();
//        }

//        public bool Register() // V
//        {
//            this.IsAdmin = false;
//            this.IsActive = true;
//            DBservices dbs = new DBservices();
//            return dbs.InsertUser(this);
//        }

//        public static Users Login(string email, string password) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.UserLogin(email, password);
//        }

//        public static Users DoesUserExist(string email) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.DoesUserExist(email);
//        }

//        public static bool InsertCourseToUsersList(int userId, int courseId) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.UserAddCourse(userId, courseId);
//        }

//        public static bool DeleteCourseFromUsersList(int userId, int courseId) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.DeleteCourseOfUser(userId, courseId);
//        }

//        public static List<Course> GetCoursesByName(string partOfName, int userId) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.GetUserCoursesByName(partOfName, userId);
//        }


//        public static List<Course> GetUsersCourseList(int userId) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.GetCoursesOfUser(userId);
//        }

//        public static bool CourseExistsInUsersList(int userId, int courseId) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.UserCheckIfCourseExists(userId, courseId);
//        }

//        public static List<Course> getDurationRange(float from, float to, int userId) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.FilterCoursesByDuration(from, to, userId);
//        }

//        public static List<Course> getRatingRange(float from, float to, int userId) // V
//        {
//            DBservices dbs = new DBservices();
//            return dbs.FilterCoursesByRating(from, to, userId);
//        }

//        public int Id { get => id; set => id = value; }
//        public string Name { get => name; set => name = value; }
//        public string Email { get => email; set => email = value; }
//        public string Password { get => password; set => password = value; }
//        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }
//        public bool IsActive { get => isActive; set => isActive = value; }

//    }


//}
