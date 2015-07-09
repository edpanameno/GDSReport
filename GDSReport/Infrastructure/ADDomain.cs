using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace GDSReport.Infrastructure
{
    using GDSReport.Models;

    /// <summary>
    /// This class represents the active directory domain environment
    /// that the application will be working with
    /// </summary>
    public class ADDomain
    {
        public string ServerName { get; set; }
        public string Container { get; set; }
        public string LDAPPath { get; set; }
        public string AdminUser { get; set; }
        public string AdminPassword { get; set; }

        private PrincipalContext Context { get; set; }

        public ADDomain()
        {
            ServerName = ConfigurationManager.AppSettings["server_name"].ToString();
            Container = ConfigurationManager.AppSettings["container"].ToString();
            LDAPPath = ConfigurationManager.AppSettings["ldap_path"].ToString();
            AdminUser = ConfigurationManager.AppSettings["username"].ToString();
            AdminPassword = ConfigurationManager.AppSettings["password"].ToString();

            Context = new PrincipalContext(ContextType.Domain, ServerName, Container, AdminUser, AdminPassword);
        }

        public int GetActiveUserCount()
        {
            int result = 0;

            // Setting a filter so that the search we are about to do
            // only searches for users that are enabled in active directory
            UserPrincipalEx userPrincipal = new UserPrincipalEx(Context)
            {
                Enabled = true
            };

            PrincipalSearcher searcher = new PrincipalSearcher();
            searcher.QueryFilter = userPrincipal;
            ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 500;

            foreach(UserPrincipalEx principalResult in searcher.FindAll())
            {
                result++;
            }

            return result;
        }

        /// <summary>
        /// Gets a list of all accounts that are active in the specified OU.
        /// </summary>
        /// <returns></returns>
        public List<ADUser> GetAllActiveUsers()
        {
            List<ADUser> users = new List<ADUser>();

            UserPrincipalEx userPrincipal = new UserPrincipalEx(Context)
            {
                Enabled = true
            };

            PrincipalSearcher searcher = new PrincipalSearcher();
            searcher.QueryFilter = userPrincipal;
            ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 1000;
            var searchResults = searcher.FindAll().ToList();

            foreach(UserPrincipalEx user in searchResults)
            {
                users.Add(new ADUser()
                {
                    UserName = user.SamAccountName,
                    FirstName = user.GivenName,
                    LastName = user.Surname,
                    Title = user.Title,
                    Email = user.EmailAddress,
                    Department = user.Department.Trim(),
                    Company = user.Company.Trim(),
                    
                    // The WhenCreated field is stored as a generalized date string
                    // as such, we have to convert it to Local time to see the date/time
                    // that the account was created
                    WhenCreated = user.WhenCreated.ToLocalTime()
                });
            }

            return users;
        }

        public List<ADUser> SearchForUsers(string searchString, string searchField)
        {
            // the wildcard character is the asterisks, therefore I will add
            // one before and after the search string.
            searchString = "*" + searchString + "*";

            List<ADUser> users = new List<ADUser>();
            UserPrincipalEx userPrincipal = new UserPrincipalEx(Context);
            userPrincipal.Enabled = true;

            switch(searchField)
            {
                case "department":
                    userPrincipal.Department = searchString;
                    break;
                case "firstName":
                    userPrincipal.GivenName = searchString;
                    break;
                case "lastName":
                    userPrincipal.Surname = searchString;
                    break;
                case "title":
                    userPrincipal.Title = searchString;
                    break;
                default:
                    break;
            }

            PrincipalSearcher searcher = new PrincipalSearcher();
            searcher.QueryFilter = userPrincipal;
            ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 500;

            foreach(UserPrincipalEx user in searcher.FindAll())
            {
                users.Add(new ADUser()
                {
                    UserName = user.SamAccountName,
                    FirstName = user.GivenName,
                    LastName = user.Surname,
                    Department = user.Department,
                    Title = user.Title,
                });
            }

            return users;
        }

        public ADUser GetUserByUserName(string userName)
        {
            ADUser user = new ADUser();
            UserPrincipalEx adUser = new UserPrincipalEx(Context);
            adUser.SamAccountName = userName;

            PrincipalSearcher searcher = new PrincipalSearcher();
            searcher.QueryFilter = adUser;
            UserPrincipalEx searchResult = (UserPrincipalEx)searcher.FindOne();

            if(searchResult != null)
            {
                user.UserName = searchResult.SamAccountName;
                user.FirstName = searchResult.GivenName;
                user.LastName = searchResult.Surname;
                user.PhoneNumber = searchResult.PhoneNumber;
                user.Department = searchResult.Department.Trim();
                user.Title = searchResult.Title;
                user.Email = searchResult.EmailAddress;
                user.Company = searchResult.Company.Trim();
                user.DisplayName = searchResult.DisplayName;
                user.GUID = searchResult.Guid.ToString();
                user.Info = searchResult.Info;
                user.LogonCount = searchResult.LogonCount;
            }

            return user;
        }

        public void UpdateUser(ADUser user)
        {
            UserPrincipalEx currentADUser = new UserPrincipalEx(Context);
            currentADUser.SamAccountName = user.UserName;

            PrincipalSearcher searcher = new PrincipalSearcher(currentADUser);
            UserPrincipalEx adUser = (UserPrincipalEx)searcher.FindOne();

            if(adUser != null)
            {
                adUser.GivenName = user.FirstName;
                adUser.Surname = user.LastName;
                adUser.Title = user.Title;
                adUser.DisplayName = user.DisplayName;
                adUser.Department = user.Department;
                adUser.Company = user.Company;
                adUser.Info = user.Info;

                adUser.Save();
            }
        }
    }
}
