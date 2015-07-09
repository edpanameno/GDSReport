using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;

namespace GDSReport.Infrastructure
{
    [DirectoryObjectClass("user")]
    [DirectoryRdnPrefix("CN")]
    public class UserPrincipalEx : UserPrincipal
    {
        public UserPrincipalEx(PrincipalContext context) : base(context) { }

        [DirectoryProperty("company")]
        public new string Company
        {
            get
            {
                if(ExtensionGet("company").Length != 1)
                {
                    return "No Company";
                }
                else
                {
                    return (string)ExtensionGet("company")[0];
                }
            }
            set
            {
                ExtensionSet("company", value);
            }
        }

        [DirectoryProperty("department")]
        public new string Department
        {
            get
            {
                if(ExtensionGet("department").Length != 1)
                {
                    return "No Department";
                }
                else
                {
                    return (string)ExtensionGet("department")[0];
                }
            }
            set
            {
                ExtensionSet("department", value);
            }
        }

        [DirectoryProperty("title")]
        public string Title
        {
            get
            {
                if(ExtensionGet("title").Length != 1)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)ExtensionGet("title")[0];
                }
            }
            set
            {
                ExtensionSet("title", value);
            }
        }
        
        [DirectoryProperty("telephoneNumber")]
        public string PhoneNumber
        {
            get
            {
                if(ExtensionGet("telephoneNumber").Length != 1)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)ExtensionGet("telephoneNumber")[0];
                }
            }
            set
            {
                ExtensionSet("telephoneNumber", value);
            }
        }

        [DirectoryProperty("info")]
        public string Info
        {
            get
            {
                if(ExtensionGet("info").Length != 1)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)ExtensionGet("info")[0];
                }
            }
            set
            {
                ExtensionSet("info", value);
            }
        }

        [DirectoryProperty("logonCount")]
        public int LogonCount
        {
            get
            {
                return (int)ExtensionGet("logonCount")[0];
            }
        }
        
        [DirectoryProperty("whenCreated")]
        public DateTime WhenCreated
        {
            get
            {
                return (DateTime)ExtensionGet("whenCreated")[0];
            }
        }
        
        [DirectoryProperty("RealLastLogon")]
        public DateTime? RealLastLoginDate
        {
            get
            {
                if(ExtensionGet("LastLogon").Length > 0)
                {
                    var lastLogonDate = ExtensionGet("LastLogon")[0];
                    var lastLogonDateType = lastLogonDate.GetType();

                    var highPart = (Int32)lastLogonDateType.InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, lastLogonDate, null);
                    var lowPart = (Int32)lastLogonDateType.InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public, null, lastLogonDate, null);

                    var longDate = ((Int64)highPart << 32 | (UInt32)lowPart);

                    return longDate > 0 ? (DateTime?)DateTime.FromFileTime(longDate) : null;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}