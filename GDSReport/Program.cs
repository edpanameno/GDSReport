using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GDSReport
{
    using NDesk.Options;
    using GDSReport.Infrastructure;

    class Program
    {
        static void Main(string[] args)
        {
            string reportToRun = string.Empty;

            var p = new OptionSet()
                        .Add("r|report", a => reportToRun = a);
            
            p.Parse(args);

            if(String.IsNullOrEmpty(reportToRun))
            {
                NoParametersPassed();
            }

            switch(reportToRun)
            {
                case "r":
                    Console.WriteLine("Running Monthly GDS Report");
                    Report report = new Report();
                    report.MonthlyReport();
                    break;
                default:
                    break;
            }

        }

        private static void NoParametersPassed()
        {
            Console.WriteLine("GDSReport.exe - No Parameters have been passed");
            Console.WriteLine("To use the application, please pass teh appropriate parameter to generated the required report");
        }
    }
}
