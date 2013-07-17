using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.App;
namespace TestBipMessageConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Get Bip Data:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            try
            {
                var bip = new GetBipData();
                bip.getData();
                if (bip.Error)
                    Console.WriteLine(bip.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("PlocyFlow Get BipData:" + ex.Message);
            }
        }
    }
}
