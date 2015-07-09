using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace GDSReport.Models
{
    /// <summary>
    /// This class represents a user object in Active Directory
    /// </summary>
    public class ADUser
    {
        public string Password { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string DisplayName { get; set; }
        public string GUID { get; set; }
        public string Info { get; set; }
        public int LogonCount { get; set; }
        //public DateTime? LastLogon { get; set; }
        //public int BadLogonCount { get; set; }
        public DateTime WhenCreated { get; set; }

    }
}