IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TipsMaster]') AND type in (N'U'))
begin
	DROP TABLE [dbo].[TipsMaster]
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipsMaster](
	[tip] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[tipid] [int] IDENTITY(1,1) NOT NULL
) 
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
go
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' Easy to install,   maintain    and operate   with existing manpower...   No  prior computer knowledge required   ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' With Udyog Customer support  You can always expect prompt response     ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' Service Satisfaction is guaranteed  ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' Save your precious time ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' Shortcuts are most wanted things in life to sweeten it.  The best way to become lazy is to adopt the shortcuts,  but you will be fast in achieving your goals too.  In Computers, it''s nothing but combination of one or more keys, Mainly ALT and CTRL keys are used to activate the shortcuts along with the corresponding key, e.g. ALT + F4 key is to close any active program in windows, even to shut down Windows. In UDYOG we provide many shortcuts hoping that you will be fast doing task rather than slogging more and more....  ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' HOT-KEYS...    We call the Shortcut Keys as HOT-KEYS in short....  The underlined letter in menu options are the one which generally called as Hot Keys.  (Please note, many of these following short cuts work only in UDYOG Software)     ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+A : This  will take you to Account Master Entry Module directly.  It reduces the number of key strokes.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+C : This hot key gives direct access to Export Reports menu.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+D: This hot key gives direct access Dynamic Reports menu.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+F: This hot key gives direct access to File menu.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+I : This  will take you to Goods and Services Master Entry Module directly.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+M : This  will take you to Master menu.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+R : This  will take you to Report menu. ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+S : This  will take you to Setting menu.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+T : This  will take you to Transaction menu. ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+Y : This  will take you to Security menu.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+S : The great option to save Transaction/any entry after feeding the required data, without going through the whole cycle of data entry screen.  For example, say you want to modify the broker name in a particular sale bill, after selecting the Modify button, you have to go through only upto Category option, after changing this, press CTRL + S, this will save you a lot of time. ')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+Home : This is Navigation option. It will take you to First Record.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+Page Up : This is Navigation option. It will take you to Previous Record.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+Page Down : This is Navigation option. It will take you to Next Record.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+End : This is Navigation option. It will take you to Last Record.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+F : This is direct E-Mail option for Transaction Entry Report.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+L : This will take you to Locate window Transaction Entry.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+N : This  will take you to new Record.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+E : This  will take you to Edit Record.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+D : This will prompt you to Delete Record.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+C : This will allow you to copy  Entries.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+O : This  will take you to Preview the Report.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CTRL+P : This  will take you to Print Report.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+F4 : This will allow you to Shut down application.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+L : This will take you to Relogin Window without closing application.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' F12 : Pressing F12 activates CALCULATOR, so you can do arithmetic calculations any time while you are in Udyog package.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+A :  In Transaction Entry, pressing this key shows the Additional information entered without modifying the Voucher, so you can re-confirm or view the same.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' Alter_T :  In TDS related Transaction Entry, pressing this key shows the TDS Details entered without modifying the Voucher, so you can re-confirm or view the same.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+S :  In Service Tax related Transaction Entry, pressing this key shows the Service Tax Details entered without modifying the Voucher, so you can re-confirm or view the same.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+N :  In Transaction Entry, pressing this key shows the Narration entered without modifying the Voucher, so you can re-confirm or view the same.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' ALT+E :  In Goods and Services Tax related Transaction Entry, pressing this key shows the Goods and Services Details entered without modifying the Voucher, so you can re-confirm or view the same.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' VOUCHER SETTING: In Voucher setting, you can activate / deactivate  Due Date, Automatic Voucher Number Generation, Narration, etc. for any particular Voucher. There is a options to get narration automatically in Sales Payment and Purchase Payment Vouchers(BP/CP/BR/CR), i.e. type "FORM ALLOCATION WITH DATE" in default narration option of these voucher types. If one don''t want dates, remove ''WITH DATE''')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' CONTINIOUS PRINTING: PRINTING / VIEWING OF VOUCHER FROM 1 TO N  nos. with N  number of copies is possible.  At a time if one wants to take printout of more than 1 Bill/Transaction, this is the option.  Mention Starting & Ending number, or date or any condition which suits & put continuous stationery & print.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' BACK UP: In Utilities Menu, Back Up Option will take your precious data. You can also set shedular for database Backup.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' HOLIDAY:  Is in Utilities, will help to put all the holidays of the year to prevent any entry to be passed on these days.')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' SPECIAL DAY:  In utilities.  It''s an Reminder.  Accounting Software is the only one thing which is used at least once in a day, just to make bill, to view the outstanding, for bank balances etc.  So here you get the appropriate reminder for that day. You may use this for keeping records like, Goods and Services tax returning day, Railway pass expiry day, your boss''s birth day etc...')
INSERT INTO [TipsMaster] ([tip]) VALUES (char(13)+' SQL Editor : This option will allow you to Database related task. i.e. Alter table structure, Execute .sql script files, Alter Stored Procedure.')





