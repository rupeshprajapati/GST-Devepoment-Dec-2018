using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UdAlert
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
          static int  Main(string[] args)
           {
            if (args.Length < 1)
            {
                //args = new string[] { "6", "N011112", "udyog5\\usqare", "sa", "sa1985", "ADMIN", @"D:\VU10\VudyogSDK\Bmp\Icon_usquare.ico", "Usquare pack", "Usquare.exe", "1", "udpid6096DTM20110307112715", "ADMINISTRATOR" };/*Rup Add ICO Path,Parent Appl Caption,Parent Appl. Name,Parent Appl PId,Application Log Table*/
                //args = new string[] { "11", "B011819", "AIPLDTM001\\SQLEXPRESS2014", "sa", "sa1985", "ADMIN", @"D:\UDYOGGST\Bmp\icon_VudyogGST.ico", "Udyog GST", "UdyogGST.exe", "10596", "udPID10596DTM20180901142151", "ADMINISTRATOR" };/*Rup Add ICO Path,Parent Appl Caption,Parent Appl. Name,Parent Appl PId,Application Log Table*/
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmAlert(args));
            return 1;
          }
    }
}
