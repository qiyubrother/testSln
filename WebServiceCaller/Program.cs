using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using qiyubrother;

namespace WebServiceCaller
{
    class Program
    {
        static void Main(string[] args)
        {
            //{
            //    var ht = new System.Collections.Hashtable();
            //    ht.Add("a", "100");
            //    ht.Add("b", "200");
            //    var rtn = SOAPWebServiceHelper.QuerySoapWebService("http://localhost:8077/WebServiceDemo.asmx?wsdl", "FmtToJson", ht);
            //    var v = rtn.InnerText;
            //}
            {
                var ht = new System.Collections.Hashtable();
                ht.Add("a", "e");
                try
                {
                    var rtn = SOAPWebServiceHelper.QuerySoapWebService("http://localhost:8077/WebServiceDemo.asmx?wsdl", "ExceptionFun1", ht, 1000);
                    var v = rtn.InnerText;
                }
                catch (WebException wex)
                {
                    // 请求的超时期限到期。 - 或 - 处理请求时出错。
                }
                catch (Exception ex)
                {

                }

            }
        }
    }
}
