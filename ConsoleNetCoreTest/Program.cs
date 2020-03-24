using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qiyubrother;
namespace ConsoleNetCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var counter = 0;
            Parallel.For(0, 10000, new Action<int>((i) => {
                Parallel.Invoke(new Action[]
                {
                    ()=>{
                        Console.WriteLine($"Counter:{counter++}");
                        TokenHelper.HttpPost($"http://localhost:5000/api/Values", string.Empty);
                    },
                    ()=>{
                        Console.WriteLine($"Counter:{counter++}");
                        TokenHelper.HttpPost($"http://localhost:5000/api/Values", string.Empty);
                    },
                    ()=>{
                        Console.WriteLine($"Counter:{counter++}");
                        TokenHelper.HttpPost($"http://localhost:5000/api/Values", string.Empty);
                    },
                    ()=>{
                        Console.WriteLine($"Counter:{counter++}");
                        TokenHelper.HttpPost($"http://localhost:5000/api/Values", string.Empty);
                    },
                    ()=>{
                        Console.WriteLine($"Counter:{counter++}");
                        TokenHelper.HttpPost($"http://localhost:5000/api/Values", string.Empty);
                    },

                }); ;
            }));
        }
    }
}
