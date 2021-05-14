using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginPage.Models
{
    public class Taskadd
    {

        public string Taskname { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public string Teamname { get; set; }
        public string Summary { get; set; }
        public string Taskdetails { get; set; }
        public string Riskdetails { get; set; }
        public string Risksolution { get; set; }
    }
}
