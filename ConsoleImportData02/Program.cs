/*
 * 
 * 批量插入数据到mysql
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Data;

namespace ConsoleImportData02
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var conn = new MySqlConnection($"server=192.168.10.168;database=zntbkt_db_nlts;PORT=3306; uid=root;pwd=kjhzzx12+ES;charset=gb2312"))
                {
                    conn.Open();

                    var ada = new MySqlDataAdapter($"SELECT ACCOUNTID FROM zntbkt_db_nlts.T_EDGE_SYS_ACCOUNT where ssbj = '9e5ddf50722146479303b85b5a937cc6'", conn);
                    var dt = new DataTable();
                    ada.Fill(dt);

                    var r = new Random((int)DateTime.Now.ToFileTimeUtc());
                    var f = r.Next(0, 10);
                    //var t = r.Next(f, 10 + r.Next(0, 10));
                    //var s = string.Format(sql, f, t);
                    //var ada = new MySqlDataAdapter(s, conn);
                    var Total = dt.Rows.Count;
                    for (var i = 0; i < Total; i++)
                    {
                        Console.WriteLine($"[Master]{i}/{Total}");

                        var sql = $"insert into t_learner_answer(Learner_ID, Resource_ID, U_School, Status, Learner_Score, ErrorNo, Subjective_ErrorNo, AnswerList, AnswerURL, ReviewURL, AnswerFile, AnswerPreviewFile, AnswerFileName, AnswerFileSize, AnswerFileSuffix, AnswerFileType, Comment, CreateUID, CreateTime, UpdateUID, UpdateTime, TIME_TABLE_ID) values (@Learner_ID, @Resource_ID, @U_School, @Status, @Learner_Score, @ErrorNo, @Subjective_ErrorNo, @AnswerList, @AnswerURL, @ReviewURL, @AnswerFile, @AnswerPreviewFile, @AnswerFileName, @AnswerFileSize, @AnswerFileSuffix, @AnswerFileType, @Comment, @CreateUID, @CreateTime, @UpdateUID, @UpdateTime, @TIME_TABLE_ID)";
                        var cmd = new MySqlCommand(sql, conn);
                        var Learner_ID = dt.Rows[i]["ACCOUNTID"].ToString();
                        var Resource_ID = "71e4958c-23ae-4667-a744-5931fbac9662";
                        var U_School = "qinghua";
                        var Status = r.Next(1, 2).ToString();
                        var Learner_Score = r.Next(10, 100).ToString();
                        var ErrorNo = r.Next(1, 4).ToString();
                        var Subjective_ErrorNo = r.Next(1, 4).ToString();
                        var AnswerList = $"学习者客观题答案-{r.Next(1, 4)}";
                        var AnswerURL = $"http://www.baidu.com/?_={r.Next(1000, 100000)}";
                        var ReviewURL = $"http://www.baidu.com/?_={r.Next(1000, 100000)}";
                        var AnswerFile = $"abc{r.Next(1, 9)}.txt";
                        var AnswerPreviewFile = $"pre_abc{r.Next(1, 9)}.log";
                        var AnswerFileName = $"fn{r.Next(1, 9)}.txt";
                        var AnswerFileSize = r.Next(100000, 900000).ToString();
                        var AnswerFileSuffix = "fix";
                        var AnswerFileType = "1,2";
                        var Comment = Guid.NewGuid().ToString();
                        var CreateUID = "liuzhenhua";
                        var CreateTime = DateTime.Now;
                        var UpdateUID = "Liuzhenhua";
                        var UpdateTime = DateTime.Now;
                        var TIME_TABLE_ID = "2b5888c27364452690348209c9cf0fe9";
                        cmd.Parameters.AddRange(new[]
                        {
                            new MySqlParameter { ParameterName = "@Learner_ID", Value=Learner_ID, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@Resource_ID", Value=Resource_ID, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@U_School", Value=U_School, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@Status", Value=Status, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@Learner_Score", Value=Learner_Score, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@ErrorNo", Value=ErrorNo, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@Subjective_ErrorNo", Value=Subjective_ErrorNo, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@AnswerList", Value=AnswerList, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@AnswerURL", Value=AnswerURL, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@ReviewURL", Value=AnswerURL, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@AnswerFile", Value=AnswerFile, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@AnswerPreviewFile", Value=AnswerPreviewFile, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@AnswerFileName", Value=AnswerFileName, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@AnswerFileSize", Value=AnswerFileSize, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@AnswerFileSuffix", Value=AnswerFileSuffix, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@AnswerFileType", Value=AnswerFileType, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@Comment", Value=Comment, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@CreateUID", Value=CreateUID, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@CreateTime", Value=CreateTime, DbType = DbType.DateTime, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@UpdateUID", Value=UpdateUID, DbType = DbType.String, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@UpdateTime", Value=UpdateTime, DbType = DbType.DateTime, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "@TIME_TABLE_ID", Value=TIME_TABLE_ID, DbType = DbType.String, Direction = ParameterDirection.Input },
                        });

                        var iResult = cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        var dt2 = new DataTable();
                        var ada2 = new MySqlDataAdapter($"select Resource_Details_ID from t_exercise_test_details_private where  Resource_ID = '{Resource_ID}'", conn);
                        ada2.Fill(dt2);

                        for(var rowIndex = 0; rowIndex < dt2.Rows.Count; rowIndex++)
                        {
                            Console.WriteLine($"-----------::{rowIndex}/{dt2.Rows.Count}");
                            var sqlDetail = "insert into t_learner_answer_detail(Learner_ID, Course_ID, Resource_ID, Resource_Detail_ID, U_School, Answer, RightOrWrong, CreateTime, TIME_TABLE_ID) values (@Learner_ID, @Course_ID, @Resource_ID, @Resource_Detail_ID, @U_School, @Answer, @RightOrWrong, @CreateTime, @TIME_TABLE_ID )";
                            var cmdDetail = new MySqlCommand(sqlDetail, conn);
                            var Course_ID = "123-555";
                            var Resource_Detail_ID = dt2.Rows[rowIndex][0].ToString();
                            var _a = r.Next(1, 4);
                            var Answer = _a == 1 ? "A" : _a == 2 ? "B" : _a == 3 ? "C" : "D";
                            var RightOrWrong = _a % 2 == 0 ? 0 : 1;
                            cmdDetail.Parameters.AddRange(new[]
                            {
                                new MySqlParameter { ParameterName = "@Learner_ID", Value=Learner_ID, DbType = DbType.String, Direction = ParameterDirection.Input },
                                new MySqlParameter { ParameterName = "@Course_ID", Value=Course_ID, DbType = DbType.String, Direction = ParameterDirection.Input },
                                new MySqlParameter { ParameterName = "@Resource_ID", Value=Resource_ID, DbType = DbType.String, Direction = ParameterDirection.Input },
                                new MySqlParameter { ParameterName = "@Resource_Detail_ID", Value=Resource_Detail_ID, DbType = DbType.String, Direction = ParameterDirection.Input },
                                new MySqlParameter { ParameterName = "@U_School", Value=U_School, DbType = DbType.String, Direction = ParameterDirection.Input },
                                new MySqlParameter { ParameterName = "@Answer", Value=Answer, DbType = DbType.String, Direction = ParameterDirection.Input },
                                new MySqlParameter { ParameterName = "@RightOrWrong", Value=RightOrWrong, DbType = DbType.Int32, Direction = ParameterDirection.Input },
                                new MySqlParameter { ParameterName = "@CreateTime", Value=CreateTime, DbType = DbType.String, Direction = ParameterDirection.Input },
                                new MySqlParameter { ParameterName = "@TIME_TABLE_ID", Value=TIME_TABLE_ID, DbType = DbType.String, Direction = ParameterDirection.Input },
                            });
                            var iResultDetail = cmdDetail.ExecuteNonQuery();
                            cmdDetail.Dispose();
                        }
                    }
                }

                Console.WriteLine($"[OK]");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

        }
    }
}
