/*
 * 

CREATE PROCEDURE search_account (accountName char(100))
BEGIN
SELECT ACCOUNTID, SCHOOLID, ACCOUNT_SCHOOLID, ACCOUNTNAME, ACCOUNTTYPE, NAME, ACCOUNTSTATE, PASSWORD, IMAGE, SSZY, MAJORDIRECTIONID, SSBJ, FREESWITCH_USER_ID, LASTLOGINTIME, REMARK, TA_PERSON_ID, TA_COLLEGE_ID, TA_EXTERPRISE_ID, update_time, CREATE_UID, T_TCR_FICTIT 
FROM zntbkt_db_nlts.T_EDGE_SYS_ACCOUNT where ACCOUNTNAME = accountName;
END;

 * 
*/

using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MysqlProcduce
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var t = @"C:\Users\qiyub\AppData\Local\Temp\7zS0D7A8853\Teacher";
            var s = @"C:\Users\qiyub\AppData\Local\Temp\7zS0D7A8853\Teacher\AForge.Controls.dll";
            var fn = "AForge.Controls.dll";
            var basic = @"C:\Program Files (x86)\easy-sec\Teacher";
            //var t = @"{basic}\{subdir}\{fileName}";
            var newFile = GetFileFullPath(s, t, basic);


        }

        private string GetFileFullPath(string sourceFullFileName, string sourceBasicDir, string targetBasicDir)
        {
            var fn = new FileInfo(sourceFullFileName).Name;
            var index1 = sourceFullFileName.IndexOf(sourceBasicDir);
            var s1 = sourceFullFileName.Substring(index1 + sourceBasicDir.Length);
            var index2 = s1.IndexOf(fn);
            var subDir = s1.Substring(0, index2 - 1 );
            return Path.Combine(targetBasicDir, subDir, fn);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                {
                    conn.Open();

                    DataTable dt = new DataTable();

                    MySqlDataAdapter ada = new MySqlDataAdapter();
                    var cmd = new MySqlCommand("search_account", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter { DbType = DbType.String, Direction = ParameterDirection.Input,ParameterName= "accountName", Value ="18119891999" });
                    ada.SelectCommand = cmd;
                    var rst = ada.Fill(dt);

                }

                Console.WriteLine($"[OK]");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
