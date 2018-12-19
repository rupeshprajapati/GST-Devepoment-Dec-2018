IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'SalesEnquiry')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14143,'SALESTRANSACTIONS',0,'SalesEnquiry',1,'Sales Enquiry',0,'','DO UEVOUCHER WITH "EQ","","","","","",0,.T., "^14143"',0,0,0,0,0,0,0,0,'Transaction',0,'Sales Enquiry',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'SalesOrder')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14144,'SALESTRANSACTIONS',0,'SalesOrder',1,'Sales Order',0,'','DO UEVOUCHER WITH "SO","","","","","",0,.T., "^14144"',0,0,0,0,0,0,0,0,'Transaction',0,'Sales Order',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'SalesQuotation')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14145,'SALESTRANSACTIONS',0,'SalesQuotation',1,'Sales Quotation',0,'','DO UEVOUCHER WITH "SQ","","","","","",0,.T., "^14145"',0,0,0,0,0,0,0,0,'Transaction',0,'Sales Quotation',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'TerminateOrder')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14146,'SALESTRANSACTIONS',0,'TerminateOrder',1,'Terminate Order',0,'','DO UEVOUCHER WITH "TR","","","","","",0,.T., "^14146"',0,0,0,0,0,0,0,0,'Transaction',0,'Terminate Order',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'DespatchApprovalMemo')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14136,'SALESTRANSACTIONS',0,'DespatchApprovalMemo',1,'Despatch Approval Memo',0,'','DO UEVOUCHER WITH "DM","","","","","",0,.T., "^14136"',0,0,0,0,0,0,0,0,'Transaction',0,'DISPATCH APPROVAL MEMO',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'EXPORTSALES')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14135,'SALESTRANSACTIONS',0,'EXPORTSALES',1,'EXPORT SALES',0,'','DO UEVOUCHER WITH "EI","","","","","",0,.T., "^14135"',0,0,0,0,0,0,0,0,'Transaction',0,'EXPORT SALES',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'PROCUREMENTCERTIFICATE')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14130,'PURCHASETRANSACTIONS',0,'PROCUREMENTCERTIFICATE',1,'PROCUREMENT CERTIFICATE',0,'','DO UEVOUCHER WITH "PH","","","","","",0,.T., "^14130"',0,0,0,0,0,0,0,0,'General',0,'PROCUREMENT CERTIFICATE',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'InterDeptReceipt')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14141,'InventoryTransaction',0,'InterDeptReceipt',1,'Inter Dept Receipt',0,'','DO UEVOUCHER WITH "IR","","","","","",0,.T., "^14141"',0,0,0,0,0,0,0,0,'Transaction',0,'Inter Dept Receipt',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'B17BONDDEBIT')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14142,'EXCISETRANSACTIONS',0,'B17BONDDEBIT',1,'B17 BOND DEBIT',0,'','DO UEVOUCHER WITH "BD","BALANCE WITH B17-BOND","","","","B17 BOND DEBIT",0,.T., "^14142"',0,0,0,0,0,0,0,0,'Transaction',0,'B17 BOND DEBIT',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'InterDeptIssue')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14140,'InventoryTransaction',0,'InterDeptIssue',1,'Inter Dept Issue',0,'','DO UEVOUCHER WITH "II","","","","","",0,.T., "^14140"',0,0,0,0,0,0,0,0,'Transaction',0,'Inter Dept Issue',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'SERVICETAXCREDIT')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14074,'SERVICETAXEXCISETRANSACTIONS',0,'SERVICETAXCREDIT',1,'SERVICE TAX CREDIT',0,'','DO UEVOUCHER WITH "VI","","","","","SERVICE TAX (CREDIT)",0,.T., "^14074"',1,1,0,0,0,0,0,0,'Transaction',0,'',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'B17BONDCREDIT')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14138,'EXCISETRANSACTIONS',0,'B17BONDCREDIT',1,'B17 BOND CREDIT',0,'','DO UEVOUCHER WITH "BC","BALANCE WITH B17-BOND","","","","B17 BOND CREDIT",0,.T., "^14138"',0,0,0,0,0,0,0,0,'Transaction',0,'B17 BOND CREDIT',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'CESSONSERVICETAXDEBIT')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14075,'SERVICETAXEXCISETRANSACTIONS',0,'CESSONSERVICETAXDEBIT',1,'CESS ON SERVICE TAX DEBIT',0,'','DO UEVOUCHER WITH "VR","BALANCE WITH SERVICE TAX CESS A/C","","","","CESS ON SERVICE TAX (DEBIT)",0,.T., "^14075"',1,1,0,0,0,0,0,0,'Transaction',0,'',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'WORKINPROGRESSWIPISSUE')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14028,'PRODUCTIONTRANSACTIONS',0,'WORKINPROGRESSWIPISSUE',1,'WORK IN PROGRESS ( W I P ) ISSUE',0,'','DO UEVOUCHER WITH "WI","","","","","",0,.T., "^14028"',1,1,0,0,1,0,0,0,'Transaction',0,'',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'WORKINPROGRESSWIPRECEIPT')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14029,'PRODUCTIONTRANSACTIONS',0,'WORKINPROGRESSWIPRECEIPT',1,'WORK IN PROGRESS ( W I P ) RECEIPT',0,'','DO UEVOUCHER WITH "WO","","","","","",0,.T., "^14029"',1,1,0,0,1,0,0,0,'Transaction',0,'',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'EXCESSSTOCK')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14148,'InventoryTransaction',0,'EXCESSSTOCK',1,'EXCESS STOCK',0,'','DO UEVOUCHER WITH "ES","","","","","",0,.T., "^14148"',0,0,0,0,0,0,0,0,'Transaction',0,'EXCESS STOCK',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'SHORTAGESTOCK')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14149,'InventoryTransaction',0,'SHORTAGESTOCK',1,'SHORTAGE STOCK',0,'','DO UEVOUCHER WITH "SS","","","","","",0,.T., "^14149"',0,0,0,0,0,0,0,0,'Transaction',0,'SHORTAGE STOCK',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'PURCHASERETURN')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14151,'PURCHASETRANSACTIONS',0,'PURCHASERETURN',1,'PURCHASE RETURN',0,'','DO UEVOUCHER WITH "PR","","","","","",0,.T., "^14151"',0,0,0,0,0,0,0,0,'Transaction',0,'PURCHASE RETURN',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'PURCHASEORDER')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14152,'PURCHASETRANSACTIONS',0,'PURCHASEORDER',1,'PURCHASE ORDER',0,'','DO UEVOUCHER WITH "PO","","","","","",0,.T., "^14152"',0,0,0,0,0,0,0,0,'Transaction',0,'PURCHASE ORDER',0,'','','',0,0,0,0) END 
IF NOT EXISTS(SELECT [barname] FROM com_menu WHERE barname = 'CT3')BEGIN INSERT INTO com_menu([RANGE],[PADNAME],[PADNUM],[BARNAME],[BARNUM],[PROMPNAME],[NUMITEM],[HOTKEY],[PROGNAME],[E_],[N_],[R_],[T_],[I_],[O_],[B_],[X_],[MENUTYPE],[ISACTIVE],[PUSER],[MDEFAULT],[LABKEY],[SKIPFOR],[CPROG],[MARK],[XTVS_],[DSNI_],[MCUR_]) VALUES (14129,'PURCHASETRANSACTIONS',0,'CT3',1,'CT3',0,'','DO UEVOUCHER WITH "CT","","","","","",0,.T., "^14129"',0,0,0,0,0,0,0,0,'General',0,'CT3',0,'','','',0,0,0,0) END 