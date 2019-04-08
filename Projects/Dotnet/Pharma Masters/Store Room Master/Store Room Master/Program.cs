using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udStoreRoomMaster
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 8)
            {
                //args = new string[] { "1", "P011819", "AIPLDTM001\\SQLEXPRESS2014", "sa", "sa1985", "^21512", "ADMIN", @"D:\UdyogERPPRO\Bmp\Icon_VudyogGST.ico", "UdyogERPPRO", "UdyogERPPRO.exe", "6476", "udPID6476DTM20151226112732" };
                args = new string[] { "7", "N011819", "AIPLDTM001\\SQLEXPRESS2014", "sa", "sa1985", "^21517", "ADMIN", @"D:\UdyogERPENT\Bmp\Icon_VudyogGST.ico", "UdyogERPENT", "UdyogERPENT.exe", "6476", "udPID6476DTM20151226112732" };
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmStoreRoom(args));
        }
    }
}
