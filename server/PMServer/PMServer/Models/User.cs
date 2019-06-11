using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMServer.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
    }
}