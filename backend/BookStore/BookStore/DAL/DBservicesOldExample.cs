//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Data.SqlClient;
//using System.Data;
//using System.Text;
//using EX1.BL;
//using Server_Side.BL;
//using Microsoft.AspNetCore.Http;
//using System.Globalization;

/// < summary >
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
//public class DBservicesz
//{

//    public DBservices()
//    {

//    TODO: Add constructor logic here


//    }

//    --------------------------------------------------------------------------------------------------
//     Creates a connection to the database according to the connectionString name in the web.config
//    --------------------------------------------------------------------------------------------------
//    public SqlConnection connect(String conString)
//    {

//        read the connection string from the configuration file
//       IConfigurationRoot configuration = new ConfigurationBuilder()
//       .AddJsonFile("appsettings.json").Build();
//        string cStr = configuration.GetConnectionString("myProjDB");

//        SqlConnection con = new SqlConnection(cStr);
//        con.Open();
//        return con;
//    }

//    --------------------------------------------------------------------------------------------------
//     InsertFlight
//    --------------------------------------------------------------------------------------------------
//    public int InsertFlight(Flight flight)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        cmd = BuildInsertCommand("SP_InsertFlight1", con, flight); // create the command

//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }

//    ---------------------------------------------------------------------------------
//     InsertFlight uses this method to build the insert command
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildInsertCommand(String spName, SqlConnection con, Flight flight)
//    {

//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@from", flight.From);
//        cmd.Parameters.AddWithValue("@to", flight.To);
//        cmd.Parameters.AddWithValue("@price", flight.Price);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     DeleteFlight
//    --------------------------------------------------------------------------------------------------
//    public int DeleteFlight(Flight flight)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        cmd = BuildDeleteCommand("SP_DeleteFlight1", con, flight); // create the command

//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }

//    ---------------------------------------------------------------------------------
//     DeleteFlight uses this method to build the delete command
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildDeleteCommand(String spName, SqlConnection con, Flight flight)
//    {

//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@id", flight.Id);
//        return cmd;
//    }



//    --------------------------------------------------------------------------------------------------
//     ReadFlights
//    --------------------------------------------------------------------------------------------------
//    public List<Flight> ReadFlights()
//    {
//        SqlConnection con = null;
//        List<Flight> flightsList = new List<Flight>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildReadFlightsCommand("SP_ReadFlights", con); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Flight flight = new Flight();
//                flight.Id = Convert.ToInt32(dr["Id"]);
//                flight.From = (string)dr["From"];
//                flight.To = (string)dr["To"];
//                flight.Price = Convert.ToDouble(dr["Price"]);

//                flightsList.Add(flight);
//            }

//            return flightsList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }

//    ---------------------------------------------------------------------------------
//     ReadFlights uses this method to build the delete command
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildReadFlightsCommand(String spName, SqlConnection con)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        return cmd;
//    }


























































//    ---------------------------------------------------------------------------------
//     Read Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildReadCommand(String spName, SqlConnection con)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        return cmd;
//    }


//    --------------------------------------------------------------------------------------------------
     
//     *Instructor*
    
//    --------------------------------------------------------------------------------------------------





//    --------------------------------------------------------------------------------------------------
//     InsertInstructor
//    --------------------------------------------------------------------------------------------------
//    public bool InsertInstructor(Instructor instructor)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        cmd = BuildInsertInstructorCommand("SP_InsertInstructor", con, instructor); // create the command

//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     InsertInstructor Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildInsertInstructorCommand(String spName, SqlConnection con, Instructor instructor)
//    {

//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@title", instructor.Title);
//        cmd.Parameters.AddWithValue("@name", instructor.Name);
//        cmd.Parameters.AddWithValue("@jobTitle", instructor.JobTitle);
//        cmd.Parameters.AddWithValue("@image", instructor.Image);
//        return cmd;
//    }


//    --------------------------------------------------------------------------------------------------
//     DoesInstructorExist
//    --------------------------------------------------------------------------------------------------
//    public Instructor DoesInstructorExist(int instructorId)
//    {
//        SqlConnection con = null;
//        Instructor instructor = new Instructor();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildDoesInstructorExistCommand("SP_DoesInstructorExist", con, instructorId); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                instructor.Id = Convert.ToInt32(dr["Id"]);
//                instructor.Title = (string)dr["Title"];
//                instructor.Name = (string)dr["Name"];
//                instructor.Image = (string)dr["Image"];
//                instructor.JobTitle = (string)dr["JobTitle"];
//            }

//            return instructor;

//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     DoesInstructorExist Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildDoesInstructorExistCommand(String spName, SqlConnection con, int instructorId)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text


//        cmd.Parameters.AddWithValue("@InstructorId", instructorId);
//        return cmd;
//    }


//    --------------------------------------------------------------------------------------------------
//     ReadInstructors
//    --------------------------------------------------------------------------------------------------
//    public List<Instructor> ReadInstructors()
//    {
//        SqlConnection con = null;
//        List<Instructor> instructorsList = new List<Instructor>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildReadCommand("SP_ReadInstructors", con); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Instructor instructor = new Instructor();
//                instructor.Id = Convert.ToInt32(dr["Id"]);
//                instructor.Title = (string)dr["Title"];
//                instructor.Name = (string)dr["Name"];
//                instructor.Image = (string)dr["Image"];
//                instructor.JobTitle = (string)dr["JobTitle"];

//                instructorsList.Add(instructor);
//            }

//            return instructorsList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }

//    --------------------------------------------------------------------------------------------------
//     GetInstructorsByName
//    --------------------------------------------------------------------------------------------------
//    public List<Instructor> GetInstructorsByName(string partOfName)
//    {
//        SqlConnection con = null;
//        List<Instructor> instructorsList = new List<Instructor>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildGetInstructorsByNameCommand("SP_GetInstuctorsByName", con, partOfName); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Instructor instructor = new Instructor();
//                instructor.Id = Convert.ToInt32(dr["Id"]);
//                instructor.Title = (string)dr["Title"];
//                instructor.Name = (string)dr["Name"];
//                instructor.Image = (string)dr["Image"];
//                instructor.JobTitle = (string)dr["JobTitle"];

//                instructorsList.Add(instructor);
//            }

//            return instructorsList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }

//    ---------------------------------------------------------------------------------
//     GetInstructorsByName Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildGetInstructorsByNameCommand(String spName, SqlConnection con, string partOfName)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text


//        cmd.Parameters.AddWithValue("@name", partOfName);
//        return cmd;
//    }


//    --------------------------------------------------------------------------------------------------
     
//     *Course*
    
//    --------------------------------------------------------------------------------------------------










//    --------------------------------------------------------------------------------------------------
//     FilterCoursesByDuration
//    --------------------------------------------------------------------------------------------------
//    public List<Course> FilterCoursesByDuration(float from, float to, int userId)
//    {
//        SqlConnection con = null;
//        List<Course> courseList = new List<Course>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildFilterCommand("SP_CourseFilterDuration", con, from, to, userId); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Course course = new Course();
//                course.Id = Convert.ToInt32(dr["id"]);
//                course.Title = (string)dr["title"];
//                course.Url = (string)dr["url"];
//                course.Rating = Convert.ToSingle(dr["rating"]);
//                course.NumberOfReviews = Convert.ToInt32(dr["num_reviews"]);
//                course.InstructorId = Convert.ToInt32(dr["instructors_id"]); ;
//                course.ImageReference = (string)dr["image"];
//                course.Duration = Convert.ToSingle(dr["duration"]);
//                course.LastUpdate = Convert.ToDateTime(dr["last_update_date"]);
//                course.IsActive = Convert.ToBoolean(dr["isActive"]);
//                string format = "M/d/yyyy";
//                course.LastUpdate = DateTime.ParseExact((string)dr["last_update_date"], format, CultureInfo.InvariantCulture);
//                courseList.Add(course);

//            }

//            return courseList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }


//    --------------------------------------------------------------------------------------------------
//     FilterCoursesByRating
//    --------------------------------------------------------------------------------------------------
//    public List<Course> FilterCoursesByRating(float from, float to, int userId)
//    {
//        SqlConnection con = null;
//        List<Course> courseList = new List<Course>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildFilterCommand("SP_CourseFilterRating", con, from, to, userId); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Course course = new Course();
//                course.Id = Convert.ToInt32(dr["id"]);
//                course.Title = (string)dr["title"];
//                course.Url = (string)dr["url"];
//                course.Rating = Convert.ToSingle(dr["rating"]);
//                course.NumberOfReviews = Convert.ToInt32(dr["num_reviews"]);
//                course.InstructorId = Convert.ToInt32(dr["instructors_id"]); ;
//                course.ImageReference = (string)dr["image"];
//                course.Duration = Convert.ToSingle(dr["duration"]);
//                course.LastUpdate = Convert.ToDateTime(dr["last_update_date"]);
//                course.IsActive = Convert.ToBoolean(dr["isActive"]);
//                string format = "M/d/yyyy";
//                course.LastUpdate = DateTime.ParseExact((string)dr["last_update_date"], format, CultureInfo.InvariantCulture);
//                courseList.Add(course);
//            }

//            return courseList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     FilterCourses Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildFilterCommand(String spName, SqlConnection con, float from, float to, int userId)
//    {

//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@from", from);
//        cmd.Parameters.AddWithValue("@to", to);
//        cmd.Parameters.AddWithValue("@userId", userId);
//        return cmd;
//    }


//    --------------------------------------------------------------------------------------------------
//     UpdateCourse
//    --------------------------------------------------------------------------------------------------
//    public bool UpdateCourse(Course course)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        cmd = BuildUpdateCourseCommand("SP_UpdateCourse", con, course); // create the command

//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     UpdateCourse Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildUpdateCourseCommand(String spName, SqlConnection con, Course course)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@id", course.Id);
//        cmd.Parameters.AddWithValue("@title", course.Title);
//        cmd.Parameters.AddWithValue("@url", course.Url);
//        cmd.Parameters.AddWithValue("@rating", course.Rating);
//        cmd.Parameters.AddWithValue("@num_reviews", course.NumberOfReviews);
//        cmd.Parameters.AddWithValue("@last_update_date", course.LastUpdate);
//        cmd.Parameters.AddWithValue("@duration", course.Duration);
//        cmd.Parameters.AddWithValue("@instructors_id", course.InstructorId);
//        cmd.Parameters.AddWithValue("@image", course.ImageReference);
//        cmd.Parameters.AddWithValue("@isActive", course.IsActive);
//        return cmd;
//    }




//    --------------------------------------------------------------------------------------------------
//     InsertCourse
//    --------------------------------------------------------------------------------------------------
//    public bool InsertCourse(Course course)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        cmd = BuildInsertCourseCommand("SP_InsertCourse", con, course); // create the command

//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     InsertCourse Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildInsertCourseCommand(String spName, SqlConnection con, Course course)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@title", course.Title);
//        cmd.Parameters.AddWithValue("@url", course.Url);
//        cmd.Parameters.AddWithValue("@rating", course.Rating);
//        cmd.Parameters.AddWithValue("@num_reviews", course.NumberOfReviews);
//        cmd.Parameters.AddWithValue("@last_update_date", course.LastUpdate);
//        cmd.Parameters.AddWithValue("@duration", course.Duration);
//        cmd.Parameters.AddWithValue("@instructors_id", course.InstructorId);
//        cmd.Parameters.AddWithValue("@image", course.ImageReference);
//        cmd.Parameters.AddWithValue("@isActive", course.IsActive);
//        return cmd;
//    }



//    --------------------------------------------------------------------------------------------------
//     DoesCourseExist
//    --------------------------------------------------------------------------------------------------
//    public Course DoesCourseExist(int courseId)
//    {
//        SqlConnection con = null;
//        Course course = new Course();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildDoesCourseExistCommand("SP_DoesCourseExist", con, courseId); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                course.Id = Convert.ToInt32(dr["id"]);
//                course.Title = (string)dr["title"];
//                course.Url = (string)dr["url"];
//                course.Rating = Convert.ToSingle(dr["rating"]);
//                course.NumberOfReviews = Convert.ToInt32(dr["num_reviews"]);
//                course.InstructorId = Convert.ToInt32(dr["instructors_id"]); ;
//                course.ImageReference = (string)dr["image"];
//                course.Duration = Convert.ToSingle(dr["duration"]);
//                course.LastUpdate = Convert.ToDateTime(dr["last_update_date"]);
//                course.IsActive = Convert.ToBoolean(dr["isActive"]);
//                string format = "M/d/yyyy";
//                course.LastUpdate = DateTime.ParseExact((string)dr["last_update_date"], format, CultureInfo.InvariantCulture);
//            }

//            return course;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     DoesCourseExist Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildDoesCourseExistCommand(String spName, SqlConnection con, int courseId)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@CourseId", courseId);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     ReadCourses
//    --------------------------------------------------------------------------------------------------
//    public List<Course> ReadCourses()
//    {
//        SqlConnection con = null;
//        List<Course> courseList = new List<Course>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildReadCommand("SP_ReadAllCourses", con); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Course course = new Course();
//                course.Id = Convert.ToInt32(dr["id"]);
//                course.Title = (string)dr["title"];
//                course.Url = (string)dr["url"];
//                course.Rating = Convert.ToSingle(dr["rating"]);
//                course.NumberOfReviews = Convert.ToInt32(dr["num_reviews"]);
//                course.InstructorId = Convert.ToInt32(dr["instructors_id"]); ;
//                course.ImageReference = (string)dr["image"];
//                course.Duration = Convert.ToSingle(dr["duration"]);
//                course.LastUpdate = Convert.ToDateTime(dr["last_update_date"]);
//                course.IsActive = Convert.ToBoolean(dr["isActive"]);
//                string format = "M/d/yyyy";
//                course.LastUpdate = DateTime.ParseExact((string)dr["last_update_date"], format, CultureInfo.InvariantCulture);
//                courseList.Add(course);
//            }

//            return courseList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }

//    --------------------------------------------------------------------------------------------------
//     DeleteCourse 
//    --------------------------------------------------------------------------------------------------
//    public bool DeleteCourse(int courseId)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        cmd = BuildDeleteCourseCommand("SP_DeleteCourse", con, courseId); // create the command
//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     DeleteCourse Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildDeleteCourseCommand(String spName, SqlConnection con, int courseId)
//    {

//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@id", courseId);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     GetCoursesByName
//    --------------------------------------------------------------------------------------------------
//    public List<Course> GetCoursesByName(string partOfName)
//    {
//        SqlConnection con = null;
//        List<Course> courseList = new List<Course>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildGetCoursesByNameCourseCommand("SP_GetCoursesByName", con, partOfName); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Course course = new Course();
//                course.Id = Convert.ToInt32(dr["id"]);
//                course.Title = (string)dr["title"];
//                course.Url = (string)dr["url"];
//                course.Rating = Convert.ToSingle(dr["rating"]);
//                course.NumberOfReviews = Convert.ToInt32(dr["num_reviews"]);
//                course.InstructorId = Convert.ToInt32(dr["instructors_id"]); ;
//                course.ImageReference = (string)dr["image"];
//                course.Duration = Convert.ToSingle(dr["duration"]);
//                course.LastUpdate = Convert.ToDateTime(dr["last_update_date"]);
//                course.IsActive = Convert.ToBoolean(dr["isActive"]);
//                string format = "M/d/yyyy";
//                course.LastUpdate = DateTime.ParseExact((string)dr["last_update_date"], format, CultureInfo.InvariantCulture);
//                courseList.Add(course);
//            }

//            return courseList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     GetCoursesByName Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildGetCoursesByNameCourseCommand(String spName, SqlConnection con, string partOfName)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@title", partOfName);
//        return cmd;
//    }


//    ---------------------------------------------------------------------------------
//     AD-Hoc 
//    ---------------------------------------------------------------------------------

//    public Object TopFiveCourses()
//    {

//        SqlConnection con = null;
//        List<Object> topFiveCourses = new List<Object>();

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildReadCommand("SP_TopFiveCourses", con); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                topFiveCourses.Add(new
//                {
//                    CourseId = Convert.ToInt32(dr["CourseID"]),
//                    CourseTitle = (string)dr["title"],
//                    CourseRating = Convert.ToSingle(dr["rating"]),
//                    NumOfRegisters = Convert.ToInt32(dr["RegistrationCount"])
//                });
//            }
//            return topFiveCourses;

//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }


//    ---------------------------------------------------------------------------------
//     UpdateCourseTitleInDatatable
//    ---------------------------------------------------------------------------------

//    public bool UpdateCourseTitleInDatatable(int courseId, string courseTitle)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        cmd = BuildUpdateCourseTitleInDatatableCommand("SP_UpdateCourseTitleInDatatable", con, courseId, courseTitle); // create the command

//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     UpdateCourseTitleInDatatable Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildUpdateCourseTitleInDatatableCommand(String spName, SqlConnection con, int courseId, string courseTitle)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@id", courseId);
//        cmd.Parameters.AddWithValue("@title", courseTitle);
//        return cmd;
//    }



//    ---------------------------------------------------------------------------------
//     UpdateCourseIsActiveInDatatable
//    ---------------------------------------------------------------------------------
//    public bool UpdateCourseIsActiveInDatatable(int courseId, bool isActive)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        cmd = BuildUpdateCourseIsActiveInDatatableCommand("SP_UpdateCourseIsActiveInDatatable", con, courseId, isActive); // create the command

//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     UpdateCourseIsActiveInDatatable Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildUpdateCourseIsActiveInDatatableCommand(String spName, SqlConnection con, int courseId, bool isActive)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@id", courseId);
//        cmd.Parameters.AddWithValue("@isActive", isActive);
//        return cmd;
//    }



//    --------------------------------------------------------------------------------------------------
     
//     *User*
    
//    --------------------------------------------------------------------------------------------------







//    --------------------------------------------------------------------------------------------------
//     InsertUser
//    --------------------------------------------------------------------------------------------------
//    public bool InsertUser(Users user)
//    {
//        SqlConnection con;
//        SqlCommand cmd;
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        cmd = BuildInsertUserCommand("SP_InsertUser", con, user); // create the commandDoesBookUserConnectionExist

//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     InsertUser Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildInsertUserCommand(String spName, SqlConnection con, Users user)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@name", user.Name);
//        cmd.Parameters.AddWithValue("@email", user.Email);
//        cmd.Parameters.AddWithValue("@password", user.Password);
//        cmd.Parameters.AddWithValue("@isAdmin", user.IsAdmin);
//        cmd.Parameters.AddWithValue("@isActive", user.IsActive);
//        return cmd;
//    }


//    --------------------------------------------------------------------------------------------------
//     DoesUserExist
//    --------------------------------------------------------------------------------------------------
//    public Users DoesUserExist(string email)
//    {
//        SqlConnection con = null;
//        Users user = new Users();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildDoesUserExistCommand("SP_ReadUserByEmail", con, email); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                user.Id = Convert.ToInt32(dr["id"]);
//                user.Name = (string)dr["name"];
//                user.Email = (string)dr["email"];
//                user.Password = (string)dr["password"];
//                user.IsAdmin = Convert.ToBoolean(dr["isAdmin"]);
//                user.IsActive = Convert.ToBoolean(dr["isActive"]);
//            }

//            return user;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     DoesUserExist Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildDoesUserExistCommand(String spName, SqlConnection con, string email)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@email", email);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     ReadUsers
//    --------------------------------------------------------------------------------------------------
//    public List<Users> ReadUsers()
//    {
//        SqlConnection con = null;
//        List<Users> usersList = new List<Users>();

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildReadCommand("SP_ReadUser", con); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Users user = new Users();
//                user.Id = Convert.ToInt32(dr["id"]);
//                user.Name = (string)dr["name"];
//                user.Email = (string)dr["email"];
//                user.Password = (string)dr["password"];
//                user.IsAdmin = Convert.ToBoolean(dr["isAdmin"]);
//                user.IsActive = Convert.ToBoolean(dr["isActive"]);
//                usersList.Add(user);
//            }

//            return usersList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }


//    --------------------------------------------------------------------------------------------------
//     GetCoursesOfUser
//    --------------------------------------------------------------------------------------------------
//    public List<Course> GetCoursesOfUser(int id)
//    {
//        SqlConnection con = null;
//        List<Course> courseList = new List<Course>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildGetCoursesOfUserCommand("SP_GetCoursesOfUser", con, id); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Course course = new Course();
//                course.Id = Convert.ToInt32(dr["id"]);
//                course.Title = (string)dr["title"];
//                course.Url = (string)dr["url"];
//                course.Rating = Convert.ToSingle(dr["rating"]);
//                course.NumberOfReviews = Convert.ToInt32(dr["num_reviews"]);
//                course.InstructorId = Convert.ToInt32(dr["instructors_id"]); ;
//                course.ImageReference = (string)dr["image"];
//                course.Duration = Convert.ToSingle(dr["duration"]);
//                course.LastUpdate = Convert.ToDateTime(dr["last_update_date"]);
//                course.IsActive = Convert.ToBoolean(dr["isActive"]);
//                string format = "M/d/yyyy";
//                course.LastUpdate = DateTime.ParseExact((string)dr["last_update_date"], format, CultureInfo.InvariantCulture);
//                courseList.Add(course);
//            }

//            return courseList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     GetCoursesOfUser Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildGetCoursesOfUserCommand(String spName, SqlConnection con, int id)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@id", id);
//        return cmd;
//    }


//    --------------------------------------------------------------------------------------------------
//     GetCoursesOfInstructor
//    --------------------------------------------------------------------------------------------------
//    public List<Course> GetCoursesOfInstructor(int instructorsId)
//    {
//        SqlConnection con = null;
//        List<Course> courseList = new List<Course>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildGetCoursesOfInstructorCommand("SP_ReadAllCoursesByInstructor", con, instructorsId); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Course course = new Course();
//                course.Id = Convert.ToInt32(dr["id"]);
//                course.Title = (string)dr["title"];
//                course.Url = (string)dr["url"];
//                course.Rating = Convert.ToSingle(dr["rating"]);
//                course.NumberOfReviews = Convert.ToInt32(dr["num_reviews"]);
//                course.InstructorId = Convert.ToInt32(dr["instructors_id"]); ;
//                course.ImageReference = (string)dr["image"];
//                course.Duration = Convert.ToSingle(dr["duration"]);
//                course.LastUpdate = Convert.ToDateTime(dr["last_update_date"]);
//                course.IsActive = Convert.ToBoolean(dr["isActive"]);
//                string format = "M/d/yyyy";
//                course.LastUpdate = DateTime.ParseExact((string)dr["last_update_date"], format, CultureInfo.InvariantCulture);
//                courseList.Add(course);
//            }

//            return courseList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     GetCoursesOfInstructor Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildGetCoursesOfInstructorCommand(String spName, SqlConnection con, int instructorsId)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@instructorsId", instructorsId);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     DeleteCourseOfUser 
//    --------------------------------------------------------------------------------------------------
//    public bool DeleteCourseOfUser(int userId, int courseId)
//    {
//        SqlConnection con;
//        SqlCommand cmd;

//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        cmd = BuildDeleteCourseOfUserCommand("SP_DeleteCourseOfUser", con, userId, courseId); // create the command
//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     DeleteCourseOfUser Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildDeleteCourseOfUserCommand(String spName, SqlConnection con, int userId, int courseId)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@UsersID", userId);
//        cmd.Parameters.AddWithValue("@CourseID", courseId);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     UserLogin
//    --------------------------------------------------------------------------------------------------
//    public Users UserLogin(string email, string password)
//    {
//        SqlConnection con = null;
//        Users user = new Users();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildUserLoginCommand("SP_UserLogin", con, email, password); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                user.Id = Convert.ToInt32(dr["id"]);
//                user.Name = (string)dr["name"];
//                user.Email = (string)dr["email"];
//                user.Password = (string)dr["password"];
//                user.IsAdmin = Convert.ToBoolean(dr["isAdmin"]);
//                user.IsActive = Convert.ToBoolean(dr["isActive"]);
//            }

//            return user;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     UserLogin Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildUserLoginCommand(String spName, SqlConnection con, string email, string password)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@email", email);
//        cmd.Parameters.AddWithValue("@password", password);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     UserAddCourse
//    --------------------------------------------------------------------------------------------------
//    public bool UserAddCourse(int userId, int courseId)
//    {
//        SqlConnection con;
//        SqlCommand cmd;
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        cmd = BuildUserAddCourseCommand("SP_UserAddCourse", con, userId, courseId); // create the command
//        try
//        {
//            int numEffected = cmd.ExecuteNonQuery(); // execute the command
//            return numEffected > 0; // return true if any rows were affected
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     UserAddCourse Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildUserAddCourseCommand(String spName, SqlConnection con, int userId, int courseId)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@userId", userId);
//        cmd.Parameters.AddWithValue("@courseId", courseId);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     UserCheckIfCourseExists
//    --------------------------------------------------------------------------------------------------
//    public bool UserCheckIfCourseExists(int userId, int courseId)
//    {
//        SqlConnection con;
//        SqlCommand cmd;
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        cmd = BuildUserCheckIfCourseExistsCommand("SP_UserCheckIfCourseExists", con, userId, courseId); // create the command
//        try
//        {
//            int count = (int)cmd.ExecuteScalar(); // execute the command and get the result
//            return (count > 0);
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }

//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     UserCheckIfCourseExists Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildUserCheckIfCourseExistsCommand(String spName, SqlConnection con, int userId, int courseId)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@userId", userId);
//        cmd.Parameters.AddWithValue("@courseId", courseId);
//        return cmd;
//    }

//    --------------------------------------------------------------------------------------------------
//     GetUserCoursesByName
//    --------------------------------------------------------------------------------------------------
//    public List<Course> GetUserCoursesByName(string partOfName, int userId)
//    {
//        SqlConnection con = null;
//        List<Course> courseList = new List<Course>();
//        try
//        {
//            con = connect("myProjDB"); // create the connection
//            SqlCommand cmd = BuildGetUserCoursesByNameCommand("SP_GetUserCoursesByName", con, partOfName, userId); // create the command
//            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

//            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
//            {
//                Course course = new Course();
//                course.Id = Convert.ToInt32(dr["id"]);
//                course.Title = (string)dr["title"];
//                course.Url = (string)dr["url"];
//                course.Rating = Convert.ToSingle(dr["rating"]);
//                course.NumberOfReviews = Convert.ToInt32(dr["num_reviews"]);
//                course.InstructorId = Convert.ToInt32(dr["instructors_id"]); ;
//                course.ImageReference = (string)dr["image"];
//                course.Duration = Convert.ToSingle(dr["duration"]);
//                course.LastUpdate = Convert.ToDateTime(dr["last_update_date"]);
//                course.IsActive = Convert.ToBoolean(dr["isActive"]);
//                string format = "M/d/yyyy";
//                course.LastUpdate = DateTime.ParseExact((string)dr["last_update_date"], format, CultureInfo.InvariantCulture);
//                courseList.Add(course);
//            }

//            return courseList;
//        }
//        catch (Exception ex)
//        {
//            throw (ex); // write to log
//        }
//        finally
//        {
//            if (con != null)
//            {
//                con.Close(); // close the db connection
//            }
//        }
//    }
//    ---------------------------------------------------------------------------------
//     GetUserCoursesByName Helper
//    ---------------------------------------------------------------------------------
//    private SqlCommand BuildGetUserCoursesByNameCommand(String spName, SqlConnection con, string partOfName, int userId)
//    {
//        SqlCommand cmd = new SqlCommand(); // create the command object
//        cmd.Connection = con;              // assign the connection to the command object
//        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
//        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
//        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

//        cmd.Parameters.AddWithValue("@title", partOfName);
//        cmd.Parameters.AddWithValue("@id", userId);
//        return cmd;
//    }
//}