using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ClearSqlComment
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ClearSqlComment.exe <SqlDirectoryName>");
                return;
            }
            var d = args[0];
            var files = Directory.GetFiles(d, "*.sql");
            foreach(var file in files)
            {
                var lst = new List<string>();
                bool isFirstLine = false;
                foreach (var line in File.ReadAllLines(file))
                {
                    if (!isFirstLine)
                    {
                        if (line.StartsWith("DROP"))
                        {
                            isFirstLine = true;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (!line.TrimStart(' ').StartsWith("-- ") 
                        && !line.StartsWith("SET @MYSQLDUMP_TEMP_LOG_BIN")
                        && !line.StartsWith("SET @@SESSION.SQL_LOG_BIN")
                        && !line.StartsWith("SET @@GLOBAL.GTID_PURGED")
                        && line != "--"
                        )
                    {
                        lst.Add(line.ToUpper().Replace(" ROW_FORMAT=DYNAMIC", ""));
                    }
                }
                File.WriteAllLines(file, lst);
                lst.Clear();
            }
        }
    }
}
