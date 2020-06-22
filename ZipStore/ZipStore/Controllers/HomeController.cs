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
using System.Threading;
using MailKit;
using MailKit.Net.Imap;
using System.IO;
using MimeKit;

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

        [Obsolete]
        public ActionResult Check()
        {
            List<MimeKit.MimeEntity> lst = new List<MimeKit.MimeEntity>();

            using (var imap = new ImapClient())
            {
                imap.Connect("imap.yandex.ru", 993, true);
                imap.Authenticate("partszip@yandex.ru", "OrderZip123");

                IMailFolder inbox = imap.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                                
                // Download and receive all email messages from selected mailbox folder.
                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);

                    lst = message.Attachments.ToList();
                    IEnumerable<MimeEntity> entity = message.Attachments;                   // Load All Attachments
                    if (entity != null)
                    {
                        // Looking All Attachments in File
                        foreach (var f in entity)
                        {
                            string fileName = f.ContentDisposition?.FileName;               // FileName of Attachments
                            string extension = fileName.Substring(fileName.Length - 3, 3);  // Extension of The File

                            if (extension == "csv")
                            {
                                var stream = System.IO.File.Create("/Import/"+f.ContentDisposition.FileName);
                                MimePart part = (MimePart)f;
                                part.ContentObject.DecodeTo(stream);
                                stream.Close();

                                //TODO: BULK INSERT with schemas from Stream to DB
                                // ATTENTION!!!! Before insert Data to DB, Data must check on SQL Injection script!!!
                                // ATTENTION!!!! Before insert Data to DB, Data must check on SQL Injection script!!!
                                // ATTENTION!!!! Before insert Data to DB, Data must check on SQL Injection script!!!
                                using (EFDBContext dbContext = new EFDBContext())
                                {
                                    //string query = "INSERT INTO ZipItems (Vendor, Number, SearchVendor, SearchNumber, Description, Price, Count) VALUES ('666', 'SA-1712L', '666', 'SA1712L', 'Рычаг подвески | перед лев |', '1343', '2')";
                                    string query = $"BULK INSERT ZipItems FROM '/Import/{f.ContentDisposition.FileName}'" +
                                                   $"WITH ( FORMATFILE='~/Schemes/scheme.xml', FIRSTROW = 2, FIELDTERMINATOR = ';', ROWTERMINATOR = '\n', ERRORFILE = 'ErrorRows.csv', TABLOCK)";
                                    int qan = dbContext.Database.ExecuteSqlCommand(query);
                                }
                            }
                        }
                    }
                }
                imap.Disconnect(true);
            }

            /*using (var db = new EFDBContext())
            {
                var zip = new ZipItem { Vendor = "555", Number = "SA-1712L", SearchVendor = "555", SearchNumber = "SA1712L", Description = "Рычаг подвески | перед лев |", Price = 1343, Count = 2 };
                db.ZipItems.Add(zip);
                db.SaveChanges();
            }*/
            return View(lst);
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