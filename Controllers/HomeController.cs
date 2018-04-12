using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Wall.Models;

namespace Wall.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnector _dbConnector;
        public HomeController(DbConnector connect){
            _dbConnector = connect;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
        [HttpPost]
        [Route("/register")]
        public IActionResult Register(Users Reg){
            if(ModelState.IsValid){
                List<Dictionary<string,object>> Allusers = _dbConnector.Query($"SELECT * FROM users WHERE email = '{Reg.Email}';");
                foreach(var user in Allusers){
                    if((string)user["email"] == Reg.Email){
                        TempData["emailinuse"] = "Email is already in use";
                        return RedirectToAction("Index");
                    }
                }
                _dbConnector.Execute($"INSERT INTO users(first_name, last_name, email, password) VALUES('{Reg.FirstName}', '{Reg.LastName}', '{Reg.Email}', '{Reg.Password}');");
                TempData["register"] = "Registered";
                int? UserId = HttpContext.Session.GetInt32("userid");
                List<Dictionary<string,object>> Regusers = _dbConnector.Query($"SELECT * FROM users WHERE email = '{Reg.Email}'");
                foreach (var uid in Regusers){
                    UserId = (Int32)uid["id"];
                    string username = (string)uid["first_name"];
                    string UserName = HttpContext.Session.GetString("username");
                    HttpContext.Session.SetString("username", username);
                    Console.WriteLine(UserId);
                };
                HttpContext.Session.SetInt32("userid", (int)UserId);
                return RedirectToAction("DisplayWall", "Wall");
            }
            return View("Index");
        }
        [HttpPost]
        [Route("/login")]
        public IActionResult Login(Users Login){
            int? UserId = HttpContext.Session.GetInt32("userid");
            List<Dictionary<string,object>> Allusers = _dbConnector.Query($"SELECT * FROM users WHERE email = '{Login.Email}';");
            foreach(var user in Allusers){
                if((string)user["password"] == Login.Password){
                    TempData["login"] = "Login";
                    string username = (string)user["first_name"];
                    string UserName = HttpContext.Session.GetString("username");
                    HttpContext.Session.SetString("username", username);
                    UserId = (Int32)user["id"];
                    HttpContext.Session.SetInt32("userid", (int)UserId);
                    Console.WriteLine(UserId);
                    return RedirectToAction("DisplayWall", "Wall");
                }
            }
            return View("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
