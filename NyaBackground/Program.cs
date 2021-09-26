using System;
using System.Threading.Tasks;

namespace NyaBackgroundTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome!");
            await NekosDotLife.NekoImage();
            GC.Collect();
            bool repeat = true;
            while (repeat)
            {
                Console.Write("Want a new background? (yes/no): ");
                if (Console.ReadLine().ToLower() == "yes")
                {
                    await NekosDotLife.NekoImage();
                    GC.Collect();
                }
                else repeat = false; GC.Collect();

            }
        }
    }
}
