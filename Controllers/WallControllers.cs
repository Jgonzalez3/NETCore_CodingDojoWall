using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Wall.Models;

namespace Wall.Controllers{

    public class WallController : Controller{
        private readonly DbConnector _dbConnector;
        public WallController(DbConnector connect){
            _dbConnector = connect;
        }

        [HttpGet]
        [Route("/wall")]
        public IActionResult DisplayWall(){
            int? UserId = HttpContext.Session.GetInt32("userid");
            if(UserId == null){
                return RedirectToAction("Index", "Home");
            }
            string UserName = HttpContext.Session.GetString("username");
            ViewBag.Username = UserName;
            ViewBag.Userid = UserId;
            List<Dictionary<string,object>> Allmessages = _dbConnector.Query("SELECT messages.id as messagesid, messages.messages, users.id, users.first_name, users.last_name, DATE_FORMAT(messages.created_at, '%M %D %Y') as messagedate FROM users JOIN messages ON users.id = messages.users_id ORDER BY messages.created_at DESC;");
            List<Dictionary<string,object>> Allcomments = _dbConnector.Query("SELECT comments.comment, DATE_FORMAT(comments.created_at, '%M %D %Y') as commentdate, messages.id as messagesid, concat(users.first_name, ' ', users.last_name) as commentname FROM users RIGHT JOIN comments ON users.id = comments.users_id JOIN messages on comments.messages_id = messages.id ORDER BY comments.created_at ASC");
            
            // ViewBag.Userid =  "Need to add user first name to nav bar"
            ViewBag.Messages = Allmessages;
            ViewBag.Comments = Allcomments;
            return View("Wall");
        }
        [HttpPost]
        [Route("wall/postmessage")]
        public IActionResult PostMessage(Messages NewMessage)
        {
            int? UserId = HttpContext.Session.GetInt32("userid");
            _dbConnector.Execute($"INSERT INTO messages(messages, users_id) VALUES('{NewMessage.Message}', {UserId});");
                TempData["register"] = "Registered";
            return RedirectToAction("DisplayWall");
        }
        [HttpPost]
        [Route("wall/postcomment")]
        public IActionResult PostComment(string comment, int messageid){
            int? UserId = HttpContext.Session.GetInt32("userid");
            _dbConnector.Execute($"INSERT INTO comments(comment, messages_id, users_id) VALUES('{comment}', {messageid}, {UserId});");
            return RedirectToAction("DisplayWall");
        }
    }
}