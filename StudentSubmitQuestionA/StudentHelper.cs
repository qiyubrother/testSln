using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using qiyubrother;

namespace StudentSubmitQuestionA
{
    public class StudentHelper
    {
        public static IList<string> GetStudentList()
        {
            var lst = new List<string>();

            var dt = MySqlHelper.GetData($"SELECT a.ACCOUNTNAME FROM zntbkt_db_nlts.T_EDGE_SYS_ACCOUNT a inner join zntbkt_db_nlts.T_EDGE_SYS_ACCOUNT_ROLE b on a.ACCOUNTID = b.ACCOUNTID where b.ROLEID = 3");
            foreach(DataRow dr in dt.Rows)
            {
                lst.Add(dr["ACCOUNTNAME"].ToString());
            }

            return lst;
        }

        public static IList<string> GetStudentList(string fileName) => File.ReadAllLines(fileName);
    }
}
