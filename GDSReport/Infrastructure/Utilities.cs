using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace GDSReport.Infrastructure
{
    public static class Utilities
    {
        /// <summary>
        /// Used to get the value of the specified property of the Principal object. If
        /// the property doesn't exist for the principal object an empty string is returned.
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetUserProperty(this Principal principal, string propertyName)
        {
            DirectoryEntry directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            
            if(directoryEntry.Properties.Contains(propertyName))
            {
                return directoryEntry.Properties[propertyName].Value.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
