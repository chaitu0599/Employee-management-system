using LoginPage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Session;
using RestSharp;
using System.Data;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;

namespace LoginPage.Controllers
{
    public class HomeController : Controller
    {
        db dbop = new db();
        empdb emdb = new empdb();
        SqlCommand com = new SqlCommand();
        SqlCommand com1 = new SqlCommand();
        SqlDataReader dr,dr1;
        SqlConnection con = new SqlConnection();
        SqlConnection con1 = new SqlConnection();
        employee emp = new employee();
        List<employee> employees = new List<employee>();
        List<Tasks> tasks = new List<Tasks>();
        getdb gtop = new getdb();
        List<Fetchleaves> fl = new List<Fetchleaves>();
        Fetchleaves l = new Fetchleaves();
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            con.ConnectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Login;Integrated Security=True";
            con1.ConnectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Login;Integrated Security=True";
            _webHostEnvironment = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Adlogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Adlogin([Bind] Ad_login ad)
        {
            int res = dbop.LoginCheck(ad);
            if (res == 1)
            {
                HttpContext.Session.SetString("username", ad.Username);
                string xi = HttpContext.Session.GetString("username");
                TempData["user"] = xi;
                TempData["msg"] = "Yes";
                return RedirectToAction("add");
            }
            else
            {
                TempData["msg"] = "No";
            }

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }
        public IActionResult add()
        {
            Ad_login a = new Ad_login
            {
                Username = HttpContext.Session.GetString("username")
            };
            return View(a);
        }
        public IActionResult viewr()
        {
            DataSet ds = gtop.Getrecord();
            FetchData();
            return View(employees);
        }
        public IActionResult TeamAdd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult TeamAdd([Bind] Teamadd ta)
        {
            int x = dbop.teamadd(ta);
            if (x == 1)
                TempData["msgt"] = "Yes";
            else
                TempData["msgt"] = "No";
            return View();
        }
        public IActionResult ExporttoExcel()
        {
            DataSet ds = gtop.Getrecord();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells.LoadFromDataTable(ds.Tables[0], true);
                package.Save();
            }
            stream.Position = 0;
            string excelname = $"EmployeeList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelname);
        }
        public IActionResult Edit(int id)
        {
            FetchData(id);
            return View(emp);
        }
        [HttpPost]
        public IActionResult Edit([Bind] empadd em, int id)
        {
            int x = emdb.Emp(em, id);
            if (x == 1)
            {
                TempData["msgemp"] = "Yes";
            }
            else
                TempData["msgemp"] = "No";
            return View();
        }
        public IActionResult Delete(int id)
        {
            con.Open();
            com.Connection = con;
            var parameter = com.CreateParameter();
            parameter.Value = id;
            parameter.ParameterName = "@id";
            com.Parameters.Add(parameter);
            com.CommandText = "UPDATE employees SET isactive='0' WHERE id=@id";
            com.ExecuteNonQuery();
            con.Close();
            com.Parameters.Clear();
            con.Open();
            com.Connection = con;
            var p = com.CreateParameter();
            p.Value = id;
            p.ParameterName = "@id";
            com.Parameters.Add(p);
            com.CommandText = "UPDATE Emp_login SET isverify='0' WHERE empid=@id";
            com.ExecuteNonQuery();
            con.Close();
            com.Parameters.Clear();
            return RedirectToAction("viewr");
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

                com.CommandText = "Select * from employees where id = @id";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    emp = new employee()
                    {
                        id = dr["id"].ToString()
                    ,
                        name = dr["name"].ToString()
                    ,
                        dob = Convert.ToDateTime(dr["dob"].ToString())
                    ,
                        father = dr["father"].ToString()
                    ,
                        mother = dr["mother"].ToString()
                    ,
                        address = dr["address"].ToString()
                    ,
                        salary = dr["salary"].ToString()
                    ,
                        fresher = dr["fresher"].ToString()
                    ,
                        role = dr["role"].ToString()
                    ,
                        notes = dr["notes"].ToString(),
                    };
                }
                con.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void FetchData()
        {
            if (employees.Count > 0)
            {
                employees.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT TOP (1000) [id],[name],[dob],[father],[mother],[address],[salary],[fresher],[role],[notes] FROM [Login].[dbo].[employees] WHERE [isactive]='1'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    employees.Add(new employee()
                    {
                        id = dr["id"].ToString()
                    ,
                        name = dr["name"].ToString()
                    ,
                        dob = Convert.ToDateTime(dr["dob"].ToString())
                    ,
                        father = dr["father"].ToString()
                    ,
                        mother = dr["mother"].ToString()
                    ,
                        address = dr["address"].ToString()
                    ,
                        salary = dr["salary"].ToString()
                    ,
                        fresher = dr["fresher"].ToString()
                    ,
                        role = dr["role"].ToString()
                    ,
                        notes = dr["notes"].ToString(),
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IActionResult form()
        {
            return View();
        }
        [HttpPost]
        public IActionResult form([Bind] empadd em)
        {
            int x = emdb.Emp(em);
            if (x == 1)
            {
                TempData["msgf"] = "Yes";
            }
            else
                TempData["msgf"] = "No";
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Export()
        {
            DataSet ds = gtop.Gettasks();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells.LoadFromDataTable(ds.Tables[0], true);
                package.Save();
            }
            stream.Position = 0;
            string excelname = $"EmployeeTaskList.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelname);
        }
        public IActionResult ViewTasks()
        {
            if (tasks.Count > 0)
            {
                tasks.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;

                com.CommandText = "SELECT TOP (1000) [id],[empid],[Taskname],[Startdate],[Enddate],[Taskduration],[Teamname],[summary],[Taskdetails],[Riskdetails],[Risksolution] FROM [Login].[dbo].[Task1] WHERE [isactive]='1'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    tasks.Add(new Tasks()
                    {
                        id = dr["id"].ToString()
                    ,
                        Empid = dr["empid"].ToString()
                    ,
                        Taskname = dr["Taskname"].ToString()
                    ,
                        Startdate = Convert.ToDateTime(dr["Startdate"].ToString())
                    ,
                        Enddate = Convert.ToDateTime(dr["Enddate"].ToString())
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
                con.Close();
            }
            catch (Exception)
            {

                throw;
            }
            return View(tasks);
        }
        public IActionResult Viewleaves()
        {
            if (fl.Count > 0)
            {
                fl.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;

                com.CommandText = "SELECT TOP (1000) [id],[empid],[Startdate],[Enddate],[Leavetype],[Reason],[Doc],[Status],[comments] FROM [Login].[dbo].[Leaves1] WHERE [Isactive]='1'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    con1.Open();
                    com1.Connection = con1;
                    var parameter2 = com1.CreateParameter();
                    parameter2.Value = Int32.Parse(dr["empid"].ToString());
                    parameter2.ParameterName = "@xyz";
                    com1.Parameters.Add(parameter2);
                    com1.CommandText = "SELECT [name] from [Login].[dbo].[employees] WHERE id=@xyz";
                    dr1 = com1.ExecuteReader();
                    com1.Parameters.Clear();
                    while (dr1.Read())
                    fl.Add(new Fetchleaves()
                    {
                        id = Int32.Parse(dr["id"].ToString())
                    ,
                        name = dr1["name"].ToString()
                    ,
                        Startdate = Convert.ToDateTime(dr["Startdate"].ToString())
                    ,
                        Enddate = Convert.ToDateTime(dr["Enddate"].ToString())
                    ,
                        Reason = dr["Reason"].ToString()
                    ,
                        Leavetype = dr["Leavetype"].ToString()
                    ,
                        Doc = dr["Doc"].ToString()
                    ,
                        Status = dr["Status"].ToString()
                    ,
                        comment = dr["comments"].ToString()
                    });
                    con1.Close();
                }
                con.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return View(fl);

        }
        public IActionResult Leavedetails(int id)
        {
            try
            {
                con.Open();
                com.Connection = con;

                var parameter = com.CreateParameter();
                parameter.Value = id;
                parameter.ParameterName = "@id";
                com.Parameters.Add(parameter);
                com.CommandText = "SELECT TOP (1000) [id],[empid],[Startdate],[Enddate],[Leavetype],[Reason],[Doc],[Status],[verify],[comments] FROM [Login].[dbo].[Leaves1] WHERE [Isactive]='1' AND id=@id";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    con1.Open();
                    com1.Connection = con1;
                    com1.Parameters.Clear();
                    var parameter1 = com1.CreateParameter();
                    parameter1.Value = Int32.Parse(dr["empid"].ToString());
                    parameter1.ParameterName = "@mln";
                    com1.Parameters.Add(parameter1);
                    com1.CommandText = "SELECT [name] from employees WHERE id=@mln";
                    dr1 = com1.ExecuteReader();
                    com1.Parameters.Clear();
                    while (dr1.Read())
                    l = new Fetchleaves()
                    {
                        id = Int32.Parse(dr["id"].ToString())
                    ,
                        name = dr1["name"].ToString()
                    ,
                        Startdate = Convert.ToDateTime(dr["Startdate"].ToString())
                    ,
                        Enddate = Convert.ToDateTime(dr["Enddate"].ToString())
                    ,
                        Reason = dr["Reason"].ToString()
                    ,
                        Leavetype = dr["Leavetype"].ToString()
                    ,
                        Doc = dr["Doc"].ToString()
                    ,
                        Status = dr["Status"].ToString()
                    ,
                        verify = Int32.Parse(dr["verify"].ToString())
                    ,
                        comment = dr["comments"].ToString()
                    };
                    con1.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(l);
        }
        public IActionResult Leaveaction([Bind] Fetchleaves l,string approve,string reject)
        {
            if (!string.IsNullOrEmpty(approve))
                dbop.Approve(l.id, l.comment);
            if (!string.IsNullOrEmpty(reject))
                dbop.Reject(l.id, l.comment);
            return RedirectToAction("Viewleaves");
        }
        public FileResult Download(string path)
        {

            string actpath = Path.Combine(_webHostEnvironment.WebRootPath, path);
            byte[] filebytes = System.IO.File.ReadAllBytes(actpath);
            return File(filebytes, GetContentType(actpath), Path.GetFileName(actpath));
        }
        public ActionResult viewd(string path)
        {
            string actpath = Path.Combine(_webHostEnvironment.WebRootPath, path);
            return Redirect(actpath);
        }
        private string GetContentType(string path)
        {
            Dictionary<string, string> types = new Dictionary<string, string> {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
            string ext = Path.GetExtension(path).ToLower();
            return types[ext];
        }
    }
}
