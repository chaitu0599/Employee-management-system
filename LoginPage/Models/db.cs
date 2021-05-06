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
                com1.Parameters.AddWithValue("@Username", n.Username);
                com1.Parameters.AddWithValue("@Password", n.Password);
                con.Open();
                com1.ExecuteNonQuery();
                con.Close();
            }
            return res;
        }
    }
}
