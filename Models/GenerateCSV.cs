using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.Models
{
    class GenerateCSV
    {
        public static string GenerateCSVFile<T>(IList<T> list, string name, int storeId, string baseUrl)
        {
            if (list == null || list.Count == 0) return "";
            if (!Directory.Exists(baseUrl + "\\" + storeId + "\\Upload\\"))
            {
                Directory.CreateDirectory(baseUrl + "\\" + storeId + "\\Upload\\");
            }
            string fileName = name + storeId + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";
            string fcname = baseUrl + "\\" + storeId + "\\Upload\\" + fileName;

            Type t = list[0].GetType();
            string newLine = Environment.NewLine;

            using (var sw = new StreamWriter(fcname))
            {
                object o = Activator.CreateInstance(t);
                PropertyInfo[] props = o.GetType().GetProperties();

                foreach (PropertyInfo pi in props)
                {
                    sw.Write(pi.Name + ",");
                }
                sw.Write(newLine);
                foreach (T item in list)
                {
                    //this acts as datacolumn
                    foreach (PropertyInfo pi in props)
                    {
                        string whatToWrite =
                        Convert.ToString(item.GetType()
                                             .GetProperty(pi.Name)
                                             .GetValue(item, null))
                            .Replace(',', ' ') + ',';

                        sw.Write(whatToWrite);
                    }
                    sw.Write(newLine);
                }

                return fileName;
            }


        }
    }
}
