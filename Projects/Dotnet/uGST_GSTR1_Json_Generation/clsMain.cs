using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace uGST_GSTR1_Json_Generation
{
    static class clsMain
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                args = new string[] { "1", "U061819", "AIPLDTM024\\SQLExpress", "sa", "sa1985", "^21001", "ADMIN", @"D:\UdyogERP\Bmp\Icon_VudyogGST.ico", "uGST SDK", "UdyogGSTSDK", "1", "udpid6096DTM20110307112715", "ADMINISTRATOR", "eWAY BIll Generation" };// "Employee Master Updation" };/*Rup Add ICO Path,Parent Appl Caption,Parent Appl. Name,Parent Appl PId,Application Log Table*/
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(args));
            return 1;
        }
    }
}
