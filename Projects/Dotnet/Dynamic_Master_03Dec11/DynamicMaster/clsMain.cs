﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;


namespace DynamicMaster
{
    class clsMain
    {
        public clsMain(string[] args)
        {
            

            //{ "209", "T071011", "Udyog11", "sa", "sa@1985", "13032", "DEG", "ADMIN", @"D:\USquare\Bmp\Icon_usquare.ico", "Usquare pack", "Usquare.exe", "1", "udpid6096DTM20110307112715" };/*Rup Add ICO Path,Parent Appl Caption,Parent Appl. Name,Parent Appl PId,Application Log Table*/

            //args = new string[] { "02", "X011516", "PRO_PANKAJ\\SQLEXPRESS", "sa", "sa1985", "^13074", "ADMIN", @"F:\Installer12.0\Bmp\icon_VudyogSDK.ico", "VudyogSDK", "VudyogSDK.exe", "4360", "udPID4360DTM20150921100032" };

            //args = new string[] { "32", "A051819", "AIPLDTM001\\SQLEXPRESS2014", "sa", "sa1985", "^13074", "EXP", "ADMIN", @"D:\UdyogGST\Bmp\Icon_vudyoggst.ico", "UdyogGST", "UdyogERP.exe", "4360", "udPID4360DTM20150921100032" };

            MasterForm oFrmDynamic = new MasterForm();
            oFrmDynamic.pFrmCaption = "Dynamic Master";
            oFrmDynamic.pCompId = Convert.ToInt16(args[0]);
            oFrmDynamic.pComDbnm = args[1];
            oFrmDynamic.pServerName = args[2];
            oFrmDynamic.pUserId = args[3];
            oFrmDynamic.pPassword = args[4];
            if (args[5] != "")
            {
                oFrmDynamic.pPApplRange = args[5].ToString().Replace("^", "");
            }
            oFrmDynamic.pPApplRange = args[5];
            oFrmDynamic.pAppUerName = args[7];
            Icon MainIcon = new System.Drawing.Icon(args[8].Replace("<*#*>", " "));
            oFrmDynamic.pFrmIcon = MainIcon;
            oFrmDynamic.pPApplText = args[9].Replace("<*#*>", " ");
            oFrmDynamic.pPApplName = args[10];
            oFrmDynamic.pPApplPID = Convert.ToInt16(args[11]);
            oFrmDynamic.pPApplCode = args[12];
            oFrmDynamic.PMasterCode = args[6];
            oFrmDynamic.pPara = args;//Alert Master
            Application.Run(oFrmDynamic);            
        }
    }
}
