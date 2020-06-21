using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ZipStore.Concrete;
using ZipStore.Entities;
using ZipStore.Models;

namespace ZipStore.Controllers
{
    public class HomeController : Controller
    {
        
        //private readonly string conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\zipstore_db.mdf;Integrated Security=True";
        //    connectionString= "Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\aspnet-owinTest-20140519094353.mdf;Initial Catalog=aspnet-owinTest-20140519094353;Integrated Security=True" providerName="System.Data.SqlClient" />
        public ViewResult Index()
        {

            //DataTable ZipList = new DataTable();
            /*  using (SqlConnection sqlCon = new SqlConnection(conStr))
              {
                  sqlCon.Open();
                  SqlDataAdapter sqlData = new SqlDataAdapter(
                      "SELECT s.Id, s.Vendor, s.Number, s.SearchVendor, s.SearchNumber, s.Description, s.Price, s.Count FROM zipItem s", sqlCon);
                  sqlData.Fill(ZipList);
                  sqlCon.Close();
              }*/

            /* using (var db = new EFDBContext())
             { 
                 var zip = new ZipItem { Vendor = "555", Number = "SA-1712L", SearchVendor = "555", SearchNumber = "SA1712L", Description = "Рычаг подвески | перед лев |", Price = 1343.68F, Count = 2 };
                 db.ZipItems.Add(zip);
                 db.SaveChanges();
             }*/


            EFDBContext context = new EFDBContext();
            ZipListViewModel viewModel = new ZipListViewModel { ZipItems = context.ZipItems };
            
            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}