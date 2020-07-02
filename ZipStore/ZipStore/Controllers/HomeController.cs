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
using System.Xml.Schema;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

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
                                
                                // Load file to server in Import derectory 
                                FileStream stream = System.IO.File.Create(Server.MapPath("~/Import/")+f.ContentDisposition.FileName);
                                MimePart part = (MimePart)f;
                                part.ContentObject.DecodeTo(stream);
                                stream.Close();

                                ViewBag.Message = SaveToBaseCSV(Server.MapPath("~/Import/") + f.ContentDisposition.FileName, Server.MapPath("~/Schemes/") + "scheme.xml");

                                //TODO: BULK INSERT with schemas from Stream to DB
                                // ATTENTION!!!! Before insert Data to DB, Data must check on SQL Injection script!!!
                                // ATTENTION!!!! Before insert Data to DB, Data must check on SQL Injection script!!!
                                // ATTENTION!!!! Before insert Data to DB, Data must check on SQL Injection script!!!
/*                                using (EFDBContext dbContext = new EFDBContext())
                                {
                                    //dbContext.ZipItems = ReadCSV
                                    //string query = "INSERT INTO ZipItems (Vendor, Number, SearchVendor, SearchNumber, Description, Price, Count) VALUES ('666', 'SA-1712L', '666', 'SA1712L', 'Рычаг подвески | перед лев |', '1343', '2')";
                                    string query = $"BULK INSERT ZipItems FROM '{Server.MapPath("~/Import/") + f.ContentDisposition.FileName}'" +
                                                   $"WITH (FORMATFILE='{Server.MapPath("~/Schemes/")}scheme.xml', FIRSTROW = 2, FIELDTERMINATOR = ';', ROWTERMINATOR = '\n', CODEPAGE ='OEM', KEEPNULLS, ERRORFILE = '{Server.MapPath("~/Import/")}ErrorRows.csv', TABLOCK)";   
                                    int qan = dbContext.Database.ExecuteSqlCommand(query);
                                    dbContext.Dispose();
                                }
*/                                                      
                                // Save to base throw entity datebase
                                /*using (var db = new EFDBContext())
                                {
                                    var zip = new ZipItem { Vendor = "555", Number = "SA-1712L", SearchVendor = "555", SearchNumber = "SA1712L", Description = "Рычаг подвески | перед лев |", Price = 1343, Count = 2 };
                                    db.ZipItems.Add(zip);
                                    db.SaveChanges();
                                }*/
                            }
                        }
                    }
                }
                imap.Disconnect(true);
            }
            return View(lst);
        }

        private List<string> SaveToBaseCSV(string nameCSV, string nameConfig)
        {
            // Load Data from csv file
            List<string> str = new List<string>();
            DataTable csvData = GetDataCSV(nameCSV);

            // Load config from xml file
            DataSet ds = new DataSet();
            ds.ReadXml(nameConfig);
            DataTable xmlData = new DataTable();
            xmlData = ds.Tables[0];

            // ReLoad Data from csv table to DB by field from xml config file
            using (EFDBContext dbContext = new EFDBContext())
            {
                var st = from myZip in csvData.AsEnumerable()
                         where myZip.Field<string>("Бренд") == "555"
                         select myZip;

                IEnumerable<string> fldVendor = from p in xmlData.AsEnumerable()
                                                where p.Field<string>("FIELD") == "Vendor"
                                                select p.Field<string>("SOURCE");
                IEnumerable<string> fldNumber = from p in xmlData.AsEnumerable()
                                                where p.Field<string>("FIELD") == "Number"
                                                select p.Field<string>("SOURCE");
                IEnumerable<string> fldDescription = from p in xmlData.AsEnumerable()
                                                where p.Field<string>("FIELD") == "Description"
                                                select p.Field<string>("SOURCE");
                IEnumerable<string> fldPrice = from p in xmlData.AsEnumerable()
                                                where p.Field<string>("FIELD") == "Price"
                                                select p.Field<string>("SOURCE");
                IEnumerable<string> fldCount = from p in xmlData.AsEnumerable()
                                                where p.Field<string>("FIELD") == "Count"
                                                select p.Field<string>("SOURCE");

                foreach (DataRow row in csvData.Rows)
                {
                    string vendor = row.Field<string>(fldVendor.First());
                    string number = row.Field<string>(fldNumber.First());
                    string schVendor = Regex.Match(row.Field<string>(fldVendor.First()), @"^[A-Za-zА-Яа-я0-9]+$").Value.ToUpper();
                    string schNumber = Regex.Match(row.Field<string>(fldNumber.First()), @"^[A-Za-zА-Яа-я0-9]+$").Value.ToUpper();
                    string description = row.Field<string>(fldDescription.First());
                    string strPrice = row.Field<string>(fldPrice.First()).Replace(",", ".");
                    int count = Convert.ToInt32(Regex.Match(row.Field<string>(fldCount.First()), @"[0-9]+$").Value);
                    

                    string query = $"INSERT INTO ZipItems (Vendor, Number, SearchVendor, SearchNumber, Description, Price, Count) VALUES (N'{vendor}', N'{number}', N'{schVendor}', N'{schNumber}', N'{description}', {strPrice}, {count})";
                    int qan = dbContext.Database.ExecuteSqlCommand(query);
                }
                
                //"INSERT INTO ZipItems (Vendor, Number, Description, Price, Count) VALUES (@vendor, 'SA-1712L', '666', 'SA1712L', 'Рычаг подвески | перед лев |', '1343', '2')";
                //int qan = dbContext.Database.ExecuteSqlCommand(query);
               
                dbContext.Dispose();
            }

            ViewBag.Table = xmlData;
            return str;
        }

        private static DataTable GetDataCSV(string csvFilePath)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csvFilePath))
                {
                    csvReader.SetDelimiters(new string[] { ";" });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Fill empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                          if (fieldData[i] == "") fieldData[i] = "NULL";
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception) {}
            return csvData;
        }

        private static void GetConfigXML(string xmlFilePath)
        {

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