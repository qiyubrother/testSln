using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StudentSubmitQuestionA
{
    public class AnswerHelper
    {
        static Random r = new Random((int)DateTime.Now.ToFileTimeUtc());
        public static List<string> GetAnswerList(string workpath)
        {
            var lst = new List<string>();
            var files = System.IO.Directory.GetFiles(workpath, "*.txt");
            foreach(var f in files)
            {
                lst.Add(File.ReadAllText(f));
            }

            return lst;
        }

        public static string RandomOneOfAnswer(List<string> answerList) => answerList[r.Next(0, answerList.Count - 1)];

    }
}
