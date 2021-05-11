using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginPage.Models
{
    public class Leaveapp
    {
        public int empid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public string Leavetype { get; set; }
        public string Reason { get; set; }
        public IFormFile Doc { get; set; }
    }
}

