using DemoAPI.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI
{
    class Demo
    {
        public Demo(int storeid, decimal tax, string baseurl, string authkey, string token)
        {
            Console.WriteLine("Generating Product File For Store: " + storeid);
            ResposeToCSV(storeid, tax, baseurl, authkey, token);
        }

        public List<JArray> GetResponse(string baseurl, string authkey, string token)
        {
            List<JArray> itemList = new List<JArray>();
            try
            {
                var client1 = new RestClient(baseurl + "/Item/?page=1&Size=500");
                var request1 = new RestRequest("", Method.Get);

                request1.AddHeader("AuthKey", authkey);
                request1.AddHeader("Token", token);
                var response = client1.Execute(request1);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseContent = response.Content;
                    var pJson = JObject.Parse(responseContent);

                    var totalCountsToken = pJson["data"]?["totalCounts"];
                    int totalCounts = totalCountsToken != null ? (int)totalCountsToken : 0;
                    int itemsPerPage = 500;
                    int totalPages = (int)Math.Ceiling((double)totalCounts / itemsPerPage);

                    for (int page = 1; page <= totalPages; page++)
                    {
                        var client = new RestClient(baseurl + "/Item/?page=" + page + "&Size=" + itemsPerPage);
                        var request = new RestRequest("", Method.Get);

                        request.AddHeader("AuthKey", authkey);
                        request.AddHeader("Token", token);
                        var pageResponse = client.Execute(request);

                        if (pageResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string pageResponseContent = pageResponse.Content;


                            var pageJson = JObject.Parse(pageResponseContent);
                            var dataToken = pageJson["data"];
                            var data = dataToken["data"];

                            if (data is JArray jArray)
                            {
                                itemList.Add(jArray);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
                Console.ReadLine();
            }
            return itemList;
        }

        public void ResposeToCSV(int storeid, decimal tax, string baseurl, string authkey, string token)
        {
            var productlist = GetResponse(baseurl, authkey, token);
            string BaseUrl = ConfigurationManager.AppSettings.Get("BaseDirectory");
            List<ProductModel> pm = new List<ProductModel>();
            List<FullName> fullname = new List<FullName>();

            ProductModel pdf;
            FullName fnf;
            try
            {
                foreach (var item in productlist)
                {
                    foreach (var itm in item)
                    {
                        pdf = new ProductModel();
                        fnf = new FullName();

                        pdf.StoreID = storeid;
                        pdf.sku = "#" + itm["sku"].ToString();

                        fnf.sku = "#" + itm["sku"].ToString();
                        pdf.upc = "#" + itm["item_Upc"].ToString();
                        fnf.upc = "#" + itm["item_Upc"].ToString();
                        pdf.uom = itm["sizeName"].ToString();
                        fnf.uom = itm["sizeName"].ToString();

                        //var totalTax = 0m;
                        //JArray taxArray = (JArray)itm["tax"];
                        //List<string> t1 = itm["tax"].ToString().ToList();

                        /*  if (taxArray != null && taxArray.Count > 0)
                          {
                              // Loop through all tax items
                              foreach (var t in taxArray)
                              {
                                  totalTax += Convert.ToDecimal(t["persentage"]) / 100m; // sum or process each persentage
                                  Console.WriteLine(totalTax);
                                  pdf.Tax = totalTax;
                              }
                          }
                          else {
                              pdf.Tax = tax;
                          }
                         */

                        var totaltax = JsonConvert.DeserializeObject<>();


                        pdf.StoreProductName = itm["name"].ToString();
                        fnf.pname = itm["name"].ToString();
                        pdf.StoreDescription = itm["name"].ToString();
                        fnf.pdesc = itm["name"].ToString();
                        pdf.Start = "";
                        pdf.sprice = 0;
                        /*  if (StaticQuantity.Contains(storeid.ToString()))
                          {
                            pdf.Qty = 999;
                           }
                         else
                          {
                              pdf.Qty = Convert.ToInt32(itm["storeQty"]) > 0 ? Convert.ToInt32(itm["storeQty"]) : 0;
                          }*/
                        pdf.Qty = Convert.ToInt32(itm["storeQty"]);

                        pdf.Price = Convert.ToDecimal(itm["priceperUnit"]);
                        fnf.Price = Convert.ToDecimal(itm["priceperUnit"]);
                        pdf.pack = 1;
                        fnf.pack = 1;
                        pdf.End = "";
                        pdf.deposit = "";
                        pdf.altupc5 = "";
                        pdf.altupc4 = "";
                        pdf.altupc3 = "";
                        pdf.altupc2 = "";
                        pdf.altupc1 = "";
                        fnf.region = "";
                        fnf.pcat2 = "";
                        fnf.region = "";
                        fnf.country = "";
                        fnf.pcat = itm["departmentName"].ToString();

                        if (pdf.Qty > 0 && pdf.Price > 0)
                        {
                            pm.Add(pdf);
                            fullname.Add(fnf);
                        }

                        if (pdf.Price > 0)
                        {
                            pm.Add(pdf);
                            fullname.Add(fnf);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            pm = pm.GroupBy(p => p.sku)
                                    .Select(g => g.First())
                                    .ToList();
            fullname = fullname.GroupBy(p => p.sku)
                                     .Select(g => g.First())
                                     .ToList();

            GenerateCSV.GenerateCSVFile(pm, "Product", storeid, BaseUrl);
            GenerateCSV.GenerateCSVFile(fullname, "FullName", storeid, BaseUrl);
            Console.WriteLine("Product file generated for RetailZAPI " + storeid);
            Console.WriteLine("FullName file generated for RetailZAPI " + storeid);


            
              #region      demo code for commit changes    
            //testing the code on 13-10-2025  by pushing this code from code editor to --
            // git hub account by using push option  (pos team  github)
            #endregion

            #region
            //Testing in progress
            //Testing in progress2
            //Testing in progress3
            //Testing in progress4
            #endregion
        }
    }
}
