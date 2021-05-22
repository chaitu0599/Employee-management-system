using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using LoginPage.Models;
using System.Security.Cryptography;

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
            SqlDataReader dr;
            SqlConnection con1 = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=Login;Integrated Security=True");
            SqlCommand co1 = new SqlCommand();
            co1.Connection = con1;
            con1.Open();
            var parameter = co1.CreateParameter();
            parameter.Value = emp.empid;
            parameter.ParameterName = "@id";
            co1.Parameters.Add(parameter);
            co1.CommandText = "SELECT Password from Emp_login WHERE empid=@id";
            dr = co1.ExecuteReader();
            string pwd;
            int res=0;
            while (dr.Read()) {
                pwd = dr["Password"].ToString();
                if (IsValid(emp.Upassword, pwd))
                    res = 1;
                else
                    res = 0;
              }
            return res;
        }
        public bool IsValid(string testPassword, string origDelimHash)
        {
            var origHashedParts = origDelimHash.Split('|');
            var origSalt = Convert.FromBase64String(origHashedParts[0]);
            var origIterations = Int32.Parse(origHashedParts[1]);
            var origHash = origHashedParts[2];

            //generate hash from test password and original salt and iterations
            var pbkdf2 = new Rfc2898DeriveBytes(testPassword, origSalt, origIterations);
            byte[] testHash = pbkdf2.GetBytes(24);

            //if hash values match then return success
            if (Convert.ToBase64String(testHash) == origHash)
                return true;

            //no match return false
            return false;

        }
        public string Generate(string password, int iterations = 1000)
        {
            var salt = new byte[24];
            new RNGCryptoServiceProvider().GetBytes(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(24);
            return Convert.ToBase64String(salt) + "|" + iterations + "|" + Convert.ToBase64String(hash);
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
            SqlCommand co = new SqlCommand("SP_newemp1", con);
            co.CommandType = CommandType.StoredProcedure;
            co.Parameters.AddWithValue("@id", n.id);
            SqlParameter oblogin1 = new SqlParameter();
            oblogin1.ParameterName = "@Isvalid";
            oblogin1.SqlDbType = SqlDbType.Bit;
            oblogin1.Direction = ParameterDirection.Output;
            co.Parameters.Add(oblogin1);
            con.Open();
            co.ExecuteNonQuery();
            int res1 = Convert.ToInt32(oblogin1.Value);
            con.Close();

            if (res == 1 && res1==0)
            {
                SqlCommand com1 = new SqlCommand("SP_emplogin", con);
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.AddWithValue("@Empid", n.id);
                string x = Generate(n.Password);
                com1.Parameters.AddWithValue("@Password",x);
                con.Open();
                com1.ExecuteNonQuery();
                con.Close();
                return res;
            }
            else if (res1 == 1 && res==1)
                return 2;
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
        public int editleaves(Leaveapp la, int id,string f)
        {
            SqlCommand com = new SqlCommand("SP_leaveupdate", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", id);
            com.Parameters.AddWithValue("@Startdate", la.Startdate);
            com.Parameters.AddWithValue("@Enddate", la.Enddate);
            com.Parameters.AddWithValue("@Leavetype", la.Leavetype);
            com.Parameters.AddWithValue("@Doc", f);
            com.Parameters.AddWithValue("@Reason", la.Reason);
            con.Open();
            com.ExecuteNonQuery();
            int x = 1;
            con.Close();
            return (x);
        }
        public int leaveapp(Leaveapp la, string f,string empid)
        {
            SqlCommand com = new SqlCommand("SP_leaveinsert", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@empid", Int32.Parse(empid));
            com.Parameters.AddWithValue("@Startdate", la.Startdate);
            com.Parameters.AddWithValue("@Enddate",la.Enddate);
            com.Parameters.AddWithValue("@Leavetype", la.Leavetype);
            com.Parameters.AddWithValue("@Doc",f);
            com.Parameters.AddWithValue("@Reason", la.Reason);
            con.Open();
            com.ExecuteNonQuery();
            int x = 1;
            con.Close();
            return(x);
        }
        public void Approve(int id,string comment)
        {
            SqlCommand com = new SqlCommand("SP_adminleave", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", id);
            com.Parameters.AddWithValue("@comment", comment);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
        public void Reject(int id,string comment)
        {
            SqlCommand com = new SqlCommand("SP_adminleave", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", id);
            com.Parameters.AddWithValue("@comment", comment);
            com.Parameters.AddWithValue("@verify", 1);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
        public int teamadd(Teamadd ta)
        {
            SqlCommand com = new SqlCommand("SP_teaminsert", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Teamname",ta.Name);
            com.Parameters.AddWithValue("@Startdate", ta.Startdate);
            com.Parameters.AddWithValue("@Enddate", ta.Enddate);
            con.Open();
            com.ExecuteNonQuery();
            int x = 1;
            con.Close();
            return(x);

        }
    }
}
