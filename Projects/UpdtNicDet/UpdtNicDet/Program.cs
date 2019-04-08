using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Udyog.Library.Common;
using System.Windows.Forms;

namespace UpdtNicDet
{
    class Program
    {

        public static void Main(string[] args)
        {
            int pwdlen = 0;
            string appPath = string.Empty;
            if (args.Length <= 0)
                appPath = Environment.CurrentDirectory;
            else
                appPath = args[0].Replace("<*#*>"," ");

            string SqlConnStr = GetConnectionString(appPath);

            if (SqlConnStr.Length <= 0)
            {
                Environment.Exit(1);
                return;
            }
            SqlConnection con = new SqlConnection(SqlConnStr);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select b.length as fldlen from vudyog..sysobjects a inner join vudyog..syscolumns  b on (a.id=b.id) where a.xtype='U' and a.[Name]='co_mast' and b.[name]='nicpwd'";
            cmd.Connection = con;
            ConnOpen(con);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    pwdlen = Convert.ToInt32(reader[0]);
                }
            }
            else
            {
                Console.WriteLine("No rows found.");

            }
            reader.Close();

            if (pwdlen < 100)
            {
                cmd.CommandText = "Execute Vudyog..Add_columns 'Co_mast','NICPWD1 VARCHAR(100)'";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Update Vudyog..Co_mast set NICPWD1=NICPWD";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Execute Vudyog..ALTER_columns 'Co_mast','NICPWD VARCHAR(100)'";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Select CompId,Nicpwd,Nicpwd1 From Vudyog..Co_mast Where  NICPWD1<>''";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable compDt = new DataTable("Comp_vw");
                da.Fill(compDt);

                string UpdateString = string.Empty;
                string newPwd = string.Empty;
                for (int i = 0; i < compDt.Rows.Count; i++)
                {
                    newPwd = VU_UDFS.dec(VU_UDFS.ondecrypt(compDt.Rows[i]["Nicpwd1"].ToString().Trim()));
                    UpdateString = UpdateString + " Update Vudyog..Co_mast set Nicpwd='" + VU_UDFS.ASCIIToHexDecimal(VU_UDFS.NewENCRY(newPwd, "Ud*yog+1993NIC")) + "' Where CompId=" + compDt.Rows[i]["CompId"].ToString();
                }

                cmd.CommandText = UpdateString;
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Alter table Vudyog..co_mast drop column NICPWD1";
                cmd.ExecuteNonQuery();
            }
            ConnClose(con);

        }

        public static void ConnOpen(SqlConnection conn)
        {
            if (conn != null)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
            }
            else
            {
                conn.Open();
            }
        }
        public static void ConnClose(SqlConnection conn)
        {
            if (conn != null)
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public static string GetConnectionString(string apath)
        {
            string constr = string.Empty;
            GetInfo.iniFile ini = new GetInfo.iniFile(Path.Combine(apath, "Visudyog.ini"));
            if (!File.Exists(ini.path))
                return string.Empty;

            GetInfo.EncDec eObject = new GetInfo.EncDec();
            //_apptitle = ini.IniReadValue("Settings", "Title");
            //_appfile = ini.IniReadValue("Settings", "xfile").Substring(0, ini.IniReadValue("Settings", "xfile").Length - 4);
            string _server = ini.IniReadValue("DataServer", "Name");
            string _user = ini.IniReadValue("DataServer", eObject.OnEncrypt("myName", eObject.Enc("myName", "User")));
            string _pass = ini.IniReadValue("DataServer", eObject.OnEncrypt("myName", eObject.Enc("myName", "Pass")));
            //_itaxpath = ini.IniReadValue("Settings", "iTaxDbPath");     
            string _nenc = ini.IniReadValue("Settings", "NENC");
            if (_nenc.Trim() == "1")
            {
                _user = VU_UDFS.NewDECRY(VU_UDFS.HexDecimalToASCII(_user), "Ud*yog+1993Uid");
                _pass = VU_UDFS.NewDECRY(VU_UDFS.HexDecimalToASCII(_pass), "Ud*yog+1993Pwd");
            }
            else
            {
                _user = eObject.Dec("myName", eObject.OnDecrypt("myName", _user));
                _pass = eObject.Dec("myName", eObject.OnDecrypt("myName", _pass));
            }
            constr = "Data Source=" + _server.Trim() + ";Initial Catalog=Master;Uid=" + _user.Trim() + ";Pwd=" + _pass;
            return constr; 
        }
    }
}
