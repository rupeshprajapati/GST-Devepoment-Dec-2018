﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udBarcodePrinting
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                //args = new string[] { "12", "P071819", "AIPLDTM024\\SQLExpress", "sa", "sa1985", "^21001", "ADMIN", @"D:\Pharma Installer\PROF\UdyogERP\Bmp\Icon_VudyogGST.ico", "UdyogERPSDK", "UdyogERPSDK", "1", "udpid6096DTM20110307112715", "ADMINISTRATOR", "Cheque Management" };
                args = new string[] { "12", "T021819", "AIPLDTM024", "sa", "sa@1985", "^21001", "ADMIN", @"D:\Pharma Installer\PROF\UdyogERP\Bmp\Icon_VudyogGST.ico", "UdyogERPSDK", "UdyogERPSDK", "1", "udpid6096DTM20110307112715", "ADMINISTRATOR", "Cheque Management" };
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(args));
        }
    }
}