/*

  测试数据库

*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace MySQLConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var sql = string.Empty;

            var r = new Random((int)DateTime.Now.ToFileTimeUtc());
            sql = File.ReadAllText("sql.txt");
                var counter = 0;
                Parallel.For(0, 10000, new Action<int>((i) => {
                    Parallel.Invoke(new Action[]
                    {
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                        ()=>{
                            using(var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                            {
                                try
                                {
                                    conn.Open();
                                    Console.WriteLine($"Counter:{counter++}");
                                    var f = r.Next(0, 10);
                                    var t = r.Next(f, 10 + r.Next(0, 10));
                                    var s = string.Format(sql, f, t);
                                    var ada = new MySqlDataAdapter(s, conn);
                                    var dt = new DataTable();
                                    ada.Fill(dt);
                                    ada.Dispose();
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        },
                    }); ;

                }));

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}]Task finished.");
        }
    }
}
