using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using LoginPage.Models;
namespace LoginPage.Models
{
    public class db
    {
        SqlConnection con = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=Login;Integrated Security=True");
        
        public int LoginCheck(Ad_login ad)
        {
            SqlCommand com = new SqlCommand("SP_login", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Username", ad.Username);
            com.Parameters.AddWithValue("@Upassword", ad.Upassword);
            SqlParameter oblogin = new SqlParameter();
            oblogin.ParameterName = "@Isvalid";
            oblogin.SqlDbType = SqlDbType.Bit;
            oblogin.Direction = ParameterDirection.Output;
            com.Parameters.Add(oblogin);
            con.Open();
            com.ExecuteNonQuery();
            int res = Convert.ToInt32(oblogin.Value);
            con.Close();
            return res;
        }
        public int LoginCheck(Emp_login emp)
        {
            SqlCommand com = new SqlCommand("SP_elogin", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Username", emp.Username);
            com.Parameters.AddWithValue("@Upassword", emp.Upassword);
            com.Parameters.AddWithValue("@Empid", emp.empid);
            SqlParameter oblogin = new SqlParameter();
            oblogin.ParameterName = "@Isvalid";
            oblogin.SqlDbType = SqlDbType.Bit;
            oblogin.Direction = ParameterDirection.Output;
            com.Parameters.Add(oblogin);
            con.Open();
            com.ExecuteNonQuery();
            int res = Convert.ToInt32(oblogin.Value);
            con.Close();
            return res;
        }
        public int RegisterCheck(Newemp n)
        {
            SqlCommand com = new SqlCommand("SP_newemp", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", n.id);
            SqlParameter oblogin = new SqlParameter();
            oblogin.ParameterName = "@Isvalid";
            oblogin.SqlDbType = SqlDbType.Bit;
            oblogin.Direction = ParameterDirection.Output;
            com.Parameters.Add(oblogin);
            con.Open();
            com.ExecuteNonQuery();
            int res = Convert.ToInt32(oblogin.Value);
            con.Close();
            if (res == 1)
            {
                SqlCommand com1 = new SqlCommand("SP_emplogin", con);
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.AddWithValue("@Empid", n.id);
                com1.Parameters.AddWithValue("@Username", n.Username);
                com1.Parameters.AddWithValue("@Password", n.Password);
                con.Open();
                com1.ExecuteNonQuery();
                con.Close();
            }
            return res;
        }
        public int taskadd(Taskadd ta,string empid)
        {
            SqlCommand com = new SqlCommand("SP_taskinsert", con);
            int id = Int32.Parse(empid);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Empid", id);
            com.Parameters.AddWithValue("@Taskname", ta.Taskname);
            com.Parameters.AddWithValue("@Startdate", ta.Startdate);
            com.Parameters.AddWithValue("@Enddate", ta.Enddate);
            DateTime firstDay = ta.Startdate;
            firstDay = firstDay.Date;
            DateTime lastDay = ta.Enddate;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect End date " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
           
            if (businessDays > fullWeekCount * 7)
            {
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)
                    businessDays -= 1;
            }
            businessDays -= fullWeekCount + fullWeekCount;
            com.Parameters.AddWithValue("@Duration", businessDays);
            com.Parameters.AddWithValue("@Teamname", ta.Teamname);
            com.Parameters.AddWithValue("@Summary", ta.Summary);
            com.Parameters.AddWithValue("@Taskdetails", ta.Taskdetails);
            if (ta.Riskdetails == null)
            {
                com.Parameters.AddWithValue("@Riskdetails", "No issues/risks");
                com.Parameters.AddWithValue("@Risksolution", "No issues/risks");
            }
            else
            {
                com.Parameters.AddWithValue("@Riskdetails", ta.Riskdetails);
                com.Parameters.AddWithValue("@Risksolution", ta.Risksolution);
            }
            con.Open();
            com.ExecuteNonQuery();
            int x = 1;
            con.Close();
            return x;
        }
        public int taskadd(Taskadd ta,int id)
        {
            SqlCommand com = new SqlCommand("SP_taskupdate", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", id);
            com.Parameters.AddWithValue("@Taskname", ta.Taskname);
            com.Parameters.AddWithValue("@Startdate", ta.Startdate);
            com.Parameters.AddWithValue("@Enddate", ta.Enddate);
            DateTime firstDay = ta.Startdate;
            firstDay = firstDay.Date;
            DateTime lastDay = ta.Enddate;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect End date " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;

            if (businessDays > fullWeekCount * 7)
            {
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)
                    businessDays -= 1;
            }
            businessDays -= fullWeekCount + fullWeekCount;
            com.Parameters.AddWithValue("@Duration", businessDays);
            com.Parameters.AddWithValue("@Teamname", ta.Teamname);
            com.Parameters.AddWithValue("@Summary", ta.Summary);
            com.Parameters.AddWithValue("@Taskdetails", ta.Taskdetails);
            if (ta.Riskdetails == null)
            {
                com.Parameters.AddWithValue("@Riskdetails", "No issues/risks");
                com.Parameters.AddWithValue("@Risksolution", "No issues/risks");
            }
            else
            {
                com.Parameters.AddWithValue("@Riskdetails", ta.Riskdetails);
                com.Parameters.AddWithValue("@Risksolution", ta.Risksolution);
            }
            con.Open();
            com.ExecuteNonQuery();
            int x = 1;
            con.Close();
            return x;

        }
    }
}
