using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GDSReport.Infrastructure
{
    using System.IO;
    using OfficeOpenXml;
    using GDSReport.Models;

    /// <summary>
    /// This class is used to generated the desired report.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Generates the list of all users on the first day of each month.
        /// </summary>
        public void MonthlyReport()
        {
            ADDomain domain = new ADDomain();
            List<ADUser> users = domain.GetAllActiveUsers();

            MemoryStream outputStream = new MemoryStream();

            using(ExcelPackage excelReport = new ExcelPackage(outputStream))
            {
                // I am selecting only the data that we want to show up in the
                // excel file. I am also formatting this data so that it shows
                // up in a nicely formatted way.
                var formattedUsers = users.Select(row =>
                {
                    return new
                    {
                        User_Name = row.UserName,
                        First_Name = row.FirstName,
                        Last_Name = row.LastName,
                        Phone_Number = row.PhoneNumber,
                        Title = row.Title,
                        Email = row.Email,
                        Department = row.Department,
                        Company = row.Company,
                        When_Created = row.WhenCreated.ToString("MM/dd/yyyy h:mm tt")
                    };
                });

                ExcelWorksheet workSheet = excelReport.Workbook.Worksheets.Add("GDS Report - " + DateTime.Now.ToString("MM-dd-yyyy"));
                workSheet.Cells["A1"].LoadFromCollection(formattedUsers, true).AutoFitColumns();
                excelReport.Save();
            }

            var userSummary = users.GroupBy(u => u.Company)
                                   .Select(d =>
                                           new CompanySummary 
                                           {
                                               // Quick little check if the company name is blank, then 
                                               // we want to show a 'No Company' in the table that gets
                                               // generated in the body of the email. If this is not done
                                               // then the user will just see a blank cell which is not good!
                                               CompanyName = d.Key == string.Empty ? "No Company" : d.Key,
                                               UserCount = d.Count()
                                           }).OrderByDescending(d => d.UserCount);

            Notifications notification = new Notifications();
            notification.SendMonthlyReport(userSummary.ToList(), outputStream);
        }
    }
}
