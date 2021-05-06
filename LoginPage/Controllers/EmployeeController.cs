using LoginPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginPage.Controllers
{
    public class EmployeeController : Controller
    {
        db dbop = new db();
        public IActionResult Emplogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Emplogin([Bind] Emp_login emp)
        {
            int res = dbop.LoginCheck(emp);
            if (res == 1)
            {
                HttpContext.Session.SetString("username", emp.Username);
                string xi = HttpContext.Session.GetString("username");
                TempData["user"] = xi;
                TempData["msg"] = "Yes";
            }
            else
            {
                TempData["msg"] = "No";
            }
            return View();
        }
        public IActionResult empop()
        {
            Emp_login a = new Emp_login
            {
                Username = HttpContext.Session.GetString("username")
            };
            return View(a);
        }
        public IActionResult newe()
        {
            return View();
        }
        [HttpPost]
        public IActionResult newe([Bind] Newemp n)
        {
            int res = dbop.RegisterCheck(n);
            if (res == 1)
            {
                TempData["msg"] = "Yes";
            }
            else
            {
                TempData["msg"] = "No";
            }
            return View();
        }
    }
}
