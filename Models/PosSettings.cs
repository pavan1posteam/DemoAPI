using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace DemoAPI.Models
{
    class PosSettings
    {
        public List<POSSetting> PosDetails { get; set; }


        public void IntializeStoreSettings()
        {
            DataSet dsResult = new DataSet();
            List<POSSetting> posdetails = new List<POSSetting>();
            try
            {
                string st = ConfigurationManager.AppSettings["LiquorAppsConnectionString"];
                List<SqlParameter> sparams = new List<SqlParameter>();
                // sparams.Add(new SqlParameter("@PosId",24069 ));      // add locol pos id if needed 
                using (SqlConnection con = new SqlConnection(st))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        //cmd.Parameters.Add(sparams[0]);
                        cmd.CommandText = "usp_ts_GetStorePosSetting";
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dsResult);
                        }
                    }
                }
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    foreach (DataRow dr in dsResult.Tables[0].Rows)
                    {
                        POSSetting pobj = new POSSetting();
                        pobj.Setting = dr["Settings"].ToString();
                        StoreSetting obj = new StoreSetting();
                        obj.StoreId = Convert.ToInt32(dr["StoreId"] == DBNull.Value ? 0 : dr["StoreId"]);
                        obj.POSSettings = JsonConvert.DeserializeObject<Setting>(pobj.Setting);
                        pobj.PosName = dr["PosName"].ToString();
                        pobj.PosId = Convert.ToInt32(dr["PosId"]);
                        pobj.StoreSettings = obj;
                        if (pobj.StoreSettings.POSSettings != null)
                        {
                            pobj.StoreSettings.POSSettings.categoriess = obj.POSSettings.categoriess;
                            pobj.StoreSettings.POSSettings.Upc = obj.POSSettings.Upc;
                        }
                        posdetails.Add(pobj);
                    }
                }
                PosDetails = posdetails;

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }

        public class POSSetting
        {
            public int PosId { get; set; }
            public string PosName { get; set; }

            public StoreSetting StoreSettings { get; set; }
            public string Setting { get; set; }


        }

        public class StoreSetting
        {
            public int StoreId { get; set; }
            public Setting POSSettings { get; set; }
        }
        public class Setting
        {
            public string AuthKey { get; set; }
            public decimal tax { get; set; }
            public string Token { get; set; }
            public string BaseUrl { get; set; }
            public List<categories> categoriess { set; get; }
            public List<UPC> Upc { get; set; }

        }
        public class categories
        {
            public string id { get; set; }
            public string name { get; set; }
            public decimal taxrate { get; set; }
            public Boolean selected { get; set; }
        }

        public class UPC
        {
            public string upccode { get; set; }
        }
    
}