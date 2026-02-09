using DemoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PosSettings posSettings = new PosSettings();
                posSettings.IntializeStoreSettings();
                foreach (POSSetting current in posSettings.PosDetails)//testing by PK
                {
                    try
                    {
                        if (current.StoreSettings.StoreId == 10983)
                        {
                            Demo demo = new Demo(current.StoreSettings.StoreId, current.StoreSettings.POSSettings.tax, current.StoreSettings.POSSettings.BaseUrl, current.StoreSettings.POSSettings.AuthKey, current.StoreSettings.POSSettings.Token);
                            Console.WriteLine();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
