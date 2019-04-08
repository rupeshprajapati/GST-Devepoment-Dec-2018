using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace ugstEWayBill
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
                //args = new string[] { "14", "R011819", "AIPLDTM019\\SQLEXPRESS", "sa", "sa1985", "^21001", "ADMIN", @"E:\U6\UdyogERP\Bmp\Icon_VudyogGST.ico", "uGST SDK", "UdyogERPENT", "1", "udpid6096DTM20110307112715", "ADMINISTRATOR", "eWAY BIll Generation" };
                //args = new string[] { "1", "U171819", "AIPLDTM019\\SQLExpress", "sa", "sa1985", "^21468", "ADMIN", @"D:\U\UdyogERPSDK\Bmp\icon_VudyogGSSDK.ico", "UdyogERPSDK", "UdyogERPSDK", "1", "udpid6096DTM20110307112715", "ADMINISTRATOR", "eWAY BIll Generation" };
                args = new string[] { "28", "B041819", "AIPLDTM001\\SQLEXPRESS2014", "sa", "sa1985", "^21001", "ADMIN", @"D:\UdyogGST\Bmp\Icon_VudyogGST.ico", "Udyog ERP", "UdyogERP", "1", "udpid6096DTM20110307112715", "ADMINISTRATOR", "eWAY BIll Generation" };
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(args));
            return 1;
        }
    }
}
