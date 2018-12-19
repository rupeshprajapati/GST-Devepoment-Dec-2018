using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace udPosSettings
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 15)
            {
                //args = new string[] { "1", "P031819", @"AIPLLTM001\AIPLLTM001", "sa", "sa1985", "^21289", "ADMIN", @"D:\VudyogGST\Bmp\Icon_uerpstd.ico", "VudyogGST", "VUDYOGGST.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2017", "31-03-2018" };
                args = new string[] { "1", "T011819", @"AIPLDTM024\SQLExpress", "sa", "sa1985", "^21289", "ADMIN", @"D:\Pharma Installer\PRENT\UdyogERP\Bmp\Icon_uerpent.ico", "VudyogGST", "VUDYOGGST.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2017", "31-03-2018" };
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new udPosSettings(args));
        }
    }
}
