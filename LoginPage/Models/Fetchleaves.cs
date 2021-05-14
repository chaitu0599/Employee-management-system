using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginPage.Models
{
    public class Fetchleaves
    {
        public int id { get; set; }
        public int empid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public string Leavetype { get; set; }
        public string Reason { get; set; }
        public string Doc { get; set; }
        public string Status { get; set; }
        public int verify { get; set; }
        public string answer { get; set; }
        public string comment { get; set; }
    }
}
