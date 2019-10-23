using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlawHess
{
    /// <summary>
    /// Class for database entities
    /// </summary>
    public class dataItem
    {
        //customer
        public string id { get; set; }
        public string title { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string DoB { get; set; }
        public string natins { get; set; }
        public string email { get; set; }
        public string allowence { get; set; }
        //account
        public string accID { get; set; }
        public string custID { get; set; }
        public string prodID { get; set; }
        public string balance { get; set; }
        public string accrued { get; set; }
        public string active { get; set; }
        //transaction
        public string tranxID { get; set; }
        public string action { get; set; }
        public string amnt { get; set; }
        public string theEvent { get; set; }
        //product
        public string prodName { get; set; }
        public string status { get; set; }
        public string transin { get; set; }
        public string intRate { get; set; }
    }
}
