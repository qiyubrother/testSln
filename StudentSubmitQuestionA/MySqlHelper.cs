using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qiyubrother
{
    public class MySqlHelper
    {
        public static string ConnectionString = $"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312";
        Random r = new Random((int)DateTime.Now.ToFileTimeUtc());
        public static DataTable GetData(string sql)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                var ada = new MySqlDataAdapter(sql, conn);
                try
                {
                    conn.Open();
                    var dt = new DataTable();
                    ada.Fill(dt);

                    return dt;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new DataTable();
                }
                finally
                {
                    ada.Dispose();
                }
            }
        }
    }
}
