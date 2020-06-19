using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace ZipStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly string conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Projects\ASP.NET\ZipStore\ZipStore\ZipStore\App_Data\zipstore_db.mdf;Integrated Security=True";

        public ActionResult Index()
        {
            DataTable ZipList = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {
                sqlCon.Open();
                SqlDataAdapter sqlData = new SqlDataAdapter(
                    "SELECT s.Id, s.Vendor, s.Number, s.SearchVendor, s.SearchNumber, s.Description, s.Price, s.Count FROM zipItem s", sqlCon);
                sqlData.Fill(ZipList);
                sqlCon.Close();
            }
            return View(ZipList);
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