using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace udGSPAPILicUploader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {

            if (args.Length < 8)
            {
                args = new string[] { "7", "P031819", "AIPLDTM002", "sa", "sa1985", "^13048", "ADMIN", @"F:\\UdyogERPENT2.1.1\Bmp\Icon_VudyogGST.ico", "UdyogERPENT", "UdyogERPENT.EXE", "4764", "udPID4764DTM20111213125821" };
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmgspapilicensemast(args));
            return 1;
        }
    }
}
