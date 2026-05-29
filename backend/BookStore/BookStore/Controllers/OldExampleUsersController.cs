//using ex1.bl;
//using microsoft.aspnetcore.mvc;
//using server_side.bl;
////using static server_side.controllers.userscontroller;

//// for more information on enabling web api for empty projects, visit https://go.microsoft.com/fwlink/?linkid=397860

//namespace server_side.controllers
//{
//    [route("api/[controller]")]
//    [apicontroller]
//    public class userscontroller : controllerbase
//    {

//        // get: api/<userscontroller>
//        [httpget]
//        public list<users> get() // v
//        {
//            return users.read();
//        }


//        [httpget("usercourselist")] // check happens on the client side
//        public list<course> getuserscourselist(int userid) // v
//        {
//            return users.getuserscourselist(userid);
//        }


//        // get api/<userscontroller>/5
//        [httpget("{id}")]
//        public string get(int id) // get user by id
//        {
//            return "value";
//        }


//        // post api/<userscontroller>
//        [httppost("usersignup")]
//        public iactionresult post([frombody] users user) // v
//        {
//            if (users.doesuserexist(user.email) == null)
//            {
//                return badrequest(new { message = "user already exists!" });
//            }

//            if (user.register() == false)
//            {
//                return badrequest(new { message = "registering user failed!" });
//            }
//            return ok(new { message = "register is a success!" });
//        }

//        [httpget("courseexistsinuserslist/userid/{userid}/courseid/{courseid}")]
//        public bool courseexistsinuserslist(int userid, int courseid)
//        {
//            return users.courseexistsinuserslist(userid, courseid);
//        }

//        [httpget("searchstring/{searchstring}/id/{userid}")]
//        public list<course> getusercoursesbyname(string searchstring, int userid) // v
//        {
//            return users.getcoursesbyname(searchstring, userid);
//        }



//        [httppost("addcoursetouser/userid/{userid}/courseid/{courseid}")]
//        public bool post(int userid, int courseid) // v
//        {
//            return users.insertcoursetouserslist(userid, courseid);
//        }

//        // put api/<userscontroller>/5
//        [httpput("{id}")]
//        public void put(int id, [frombody] string value)
//        {

//        }

//        [httpput("login")] // uses put method to login
//        public iactionresult login([frombody] userdata usercreds) // v
//        {
//            if (string.isnullorempty(usercreds.email) || string.isnullorempty(usercreds.password))
//            {
//                return badrequest("please enter both an email address and a password.");
//            }

//            //users tempuser = users.doesuserexist(usercreds.email);

//            if (users.doesuserexist(usercreds.email) == null)
//            {
//                return badrequest(new { message = "user email not registered!" });
//            }
//            users tempuser = users.login(usercreds.email, usercreds.password);
//            if (tempuser.email == null)
//            {
//                return badrequest(new { message = "invalid email or password!" });
//            }
//            return ok(new { message = "success!", user = tempuser });
//        }


//        [httpget("searchbyrouting/ratingfrom/{from}/ratingto/{to}/{userid}")]
//        public list<course> getbyratingrange(float from, float to, int userid) // v
//        {
//            return users.getratingrange(from, to, userid);

//        }

//        [httpget("searchbyquerystring/filtersbyduration")]
//        public list<course> getbydurationrange(float from, float to, int userid) // v
//        {
//            return users.getdurationrange(from, to, userid);
//        }

//        [httpdelete("userid/{userid}/courseid/{courseid}")] // checks happen on client side
//        public bool deletecoursebyidfromuserslist(int userid, int courseid) // v
//        {
//            return users.deletecoursefromuserslist(userid, courseid);
//        }
//    }
//}
