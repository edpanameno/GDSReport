# GDS Report

A .NET console application that is used to query active directory and generate and email a report to interested parties.  The application is executed thru a scheduled task in windows on the first day of month.  It uses NDesk.Options to allow users to use different paramters to generate different reports (at this time only one type of report exists, but more can be added later). The EPPLus library is used to also generate an excel file with the results of the query. The excel file is then emailed to users along with a count summary of users by department.

