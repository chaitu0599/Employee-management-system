using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LoginPage.Models
{
    public class getdb
    {
        SqlConnection con;
        public getdb()
        {
            var configuration = GetConfiguration();
            con = new SqlConnection(configuration.GetSection("Data").GetSection("ConnectionString").Value);

        }
        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
        public DataSet Getrecord()
        {
            SqlCommand com = new SqlCommand("SP_getdata", con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        public DataSet Getrecord(string id)
        {
            int x = Int32.Parse(id);
            SqlCommand com = new SqlCommand("SP_gettask", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@id", x);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        public DataSet Gettasks()
        {
            SqlCommand com = new SqlCommand("SP_gettask", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@verify",1);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
    }
    
}
