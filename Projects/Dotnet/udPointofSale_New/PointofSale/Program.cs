using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PointofSale
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 16)
            {
                //args = new string[] { "2", "B011617", @"AIPLLTM001\AIPLLTM001", "sa", "sa1985", "^21289", "ADMIN", @"D:\Vudyogsdk\Bmp\icon_VUDYOGSDK.ico", "VudyogSDK", "VUDYOGSDK.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2016", "31-03-2017", ".T." };
                //args = new string[] { "221", "P031819", @"AIPLLTM001\AIPLLTM001", "sa", "sa1985", "^21289", "ADMIN", @"D:\Vudyoggst\Bmp\Icon_VudyogGST.ico", "Vudyoggst", "Vudyoggst.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2018", "31-03-2019", ".F.", "PS", "Point of Sale" };
                args = new string[] { "228", "P011819", @"AIPLLTM001\AIPLLTM001", "sa", "sa1985", "^21289", "ADMIN", @"D:\Vudyoggst\Bmp\Icon_VudyogGST.ico", "Vudyoggst", "Vudyoggst.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2018", "31-03-2019", ".F.", "PS", "Point of Sale" };

                //args = new string[] { "1", "P041819", @"AIPLDTM024\SQLExpress", "sa", "sa1985", "^21289", "ADMIN", @"D:\Prof\UdyogERP\Bmp\Icon_VudyogGST.ico", "Udyog ERP Pro", "UdyogERPPRO.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2017", "31-03-2018", ".T." };
                //args = new string[] { "1", "T031819", @"AIPLDTM024\SQLExpress", "sa", "sa1985", "^21289", "ADMIN", @"D:\UdyogERP\Bmp\Icon_VudyogGST.ico", "VudyogGST", "VUDYOGGST.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2017", "31-03-2018", ".T.", "HS" };
                //args = new string[] { "1", "U011819", @"AIPLDTM010\SQL2014", "sa", "sa@1985", "^21289", "ADMIN", @"D:\u2\UdyogERP\Bmp\icon_VUDYOGGST.ico", "UdyogERPENT", "UdyogERPENT.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2018", "31-03-2019", ".T.", "HS", "Pharma Point of Sale" };
                //args = new string[] { "1", "T011819", @"AIPLDTM024\SQLExpress", "sa", "sa1985", "^21289", "ADMIN", @"D:\Pharma Installer\PRENT\UdyogERP\Bmp\Icon_VudyogGST.ico", "Vudyoggst", "Vudyoggst.EXE", "4764", "udPID4764DTM20111213125821", "udPOS", "01-04-2018", "31-03-2019", ".T.", "HS" };
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new udFrmPOSLoginSettings(args));
        }
    }
}
