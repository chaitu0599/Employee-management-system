using LoginPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace LoginPage.Controllers
{
    
    public class EmployeeController : Controller
    {
        db dbop = new db();
        List<Tasks> tasks = new List<Tasks>();
        Tasks tk = new Tasks();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        private readonly ILogger<EmployeeController> _logger;
        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
            con.ConnectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Login;Integrated Security=True";
        }
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
                HttpContext.Session.SetString("empid", emp.empid.ToString());
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
        public IActionResult Edit(int id)
        {
            FetchData(id);
            return View(tk);
        }
        [HttpPost]
        public IActionResult Edit([Bind] Taskadd ta,int id)
        {
            int x = dbop.taskadd(ta,id);
            if (x == 1)
                TempData["msg"] = "Yes";
            else
                TempData["msg"] = "No";
            return View();
        }
        public IActionResult Createtask()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Createtask([Bind] Taskadd ta)
        {
            int x = dbop.taskadd(ta,HttpContext.Session.GetString("empid"));
            if (x == 1)
            {
                TempData["msg"] = "Yes";
            }
            else
            {
                TempData["msg"] = "No";
            }
            return View();
        }
        public IActionResult Viewtask()
        {
            FetchData();
            return View(tasks);
        }
        public IActionResult Delete(int id)
        {
            con.Open();
            com.Connection = con;
            var parameter = com.CreateParameter();
            parameter.Value = id;
            parameter.ParameterName = "@id";
            com.Parameters.Add(parameter);
            com.CommandText = "UPDATE Task1 SET Isactive='0' WHERE id=@id";
            com.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Viewtask");
        }
        public IActionResult Details(int id)
        {
            FetchData(id);
            return View(tk);
        }
        private void FetchData()
        {
            if (tasks.Count > 0)
            {
                tasks.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                var parameter = com.CreateParameter();
                parameter.Value = Int32.Parse(HttpContext.Session.GetString("empid"));
                parameter.ParameterName = "@empid";
                com.Parameters.Add(parameter);
                com.CommandText = "SELECT TOP (1000) [id],[Taskname],[Startdate],[Enddate],[Taskduration],[Teamname],[summary],[Taskdetails],[Riskdetails],[Risksolution] FROM [Login].[dbo].[Task1] WHERE [isactive]='1' AND empid=@empid";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    tasks.Add(new Tasks()
                    {
                        id = dr["id"].ToString()
                    ,
                        Taskname = dr["Taskname"].ToString()
                    ,
                        Startdate = dr["Startdate"].ToString()
                    ,
                        Enddate = dr["Enddate"].ToString()
                    ,
                        Duration = dr["Taskduration"].ToString()
                    ,
                        Teamname = dr["Teamname"].ToString()
                    ,
                        Taskdetails = dr["Taskdetails"].ToString()
                    ,
                        Summary = dr["summary"].ToString()
                    ,
                        Riskdetails = dr["Riskdetails"].ToString()
                    ,
                        Risksolution = dr["Risksolution"].ToString(),
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void FetchData(int id)
        {
            try
            {
                con.Open();
                com.Connection = con;

                var parameter = com.CreateParameter();
                parameter.Value = id;
                parameter.ParameterName = "@id";
                com.Parameters.Add(parameter);

                com.CommandText = "Select [id],[Taskname],[StartDate],[Enddate],[Taskduration],[Teamname],[summary],[Taskdetails],[Riskdetails],[Risksolution] from Task1 where id = @id";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    tk = new Tasks()
                    {
                        id = dr["id"].ToString()
                    ,
                        Taskname = dr["Taskname"].ToString()
                    ,
                        Startdate = dr["Startdate"].ToString()
                    ,
                        Enddate = dr["Enddate"].ToString()
                    ,
                        Duration = dr["Taskduration"].ToString()
                    ,
                        Teamname = dr["Teamname"].ToString()
                    ,
                        Taskdetails = dr["Taskdetails"].ToString()
                    ,
                        Riskdetails = dr["Riskdetails"].ToString()
                    ,
                        Risksolution = dr["Risksolution"].ToString()
                    ,
                        Summary = dr["summary"].ToString(),
                    };
                }
                con.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
