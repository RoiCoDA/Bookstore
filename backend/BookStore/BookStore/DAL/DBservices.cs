using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using BookStore.BL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.Xml;
using System.Reflection.PortableExecutable;
using System.Net;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.SignalR;



public class DBservices
{
    public DBservices()
    {
        
    }

    private readonly IHubContext<NotificationHub> _hubContext;

    public DBservices(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }



    public SqlConnection connect(String conString)
    {

        // read the connection string from the configuration file
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json").Build();
        string cStr = configuration.GetConnectionString("myProjDB");

        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }

    //--------------------------------------------------------------------------------------------------
    // Creates a connection to the database according to the connectionString name in the web.config 
    //--------------------------------------------------------------------------------------------------



    //---------------------------------------------------------------------------------
    // Read Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildReadCommand(String spName, SqlConnection con)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        return cmd;
    }

    //--------------------------------------------------------------------------------------------------
    // 
    // *BookStoreUsers*
    //
    //--------------------------------------------------------------------------------------------------


    //---------------------------------------------------------------------------------
    // CreateNewUser
    //---------------------------------------------------------------------------------

    public bool CreateNewUser(BookStoreUser user)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildCreateNewUserCommand("SP_F_CreateNewUser", con, user); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected > 0; // return true if any rows were affected
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // CreateNewUser Helper
    //---------------------------------------------------------------------------------


    private SqlCommand BuildCreateNewUserCommand(String spName, SqlConnection con, BookStoreUser user)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@name", user.UserName);
        cmd.Parameters.AddWithValue("@email", user.UserEmail);
        cmd.Parameters.AddWithValue("@password", user.UserPassword);
        cmd.Parameters.AddWithValue("@dateCreated", user.DateCreated);
        cmd.Parameters.AddWithValue("@image", user.Image);
        cmd.Parameters.AddWithValue("@isActive", user.IsActive);
        cmd.Parameters.AddWithValue("@isAdmin", user.IsAdmin);
        cmd.Parameters.AddWithValue("@signature", user.Signature);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetUserData
    //---------------------------------------------------------------------------------
    public Object GetUserData(string userEmail)
    {

        SqlConnection con = null;
        List<Object> userData = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetUserDataHelper("SP_F_GetUserData", con, userEmail); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                userData.Add(new
                {
                    UserID = Convert.ToInt32(dr["UserID"]),
                    UserName = dr["UserName"].ToString(),
                    UserEmail = dr["UserEmail"].ToString(),
                    DateCreated = (DateTime)dr["DateCreated"],
                    Image = dr["Image"].ToString(),
                    IsAdmin = Convert.ToBoolean(dr["isAdmin"]),
                    UserSignature = dr["Signature"] is DBNull ? "" : dr["Signature"].ToString()
                });
            }
            return userData;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetUserData Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetUserDataHelper(String spName, SqlConnection con, string userEmail)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@email", userEmail);
        return cmd;
    }



    //---------------------------------------------------------------------------------
    // getUserDataByID
    //---------------------------------------------------------------------------------
    public Object getUserDataByID(int UserID)
    {
        SqlConnection con = null;
        List<Object> userData = new List<Object>();
        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildgetUserDataByIDHelper("SP_F_getUserDataByID", con, UserID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished
            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                userData.Add(new
                {
                    UserID = Convert.ToInt32(dr["UserID"]),
                    UserName = dr["UserName"].ToString(),
                    DateCreated = (DateTime)dr["DateCreated"],
                    Image = dr["Image"].ToString(),
                    UserSignature = dr["Signature"] is DBNull ? "" : dr["Signature"].ToString()
                });
            }
            return userData;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // getUserDataByID Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildgetUserDataByIDHelper(String spName, SqlConnection con, int UserID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // IsUserAdmin
    //--------------------------------------------------------------------------------------------------
    public bool IsUserAdmin(int userID)
    {
        SqlConnection con = null;
        try
        {
            con = connect("myProjDB");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SP_F_IsUserAdmin";
            cmd.CommandTimeout = 10;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userID);

            object result = cmd.ExecuteScalar();
            return result != null && Convert.ToBoolean(result);
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // DoesBookUserConnectionExist
    //--------------------------------------------------------------------------------------------------
    public bool DoesBookUserConnectionExist(int UserID, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildDoesBookUserConnectionExistCommand("SP_F_DoesBookUserConnectionExist", con, UserID, BookID); // create the command
        try
        {
            int count = (int)cmd.ExecuteScalar(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // DoesBookUserConnectionExist Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildDoesBookUserConnectionExistCommand(String spName, SqlConnection con, int UserID, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);
        return cmd;
    }

    //SP_f_CheckIfUserOwnsBook 

    //--------------------------------------------------------------------------------------------------
    // CheckIfUserOwnsBook
    //--------------------------------------------------------------------------------------------------
    public bool CheckIfUserOwnsBook(int UserID, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildCheckIfUserOwnsBookCommand("SP_f_CheckIfUserOwnsBook", con, UserID, BookID); // create the command
        try
        {
            int count = (int)cmd.ExecuteScalar(); // execute the command and get the result
            //Console.WriteLine("This is count from DBs ( of CheckIfUserOwnsBook)");
            //Console.WriteLine(count);
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // CheckIfUserOwnsBook Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildCheckIfUserOwnsBookCommand(String spName, SqlConnection con, int UserID, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // AddNewUserBookRelation
    //---------------------------------------------------------------------------------

    public bool AddNewUserBookRelation(int UserID, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildAddNewUserBookRelationCommand("SP_F_AddNewUserBookRelation", con, UserID, BookID); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected > 0; // return true if any rows were affected
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // AddNewUserBookRelation Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildAddNewUserBookRelationCommand(String spName, SqlConnection con, int UserID, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);

        return cmd;
    }



    //---------------------------------------------------------------------------------
    // ToggleBookUserToWishlist
    //---------------------------------------------------------------------------------
    public int ToggleBookUserToWishlist(int UserID, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildToggleBookUserToWishlistCommand("SP_F_ToggleBookUserToWishlist", con, UserID, BookID); // create the command

        try
        {
            // Execute the command and retrieve the new WishlistStatus
            var result = (int)cmd.ExecuteScalar(); // Use ExecuteScalar to get the single value
            // bool newWishlistStatus = (result == DBNull.Value) ? false : Convert.ToBoolean(result);
            //Console.WriteLine("this is result:");
            //Console.WriteLine(result);
            return result; // Return the new wishlist status
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // ToggleBookUserToWishlist Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildToggleBookUserToWishlistCommand(String spName, SqlConnection con, int UserID, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);

        return cmd;
    }



    //---------------------------------------------------------------------------------
    // AddNewUserBookRelation
    //---------------------------------------------------------------------------------
    public bool UserBookApplyOwnership(int UserID, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildUserBookApplyOwnershipCommand("SP_F_UserBookApplyOwnership", con, UserID, BookID); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected > 0; // return true if any rows were affected
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // AddNewUserBookRelation Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildUserBookApplyOwnershipCommand(String spName, SqlConnection con, int UserID, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);

        return cmd;
    }

    //---------------------------------------------------------------------------------
    // GetUserOwnedBooksAdHoc  (for library)
    //---------------------------------------------------------------------------------
    //SP_F_GetAllTradableBooks
    public Object GetUserOwnedBooksAdHoc(int UserID)
    {

        SqlConnection con = null;
        List<Object> userOwnedBooksAdHoc = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetUserOwnedBooksAdHocCommand("SP_F_GetUserOwnedBooksAdHoc", con, UserID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                userOwnedBooksAdHoc.Add(new
                {
                    Title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    Authors = dr["authors"] == DBNull.Value || dr["authors"] == null || string.IsNullOrWhiteSpace(dr["authors"].ToString()) ? "No authors mentioned." : dr["authors"].ToString(),
                    Thumbnail = dr["thumbnail"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["thumbnail"],
                    Description = dr["description"] is DBNull ? "No description available." : (string)dr["description"],
                    BookID = Convert.ToInt32(dr["BookID"]),
                    ReadStatus = dr["ReadStatus"] is DBNull ? false : (bool)dr["ReadStatus"]
                });
            }
            return userOwnedBooksAdHoc;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetUserOwnedBooksAhHoc Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetUserOwnedBooksAdHocCommand(String spName, SqlConnection con, int UserID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);

        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetUserReadBooksAdHoc  (for library)
    //---------------------------------------------------------------------------------
    public Object GetUserReadBooksAhHoc(int UserID)
    {

        SqlConnection con = null;
        List<Object> userReadBooksAdHoc = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetUserReadBooksAdHocCommand("SP_F_GetUserReadBooksAdHoc", con, UserID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                userReadBooksAdHoc.Add(new
                {
                    Title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    Authors = dr["authors"] == DBNull.Value || dr["authors"] == null || string.IsNullOrWhiteSpace(dr["authors"].ToString()) ? "No authors mentioned." : dr["authors"].ToString(),
                    Thumbnail = dr["thumbnail"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["thumbnail"],
                    Description = dr["description"] is DBNull ? "No description available." : (string)dr["description"],
                    BookID = Convert.ToInt32(dr["BookID"])
                });
            }
            return userReadBooksAdHoc;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetUserReadBooksAhHoc Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetUserReadBooksAdHocCommand(String spName, SqlConnection con, int UserID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);

        return cmd;
    }


    //---------------------------------------------------------------------------------
    // MarkBookAsRead
    //---------------------------------------------------------------------------------
    public bool MarkBookAsRead(int UserID, int BookID, bool readStatus)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildMarkBookAsReadCommand("SP_F_MarkBookAsRead", con, UserID, BookID, readStatus); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected > 0; // return true if any rows were affected
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // MarkBookAsRead Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildMarkBookAsReadCommand(String spName, SqlConnection con, int UserID, int BookID, bool readStatus)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);
        cmd.Parameters.AddWithValue("@readStatus", readStatus);

        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetUserTradedBooksAdHoc  (for library)
    //---------------------------------------------------------------------------------
    public Object GetUserTradedBooksAdHoc(int UserID)
    {

        SqlConnection con = null;
        List<Object> userTradedBooksAdHoc = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetUserTradedBooksAdHocCommand("SP_F_GetUserTradedBooksAdHoc", con, UserID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                userTradedBooksAdHoc.Add(new
                {
                    Title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    Authors = dr["authors"] == DBNull.Value || dr["authors"] == null || string.IsNullOrWhiteSpace(dr["authors"].ToString()) ? "No authors mentioned." : dr["authors"].ToString(),
                    Thumbnail = dr["thumbnail"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["thumbnail"],
                    Description = dr["description"] is DBNull ? "No description available." : (string)dr["description"],
                    BookID = Convert.ToInt32(dr["BookID"])
                });
            }
            return userTradedBooksAdHoc;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetUserTradedBooksAdHoc Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetUserTradedBooksAdHocCommand(String spName, SqlConnection con, int UserID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);

        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetUserWishlistedBooksAdHoc  (for library)
    //---------------------------------------------------------------------------------
    public Object GetUserWishlistedBooksAdHoc(int UserID)
    {

        SqlConnection con = null;
        List<Object> userWishlistedBooksAdHoc = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetUserWishlistedBooksAdHocCommand("SP_F_GetUserWishlistedBooksAdHoc", con, UserID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                userWishlistedBooksAdHoc.Add(new
                {
                    Title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    Authors = dr["authors"] == DBNull.Value || dr["authors"] == null || string.IsNullOrWhiteSpace(dr["authors"].ToString()) ? "No authors mentioned." : dr["authors"].ToString(),
                    Thumbnail = dr["thumbnail"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["thumbnail"],
                    Description = dr["description"] is DBNull ? "No description available." : (string)dr["description"],
                    BookID = Convert.ToInt32(dr["BookID"])
                });
            }
            return userWishlistedBooksAdHoc;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetUserWishlistedBooksAdHoc Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetUserWishlistedBooksAdHocCommand(String spName, SqlConnection con, int UserID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);

        return cmd;
    }

    //--------------------------------------------------------------------------------------------------
    // UserBuyBook
    //--------------------------------------------------------------------------------------------------
    public bool UserBuyBook(int UserID, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildUserBuyBookCommand("SP_F_UserBuyBook", con, UserID, BookID); // create the command
        try
        {
            //Console.WriteLine(UserID);
            //Console.WriteLine(BookID);
            //Console.WriteLine(UserID.GetType());
            //Console.WriteLine(BookID.GetType());
            int count = (int)cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // UserBuyBook Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildUserBuyBookCommand(String spName, SqlConnection con, int UserID, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);
        return cmd;
    }




    //---------------------------------------------------------------------------------
    // GetAllTradableBooks 
    //---------------------------------------------------------------------------------
    public Object GetAllTradableBooks(int UserID)
    {

        SqlConnection con = null;
        List<Object> tradableBooksAdHoc = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetAllTradableBooksCommand("SP_F_GetAllTradableBooks", con, UserID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                tradableBooksAdHoc.Add(new
                {

                    userID = Convert.ToInt32(dr["UserID"]),
                    userName = dr["UserName"].ToString(),
                    bookID = Convert.ToInt32(dr["BookID"]),
                    title = dr["title"].ToString(),

                });
            }
            return tradableBooksAdHoc;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetAllTradableBooks Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetAllTradableBooksCommand(String spName, SqlConnection con, int UserID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // ChangeUserPassword
    //--------------------------------------------------------------------------------------------------
    public bool ChangeUserPassword(int UserID, string UserPassword)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildChangeUserPasswordCommand("SP_F_ChangeUserPassword", con, UserID, UserPassword); // create the command
        try
        {
            int count = (int)cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // ChangeUserPassword Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildChangeUserPasswordCommand(String spName, SqlConnection con, int UserID, string UserPassword)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@UserPassword", UserPassword);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // ChangeUserImage
    //--------------------------------------------------------------------------------------------------
    public bool ChangeUserImage(int UserID, string userImage)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildChangeUserImageCommand("SP_F_ChangeUserImage", con, UserID, userImage); // create the command
        try
        {
            int count = (int)cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // ChangeUserImage Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildChangeUserImageCommand(String spName, SqlConnection con, int UserID, string userImage)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@UserImage", userImage);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // ChangeUserSignature
    //--------------------------------------------------------------------------------------------------
    public bool ChangeUserSignature(int UserID, string userSignature)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildChangeUserSignatureCommand("SP_F_ChangeUserSignature", con, UserID, userSignature); // create the command
        try
        {
            int count = (int)cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // ChangeUserSignature Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildChangeUserSignatureCommand(String spName, SqlConnection con, int UserID, string userSignature)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@UserSignature", userSignature);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetUserDataForAdminTable
    //---------------------------------------------------------------------------------

    public Object GetUserDataForAdminTable()
    {

        SqlConnection con = null;
        List<Object> UserDataForAdminTable = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildReadCommand("SP_F_GetUserDataForAdminTable", con); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                UserDataForAdminTable.Add(new
                {
                    UserID = Convert.ToInt32(dr["UserID"]),
                    UserName = (string)dr["UserName"],
                    UserEmail = (string)dr["UserEmail"],
                    UniqueBooks = Convert.ToInt32(dr["UniqueBooks"])
                });
            }
            return UserDataForAdminTable;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }

    //---------------------------------------------------------------------------------
    // GetBookDataForAdminTable
    //---------------------------------------------------------------------------------

    public Object GetBookDataForAdminTable()
    {

        SqlConnection con = null;
        List<Object> BookDataForAdminTable = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildReadCommand("SP_F_GetBookDataForAdminTable", con); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                BookDataForAdminTable.Add(new
                {
                    BookID = Convert.ToInt32(dr["BookID"]),
                    BookTitle = (string)dr["BookTitle"],
                    BookAuthor = (string)dr["BookAuthor"],
                    UniqueOwners = Convert.ToInt32(dr["UniqueOwners"])
                });
            }
            return BookDataForAdminTable;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }





    //--------------------------------------------------------------------------------------------------
    // CheckUserConnectionToBookForTrade
    //--------------------------------------------------------------------------------------------------
    public int CheckUserConnectionToBookForTrade(int UserID, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildCheckUserConnectionToBookForTradeCommand("SP_F_CheckUserConnectionToBookForTrade", con, UserID, BookID); // create the command
        try
        {
            int status = (int)cmd.ExecuteScalar(); // execute the command and get the result
            return status;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // CheckUserConnectionToBookForTrade Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildCheckUserConnectionToBookForTradeCommand(String spName, SqlConnection con, int UserID, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // CreateBookTradesHistoryEntry
    //---------------------------------------------------------------------------------

    public int CreateBookTradesHistoryEntry(int BookID, int UserIDInitiator, int UserIDRecipient)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildCreateBookTradesHistoryEntryCommand("SP_F_CreateBookTradesHistoryEntry", con, BookID, UserIDInitiator, UserIDRecipient); // create the command

        try
        {
            //Console.WriteLine("Book ID:");
            //Console.WriteLine(BookID);
            //Console.WriteLine("Initiator ID:");
            //Console.WriteLine(UserIDInitiator);
            //Console.WriteLine("Recipient ID:");
            //Console.WriteLine(UserIDRecipient);
            int TradeID = (int)cmd.ExecuteScalar(); // execute the command
            return TradeID; // return true if any rows were affected
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // CreateBookTradesHistoryEntry Helper
    //---------------------------------------------------------------------------------


    private SqlCommand BuildCreateBookTradesHistoryEntryCommand(String spName, SqlConnection con, int BookID, int UserIDInitiator, int UserIDRecipient)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@BookID", BookID);
        cmd.Parameters.AddWithValue("@UserID_TradeInitiator", UserIDInitiator);
        cmd.Parameters.AddWithValue("@UserID_TradeOfferRecipient", UserIDRecipient);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // SendTradeNotificationToOfferingUser
    //--------------------------------------------------------------------------------------------------
    public int SendTradeNotificationToOfferingUser(int TradeID, int NotificationRecipient)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildSendTradeNotificationToOfferingUserCommand("SP_F_SendTradeNotificationToOfferingUser", con, TradeID); // create the command
        try
        {
            //Console.WriteLine("This is TradeID!");
            //Console.WriteLine(TradeID);
            int NotifID = (int)cmd.ExecuteScalar(); // execute the command and get the result
            
            return NotifID;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // SendTradeNotificationToOfferingUser Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildSendTradeNotificationToOfferingUserCommand(String spName, SqlConnection con, int TradeID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@TradeID", TradeID);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // TradeConclusionNotification 
    //--------------------------------------------------------------------------------------------------
    public (bool isSuccess, int TradeInitiator) TradeConclusionNotification(int TradeID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // handle connection error
        }

        cmd = BuildTradeConclusionNotificationCommand("SP_F_TradeConclusionNotification", con, TradeID); // create the command

        // Add output parameter to capture the TradeOfferRecipient
        SqlParameter recipientParam = new SqlParameter("@UserID_TradeInitiator", SqlDbType.Int);
        recipientParam.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(recipientParam);

        try
        {
            int affectedRows = cmd.ExecuteNonQuery(); // execute the command and get the affected rows

            // Retrieve the output parameter value
            int TradeInitiator = (recipientParam.Value != DBNull.Value) ? (int)recipientParam.Value : 0;

            // Return a tuple with the success status and TradeOfferRecipient
            return (affectedRows > 0, TradeInitiator);
        }
        catch (Exception ex)
        {
            throw (ex); // handle execution error
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }

    //---------------------------------------------------------------------------------
    // TradeConclusionNotification  Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildTradeConclusionNotificationCommand(String spName, SqlConnection con, int TradeID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@TradeID", TradeID);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // DenyDuplicateTradeRequests
    //--------------------------------------------------------------------------------------------------
    public bool DenyDuplicateTradeRequests(int BookID, int UserIDInitiator, int UserIDRecipient)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildDenyDuplicateTradeRequestsCommand("SP_F_DenyDuplicateTradeRequests", con, BookID, UserIDInitiator, UserIDRecipient); // create the command
        try
        {
            int count = (int)cmd.ExecuteScalar(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // DenyDuplicateTradeRequests Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildDenyDuplicateTradeRequestsCommand(String spName, SqlConnection con, int BookID, int UserIDInitiator, int UserIDRecipient)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@BookID", BookID);
        cmd.Parameters.AddWithValue("@UserID_TradeInitiator", UserIDInitiator);
        cmd.Parameters.AddWithValue("@UserID_TradeOfferRecipient", UserIDRecipient);
        return cmd;
    }



    //--------------------------------------------------------------------------------------------------
    // TradeOfferAccepted
    //--------------------------------------------------------------------------------------------------
    public bool TradeOfferAccepted(int TradeID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildTradeOfferAcceptedCommand("SP_F_TradeOfferAccepted", con, TradeID); // create the command
        try
        {
            int count = cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // TradeOfferAccepted Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildTradeOfferAcceptedCommand(String spName, SqlConnection con, int TradeID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@TradeID", TradeID);

        return cmd;
    }



    //--------------------------------------------------------------------------------------------------
    // TradeOfferAccepted
    //--------------------------------------------------------------------------------------------------
    public bool TradeOfferRejected(int TradeID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildTradeOfferRejectedCommand("SP_F_TradeOfferRejected", con, TradeID); // create the command
        try
        {
            int count = cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // TradeOfferAccepted Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildTradeOfferRejectedCommand(String spName, SqlConnection con, int TradeID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@TradeID", TradeID);

        return cmd;
    }




    //--------------------------------------------------------------------------------------------------
    // GetBookIDAndInitiatorIDFromBookTradesHistoryEntry
    //--------------------------------------------------------------------------------------------------
    public Object GetBookIDAndInitiatorIDFromBookTradesHistoryEntry(int TradeID)
    {
        SqlConnection con = null;
        Object bookIDandUserInfo = null;

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetBookIDAndInitiatorIDFromBookTradesHistoryEntryHelper("SP_F_GetBookIDAndInitiatorIDFromBookTradesHistoryEntry", con, TradeID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            if (dr.Read()) // Read till the end of the data into a row, then True if there are more rows
            {
                bookIDandUserInfo = new
                {
                    bookID = Convert.ToInt32(dr["BookID"]),
                    initiatorID = Convert.ToInt32(dr["UserID_TradeInitiator"])
                };
            }

            return bookIDandUserInfo;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }

    //---------------------------------------------------------------------------------
    // GetBookIDAndInitiatorIDFromBookTradesHistoryEntry Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetBookIDAndInitiatorIDFromBookTradesHistoryEntryHelper(String spName, SqlConnection con, int TradeID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@TradeID", TradeID);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // SendForumMentionNotification
    //--------------------------------------------------------------------------------------------------
    public (bool isSuccess, int? originalPostUserID) SendForumMentionNotification(int AuthorID, int UserID, int ResponseToPostID)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;

        try
        {
            con = connect("myProjDB"); // Create the connection

            // Create the command
            cmd = BuildSendForumMentionNotificationCommand("SP_F_SendForumMentionNotification", con, AuthorID, UserID, ResponseToPostID);

            // Add output parameter to capture the OriginalPostUserID
            SqlParameter originalPostUserIDParam = new SqlParameter("@OriginalPostUserID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(originalPostUserIDParam);

            // Execute the command
            int affectedRows = cmd.ExecuteNonQuery();

            // Retrieve the output parameter value
            int? originalPostUserID = originalPostUserIDParam.Value != DBNull.Value ? (int?)originalPostUserIDParam.Value : null;

            // Print affected rows for debugging
            //Console.WriteLine($"Affected Rows: {affectedRows}");

            // Return a tuple with the success status and OriginalPostUserID
            return (affectedRows > 0, originalPostUserID);
        }
        catch (Exception ex)
        {
            // Handle the exception
            throw ex;
        }
        finally
        {
            // Ensure the connection is closed
            if (con != null)
            {
                con.Close();
            }
        }
    }
    //---------------------------------------------------------------------------------
    // SendForumMentionNotification Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildSendForumMentionNotificationCommand(String spName, SqlConnection con, int AuthorID, int UserID, int ResponseToPostI)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorID", AuthorID);
        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@ResponseToPostID", ResponseToPostI);

        return cmd;
    }

    // SP_F_SendBookBoughtNotification 

    //--------------------------------------------------------------------------------------------------
    // SendBookBoughtNotification 
    //--------------------------------------------------------------------------------------------------
    public bool SendBookBoughtNotification(int BookID, int UserID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildSendBookBoughtNotificationCommand("SP_F_SendBookBoughtNotification", con, BookID, UserID); // create the command
        try
        {
            int count = cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // SendBookBoughtNotification Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildSendBookBoughtNotificationCommand(String spName, SqlConnection con, int BookID, int UserID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@BookID", BookID);
        cmd.Parameters.AddWithValue("@UserID", UserID);

        return cmd;
    }



    // SP_F_RecordBookPurchaseInHistory

    //--------------------------------------------------------------------------------------------------
    // RecordBookPurchaseInHistory 
    //--------------------------------------------------------------------------------------------------
    public bool RecordBookPurchaseInHistory(int UserID, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildRecordBookPurchaseInHistoryCommand("SP_F_RecordBookPurchaseInHistory", con, UserID, BookID); // create the command
        try
        {
            int count = cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // RecordBookPurchaseInHistory Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildRecordBookPurchaseInHistoryCommand(String spName, SqlConnection con, int UserID, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@BookID", BookID);
        

        return cmd;
    }


    // SP_F_DoesUserHaveUnreadNotifications


    //--------------------------------------------------------------------------------------------------
    // DoesUserHaveUnreadNotifications 
    //--------------------------------------------------------------------------------------------------
    public bool DoesUserHaveUnreadNotifications(int UserID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildDoesUserHaveUnreadNotificationsCommand("SP_F_DoesUserHaveUnreadNotifications", con, UserID); // create the command
        try
        {
            int count = (int)cmd.ExecuteScalar(); // execute the command and get the result

            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // DoesUserHaveUnreadNotifications Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildDoesUserHaveUnreadNotificationsCommand(String spName, SqlConnection con, int UserID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);

        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // UpdateUserScore (for quiz page) 
    //--------------------------------------------------------------------------------------------------
    public bool UpdateUserScore(int UserID, int UserScore)
    {
        using (SqlConnection con = connect("myProjDB")) // create the connection
        {
            SqlCommand cmd = BuildUpdateUserScoreCommand("SP_F_UpdateUserScore", con, UserID, UserScore); // create the command

            // Add the output parameter
            cmd.Parameters.Add(new SqlParameter("@RowsAffected", SqlDbType.Int));
            cmd.Parameters["@RowsAffected"].Direction = ParameterDirection.Output;

            try
            {
                cmd.ExecuteNonQuery(); // execute the command
                int count = (int)cmd.Parameters["@RowsAffected"].Value; // get the output parameter value
                return (count > 0); // return true if rows were affected
            }
            catch (Exception ex)
            {
                throw; // write to log
            }
        }
    }
    //---------------------------------------------------------------------------------
    // UpdateUserScore Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildUpdateUserScoreCommand(String spName, SqlConnection con, int UserID, int UserScore)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@UserScore", UserScore);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // GetTop10UsersByScore (for quiz page) 
    //--------------------------------------------------------------------------------------------------
    public List<object> GetTop10UsersByScore()
    {
        SqlConnection con = null;
        List<object> topUsers = new List<object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildReadCommand("SP_F_GetTop10UsersByScore", con); // create the command

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read())
            {
                // Creating an anonymous type for each user with their highest score
                var user = new
                {
                    UserName = dr["UserName"].ToString(),
                    UserID = (int)dr["UserID"],
                    HighestScore = (int)dr["HighestScore"]
                };

                topUsers.Add(user);
            }
            return topUsers;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //--------------------------------------------------------------------------------------------------
    // 
    // *Authors*
    //
    //--------------------------------------------------------------------------------------------------


    //---------------------------------------------------------------------------------
    // GetAuthorAndBooksByAuthorAdHoc  (for the author page)
    //---------------------------------------------------------------------------------
    public Object GetAuthorAndBooksByAuthorAdHoc(int authorID)
    {

        SqlConnection con = null;
        List<Object> authorsWithBooks = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetAuthorAndBooksByAuthorAdHocHelper("SP_F_GetAuthorAndBooksByAuthorAdHoc", con, authorID); // create the command


            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            if (dr.Read())
            {
                // Creating an anonymous type for author with dynamic books list
                var authorWithBooks = new
                {
                    AuthorID = (int)dr["AuthorID"],
                    Author = dr["author"].ToString(),
                    Summary = dr["summary"].ToString(),
                    Image = dr["image"].ToString(),
                    Link = dr["link"].ToString(),
                    Books = new List<object>()
                };

                // Check if Books column has data and parse it
                if (dr["Books"] != DBNull.Value)
                {

                    var booksJson = dr["Books"].ToString();
                    var booksArray = JArray.Parse(booksJson);

                    // Convert JArray to List<object>
                    foreach (var bookToken in booksArray)
                    {
                        var book = new
                        {
                            BookID = (int)bookToken["BookID"],
                            Description = (string)bookToken["description"],
                            Title = (string)bookToken["title"],
                            Thumbnail = (string)bookToken["thumbnail"]

                        };
                        ((List<object>)authorWithBooks.Books).Add(book);
                    }
                }

                authorsWithBooks.Add(authorWithBooks);
            }
            return authorsWithBooks;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }

    //---------------------------------------------------------------------------------
    // GetAuthorAndBooksByAuthorAdHoc Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetAuthorAndBooksByAuthorAdHocHelper(String spName, SqlConnection con, int authorID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorID", authorID);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetAuthorDataForAdminTable
    //---------------------------------------------------------------------------------

    public Object GetAuthorDataForAdminTable()
    {

        SqlConnection con = null;
        List<Object> AuthorDataForAdminTable = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildReadCommand("SP_F_GetAuthorDataForAdminTable", con); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                AuthorDataForAdminTable.Add(new
                {
                    AuthorID = Convert.ToInt32(dr["AuthorID"]),
                    AuthorName = (string)dr["AuthorName"],
                    TimesInLibraries = Convert.ToInt32(dr["TimesInLibraries"])
                });
            }
            return AuthorDataForAdminTable;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }



    //---------------------------------------------------------------------------------
    // GetAllWritersDisplayAdHoc
    //---------------------------------------------------------------------------------
    public Object GetAllWritersDisplayAdHoc()
    {

        SqlConnection con = null;
        List<Object> authorDataForDisplay = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildReadCommand("SP_F_GetAllWritersDisplayAdHoc", con); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                authorDataForDisplay.Add(new
                {
                    AuthorName = (string)dr["author"],
                    Image = dr["image"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["image"],
                    AuthorID = Convert.ToInt32(dr["AuthorID"])
                });
            }
            return authorDataForDisplay;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // GetAuthorsByName  (for the search page)
    //---------------------------------------------------------------------------------
    public Object GetAuthorsByName(string query)
    {
        SqlConnection con = null;
        List<Object> authorDataForDisplay = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetAuthorsByNameHelper("SP_F_GetAuthorsByName", con, query);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row, then True if there are more rows
            {
                authorDataForDisplay.Add(new
                {
                    AuthorName = dr["AuthorName"] is DBNull ? string.Empty : (string)dr["AuthorName"],
                    Image = dr["Image"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["image"],
                    AuthorID = Convert.ToInt32(dr["AuthorID"])
                });
            }
            return authorDataForDisplay;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetAuthorsByName Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetAuthorsByNameHelper(String spName, SqlConnection con, string query)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Query", query);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // SubmitForumPost
    //--------------------------------------------------------------------------------------------------
    public bool SubmitForumPost(int AuthorID, int UserID, string Header, string Content, int ResponseToPostID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildSubmitForumPostCommand("SP_F_SubmitForumPost", con, AuthorID, UserID, Header, Content, ResponseToPostID); // create the command
        try
        {
            int count = cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // SubmitForumPost Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildSubmitForumPostCommand(String spName, SqlConnection con, int AuthorID, int UserID, string Header, string Content, int ResponseToPostID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorID", AuthorID);
        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@Header", Header);
        cmd.Parameters.AddWithValue("@content", Content);
        cmd.Parameters.AddWithValue("@ResponseToPostID", ResponseToPostID);

        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetAllForumPosts 
    //---------------------------------------------------------------------------------
    public Object GetAllForumPosts(int AuthorID)
    {

        SqlConnection con = null;
        List<Object> ForumPosts = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetAllForumPostsCommand("SP_F_GetAllForumPosts", con, AuthorID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                ForumPosts.Add(new
                {
                    PostID = Convert.ToInt32(dr["PostID"]),
                    AuthorID = Convert.ToInt32(dr["AuthorID"]),
                    userID = Convert.ToInt32(dr["UserID"]),
                    DatePosted = dr.GetDateTime(dr.GetOrdinal("DatePosted")),
                    Header = dr["Header"].ToString(),
                    PostContent = dr["PostContent"].ToString(),
                    ResponseToPostID = Convert.ToInt32(dr["ResponseToPostID"]),
                    IsDeleted = Convert.ToInt32(dr["isDeleted"]),
                    UserName = dr["UserName"].ToString(),
                    UserImage = dr["UserImage"].ToString(),
                    Signature = dr["Signature"].ToString(),
                });
            }
            return ForumPosts;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // GetAllForumPosts Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetAllForumPostsCommand(String spName, SqlConnection con, int AuthorID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorID", AuthorID);
        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // SendNewChatMessage  --- this is the result of sending a new chat message, the output from this SP will determine of other online users should download the sent chat message 
    //--------------------------------------------------------------------------------------------------
    public Object SendNewChatMessage(int UserID, int AuthorID, string MessageContent)
    {
        SqlConnection con = null;

        try
        {
            con = connect("myProjDB"); // Create the connection
            SqlCommand cmd = BuildSendNewChatMessageCommand("SP_F_SendNewChatMessage", con, UserID, AuthorID, MessageContent); // Create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            if (dr.Read()) // Read the first row, if available
            {
                // Return a single anonymous object
                return new
                {
                    NewMessageID = Convert.ToInt32(dr["NewMessageID"]),
                    AuthorID = Convert.ToInt32(dr["AuthorID"]),
                };
            }
            return null; // Return null if no data is found
        }
        catch (Exception ex)
        {
            throw; // Re-throw the exception to be handled by the caller
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // Close the DB connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // SendNewChatMessage Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildSendNewChatMessageCommand(String spName, SqlConnection con, int UserID, int AuthorID, string MessageContent)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", UserID);
        cmd.Parameters.AddWithValue("@AuthorID", AuthorID);
        cmd.Parameters.AddWithValue("@MessageContent", MessageContent);

        return cmd;
    }


    // SP_F_Load100ChatMessages

    //---------------------------------------------------------------------------------
    // Load100ChatMessages
    //---------------------------------------------------------------------------------
    public Object Load100ChatMessages(int AuthorID, int Offset)
    {

        SqlConnection con = null;
        List<Object> ChatMessages = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildLoad100ChatMessagesCommand("SP_F_Load100ChatMessages", con, AuthorID, Offset); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                ChatMessages.Add(new
                {
                    UserID = Convert.ToInt32(dr["UserID"]),
                    AuthorID = Convert.ToInt32(dr["AuthorID"]),
                    DatePosted = dr.GetDateTime(dr.GetOrdinal("DatePosted")),
                    MessageContent = dr["MessageContent"].ToString(),
                    UserName = dr["UserName"].ToString()
                });
            }
            return ChatMessages;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // Load100ChatMessages Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildLoad100ChatMessagesCommand(String spName, SqlConnection con, int AuthorID, int Offset)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorID", AuthorID);
        cmd.Parameters.AddWithValue("@Offset", Offset);
        return cmd;
    }



    // SP_F_LoadLastChatMessage

    //---------------------------------------------------------------------------------
    // LoadLastChatMessage
    //---------------------------------------------------------------------------------
    public Object LoadLastChatMessage(int AuthorID)
    {

        SqlConnection con = null;
        List<Object> ChatMessage = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildLoadLastChatMessageCommand("SP_F_LoadLastChatMessage", con, AuthorID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                ChatMessage.Add(new
                {
                    AuthorID = Convert.ToInt32(dr["AuthorID"]),
                    DatePosted = dr.GetDateTime(dr.GetOrdinal("DatePosted")),
                    MessageContent = dr["MessageContent"].ToString(),
                    UserName = dr["UserName"].ToString()
                });
            }
            return ChatMessage;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // LoadLastChatMessage Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildLoadLastChatMessageCommand(String spName, SqlConnection con, int AuthorID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorID", AuthorID);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetChatMessageByChatMessagIDAndAuthorID
    //---------------------------------------------------------------------------------
    public Object GetChatMessageByChatMessagIDAndAuthorID(int AuthorID, int MessageID)
    {

        SqlConnection con = null;
        List<Object> ChatMessages = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetChatMessageByChatMessagIDAndAuthorIDCommand("SP_F_GetChatMessageByChatMessagIDAndAuthorID", con, AuthorID, MessageID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                ChatMessages.Add(new
                {
                    AuthorID = Convert.ToInt32(dr["AuthorID"]),
                    DatePosted = dr.GetDateTime(dr.GetOrdinal("DatePosted")),
                    MessageContent = dr["MessageContent"].ToString(),
                    UserName = dr["UserName"].ToString(),
                    UserID = Convert.ToInt32(dr["UserID"]),
                });

                
            }

            //Console.WriteLine("ChatMessages");
            foreach (var message in ChatMessages)
            {
                //Console.WriteLine(message);
            }
            return ChatMessages;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetChatMessageByChatMessagIDAndAuthorID Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetChatMessageByChatMessagIDAndAuthorIDCommand(String spName, SqlConnection con,int AuthorID, int MessageID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorID", AuthorID);
        cmd.Parameters.AddWithValue("@MessageID", MessageID);
        return cmd;
    }



    // SP_F_DoesAuthorExistInDB 

    //--------------------------------------------------------------------------------------------------
    // DoesAuthorExistInDB 
    //--------------------------------------------------------------------------------------------------
    public bool DoesAuthorExistInDB(string AuthorName)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildDoesAuthorExistInDBCommand("SP_F_DoesAuthorExistInDB", con, AuthorName); // create the command
        try
        {
            int count = cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // DoesAuthorExistInDB Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildDoesAuthorExistInDBCommand(String spName, SqlConnection con, string AuthorName)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorName", AuthorName);
        return cmd;
    }

    // SP_InsertNewAuthorAndConnectToBook


    //--------------------------------------------------------------------------------------------------
    // InsertNewAuthorAndConnectToBook
    //--------------------------------------------------------------------------------------------------
    public bool InsertNewAuthorAndConnectToBook(string AuthorName, string Summary, string Image, string Link, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildInsertNewAuthorAndConnectToBookCommand("SP_F_InsertNewAuthorAndConnectToBook", con, AuthorName, Summary, Image, Link, BookID); // create the command
        try
        {
            //Console.WriteLine($"AuthorName: {AuthorName}, Summary: {Summary}, Image: {Image}, Link: {Link}, BookID: {BookID}");
            //Console.WriteLine("Entering InsertNewAuthorAndConnectToBook for author: " + AuthorName);
            int count = cmd.ExecuteNonQuery(); // execute the command and get the result
            //Console.WriteLine("Exiting InsertNewAuthorAndConnectToBook for author: " + AuthorName);

            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // InsertNewAuthorAndConnectToBook Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildInsertNewAuthorAndConnectToBookCommand(String spName, SqlConnection con, string AuthorName, string Summary, string Image, string Link, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@AuthorName", AuthorName);
        cmd.Parameters.AddWithValue("@Summary", Summary);
        cmd.Parameters.AddWithValue("@Image", Image);
        cmd.Parameters.AddWithValue("@Link", Link);
        cmd.Parameters.AddWithValue("@BookID", BookID);

        return cmd;
    }









    //--------------------------------------------------------------------------------------------------
    // 
    // *Books*
    //
    //--------------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------
    // GetAllDbBooksAdHoc  (for the home page)
    //---------------------------------------------------------------------------------
    public Object GetAllDbBooksAdHoc()
    {

        SqlConnection con = null;
        List<Object> allDbBooksAdHoc = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildReadCommand("SP_F_GetAllDbBooksAdHoc", con); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                allDbBooksAdHoc.Add(new
                {
                    Title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    Authors = dr["authors"] == DBNull.Value || dr["authors"] == null || string.IsNullOrWhiteSpace(dr["authors"].ToString()) ? "No authors mentioned." : dr["authors"].ToString(),
                    Thumbnail = dr["thumbnail"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["thumbnail"],
                    Description = dr["description"] is DBNull ? "No description available." : (string)dr["description"],
                    BookID = Convert.ToInt32(dr["BookID"])
                });
            }
            return allDbBooksAdHoc;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // GetBooksByTitle  (for the search page)
    //---------------------------------------------------------------------------------
    public Object GetBooksByTitle(string query)
    {
        SqlConnection con = null;
        List<Object> booksByTitle = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection

            SqlCommand cmd = BuildGetBooksByTitleCommandHelper("SP_F_GetBooksByTitle", con, query);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row, then True if there are more rows
            {
                booksByTitle.Add(new
                {
                    Title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    Authors = dr["authors"] == DBNull.Value || dr["authors"] == null || string.IsNullOrWhiteSpace(dr["authors"].ToString()) ? "No authors mentioned." : dr["authors"].ToString(),
                    Thumbnail = dr["thumbnail"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["thumbnail"],
                    Description = dr["description"] is DBNull ? "No description available." : (string)dr["description"],
                    BookID = Convert.ToInt32(dr["BookID"])
                });
            }
            return booksByTitle;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetBooksByTitle Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetBooksByTitleCommandHelper(String spName, SqlConnection con, string query)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Query", query);
        return cmd;
    }



    //---------------------------------------------------------------------------------
    // GetBooksByWords  (for the search page)
    //---------------------------------------------------------------------------------
    public Object GetBooksByWords(string query)
    {
        SqlConnection con = null;
        List<Object> booksByTitle = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection

            SqlCommand cmd = BuildGetBooksByWordsCommandHelper("SP_F_GetBooksByWords", con, query);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row, then True if there are more rows
            {
                booksByTitle.Add(new
                {
                    Title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    Authors = dr["authors"] == DBNull.Value || dr["authors"] == null || string.IsNullOrWhiteSpace(dr["authors"].ToString()) ? "No authors mentioned." : dr["authors"].ToString(),
                    Thumbnail = dr["thumbnail"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["thumbnail"],
                    Description = dr["description"] is DBNull ? "No description available." : (string)dr["description"],
                    BookID = Convert.ToInt32(dr["BookID"])
                });
            }
            return booksByTitle;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetBooksByWords Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetBooksByWordsCommandHelper(String spName, SqlConnection con, string query)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Query", query);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetAllBookInfo  (for the book page)
    //---------------------------------------------------------------------------------
    public Object GetAllBookInfo(int id)
    {
        SqlConnection con = null;
        Object bookInfoAdHoc = null;

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetAllBookInfoCommandHelper("SP_F_GetAllBookInfo", con, id); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            if (dr.Read()) // Read till the end of the data into a row, then True if there are more rows
            {
                bookInfoAdHoc = new
                {
                    Title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    Subtitle = dr["subtitle"] is DBNull ? string.Empty : (string)dr["subtitle"],
                    Description = dr["description"] is DBNull ? string.Empty : (string)dr["description"],
                    Publisher = dr["publisher"] is DBNull ? string.Empty : (string)dr["publisher"],
                    PublishedDate = dr["publishedDate"] is DBNull ? (DateTime?)null : (DateTime)dr["publishedDate"],
                    PrintType = dr["printType"] is DBNull ? string.Empty : (string)dr["printType"],
                    PageCount = dr["pageCount"] is DBNull ? (int?)null : Convert.ToInt32(dr["pageCount"]),
                    PreviewLink = dr["previewLink"] is DBNull ? string.Empty : (string)dr["previewLink"],
                    MaturityRating = dr["maturityRating"] is DBNull ? string.Empty : (string)dr["maturityRating"],
                    Language = dr["language"] is DBNull ? string.Empty : (string)dr["language"],
                    InfoLink = dr["infoLink"] is DBNull ? string.Empty : (string)dr["infoLink"],
                    Isbn10 = dr["ISBN_10"] is DBNull ? string.Empty : (string)dr["ISBN_10"],
                    Isbn13 = dr["ISBN_13"] is DBNull ? string.Empty : (string)dr["ISBN_13"],
                    SmallThumbnail = dr["smallThumbnail"] is DBNull ? "Image link unavailable." : (string)dr["smallThumbnail"],
                    Thumbnail = dr["thumbnail"] is DBNull ? "Image link unavailable." : (string)dr["thumbnail"],
                    CanonicalVolumeLink = dr["canonicalVolumeLink"] is DBNull ? string.Empty : (string)dr["canonicalVolumeLink"],
                    SelfLink = dr["selfLink"] is DBNull ? string.Empty : (string)dr["selfLink"],
                    IsEbook = dr["isEbook"] is DBNull ? false : Convert.ToBoolean(dr["isEbook"]),
                    DownloadLink = dr["downloadLink"] is DBNull ? string.Empty : (string)dr["downloadLink"],
                    Price = dr["price"] is DBNull ? (float?)null : (float)Math.Round(Convert.ToSingle(dr["price"]), 2),
                    BookID = Convert.ToInt32(dr["BookID"]),
                    IsActive = dr["IsActive"] is DBNull ? false : Convert.ToBoolean(dr["IsActive"]),
                    Categories = dr["Categories"] is DBNull ? "No categories specified." : (string)dr["Categories"],
                    Authors = dr["Authors"] is DBNull ? "No authors specified." : (string)dr["Authors"]
                };
            }
            return bookInfoAdHoc;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetAllBookInfo Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetAllBookInfoCommandHelper(String spName, SqlConnection con, int id)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@BookID", id);
        return cmd;
    }






    // not needed here

    //---------------------------------------------------------------------------------
    // GetAllBookCategories
    //---------------------------------------------------------------------------------
    public Object GetAllBookCategories(int bookID)
    {
        SqlConnection con = null;
        List<Object> allBooksCategories = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildGetAllBookCategoriesCommand("SP_F_GetAllBookCategories", con, bookID); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished


            while (dr.Read()) // Read till the end of the data into a row , then True if there are more rows
            {
                allBooksCategories.Add(new
                {
                    CategoryName = dr["CategoryName"] is DBNull ? "No categories specified." : (string)dr["CategoryName"],
                });
            }
            return allBooksCategories;

        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetAllBookInfo Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetAllBookCategoriesCommand(String spName, SqlConnection con, int id)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@BookID", id);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // GetFlashSaleBooks
    //---------------------------------------------------------------------------------
    public Object GetFlashSaleBooks()
    {
        SqlConnection con = null;
        List<Object> BooksOnSale = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildReadCommand("SP_F_GetFlashSaleBooks", con); // call the stored procedure
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // execute and read data

            while (dr.Read())
            {
                BooksOnSale.Add(new
                {
                    BookID = dr["BookID"] != DBNull.Value ? (int)dr["BookID"] : 0,
                    Title = dr["title"] != DBNull.Value ? (string)dr["title"] : "Unknown Title",
                    Thumbnail = dr["thumbnail"] != DBNull.Value ? (string)dr["thumbnail"] : "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg",
                    OriginalPrice = dr["OriginalPrice"] != DBNull.Value ? (decimal)dr["OriginalPrice"] : 0.00m,
                    SalePrice = dr["SalePrice"] != DBNull.Value ? (decimal)dr["SalePrice"] : 0.00m
                });
            }
            return BooksOnSale;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }



    /// //////////////////////////////////// ONLY FOR THE CASE OF A SERVER INITIATED SALE  ////////////////////////////////////////////////////////////////////////



    //---------------------------------------------------------------------------------
    // Apply5BookDiscountForSale
    //---------------------------------------------------------------------------------
    public async Task<List<Object>> Apply5BookDiscountForSale()
    {
        SqlConnection con = null;
        List<Object> discountedBooks = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection

            SqlCommand cmd = BuildReadCommand("SP_F_Apply5BookDiscountForSale", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader dr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                discountedBooks.Add(new
                {
                    bookID = Convert.ToInt32(dr["BookID"]),
                    title = dr["title"] is DBNull ? string.Empty : (string)dr["title"],
                    originalPrice = dr["OriginalPrice"] is DBNull ? 0.00m : Convert.ToDecimal(dr["OriginalPrice"]),
                    salePrice = dr["SalePrice"] is DBNull ? 0.00m : Convert.ToDecimal(dr["SalePrice"]),
                    thumbnail = dr["thumbnail"] is DBNull ? "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg" : (string)dr["thumbnail"]
                });
            }
            return discountedBooks;
        }
        catch (Exception ex)
        {
            // Handle or log the exception as necessary
            throw; // rethrow the exception
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // Revert5BookDiscoutForSale
    //---------------------------------------------------------------------------------
    public async Task<bool> Revert5BookDiscoutForSale()
    {
        SqlConnection con = null;

        try
        {
            con = connect("myProjDB");

            SqlCommand cmd = BuildReadCommand("SP_F_Revert5BookDiscoutForSale", con);
            cmd.CommandType = CommandType.StoredProcedure;

            int rowsAffected = cmd.ExecuteNonQuery(); // ExecuteNonQuery will return the number of rows affected

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }




    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// //////////////////////////////////// FRONT END INITIATED SALE  ////////////////////////////////////////////////////////////////////////

    //---------------------------------------------------------------------------------
    // GetBookQuestion_WhichDatePublished  (for the quiz page)
    //---------------------------------------------------------------------------------
    public Object GetBookQuestion_WhichDatePublished()
    {
        SqlConnection con = null;
        List<DateTime?> incorrectAnswers = new List<DateTime?>();
        DateTime? correctAnswer = null;
        string bookTitle = null;
        try
        {
            con = connect("myProjDB"); // create the connection

            SqlCommand cmd = BuildReadCommand("SP_F_GetBookQuestion_WhichDatePublished", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row
            {
                bookTitle = dr["BookTitle"].ToString(); // Get the book title
                string answerType = dr["AnswerType"].ToString();
                DateTime? answer = dr["Answer"] != DBNull.Value ? (DateTime?)dr["Answer"] : null;

                if (answerType == "Correct")
                {
                    correctAnswer = answer;
                }
                else if (answerType == "Incorrect")
                {
                    incorrectAnswers.Add(answer);
                }
            }

            // Ensure we have 3 incorrect answers
            while (incorrectAnswers.Count < 3)
            {
                // Add a placeholder for missing answers
                incorrectAnswers.Add(null);
            }

            return new
            {
                BookTitle = bookTitle,
                CorrectAnswer = correctAnswer,
                IncorrectAnswers = incorrectAnswers
            };
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }

    //---------------------------------------------------------------------------------
    // GetBookQuestion_WhichPublisher (for the quiz page)
    //---------------------------------------------------------------------------------
    public Object GetBookQuestion_WhichPublisher()
    {
        SqlConnection con = null;
        List<string> incorrectAnswers = new List<string>();
        string correctPublisher = null;
        string bookTitle = null;
        try
        {
            con = connect("myProjDB"); // create the connection

            SqlCommand cmd = BuildReadCommand("SP_F_BookQuestion_WhichPublisher", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row
            {
                bookTitle = dr["BookTitle"].ToString(); // Get the book title
                string answerType = dr["AnswerType"].ToString();
                string publisher = dr["Publisher"].ToString();

                if (answerType == "Correct")
                {
                    correctPublisher = publisher;
                }
                else if (answerType == "Incorrect")
                {
                    incorrectAnswers.Add(publisher);
                }
            }

            // Ensure we have 3 incorrect answers
            while (incorrectAnswers.Count < 3)
            {
                // Add a placeholder for missing answers
                incorrectAnswers.Add("Dummy Answer");
            }

            return new
            {
                BookTitle = bookTitle,
                CorrectAnswer = correctPublisher,
                IncorrectAnswers = incorrectAnswers
            };
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // GetBookQuestion_WhichPageCount (for the quiz page)
    //---------------------------------------------------------------------------------
    public Object GetBookQuestion_WhichPageCount()
    {
        SqlConnection con = null;
        List<int?> incorrectAnswers = new List<int?>();
        int? correctPageCount = null;
        string bookTitle = null;

        try
        {
            con = connect("myProjDB"); // Create the connection

            SqlCommand cmd = BuildReadCommand("SP_F_GetBookQuestion_WhichPageCount", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row
            {
                bookTitle = dr["BookTitle"].ToString(); // Get the book title
                string answerType = dr["AnswerType"].ToString();
                int? pageCount = dr["PageCount"] != DBNull.Value ? (int?)dr["PageCount"] : null;

                if (answerType == "Correct")
                {
                    correctPageCount = pageCount;
                }
                else if (answerType == "Incorrect")
                {
                    incorrectAnswers.Add(pageCount);
                }
            }

            // Ensure we have 3 incorrect answers
            while (incorrectAnswers.Count < 3)
            {
                // Add a placeholder for missing answers
                incorrectAnswers.Add(0);
            }

            return new
            {
                BookTitle = bookTitle,
                CorrectAnswer = correctPageCount,
                IncorrectAnswers = incorrectAnswers
            };
        }
        catch (Exception ex)
        {
            throw ex; // Write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // Close the db connection
            }
        }
    }

    //---------------------------------------------------------------------------------
    // GetBookQuestion_WhichAuthor (for the quiz page)
    //---------------------------------------------------------------------------------
    public Object GetBookQuestion_WhichAuthor()
    {
        SqlConnection con = null;
        List<string> incorrectAuthors = new List<string>();
        string correctAuthor = null;
        string bookTitle = null;

        try
        {
            con = connect("myProjDB"); // Create the connection

            SqlCommand cmd = BuildReadCommand("SP_F_GetBookQuestion_WhichAuthor", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row
            {
                bookTitle = dr["BookTitle"].ToString(); // Get the book title
                string answerType = dr["AnswerType"].ToString();
                string author = dr["Author"].ToString();

                if (answerType == "Correct")
                {
                    correctAuthor = author;
                }
                else if (answerType == "Incorrect")
                {
                    incorrectAuthors.Add(author);
                }
            }

            // Ensure we have 3 incorrect answers
            while (incorrectAuthors.Count < 3)
            {
                // Add a placeholder for missing answers
                incorrectAuthors.Add("Dummy Answer");
            }

            return new
            {
                BookTitle = bookTitle,
                CorrectAnswer = correctAuthor,
                IncorrectAnswers = incorrectAuthors
            };
        }
        catch (Exception ex)
        {
            throw ex; // Write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // Close the db connection
            }
        }
    }

    //---------------------------------------------------------------------------------
    // GetAuthorQuestion_WhichAuthorImage (for the quiz page)
    //---------------------------------------------------------------------------------
    public Object GetAuthorQuestion_WhichAuthorImage()
    {
        SqlConnection con = null;
        List<Tuple<string, string>> incorrectAuthors = new List<Tuple<string, string>>();
        string correctAuthor = null;
        string authorImage = null;

        try
        {
            con = connect("myProjDB"); // Create the connection

            SqlCommand cmd = BuildReadCommand("SP_F_GetAuthorQuestion_WhichAuthorImage", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row
            {
                string answerType = dr["AnswerType"].ToString();
                string authorName = dr["AuthorName"].ToString();
                string image = dr["AuthorImage"] != DBNull.Value ? dr["AuthorImage"].ToString() : null;

                if (answerType == "Correct")
                {
                    correctAuthor = authorName;
                    authorImage = image;
                }
                else if (answerType == "Incorrect")
                {
                    incorrectAuthors.Add(new Tuple<string, string>(authorName, image));
                }
            }

            // Ensure we have 3 incorrect answers
            while (incorrectAuthors.Count < 3)
            {
                // Add a placeholder for missing answers
                incorrectAuthors.Add(new Tuple<string, string>("Unknown", null));
            }

            return new
            {
                CorrectAnswer = correctAuthor,
                AuthorImage = authorImage,
                IncorrectAnswers = incorrectAuthors
            };
        }
        catch (Exception ex)
        {
            throw ex; // Write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // Close the db connection
            }
        }
    }


    //SP_F_CheckIfBookIsInDB 

    //--------------------------------------------------------------------------------------------------
    // CheckIfBookIsInDB 
    //--------------------------------------------------------------------------------------------------
    public bool CheckIfBookIsInDB(string title, string isbn_10, string isbn_13)
    {
        SqlConnection con = null;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // log exception
        }
        cmd = BuildCheckIfBookIsInDBCommand("SP_F_CheckIfBookIsInDB", con, title, isbn_10, isbn_13); // create the command
        try
        {
            // ExecuteScalar is used to get the first column of the first row in the result set
            int result = (int)cmd.ExecuteScalar();
            //Console.Write(result);
            return (result > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // log exception
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // CheckIfBookIsInDB  Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildCheckIfBookIsInDBCommand(String spName, SqlConnection con, string title, string isbn_10, string isbn_13)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Title", title);
        cmd.Parameters.AddWithValue("@ISBN10", isbn_10);
        cmd.Parameters.AddWithValue("@ISBN13", isbn_13);


        return cmd;
    }


    // SP_F_AddNewBookToDB

    //---------------------------------------------------------------------------------
    // AddNewBookToDB
    //---------------------------------------------------------------------------------

    public int AddNewBookToDB(string title, string subtitle, string description, string publisher, DateTime publishedDate, string printType, int pageCount, string previewLink, string maturityRating, string language, string infoLink, string ISBN_10, string ISBN_13, string smallThumbnail, string thumbnail, string canonicalVolumeLink, string selfLink, bool isEbook, string downloadLink, double price)
    {
        SqlConnection con = null;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
            cmd = BuildAddNewBookToDBCommand("SP_F_AddNewBookToDB", con, title, subtitle, description, publisher, publishedDate, printType, pageCount, previewLink, maturityRating, language, infoLink, ISBN_10, ISBN_13, smallThumbnail, thumbnail, canonicalVolumeLink, selfLink, isEbook, downloadLink, price); // create the command

            // Logging the SQL command and parameters
            //Console.WriteLine(cmd.CommandText);
            foreach (SqlParameter param in cmd.Parameters)
            {
                //Console.WriteLine($"{param.ParameterName}: {param.Value}");
            }

            // Execute the command and get the new BookID
            int bookID = (int)cmd.ExecuteScalar();
            return bookID;
        }
        catch (SqlException sqlEx)
        {
            // Log SQL-specific exceptions
            //Console.WriteLine($"SQL Exception: {sqlEx.Message}");
            throw; // rethrow the exception after logging
        }
        catch (Exception ex)
        {
            // Log general exceptions
            //Console.WriteLine($"Exception: {ex.Message}");
            throw; // rethrow the exception after logging
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // AddNewBookToDB Helper
    //---------------------------------------------------------------------------------


    private SqlCommand BuildAddNewBookToDBCommand(String spName, SqlConnection con, string title, string subtitle, string description, string publisher, DateTime publishedDate, string printType, int pageCount, string previewLink, string maturityRating, string language, string infoLink, string ISBN_10, string ISBN_13, string smallThumbnail, string thumbnail, string canonicalVolumeLink, string selfLink, bool isEbook, string downloadLink, double price)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;          // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution; the default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Title", title);
        cmd.Parameters.AddWithValue("@Subtitle", string.IsNullOrEmpty(subtitle) ? (object)DBNull.Value : subtitle);
        cmd.Parameters.AddWithValue("@Description", description);
        cmd.Parameters.AddWithValue("@Publisher", publisher);
        cmd.Parameters.AddWithValue("@PublishedDate", publishedDate); // regular DateTime
        cmd.Parameters.AddWithValue("@PrintType", printType);
        cmd.Parameters.AddWithValue("@PageCount", pageCount);
        cmd.Parameters.AddWithValue("@PreviewLink", previewLink);
        cmd.Parameters.AddWithValue("@MaturityRating", maturityRating);
        cmd.Parameters.AddWithValue("@Language", language);
        cmd.Parameters.AddWithValue("@InfoLink", infoLink);
        cmd.Parameters.AddWithValue("@ISBN_10", string.IsNullOrEmpty(ISBN_10) ? (object)DBNull.Value : ISBN_10); // Handle null ISBN
        cmd.Parameters.AddWithValue("@ISBN_13", string.IsNullOrEmpty(ISBN_13) ? (object)DBNull.Value : ISBN_13); // Handle null ISBN
        cmd.Parameters.AddWithValue("@SmallThumbnail", smallThumbnail);
        cmd.Parameters.AddWithValue("@Thumbnail", thumbnail);
        cmd.Parameters.AddWithValue("@CanonicalVolumeLink", canonicalVolumeLink);
        cmd.Parameters.AddWithValue("@SelfLink", selfLink);
        cmd.Parameters.AddWithValue("@IsEbook", isEbook);
        cmd.Parameters.AddWithValue("@DownloadLink", string.IsNullOrEmpty(downloadLink) ? (object)DBNull.Value : downloadLink);
        cmd.Parameters.AddWithValue("@Price", price);
        return cmd;
    }




    // SP_F_InsertNewCategoryAndConnectToBook


    //--------------------------------------------------------------------------------------------------
    // InsertNewCategoryAndConnectToBook 
    //--------------------------------------------------------------------------------------------------
    public bool InsertNewCategoryAndConnectToBook(string CategoryName, int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildInsertNewCategoryAndConnectToBookCommand("SP_F_InsertNewCategoryAndConnectToBook", con,  CategoryName, BookID); // create the command
        try
        {
            int count = (int)cmd.ExecuteNonQuery(); // execute the command and get the result
            return (count > 0);
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // InsertNewCategoryAndConnectToBook  Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildInsertNewCategoryAndConnectToBookCommand(String spName, SqlConnection con, string CategoryName, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@CategoryName", CategoryName);
        cmd.Parameters.AddWithValue("@BookID", BookID);


        return cmd;
    }






    //--------------------------------------------------------------------------------------------------
    // 
    // *Rating*
    //
    //--------------------------------------------------------------------------------------------------


    // ---------------------------------------------------------------------------------
    // InsertuserReview 
    // ---------------------------------------------------------------------------------
    public int InsertUserReview(Rating rating)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildInsertUserReviewCommand("SP_F_InsertuserReview", con, rating); // create the command

        try
        {
            int output = (int)cmd.ExecuteScalar(); // execute the command
            return output; // return true if any rows were affected
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    // ---------------------------------------------------------------------------------
    // InsertuserReview Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildInsertUserReviewCommand(String spName, SqlConnection con, Rating rating)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", rating.UserID);
        cmd.Parameters.AddWithValue("@BookID", rating.BookID);
        cmd.Parameters.AddWithValue("@Rating", rating.RatingStars);
        cmd.Parameters.AddWithValue("@Description", rating.Description);
        cmd.Parameters.AddWithValue("@DateRated", rating.DateCreated);
        cmd.Parameters.AddWithValue("@Header", rating.Header);
        return cmd;
    }

    // ---------------------------------------------------------------------------------
    // GetUserRatingReview 
    // ---------------------------------------------------------------------------------
    public int GetUserRatingReview(int userID, int ratingID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildGetUserRatingReviewCommand("SP_F_GetUserRatingReview", con, userID, ratingID); // create the command

        try
        {
            int result = (int)cmd.ExecuteScalar(); // execute the command
            return result; // return the result (1, -1, or 0)
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    // ---------------------------------------------------------------------------------
    // GetUserRatingReview Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildGetUserRatingReviewCommand(String spName, SqlConnection con, int userID, int ratingID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", userID);
        cmd.Parameters.AddWithValue("@RatingID", ratingID);
        return cmd;
    }



    // ---------------------------------------------------------------------------------
    // GetUsersBookReview 
    // ---------------------------------------------------------------------------------
    public Object GetUsersBookReview(int userID, int bookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Object> usersBookRating = new List<Object>();
        try
        {
            con = connect("myProjDB"); // Create the connection
            cmd = BuildGetUsersBookReviewCommand("SP_F_GetUsersBookReview", con, userID, bookID); // Create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read())
            {
                // Read the review details from the data reader
                var tempRating = new
                {
                    RatingID = dr.IsDBNull(dr.GetOrdinal("RatingID")) ? (int?)null : dr.GetInt32(dr.GetOrdinal("RatingID")),
                    UserID = dr.GetInt32(dr.GetOrdinal("UserID")),
                    BookID = dr.GetInt32(dr.GetOrdinal("BookID")),
                    Rating = dr.IsDBNull(dr.GetOrdinal("RatingStars")) ? (int?)null : dr.GetByte(dr.GetOrdinal("RatingStars")),
                    Description = dr.IsDBNull(dr.GetOrdinal("Description")) ? null : dr.GetString(dr.GetOrdinal("Description")),
                    DateRated = dr.IsDBNull(dr.GetOrdinal("DateCreated")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("DateCreated")),
                    Header = dr.IsDBNull(dr.GetOrdinal("Header")) ? null : dr.GetString(dr.GetOrdinal("Header")),
                    TotalScore = Convert.ToInt32(dr["TotalScore"])
                };
                usersBookRating.Add(tempRating);
            }
            return usersBookRating;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
    }
    // ---------------------------------------------------------------------------------
    // GetUsersBookReview Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildGetUsersBookReviewCommand(String spName, SqlConnection con, int userID, int bookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", userID);
        cmd.Parameters.AddWithValue("@BookID", bookID);
        return cmd;
    }


    // ---------------------------------------------------------------------------------
    // GetUsersAllReviews 
    // ---------------------------------------------------------------------------------
    public Object GetUsersAllReviews(int userID)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Object> usersBookRatings = new List<Object>();
        try
        {
            con = connect("myProjDB"); // Create the connection
            cmd = BuildGetUsersAllReviewsCommand("SP_F_GetUsersAllReviews", con, userID); // Create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read())
            {
                // Read the review details from the data reader
                var tempRating = new
                {
                    RatingID = dr.IsDBNull(dr.GetOrdinal("RatingID")) ? (int?)null : dr.GetInt32(dr.GetOrdinal("RatingID")),
                    UserID = dr.GetInt32(dr.GetOrdinal("UserID")),
                    BookID = dr.GetInt32(dr.GetOrdinal("BookID")),
                    BookName = dr.IsDBNull(dr.GetOrdinal("BookName")) ? null : dr.GetString(dr.GetOrdinal("BookName")),
                    Rating = dr.IsDBNull(dr.GetOrdinal("RatingStars")) ? (int?)null : dr.GetByte(dr.GetOrdinal("RatingStars")),
                    Description = dr.IsDBNull(dr.GetOrdinal("Description")) ? null : dr.GetString(dr.GetOrdinal("Description")),
                    DateRated = dr.IsDBNull(dr.GetOrdinal("DateCreated")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("DateCreated")),
                    Header = dr.IsDBNull(dr.GetOrdinal("Header")) ? null : dr.GetString(dr.GetOrdinal("Header")),
                    TotalScore = dr.IsDBNull(dr.GetOrdinal("TotalScore")) ? (int?)null : dr.GetInt32(dr.GetOrdinal("TotalScore"))
                };
                usersBookRatings.Add(tempRating);
            }
            return usersBookRatings;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
    }
    // ---------------------------------------------------------------------------------
    // GetUsersAllReviews Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildGetUsersAllReviewsCommand(String spName, SqlConnection con, int userID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;          // the stored procedure name
        cmd.CommandTimeout = 10;           // Time to wait for the execution; the default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command

        cmd.Parameters.AddWithValue("@UserID", userID);
        return cmd;
    }

    // ---------------------------------------------------------------------------------
    // RateReview 
    // ---------------------------------------------------------------------------------
    public int RateReview(int userID, int ratingID, int score)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildRateReviewCommand("SP_F_RateReview", con, userID, ratingID, score); // create the command

        try
        {
            int result = (int)cmd.ExecuteScalar(); // execute the command
            return result; // return the result (1, -1, or 0)
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    // ---------------------------------------------------------------------------------
    // RateReview Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildRateReviewCommand(String spName, SqlConnection con, int userID, int ratingID, int score)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", userID);
        cmd.Parameters.AddWithValue("@RatingID", ratingID);
        cmd.Parameters.AddWithValue("@Score", score);
        return cmd;
    }


    // ---------------------------------------------------------------------------------
    // GetTopFiveRatingsReviews 
    // ---------------------------------------------------------------------------------
    public Object GetTopFiveRatingsReviews(int userID)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Object> topRatings = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildGetTopFiveRatingsReviewsCommand("SP_F_GetTopFiveRatingsReviews", con, userID); // create the command
        SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

        try
        {
            while (dr.Read())
            {
                var tempRating = new
                {
                    RatingID = Convert.ToInt32(dr["RatingID"]),
                    UserID = Convert.ToInt32(dr["UserID"]),
                    UserName = (string)dr["UserName"],
                    BookID = Convert.ToInt32(dr["BookID"]),
                    BookTitle = (string)dr["BookTitle"],
                    RatingStars = Convert.ToInt32(dr["RatingStars"]),
                    Description = (string)dr["Description"],
                    DateCreated = dr.GetDateTime(dr.GetOrdinal("DateCreated")),
                    Header = (string)dr["Header"],
                    TotalScore = Convert.ToInt32(dr["TotalScore"]),
                    UserRating = dr["UserRating"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["UserRating"])
                };
                topRatings.Add(tempRating);
            }
            return topRatings;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    // ---------------------------------------------------------------------------------
    // GetTopFiveRatingsReviews Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildGetTopFiveRatingsReviewsCommand(String spName, SqlConnection con, int userID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", userID);
        return cmd;
    }




    // ---------------------------------------------------------------------------------
    // GetTopFiveRatingsReviewsForBook 
    // ---------------------------------------------------------------------------------
    public Object GetTopFiveRatingsReviewsForBook(int userID, int bookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Object> topRatings = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildGetTopFiveRatingsReviewsForBookCommand("SP_F_GetTopFiveRatingsReviewsForBook", con, userID, bookID); // create the command
        SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

        try
        {
            while (dr.Read())
            {
                var tempRating = new
                {
                    RatingID = Convert.ToInt32(dr["RatingID"]),
                    UserID = Convert.ToInt32(dr["UserID"]),
                    UserName = (string)dr["UserName"],
                    BookID = Convert.ToInt32(dr["BookID"]),
                    BookTitle = (string)dr["BookTitle"],
                    RatingStars = Convert.ToInt32(dr["RatingStars"]),
                    Description = (string)dr["Description"],
                    DateCreated = dr.GetDateTime(dr.GetOrdinal("DateCreated")),
                    Header = (string)dr["Header"],
                    TotalScore = Convert.ToInt32(dr["TotalScore"]),
                    UserRating = dr["UserRating"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["UserRating"])
                };
                topRatings.Add(tempRating);
            }
            return topRatings;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    // ---------------------------------------------------------------------------------
    // GetTopFiveRatingsReviewsForBook Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildGetTopFiveRatingsReviewsForBookCommand(String spName, SqlConnection con, int userID, int bookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", userID);
        cmd.Parameters.AddWithValue("@BookID", bookID);
        return cmd;
    }


    // SP_F_GetRatingWrittenDataForSentimentAnalysis

    // ---------------------------------------------------------------------------------
    // GetRatingWrittenDataForSentimentAnalysis 
    // ---------------------------------------------------------------------------------
    public Object GetRatingWrittenDataForSentimentAnalysis(int bookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Object> ratingsText = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildGetRatingWrittenDataForSentimentAnalysisCommand("SP_F_GetRatingWrittenDataForSentimentAnalysis", con, bookID); // create the command
        SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

        try
        {
            while (dr.Read())
            {
                var tempRating = new
                {
                    Header = (string)dr["Header"],
                    Description = (string)dr["Description"]
                };
                ratingsText.Add(tempRating);
            }
            return ratingsText;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    // ---------------------------------------------------------------------------------
    // GetRatingWrittenDataForSentimentAnalysis Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildGetRatingWrittenDataForSentimentAnalysisCommand(String spName, SqlConnection con, int bookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@BookID", bookID);
        return cmd;
    }


    // SP_F_GetAverageRatingForBook


    //--------------------------------------------------------------------------------------------------
    // GetAverageRatingForBook
    //--------------------------------------------------------------------------------------------------
    public float GetAverageRatingForBook(int BookID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        cmd = BuildGetAverageRatingForBookCommand("SP_F_GetAverageRatingForBook", con, BookID); // create the command
        try
        {
            decimal ratingDecimal = (decimal)cmd.ExecuteScalar(); // execute the command and get the result
            float rating = (float)ratingDecimal;
            return rating;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // GetAverageRatingForBook Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildGetAverageRatingForBookCommand(String spName, SqlConnection con, int BookID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@BookID", BookID);
        return cmd;
    }








    // ---------------------------------------------------------------------------------
    // GetAllUserNotifications 
    // ---------------------------------------------------------------------------------
    public Object GetAllUserNotifications(int userID)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Object> userNotifications = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildGetAllUserNotificationsCommand("SP_F_GetAllUserNotifications", con, userID); // create the command
        SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

        try
        {
            while (dr.Read())
            {
                var Notification = new
                {
                    NotificationID = Convert.ToInt32(dr["NotificationID"]),
                    UserID = Convert.ToInt32(dr["UserID"]),
                    TradeID = dr["TradeID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["TradeID"]),
                    Description = (string)dr["Description"],
                    DateCreated = dr.GetDateTime(dr.GetOrdinal("DateCreated")),
                    NotificationRead = Convert.ToBoolean(dr["NotificationRead"]), // TO BOOL
                    NotificationType = (string)dr["NotificationType"]
                };
                userNotifications.Add(Notification);
            }
            return userNotifications;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    // ---------------------------------------------------------------------------------
    // GetAllUserNotifications Helper
    // ---------------------------------------------------------------------------------
    private SqlCommand BuildGetAllUserNotificationsCommand(String spName, SqlConnection con, int userID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@UserID", userID);
        return cmd;
    }


    //---------------------------------------------------------------------------------
    // MarkNotificationAsRead
    //---------------------------------------------------------------------------------

    public bool MarkNotificationAsRead(int NotificationID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildMarkNotificationAsReadCommand("SP_F_MarkNotificationAsRead", con, NotificationID); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected > 0; // return true if any rows were affected
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }


    //---------------------------------------------------------------------------------
    // MarkNotificationAsRead Helper
    //---------------------------------------------------------------------------------


    private SqlCommand BuildMarkNotificationAsReadCommand(String spName, SqlConnection con, int NotificationID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@NotificationID", NotificationID);
        return cmd;
    }


    // SP_F_DeleteNotification

    //---------------------------------------------------------------------------------
    // MarkNotificationAsRead
    //---------------------------------------------------------------------------------
    public bool DeleteNotification(int NotificationID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        cmd = BuildDeleteNotificationCommand("SP_F_DeleteNotification", con, NotificationID); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected > 0; // return true if any rows were affected
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }

        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
    //---------------------------------------------------------------------------------
    // DeleteNotification Helper
    //---------------------------------------------------------------------------------
    private SqlCommand BuildDeleteNotificationCommand(String spName, SqlConnection con, int NotificationID)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object
        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@NotificationID", NotificationID);
        return cmd;
    }






    //---------------------------------------------------------------------------------
    // GetTop10UserOwnedBooks 
    //---------------------------------------------------------------------------------
    public Object GetTop10UserOwnedBooks()
    {
        SqlConnection con = null;
        List<Object> top10UserOwnedBooks = new List<Object>();

        try
        {
            con = connect("myProjDB"); // create the connection
            SqlCommand cmd = BuildReadCommand("SP_F_GetTop10UserOwnedBooks", con); // create the command
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has finished

            while (dr.Read()) // Read till the end of the data into a row, then True if there are more rows
            {
                top10UserOwnedBooks.Add(new
                {
                    bookID = Convert.ToInt32(dr["BookID"]),
                    title = dr["title"].ToString(),
                    thumbnail = dr["thumbnail"].ToString(),
                    author = dr["author"].ToString(),
                    description = dr["description"].ToString()
                });
            }
            return top10UserOwnedBooks;
        }
        catch (Exception ex)
        {
            throw (ex); // write to log
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // close the db connection
            }
        }
    }
}