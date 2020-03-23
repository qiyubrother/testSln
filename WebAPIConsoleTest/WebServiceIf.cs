using Microsoft.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WebAPIConsoleTest
{
    public class WebServiceIf
    {
        /// <summary>
        /// 获取加密的文件地址
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Question_ID"></param>
        /// <param name="Account_Role"></param>
        /// <returns></returns>
        public  string WSgetResourceRealPath(string  FileUrl,int times)
        {
            LogHelper.Trace("---------------WSgetResoureRealPath(获取加密的文件地址)------------------------");
            string url = GlobalValue.resource_server;
            string method = "getResourceRealPath";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Resource_Path", FileUrl));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "20002")
                {//使用refresh重新认证成功后再请求一次数据
                    if (times == 1)
                    {
                       
                    }
                    else if (times == 2)
                    {
                        
                    }
                }
                else if (status == "20001")
                {//token错误

                }
                else if (status == "20102")
                {//服务器错误
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 老师回复-学生文字提问
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Question_ID"></param>
        /// <returns></returns>
        public async Task<string> WSteacherReplyFroClass(int times, string Type ,string Question_ID,string titleLb,string Content)
        {
            LogHelper.Trace("---------------WSteacherReplyFroClass(老师回复-学生文字提问)------------------------");
            string url = GlobalValue.resource_server;
            string method = "teacherReplyFroClass";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("Type_ID", Question_ID));
            jObj.Add(new JProperty("Type", Type));
            jObj.Add(new JProperty("Title", titleLb));
            jObj.Add(new JProperty("Content", Content));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {   //使用refresh重新认证成功后再请求一次数据
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSteacherReplyFroClass(2, Type,Question_ID, titleLb, Content);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {   //token错误
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSteacherReplyFroClass(2, Type, Question_ID, titleLb, Content);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {   //服务器错误
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取课堂学生文字提问详情
        /// </summary>
        /// <param name="times"></param>
        /// <param name="StudentCount"></param>
        /// <returns></returns>
        public async Task<string> WSgetQuestionDetailsFroClass(int times, string Question_ID,string Account_Role)
        {
            LogHelper.Trace("---------------WSgetQuestionDetailsFroClass(获取课堂学生文字提问详情)------------------------");
            string url = GlobalValue.resource_server;
            string method = "getQuestionDetailsFroClass";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            //jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Question_ID", Question_ID));
            jObj.Add(new JProperty("Account_Role", Account_Role));
            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "20002")
                {//使用refresh重新认证成功后再请求一次数据
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetQuestionDetailsFroClass(2, Question_ID, Account_Role);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {//token错误
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetQuestionDetailsFroClass(2, Question_ID, Account_Role);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {//服务器错误
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取学生文字提问列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="StudentCount"></param>
        /// <param name="Speak_ID"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public async Task<string> WSgetQuestionListFroClass(int times, string StudentCount)
        {
            LogHelper.Trace("---------------WSgetQuestionListFroClass(获取学生文字提问列表)------------------------");
            string url = GlobalValue.resource_server;
            string method = "getQuestionListFroClass";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Account_Name", StudentCount));
            jObj.Add(new JProperty("Plan_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200")||(status == "2001"))
                {
                    return str_res;
                }
                else if (status == "20002")
                {//使用refresh重新认证成功后再请求一次数据
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetQuestionListFroClass(2, StudentCount);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {//token错误
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetQuestionListFroClass(2, StudentCount);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {//服务器错误
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 更新学生语音回答的次数
        /// </summary>
        /// <param name="times"></param>
        /// <param name="StudentCount"></param>
        /// <returns></returns>
        public async Task<string> WSupdateStudentSpeakCount(int times, string StudentCount,string Speak_ID,int Type)
        {
            LogHelper.Trace("---------------WSupdateStudentSpeakCount(更新学生语音回答的次数)------------------------");
            string url = GlobalValue.resource_server;
            string method = "updateStudentSpeakCount";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Account_Name", StudentCount));
            jObj.Add(new JProperty("Speak_ID", Speak_ID));
            jObj.Add(new JProperty("Type", Type));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "20002")
                {//使用refresh重新认证成功后再请求一次数据
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSupdateStudentSpeakCount(2, StudentCount, Speak_ID,Type);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {//token错误
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSupdateStudentSpeakCount(2, StudentCount, Speak_ID, Type);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {//服务器错误
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 告诉服务器已经处理该学生的举手
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Account_Name_Str"></param>
        /// <returns></returns>
        public async Task<string> WSupdateStudentSpeak(int times,string Speak_ID)
        {
            LogHelper.Trace("---------------WSgetStudentListByAccount(告诉服务器已经处理该学生的举手)------------------------");
            string url = GlobalValue.resource_server;
            string method = "updateStudentSpeakCount";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Speak_ID", Speak_ID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "20002")
                {//使用refresh重新认证成功后再请求一次数据
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSupdateStudentSpeak(2, Speak_ID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {//token错误
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSupdateStudentSpeak(2, Speak_ID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {//服务器错误
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Account_Name_Str"></param>
        /// <returns></returns>
        public async Task<string> WSgetStudentListByAccount(int times,string Account_Name_Str, int pageNum, int perNum)
        {
            LogHelper.Trace("---------------WSgetStudentListByAccount(分析获取学生列表)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getStudentListByAccount";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Account_Name_Str", Account_Name_Str));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("pageNumber", pageNum));
            jObj.Add(new JProperty("pageSize", perNum));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "20002")
                {//使用refresh重新认证成功后再请求一次数据
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetStudentListByAccount(2, Account_Name_Str,pageNum, perNum);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {//token错误
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetStudentListByAccount(2, Account_Name_Str, pageNum, perNum);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {//服务器错误
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取测试分析错题排行榜
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Resource_ID"></param>
        /// <returns></returns>
        public async Task<string> WSgetExerciseAnalysisRank(int times, string Resource_ID)
        {
            LogHelper.Trace("---------------WSgetExerciseAnalysisRank(获取测试分析错题排行榜)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getExerciseAnalysisRank";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            //jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("T_T_T_BH", GlobalValue.T_T_T_BH));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                //LogHelper.Trace("resStr:{0}", str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetExerciseAnalysisRank(2, Resource_ID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetExerciseAnalysisRank(2, Resource_ID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取测试分析错题数量
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetExerciseAnalysisCount(int times, string Resource_ID)
        {
            LogHelper.Trace("---------------WSgetExerciseAnalysisCount(获取测试分析错题数量)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getExerciseAnalysisCount";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            //jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("Class_IDS", GlobalValue.T_T_T_BH));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                //LogHelper.Trace("resStr:{0}", str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetExerciseAnalysisCount(2, Resource_ID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetExerciseAnalysisCount(2, Resource_ID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取错题数统计数据
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetAllQuestionAnalysisCount(int times)
        {
            LogHelper.Trace("---------------WSgetAllQuestionAnalysisCount(获取错题数统计数据)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getAllQuestionAnalysisCount";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            //jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("Class_IDS", GlobalValue.T_T_T_BH));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                //LogHelper.Trace("resStr:{0}", str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;

                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetAllQuestionAnalysisCount(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetAllQuestionAnalysisCount(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取错题排行榜数据
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Resource_ID"></param>
        /// <returns></returns>
        public async Task<string> WSgetAllQuestionAnalysisRank(int times)
        {
            LogHelper.Trace("---------------WSgetAllQuestionAnalysisRank(获取错题排行榜数据)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getAllQuestionAnalysisRank";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            //jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("Class_IDS", GlobalValue.T_T_T_BH));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                //LogHelper.Trace("resStr:{0}", str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;

                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetAllQuestionAnalysisRank(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetAllQuestionAnalysisRank(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取展示问题分析数据
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Resource_ID"></param>
        /// <returns></returns>
        public async Task<string> WSgetShowQuestionAnalysis(int times, string Resource_ID)
        {
            LogHelper.Trace("---------------WSgetShowQuestionAnalysis(获取展示问题分析数据)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getShowQuestionAnalysis";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("Class_IDS", GlobalValue.T_T_T_BH));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                //LogHelper.Trace("resStr:{0}", str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetShowQuestionAnalysis(2, Resource_ID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetShowQuestionAnalysis(2, Resource_ID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取问题分析的问题列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Resource_ID"></param>
        /// <returns></returns>
        public async Task<string> WSgetAllQuestionList(int times,string quesType)
        {
            LogHelper.Trace("---------------WSgetAllQuestionList(获取问题分析的问题列表)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getAllQuestionList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_Type", quesType));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("Class_IDS", GlobalValue.T_T_T_BH));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                //LogHelper.Trace("resStr:{0}", str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetAllQuestionList(2, quesType);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetAllQuestionList(2, quesType);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取单个问题分析数据
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetQuestionAnalysis(int times,string Resource_ID)
        {
            LogHelper.Trace("---------------WSgetQuestionAnalysis(获取单个问题分析数据)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getQuestionAnalysis";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("Class_IDS", GlobalValue.T_T_T_BH));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                //LogHelper.Trace("resStr:{0}", str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetQuestionAnalysis(2, Resource_ID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetQuestionAnalysis(2, Resource_ID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取老师发送历史记录列表
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetResourceSendList(int times)
        {
            LogHelper.Trace("---------------WSgetResourceSendList(获取老师发送历史记录列表)------------------------");
            string url = GlobalValue.resource_server;
            string method = "getResourceSendList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("Class_IDS", GlobalValue.T_T_T_BH));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetResourceSendList(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetResourceSendList(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 备课资料中老师批量发送资源
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSsendResourceTeacher(int times,int Resend, JObject jObjs)
        {
            LogHelper.Trace("---------------WSsendResourceTeacher(备课资料中老师批量发送资源)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "sendResourceTeacher";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = jObjs;
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Teacher_ID", GlobalValue.UserCount));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resend", Resend));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSsendResourceTeacher(2, Resend, jObjs);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSsendResourceTeacher(2, Resend, jObjs);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        public async Task<string> WSgetFileTypeList(int times)
        {
            LogHelper.Trace("---------------WSgetFileTypeList(获取资源格式)------");
            string url = GlobalValue.resource_server;
            string method = "getFileTypeList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            //jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            //jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSselectResourceTypeListByParam(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSselectResourceTypeListByParam(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取资源列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="ResourceType"></param>
        /// <param name="ResourceFormat"></param>
        /// <param name="ResourceSort"></param>
        /// <returns></returns>
        public async Task<string> WSselectResourceTypeListByParam(int times)
        {
            LogHelper.Trace("---------------WSselectResourceTypeListByParam(获取资源列表)------");
            string url = GlobalValue.resource_server;
            string method = "selectResourceTypeListByParam";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSselectResourceTypeListByParam(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSselectResourceTypeListByParam(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取校本资源列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="ResourceType"></param>
        /// <param name="ResourceFormat"></param>
        /// <param name="ResourceSort"></param>
        /// <returns></returns>
        public async Task<string> WSgetResourceList(int times, string ResourceType, string ResourceFormat, 
            string ResourceSort,string SpecialyClassify,string SpecialtyDirection)
        {
            LogHelper.Trace("---------------WSgetResourceList(获取校本资源列表)----------");
            string url = GlobalValue.resource_server;
            string method = "getResourceList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));

            jObj.Add(new JProperty("ttrtid", ResourceType));
            jObj.Add(new JProperty("Major_PID", SpecialyClassify));
            jObj.Add(new JProperty("Major_ID", SpecialtyDirection));
            jObj.Add(new JProperty("ttrfiletype", ResourceFormat));
            jObj.Add(new JProperty("sort", ResourceSort));

            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetResourceList(2, ResourceType, ResourceFormat, ResourceSort, SpecialyClassify, SpecialtyDirection);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetResourceList(2, ResourceType, ResourceFormat, ResourceSort, SpecialyClassify, SpecialtyDirection);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取校本资料库后缀
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetResourceFileSuff(int times,string  ResourceType, string ResourceFormat,
            string ResourceSort)
        {
            LogHelper.Trace("---------------WSgetResourceFileSuff(获取校本资料库后缀)------------------------");
            string url = GlobalValue.resource_server;
            string method = "getResourceFileSuff";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("ttrtid", ResourceType));
            jObj.Add(new JProperty("ttrfiletype", ResourceFormat));
            //jObj.Add(new JProperty("Course_ID", ResourceSort));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("【{0}】 return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetResourceFileSuff(2, ResourceType, ResourceFormat, ResourceSort);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetResourceFileSuff(2, ResourceType, ResourceFormat, ResourceSort);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取我的资源列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="ResourceType"></param>
        /// <param name="ResourceFormat"></param>
        /// <param name="ResourceSort"></param>
        /// <returns></returns>
        public async Task<string> WSgetResourceListByaid(int times, string ResourceType, string ResourceFormat, string ResourceSort)
        {
            LogHelper.Trace("---------------WSgetResourceListByaid(获取我的资源列表)----------");
            string url = GlobalValue.resource_server;
            string method = "getResourceListByaid";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("ttrtid", ResourceType));
            jObj.Add(new JProperty("ttrfiletype", ResourceFormat));
            jObj.Add(new JProperty("sortNum", ResourceSort));
            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("【{0}】 return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetResourceListByaid(2, ResourceType, ResourceFormat, ResourceSort);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetResourceListByaid(2, ResourceType, ResourceFormat, ResourceSort);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取我的资源筛选框（资源类型，资源后缀）
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetResourceScreen(int times)
        {
            LogHelper.Trace("---------------WSgetResourceScreen(获取我的资源筛选框)-----");
            string url = GlobalValue.resource_server;
            string method = "getResourceScreen";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("【{0}】 return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken)&&(NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetResourceScreen(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetResourceScreen(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取备课资料列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="ResourceType"></param>
        /// <param name="ResourceFormat"></param>
        /// <param name="ResourceSort"></param>
        /// <returns></returns>
        public async Task<string> WSgetResourceListBycourseId(int times,string  ResourceType,string ResourceFormat)
        {
            LogHelper.Trace("---------------WSgetResourceListBycourseId(获取备课资料列表)-------");
            string url = GlobalValue.preclass_server;
            string method = "getResourceListBycourseId";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("Resource_Type", ResourceType));
            jObj.Add(new JProperty("Resource_File_Type", ResourceFormat));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200")||(status == "2001"))
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetResourceListBycourseId(2, ResourceType, ResourceFormat);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetResourceListBycourseId(2, ResourceType, ResourceFormat);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取班级测试情况列表
        /// </summary>
        public async Task<string> WSgetClassTestStudentDetail(int times, string  Resource_ID, int pageNum, int perNum)
        {
            LogHelper.Trace("---------------WSgetClassTestStudentDetail(获取班级测试情况列表)----------");
            string url = GlobalValue.preclass_server;
            string method = "getClassTestStudentDetail";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_ID", Resource_ID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("pageNumber", pageNum));
            jObj.Add(new JProperty("pageSize", perNum));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetClassTestStudentDetail(2, Resource_ID, pageNum, perNum);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetClassTestStudentDetail(2, Resource_ID, pageNum, perNum);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 老师批阅主观题和接收学生上传的信息是否符合标准
        /// </summary>
        /// <param name="times"></param>
        /// <param name="jObjs"></param>
        /// <returns></returns>
        public async Task<string> WSaddStudentScore(int times, JObject jObjs)
        {
            LogHelper.Trace("---------------WSaddStudentScore(批阅主观题和接收学生上传的信息是否符合标准)--");
            string url = GlobalValue.preclass_server;
            string method = "addStudentScore";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = jObjs;
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSaddStudentScore(2, jObjs);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSaddStudentScore(2, jObjs);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 老师点击接收上传,将接收信息发给服务器
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Show_Time"></param>
        /// <param name="Resource_Title"></param>
        /// <param name="Resource_Type"></param>
        /// <returns></returns>
        public async Task<string> WSsendTemporaryQuestion(int times,string Show_Time, string Resource_Title,  int Resource_Type)
        {
            LogHelper.Trace("---------------WSsendTemporaryQuestion(老师点击接收上传)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "sendTemporaryQuestion";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("Course_ID", GlobalValue.curriculumId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Plan_ID", GlobalValue.teachingPlanId));
            //jObj.Add(new JProperty("Course_ID", GlobalValue.cource_tTcId));
            jObj.Add(new JProperty("Show_Time", Show_Time));
            jObj.Add(new JProperty("Resource_Title", Resource_Title));
            jObj.Add(new JProperty("Resource_Type", Resource_Type));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSsendTemporaryQuestion(2, Show_Time, Resource_Title, Resource_Type);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSsendTemporaryQuestion(2, Show_Time, Resource_Title, Resource_Type);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取接收上传历史列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Resource_Type"></param>
        /// <returns></returns>
        public async Task<string> WSgetShowExeciseQuestionListHis(int times,int Resource_Type)
        {
            LogHelper.Trace("---------------WSgetShowExeciseQuestionListHis(获取接收上传历史列表)---------");
            string url = GlobalValue.preclass_server;
            string method = "getShowExeciseQuestionListHis";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_Type", Resource_Type));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetShowExeciseQuestionListHis(2, Resource_Type);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetShowExeciseQuestionListHis(2, Resource_Type);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 提示屏获取学生举手列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="classID"></param>
        /// <returns></returns>
        public async Task<string> WSgetStudentSpeakList(int times, string classID,int pageNum,int perNum)
        {
            LogHelper.Trace("---------------WSgetStudentSpeakList(获取学生举手列表)------------------------");
            string url = GlobalValue.resource_server;
            string method = "getStudentSpeakList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Class_IDS", classID));
            //jObj.Add(new JProperty("Class_ID", classID));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("pageNumber", pageNum));
            jObj.Add(new JProperty("pageSize", perNum));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetStudentSpeakList(2, classID, pageNum, perNum);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetStudentSpeakList(2, classID, pageNum, perNum);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 语音问题获取学生列表
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        /*
        public async Task<string> WSgetVoiceAnswerList(int times,string classID)
        {
            LogHelper.Trace("---------------WSgetVoiceAnswerList(语音问题获取学生列表)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getVoiceAnswerList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Class_ID", classID));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetVoiceAnswerList(2, classID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetVoiceAnswerList(2, classID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        */
        /// <summary>
        /// 语音问题班级列表
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetClassList(int times)
        {
            LogHelper.Trace("---------------WSgetClassList(语音问题班级列表)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getClassList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetClassList(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetClassList(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取学生主观问题回答详情
        /// </summary>
        /// <param name="times"></param>
        /// <param name="resouceID"></param>
        /// <param name="isAnnotation"></param>
        /// <returns></returns>
        public async Task<string> WSgetShowExeciseListNow(int times, string resouceID,string isAnnotation)
        {
            LogHelper.Trace("---------------WSgetShowExeciseListNow(获取学生主观问题回答详情)------------");
            string url = GlobalValue.preclass_server;
            string method = "getShowExeciseListNow";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;
            //string method = "getShowExeciseQuestionListHis";

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_ID", resouceID));
            jObj.Add(new JProperty("isAnnotation", isAnnotation));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetShowExeciseListNow(2, resouceID, isAnnotation);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetShowExeciseListNow(2, resouceID, isAnnotation);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }

            return "";
        }
        /// <summary>
        /// 获取主观题内容详情，标题，描述等信息
        /// </summary>
        /// <param name="times"></param>
        /// <param name="resouceID"></param>
        /// <returns></returns>
        public async Task<string> WSgetShowExeciseDetailHis(int times,string resouceID, int pageNum, int perNum)
        {
            LogHelper.Trace("---------------WSgetShowExeciseDetailHis(获取主观题内容详情，标题，描述等信息)--");
            string url = GlobalValue.preclass_server;
            string method = "getShowExeciseDetailHis";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_ID", resouceID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("pageNumber", pageNum));
            jObj.Add(new JProperty("pageSize", perNum));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetShowExeciseDetailHis(2, resouceID, pageNum, perNum);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetShowExeciseDetailHis(2, resouceID, pageNum, perNum);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取展示问题列表
        /// </summary>
        /// <param name="times"></param>
        /// <param name="Resource_Type"></param>
        /// <returns></returns>
        public async Task<string> WSgetExerciseShowList(int times,int Resource_Type)
        {
            LogHelper.Trace("---------------WSgetExerciseShowList(获取展示问题列表)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getExerciseShowList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_Type", Resource_Type));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetExerciseShowList(2, Resource_Type);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetExerciseShowList(2, Resource_Type);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else 
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 老师发送客观题
        /// </summary>
        /// <param name="times"></param>
        /// <param name="teacher_count"></param>
        /// <param name="courceID"></param>
        /// <param name="resourceID"></param>
        /// <param name="limit_time"></param>
        /// <param name="Resource_Type"></param>
        /// <returns></returns>
        public async Task<int> WSsendExciseTeacher(int times,string teacher_count,
            string resourceID,string limit_time,int Resource_Type)
        {
            LogHelper.Trace("---------------WSsendExciseTeacher(老师发送客观题)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "sendExciseTeacher";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Account_Name", teacher_count));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            //jObj.Add(new JProperty("Course_ID", "8"));
            jObj.Add(new JProperty("Resource_ID", resourceID));
            jObj.Add(new JProperty("Resource_Type", Resource_Type));
            jObj.Add(new JProperty("Show_Time", limit_time));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return -1;
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return 0;
                }
                else if (status == "2001")
                {
                    return -1;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        int resStr = -1;
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSsendExciseTeacher(2, teacher_count, resourceID, limit_time, Resource_Type);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSsendExciseTeacher(2, teacher_count, resourceID, limit_time, Resource_Type);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else 
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return -1;
            }
            finally
            {
                ;
            }
            return -1;
        }
        /// <summary>
        /// 获取客观题学生回答情况信息，表格
        /// </summary>
        /// <param name="times"></param>
        /// <param name="recourceID"></param>
        /// <returns></returns>
        public async Task<string> WSgetExerciseAnswer(int times, string recourceID, int pageNum, int perNum)
        {
            LogHelper.Trace("---------------WSgetExerciseAnswer(获取客观题学生回答情况信息，表格)----------------------");
            string url = GlobalValue.preclass_server;
            string method = "getExerciseAnswer";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Resource_ID", recourceID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("pageNumber", pageNum));
            jObj.Add(new JProperty("pageSize", perNum));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetExerciseAnswer(2, recourceID, pageNum, perNum);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return  await WSgetExerciseAnswer(2, recourceID, pageNum, perNum);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else 
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取选择题题目内容详情
        /// </summary>
        /// <param name="times"></param>
        /// <param name="recourceID"></param>
        /// <param name="answerStatus"></param>
        /// <returns></returns>
        public async Task<string> WSgetTestDetails(int times, string recourceID, 
            int answerStatus,string Account_Name)
        {
            LogHelper.Trace("---------------WSgetTestDetails(获取选择题题目内容详情)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getTestDetails";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Account_Name", Account_Name));
            jObj.Add(new JProperty("Resource_ID", recourceID));
            jObj.Add(new JProperty("statusId", answerStatus));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetTestDetails(2, recourceID, answerStatus, Account_Name);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetTestDetails(2, recourceID, answerStatus, Account_Name);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else 
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取单个问题列表
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetTestSigleList(int times)
        {
            LogHelper.Trace("---------------WSgetTestSigleList(获取单个问题列表)------------------------");
            string url = GlobalValue.preclass_server;
            string method = "getTestSigleList";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                //LogHelper.Trace("resStr:{0}", str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200")||(status == "2001"))
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetTestSigleList(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetTestSigleList(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取课堂测试列表
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetTestListBycourseId(int times)
        {
            LogHelper.Trace("---------------WSgetTestListBycourseId(获取课堂测试列表)------------------------");
            string url = GlobalValue.preclass_server;
            string couseID = GlobalValue.cource_tTcId;
            string method = "getTestListBycourseId";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            //jObj.Add(new JProperty("Course_ID", "8"));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetTestListBycourseId(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetTestListBycourseId(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取提示屏测试分析
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetExerciseAnalysisCountVice(int times, string classID)
        {
            LogHelper.Trace("---------------WSgetExerciseAnalysisCountVice(获取提示屏测试分析)------------------------");
            string url = GlobalValue.preclass_server;
            string couseID = GlobalValue.cource_tTcId;
            string method = "getExerciseAnalysisCountVice";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Class_IDS", classID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetExerciseAnalysisCountVice(2, classID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetExerciseAnalysisCountVice(2, classID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取提示屏单次分析
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetQuestionAnalysisVice(int times, string classID)
        {
            LogHelper.Trace("---------------WSgetQuestionAnalysisVice(获取提示屏单次分析)------------------------");
            string url = GlobalValue.preclass_server;
            string couseID = GlobalValue.cource_tTcId;
            string method = "getQuestionAnalysisVice";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Class_IDS", classID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetQuestionAnalysisVice(2,classID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetQuestionAnalysisVice(2,classID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        public async Task<string> WSgetShowQuestionAnalysisVice(int times,string classID)
        {
            LogHelper.Trace("---------------WSgetShowQuestionAnalysisVice(获取提示屏展示问题分析)------------------------");
            string url = GlobalValue.preclass_server;
            string couseID = GlobalValue.cource_tTcId;
            string method = "getShowQuestionAnalysisVice";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", GlobalValue.teachingPlanId));
            jObj.Add(new JProperty("Time_Table_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("Class_IDS", classID));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServicePreClassServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetShowQuestionAnalysisVice(2, classID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetShowQuestionAnalysisVice(2, classID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        //以下是yuwl的所有接口
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public string WSgetVerificationCode()
        {
            LogHelper.Trace("------------WSgetVerificationCode(获取验证码)-----------------");
            string url = GlobalValue.basic_server;
            string method = "getVerificationCode";
            string phoneNumber = GlobalValue.userPhoneNumber;
            string[] param = new string[] { phoneNumber };
            LogHelper.Trace("url:{0},methed:{1},telephoneNumber:{2}", url, method, param[0]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("【{0}】 return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("WSgetVerificationCode:请求结果为空!");
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    //GlobalValue.userVerCode = jo["data"]["verificationCode"].ToString();
                    return status;
                }
                else if (status == "20003")
                {
                    return status;
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        public string WSapp_phone_login()
        {
            LogHelper.Trace("------------WSapp_phone_login(验证码登录)-----------------");
            string url = GlobalValue.basic_server;
            string method = "app_phone_login_PC";
            string phoneNumber = GlobalValue.userPhoneNumber;
            string verificationCode = GlobalValue.userVerCode;
            string[] param = new string[] { phoneNumber, verificationCode };
            LogHelper.Trace("url:{0},methed:{1},telephoneNumber:{2},verificationCode:{3}",
                url, method, param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("【{0}】 return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("WSgetVerificationCode:请求结果为空!");
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    GlobalValue.UserCount = jo["accountName"].ToString();
                    GlobalValue.access_tocken = jo["access_token"].ToString();
                    GlobalValue.refresh_tocken = jo["refresh_token"].ToString();
                    GlobalValue.state = jo["state"].ToString();
                    GlobalValue.UserID = jo["user_id"].ToString();
                    return status;
                }
                else
                {
                    return status;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 点击退出课堂，告诉服务器退出课堂
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSteacherQuitCourse(int times)
        {
            LogHelper.Trace("---------------WSteacherQuitCourse(点击退出课堂，告诉服务器)--------");
            string url = GlobalValue.basic_server;
            
            string teacherCount = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;
            string courseId = GlobalValue.curriculumId;

            string method = "teacherQuitCourse";
            string[] param = new string[] { teacherCount,timeTableId,courseId,NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::teacherCount:{0},timeTableId:{1},token:{2}", param[0], param[1], param[2]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSteacherQuitCourse(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSteacherQuitCourse(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 点击开始上课，告诉服务器开始上课
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSaddTeacherStartClass(int times)
        {
            LogHelper.Trace("---------------WSaddTeacherStartClass(点击开始上课，告诉服务器开始上课)--------");
            string url = GlobalValue.basic_server;
            string timeTableId = GlobalValue.timeTableId;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "addTeacherStartClass";
            string[] param = new string[] { timeTableId,GlobalValue.teachingPlanId, GlobalValue.ConferenceID, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("timeTableId:{0},teachingPlanId:{1},ConferenceID:{2},token:{3}", param[0], param[1], param[2], param[3]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSaddTeacherStartClass(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSaddTeacherStartClass(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取校本资源库筛选框
        /// </summary>
        /// <param name="times"></param>
        /// <param name="StuID"></param>
        /// <returns></returns>
        public async Task<string> WSgetlistScreenCondition(int times)
        {
            LogHelper.Trace("---------------WSgetlistScreenCondition(获取校本资源库筛选框)--------");
            string url = GlobalValue.basic_server;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getlistScreenCondition";
            string[] param = new string[] { NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::token:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200")|| (status == "2001"))
                {
                    return str_res;

                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetlistScreenCondition(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetlistScreenCondition(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取学生的的考勤信息详情
        /// </summary>
        /// <param name="times"></param>
        /// <param name="classid"></param>
        /// <returns></returns>
        public async Task<string> WSgetStudentCheckWorksInfo(int times, string StuID)
        {
            LogHelper.Trace("---------------WSgetPolygonalChartCheckWorkStatistics(获取学生的的考勤信息详情)--------");
            string url = GlobalValue.basic_server;
            string timeTableId = GlobalValue.timeTableId;
            string StudentID = StuID;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getStudentCheckWorksInfo";
            string[] param = new string[] { timeTableId, StudentID, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::timeTableId:{0},StudentID:{1},token:{2}", param[0], param[1], param[2]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetStudentCheckWorksInfo(2, StuID);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetStudentCheckWorksInfo(2, StuID);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取班级的考勤信息折线图
        /// </summary>
        /// <param name="times"></param>
        /// <param name="classid"></param>
        /// <returns></returns>
        public async Task<string> WSgetPolygonalChartCheckWorkStatistics(int times,string classid)
        {
            LogHelper.Trace("---------------WSgetPolygonalChartCheckWorkStatistics(获取班级的考勤信息折线图)--------");
            string url = GlobalValue.basic_server;
            string timeTableId = GlobalValue.timeTableId;
            string classID = classid;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getPolygonalChartCheckWorkStatistics";
            string[] param = new string[] { timeTableId, classID, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::timeTableId:{0},classID:{1},token:{2}", param[0], param[1], param[2]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetPolygonalChartCheckWorkStatistics(2, classid);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetPolygonalChartCheckWorkStatistics(2, classid);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取单个班级的考勤信息柱状图和表格
        /// </summary>
        /// <param name="times"></param>
        /// <param name="classuuid"></param>
        /// <returns></returns>
        public async Task<string> WSgetClassCheckWorkStatistics(int times,string classuuid)
        {
            LogHelper.Trace("---------------WSgetClassCheckWorkStatistics(获取单个班级的考勤信息柱状图和表格)--------");
            string url = GlobalValue.basic_server;
            string timeTableId = GlobalValue.timeTableId;
            string classUuid = classuuid;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getClassCheckWorkStatistics";
            string[] param = new string[] { timeTableId, classuuid, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::timeTableId:{0},classuuid:{1},token:{2}", param[0], param[1], param[2]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetCheckWorkStatistics(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetCheckWorkStatistics(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 主讲老师获取全部班级的考勤信息柱状图和表格
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetCheckWorkStatistics(int times)
        {
            LogHelper.Trace("---------------WSgetCheckWorkStatistics(主讲老师获取全部班级的考勤信息柱状图和表格)--------");
            string url = GlobalValue.basic_server;
            string timeTableId = GlobalValue.timeTableId;
            string T_T_T_BH = GlobalValue.T_T_T_BH;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getCheckWorkStatistics";
            string[] param = new string[] { timeTableId, T_T_T_BH, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::timeTableId:{0},T_T_T_BH:{1},token:{2}", param[0], param[1], param[2]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if ((status == "200") || (status == "2001"))
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetCheckWorkStatistics(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetCheckWorkStatistics(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }

        /// <summary>
        /// 20.助教-获取教室所有班级考勤基本信息
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetTeachingAssistantCheckWorkDatas(int times)
        {
            LogHelper.Trace("---------------WSgetTeachingAssistantCheckWorkDatas(20.助教-获取教室所有班级考勤基本信息)----------------");
            string url = GlobalValue.basic_server;
            //string UserName = GlobalValue.UserName;
            string timeTableId = GlobalValue.timeTableId;
            string class_ids = GlobalValue.T_T_T_BH;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getTeachingAssistantCheckWorkDatas";
            string[] param = new string[] { timeTableId, class_ids, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace($"param:::timeTableId{timeTableId},class_ids:{class_ids},Access_tocken:{NowAccess_tocken}");

            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetTeachingAssistantCheckWorkDatas(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetTeachingAssistantCheckWorkDatas(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }

        /// <summary>
        /// 老师点击【课堂考勤】获取考勤状态，考勤班级
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetCourseCheckWorkBasicInfo(int times)
        {
            LogHelper.Trace("---------------WSgetCourseCheckWorkBasicInfo(考勤状态，考勤班级)----------------");
            string url = GlobalValue.basic_server;
            string UserName = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getCourseCheckWorkBasicInfo";
            string[] param = new string[] { UserName, timeTableId, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::UserName:{0},timeTableId:{1},token:{2}", param[0], param[1], param[2]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetCourseCheckWorkBasicInfo(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetCourseCheckWorkBasicInfo(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 老师点击开始考勤按钮
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSaddCourseCheckWorkTime(int times)
        {
            LogHelper.Trace("-------------------WSaddCourseCheckWorkTime(点击开始考勤按钮)----------------------");
            string url = GlobalValue.basic_server;
            string UserName = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "addCourseCheckWorkTime";
            string[] param = new string[] { UserName, timeTableId, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("UserName:{0},timeTableId:{1},token:{2}", param[0], param[1], param[2]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSaddCourseCheckWorkTime(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSaddCourseCheckWorkTime(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 暂停使用
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSaddTeacherCourseOperation(int times)
        {
            LogHelper.Trace("-------------------WSaddTeacherCourseOperation(告诉服务器所选课程）----------------------");
            string url = GlobalValue.basic_server;
            string timeTableId = GlobalValue.timeTableId;
            string teachingPlanId = GlobalValue.teachingPlanId;
            string ConferenceID = GlobalValue.ConferenceID;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "addTeacherCourseOperation";
            string[] param = new string[] { timeTableId, teachingPlanId, ConferenceID, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("timeTableId:{0},teachingPlanId:{1},ConferenceID:{2},token:{3}", param[0], param[1], param[2], param[3]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetTeacherCourseLastestInfo(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetTeacherCourseLastestInfo(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }

        /// <summary>
        /// 判断用户角色专用辅助函数 1
        /// </summary>
        /// <returns></returns>
        public string GetTeacherCourseLastestInfoRawStr()
        {
            string url = GlobalValue.basic_server;
            string UserName = GlobalValue.UserCount;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getTeacherCourseLastestInfo";
            string[] param = new string[] { UserName, NowAccess_tocken };
            LogHelper.Trace($"---------------GetTeacherCourseLastestInfoRawStr(判断用户角色专用辅助函数 1)---------------");
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("UserName:{0}", param[0]);
            LogHelper.Trace("token:{0}", param[1]);

            return (string)doInvokeWebService(url, method, param);
        }
        /// <summary>
        /// 判断用户角色专用辅助函数 2
        /// </summary>
        /// <returns></returns>
        public string GetTeacherNowCourseRawStr()
        {
            string url = GlobalValue.basic_server;
            string UserName = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string tocken = GlobalValue.access_tocken;
            string method = "getTeacherNowCourse";
            string[] param = new string[] { UserName, tocken };
            LogHelper.Trace($"---------------GetTeacherNowCourseRawStr(判断用户角色专用辅助函数 2)---------------");
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("UserName:{0}", param[0]);
            LogHelper.Trace("token:{0}", param[1]);

            return (string)doInvokeWebService(url, method, param);
        }

        /// <summary>
        /// 获取授课计划
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetTeacherCourseLastestInfo(int times)
        {
            LogHelper.Trace("-------------------WSgetTeacherCourseLastestInfo(获取授课计划)----------------------");
            string url = GlobalValue.basic_server;
            string UserName = GlobalValue.UserCount;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getTeacherCourseLastestInfo";
            string[] param = new string[] { UserName, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("UserName:{0},token:{1}", param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return status;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        GlobalValue.RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == GlobalValue.refresh_tocken) && (NowAccess_tocken == GlobalValue.access_tocken))
                        {
                            Authfunction af = new Authfunction();
                            await af.get_tokenAsync("refresh_token");
                        }
                        resStr = await WSgetTeacherCourseLastestInfo(2);
                        GlobalValue.RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetTeacherCourseLastestInfo(2);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 添加教师登录教室号
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public string WSaddClassRoomId(int times)
        {
            LogHelper.Trace("-------------------WSaddClassRoomId(添加教师登录教室号)----------------------");
            string url = GlobalValue.basic_server;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            var jobject = new JObject();
            jobject["classRoomID"] = GlobalValue.SignInLocationDescription;
            jobject["sortNumber"] = GlobalValue.SignInLocationNo;
            jobject["Role"] = GlobalValue.AccountType.ToString();
            jobject["Position"] = GlobalValue.SignInLocationPosition;
            var s = jobject.ToString();

            string method = "addClassRoomId";
            string[] param = new string[] { GlobalValue.UserID, s, GlobalValue.timeTableId, NowAccess_tocken };         
            LogHelper.Trace("url:{0},json:{0},token:{1}", url, param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                return str_res;
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }
        /// <summary>
        /// 获取老师登录教室号
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public string WSgetTeacherClassRoomId(string userCode)
        {
            LogHelper.Trace("-------------------getTeacherClassRoomId(获取老师登录教室号)----------------------");
            string url = GlobalValue.basic_server;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getTeacherClassRoomId";
            string[] param = new string[] { userCode, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("json:{0},token:{1}", param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                return str_res;
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }
        /// <summary>
        /// 获取老师登录FreeSwitchId
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public string WSgetFreeSwitchIdByClassNum(string classRoomDesc)
        {
            LogHelper.Trace("-------------------getFreeSwitchIdByClassNum(获取老师登录FreeSwitchId)----------------------");
            string url = GlobalValue.basic_server;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getFreeSwitchIdByClassNum";
            string[] param = new string[] { classRoomDesc, NowAccess_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("json:{0},token:{1}", param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                return str_res;
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }
        /// <summary>
        /// 获取各个服务器的地址
        /// </summary>
        /// <returns></returns>
        public string WSgetServerURLByType()
        {
            LogHelper.Trace("---------------WSgetServerURLByType(获取服务器地址)----------------------");
            string url = GlobalValue.basic_server;
            string method = "getServerURLByType";
            string UserConfidenceID = GlobalValue.UserID;
            string UserCount = GlobalValue.UserCount;
            string[] param = new string[] { UserCount,UserConfidenceID};
            LogHelper.Trace("url:{0},methed:{1},UserCount:{2},UserConfidenceID:{3}", 
                url,  method, param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("WSgetTeacherCourseLastest:请求结果为空!");
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);

                string status = jo["err_code"].ToString();

                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return str_res;
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
  
        /// <summary>
        /// 初始化基础服务器webservice
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        public  void InvokeWebService(string url, string methodname, object[] args)
        {
        //这里的namespace是需引用的webservices的命名空间，我没有改过，也可以使用。也可以加一个参数从外面传进来。
        string @namespace = "client";
        long t0 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

        try
        {
            //获取WSDL
            WebClient wc = new WebClient();
            //Stream stream = wc.OpenRead(url + "?WSDL");
            Stream stream = wc.OpenRead(url);
            ServiceDescription sd = ServiceDescription.Read(stream);
            GlobalValue.classname = sd.Services[0].Name;
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace(@namespace);
            //生成客户端代理类代码
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider csc = new CSharpCodeProvider();
            //ICodeCompiler icc = csc.CreateCompiler();
            //long t1 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

            //设定编译参数
            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;//动态编译后的程序集不生成可执行文件
            cplist.GenerateInMemory = true;//动态编译后的程序集只存在于内存中，不在硬盘的文件上
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");
            //long t2 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

            //编译代理类
            GlobalValue.cr = csc.CompileAssemblyFromDom(cplist, ccu);
            if (true == GlobalValue.cr.Errors.HasErrors)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError ce in GlobalValue.cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }

                throw new Exception(sb.ToString());
            }
            //long t3 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

        }
        catch (Exception ex)
        {
            CodeHelper.xMessageBox($"[InvokeWebService]Exception::{ex.Message}");
            return;
        }
    }
        /// <summary>
        /// 初始化备课服务器webservice
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        public  void InvokeWebServicePreClassServer(string url, string methodname, object[] args)
        {
            //这里的namespace是需引用的webservices的命名空间，我没有改过，也可以使用。也可以加一个参数从外面传进来。
            string @namespace = "client";
            long t0 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

            try
            {
                //获取WSDL
                WebClient wc = new WebClient();
                //Stream stream = wc.OpenRead(url + "?WSDL");
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                GlobalValue.preclass_classname = sd.Services[0].Name;
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                //ICodeCompiler icc = csc.CreateCompiler();
                long t1 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;//动态编译后的程序集不生成可执行文件
                cplist.GenerateInMemory = true;//动态编译后的程序集只存在于内存中，不在硬盘的文件上
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                long t2 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

                //编译代理类
                GlobalValue.preclass_cr = csc.CompileAssemblyFromDom(cplist, ccu);
                //CompilerResults cr = csc.CompileAssemblyFromDom(cplist, ccu);
                if (true == GlobalValue.cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in GlobalValue.cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }

                    throw new Exception(sb.ToString());
                }
                long t3 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

            }
            catch (Exception ex)
            {
                LogHelper.Trace("InvokeWebServicePreClassServer:{0},{1},{2}", ex.Message, ex.Source, ex.StackTrace);
                LogHelper.Trace("初始化备课服务器异常...");
                CodeHelper.xMessageBox("初始化备课服务器异常...");
                return;
            }
        }
        /// <summary>
        /// 初始化资源服务器webservice
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        public void InvokeWebServiceResourceServer(string url, string methodname, object[] args)
        {
            //这里的namespace是需引用的webservices的命名空间，我没有改过，也可以使用。也可以加一个参数从外面传进来。
            string @namespace = "client";
            long t0 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

            try
            {
                //获取WSDL
                WebClient wc = new WebClient();
                //Stream stream = wc.OpenRead(url + "?WSDL");
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                GlobalValue.resource_classname = sd.Services[0].Name;
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                //ICodeCompiler icc = csc.CreateCompiler();
                long t1 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;//动态编译后的程序集不生成可执行文件
                cplist.GenerateInMemory = true;//动态编译后的程序集只存在于内存中，不在硬盘的文件上
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                long t2 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

                //编译代理类
                GlobalValue.resource_cr = csc.CompileAssemblyFromDom(cplist, ccu);
                //CompilerResults cr = csc.CompileAssemblyFromDom(cplist, ccu);
                if (true == GlobalValue.cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in GlobalValue.cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }

                    throw new Exception(sb.ToString());
                }
                long t3 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

            }
            catch(Exception ex)
            {
                LogHelper.Trace("InvokeWebServiceResourceServer:{0},{1},{2}", ex.Message, ex.Source, ex.StackTrace);
                LogHelper.Trace("初始化资源服务器异常...");
                CodeHelper.xMessageBox("初始化资源服务器异常...");
                return;
            }
        }
        /// <summary>
        /// 访问考勤服务器的句柄
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object doInvokeWebService(string url, string methodname, object[] args)
        {
            try
            {
                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = GlobalValue.cr.CompiledAssembly;
                Type t = assembly.GetType("client" + "." + GlobalValue.classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                if (mi == null)
                {
                    LogHelper.Trace($"[doInvokeWebService]方法名::{methodname} 不存在。");
                    return string.Empty;
                }
                //注：method.Invoke(o, null)返回的是一个Object,如果你服务端返回的是DataSet,这里也是用(DataSet)method.Invoke(o, null)转一下就行了,method.Invoke(0,null)这里的null可以传调用方法需要的参数,string[]形式的
                return (string)mi.Invoke(obj, args);
            }
            catch (System.Reflection.TargetInvocationException exSvr)
            {
                LogHelper.Trace("doInvokeWebService::Message={0} methodname={1}", exSvr.Message, methodname);
                if (args != null)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        LogHelper.Trace($"arg[{i}]={args[i]}");
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                LogHelper.Trace("doInvokeWebService:{0},{1},{2}", ex.Message,ex.Source,ex.StackTrace);
                LogHelper.Trace("初始化基础服务器异常...");
                CodeHelper.xMessageBox("初始化基础服务器异常...");
                return "";
            }
        }
        /// <summary>
        /// 访问备课服务器的句柄
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object doInvokeWebServicePreClassServer(string url, string methodname, object[] args)
        {
            try
            {
                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = GlobalValue.preclass_cr.CompiledAssembly;
                Type t = assembly.GetType("client" + "." + GlobalValue.preclass_classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                //注：method.Invoke(o, null)返回的是一个Object,如果你服务端返回的是DataSet,这里也是用(DataSet)method.Invoke(o, null)转一下就行了,method.Invoke(0,null)这里的null可以传调用方法需要的参数,string[]形式的
                return (string)mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                LogHelper.Trace("doInvokeWebServicePreClassServer:{0},{1},{2}", ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }
        /// <summary>
        /// 访问资源服务器句柄
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object doInvokeWebServiceResourceServer(string url, string methodname, object[] args)
        {
            try
            {
                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = GlobalValue.resource_cr.CompiledAssembly;
                Type t = assembly.GetType("client" + "." + GlobalValue.resource_classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                //注：method.Invoke(o, null)返回的是一个Object,如果你服务端返回的是DataSet,这里也是用(DataSet)method.Invoke(o, null)转一下就行了,method.Invoke(0,null)这里的null可以传调用方法需要的参数,string[]形式的
                return (string)mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                LogHelper.Trace("doInvokeWebServiceResourceServer:{0},{1},{2}", ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="times"></param>
        /// <param name="arg_obj"></param>
        /// <returns></returns>
        public async Task<string> WSuploadResourceMore(int times, string arg_obj)
        {
            LogHelper.Trace("---------------WSuploadResourceMore(上传文件)------------------------");
            string url = GlobalValue.resource_server;
            string method = "uplodaResourceMore";
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            JObject obj = JObject.Parse(arg_obj);
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            //jObj = arg_obj;
            jObj.Add(new JProperty("Account_Name", GlobalValue.UserCount));
            jObj.Add(new JProperty("Course_ID", GlobalValue.timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            string[] param = new string[] { jObj.ToString() };

            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::{0}", param[0]);

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        Authfunction af = new Authfunction();
                        int res = await af.get_tokenAsync("refresh_token");
                        if (res == 0)
                        {
                            string resstr2 = await WSuploadResourceMore(2, arg_obj);
                            return resstr2;
                        }
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSuploadResourceMore(2, arg_obj);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 添加电子白板图像到服务器
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSaddWhiteBoard(int times, string json)
        {
            LogHelper.Trace("---------------WSaddWhiteBoard(添加电子白板图像到服务器)----------------");
            string url = GlobalValue.preclass_server;
            string UserName = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "addWhiteBoard";
            string[] param = new string[] { json };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::Json:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                LogHelper.Trace("err_code:{0}", status);
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        Authfunction af = new Authfunction();
                        int res = await af.get_tokenAsync("refresh_token");
                        if (res == 0)
                        {
                            return await WSaddWhiteBoard(2,json);
                        }
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSaddWhiteBoard(2, json);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 删除电子白板
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSdelResourceForPc(int times, string resourceIds)
        {
            LogHelper.Trace("---------------WSdelResourceForPc(删除电子白板)----------------");
            string url = GlobalValue.resource_server;
            string UserName = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "delResourceForPc";
            var jox = new JObject();
            jox["Resource_IDs"] = resourceIds;
            jox["accessToken"] = NowAccess_tocken;
            var param = JsonConvert.SerializeObject(jox);
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::Json:{0}", param);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, new[] { param });
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                LogHelper.Trace("err_code:{0}", status);
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        Authfunction af = new Authfunction();
                        int res = await af.get_tokenAsync("refresh_token");
                        if (res == 0)
                        {
                            return await WSdelResourceForPc(2, resourceIds);
                        }
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSdelResourceForPc(2, resourceIds);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 获取电子白板列表
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetWhiteBoardList(int times, string json)
        {
            LogHelper.Trace("---------------WSgetWhiteBoardList(电子白板列表)----------------");
            string url = GlobalValue.resource_server;
            string UserName = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string NowAccess_tocken = GlobalValue.access_tocken;
            string NowRefreshToken = GlobalValue.refresh_tocken;

            string method = "getWhritBoardList";
            string[] param = new string[] { json };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("param:::Json:{0}", param[0]);
            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                LogHelper.Trace("err_code:{0}", status);
                #region 处理返回结果
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        Authfunction af = new Authfunction();
                        int res = await af.get_tokenAsync("refresh_token");
                        if (res == 0)
                        {
                            return await WSgetWhiteBoardList(2, json);
                        }
                    }
                    else if (times == 2)
                    {
                        LogHelper.Trace("需要重新登录！");
                        return "";
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetWhiteBoardList(2, json);
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
        /// <summary>
        /// 助教/协同教室登录客户端
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetTeacherNowCourse(int times)
        {
            LogHelper.Trace("---------------WSgetTeacherNowCourse(助教/协同教室登录客户端)----------------");
            string url = GlobalValue.basic_server;
            string UserName = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string tocken = GlobalValue.access_tocken;

            string method = "getTeacherNowCourse";
            string[] param = new string[] { UserName, tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("UserName:{0},token:{1}", param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                LogHelper.Trace("err_code:{0}", status);
                #region 处理返回结果
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        Authfunction af = new Authfunction();
                        int res = await af.get_tokenAsync("refresh_token");
                        if (res == 0)
                        {
                            return await WSgetTeacherNowCourse(2);
                        }
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }

        /// <summary>
        /// 教辅账号登录获取上课信息
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetCourseInfoByAuxiliary(int times)
        {
            LogHelper.Trace("---------------WSgetCourseInfoByAuxiliary(教辅账号登录获取上课信息)----------------");
            string url = GlobalValue.basic_server; // "http://192.168.20.1:8080/ws/IEasySecServer?wsdl";
            string userName = GlobalValue.UserCount;
            string timeTableId = GlobalValue.timeTableId;
            string tocken = GlobalValue.access_tocken;

            string method = "getCourseInfoByAuxiliary";
            string[] param = new string[] { userName, tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("UserId:{0},token:{1}", param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                LogHelper.Trace("err_code:{0}", status);
                #region 处理返回结果
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        Authfunction af = new Authfunction();
                        int res = await af.get_tokenAsync("refresh_token");
                        if (res == 0)
                        {
                            return await WSgetTeacherNowCourse(2);
                        }
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }

        /// <summary>
        /// 添加主讲与助教通信数据
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public string WSaddSpeakerCommunicationAssistant(string key, string value)
        {
            LogHelper.Trace("-------------------WSaddSpeakerCommunicationAssistant(添加主讲与助教通信数据)----------------------");
            string url = GlobalValue.basic_server;

            var jobject = new JObject();
            jobject["timeTableId"] = GlobalValue.timeTableId;
            jobject["key"] = key;
            jobject["datas"] = value;
            jobject["accessToken"] = GlobalValue.access_tocken;

            string method = "addSpeakerCommunicationAssistant";
            string[] param = new string[] { GlobalValue.timeTableId, key, value, GlobalValue.access_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace($"timeTableId:{GlobalValue.timeTableId},key:{key},value:{value},access_tocken:{GlobalValue.access_tocken}");
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                return str_res;
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }
        /// <summary>
        /// 获取老师登录教室号
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public string WSgetSpeakerCommunicationAssistant(string key)
        {
            LogHelper.Trace("-------------------getSpeakerCommunicationAssistant(获取主讲与助教通信数据)----------------------");
            string url = GlobalValue.basic_server;

            string method = "getSpeakerCommunicationAssistant";
            string[] param = new string[] { key, GlobalValue.access_tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace($"key:{key},accessToken:{GlobalValue.access_tocken}");
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                return str_res;
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }

        public string WSgetLogo()
        {
            LogHelper.Trace("-------------------WSgetLogo(获取Logo)----------------------");
            string url = GlobalValue.basic_server;

            string method = "getLogo";
            LogHelper.Trace("url:{0}", url);

            try
            {
                string str_res = (string)doInvokeWebService(url, method, null);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                return str_res;
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }

        /// <summary>
        /// 获取协同教师
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<string> WSgetCollaborationTeachers(int times)
        {
            LogHelper.Trace("---------------WSgetCollaborationTeachers(获取协同教师)----------------");
            string url = GlobalValue.basic_server;
            string tocken = GlobalValue.access_tocken;

            string method = "getCollaborationTeachers";
            string[] param = new string[] { GlobalValue.timeTableId, tocken };
            LogHelper.Trace("url:{0}", url);
            LogHelper.Trace("method:{0}", method);
            LogHelper.Trace("TimeTableId:{0},token:{1}", param[0], param[1]);
            try
            {
                string str_res = (string)doInvokeWebService(url, method, param);
                LogHelper.Trace("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    LogHelper.Trace("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                LogHelper.Trace("err_code:{0}", status);
                #region 处理返回结果
                if (status == "200")
                {
                    return str_res;

                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        Authfunction af = new Authfunction();
                        int res = await af.get_tokenAsync("refresh_token");
                        if (res == 0)
                        {
                            return await WSgetCollaborationTeachers(2);
                        }
                    }
                    else if (times == 2)
                    {
                        CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else if (status == "20102")
                {
                    CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }
    }
}

