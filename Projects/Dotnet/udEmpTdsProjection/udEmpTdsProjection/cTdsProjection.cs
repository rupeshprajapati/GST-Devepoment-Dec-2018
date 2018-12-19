using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace udEmpTdsProjection
{
    static class cTdsProjection
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length < 7)
            {
                //args = new string[] { "11", "u33", @"udyog65", "sa", "sa@1985", "^19003", "ADMIN", @"D:\USQUARE\Bmp\icon_USquare.ico", "USquare", "USQUARE.EXE", "4764", "udPID4764DTM20111213125821" };
                args = new string[] { "7", "B031819 ", "AIPLDTM002\\SQLEXPRESS2008", "sa", "sa1985", "^21001", "ADMIN", @"E:\UdyogERP\Bmp\Icon_VudyogGST.ico", "UdyogERP", "UdyogERP.exe", "1", "udpid6096DTM20110307112715" };
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmTdsProjection(args));
            return 1;
        }
    }
}
