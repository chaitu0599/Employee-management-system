using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace LoginPage.Models
{
    public class empdb
    {
        SqlConnection con = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=Login;Integrated Security=True");
        public int Emp(empadd em)
        {
            SqlCommand com = new SqlCommand("SP_employee", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@name", em.name);
            com.Parameters.AddWithValue("@dob", em.dob);
            com.Parameters.AddWithValue("@father", em.father);
            com.Parameters.AddWithValue("@mother", em.mother);
            com.Parameters.AddWithValue("@address", em.address);
            com.Parameters.AddWithValue("@salary", em.salary);
            com.Parameters.AddWithValue("@fresher", em.fresher);
            com.Parameters.AddWithValue("@notes", em.notes);
            com.Parameters.AddWithValue("@role", em.role);
            con.Open();
            com.ExecuteNonQuery();
            int x = 1;
            con.Close();
            return x;
        }
        public int Emp(empadd em,int id)
        {
            SqlCommand com = new SqlCommand("SP_edit", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", id);
            com.Parameters.AddWithValue("@name", em.name);
            com.Parameters.AddWithValue("@dob", em.dob);
            com.Parameters.AddWithValue("@father", em.father);
            com.Parameters.AddWithValue("@mother", em.mother);
            com.Parameters.AddWithValue("@address", em.address);
            com.Parameters.AddWithValue("@salary", em.salary);
            com.Parameters.AddWithValue("@fresher", em.fresher);
            com.Parameters.AddWithValue("@notes", em.notes);
            com.Parameters.AddWithValue("@role", em.role);
            con.Open();
            com.ExecuteNonQuery();
            int x = 1;
            con.Close();
            return x;
        }
    }
}
