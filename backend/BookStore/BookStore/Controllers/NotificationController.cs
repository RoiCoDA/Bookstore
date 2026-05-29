using BookStore.BL;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        // GET: api/<NotificationController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<NotificationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("GetAllUserNotifications")]
        public Object GetAllUserNotifications(int userID)
        {
            return Notification.GetAllUserNotifications(userID);
        }

        // POST api/<NotificationController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<NotificationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        [HttpPut("MarkNotificationAsRead")]
        public bool MarkNotificationAsRead(int notificationID) 
        {
            return Notification.MarkNotificationAsRead(notificationID); 
        }


        [HttpDelete("DeleteNotification")]
        public bool DeleteNotification(int NotificationID)
        {
            return Notification.DeleteNotification(NotificationID);
        }

        // DELETE api/<NotificationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
