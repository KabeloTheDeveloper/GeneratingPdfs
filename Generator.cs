using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Syncfusion.Pdf.Parsing;
using System.Web.Mvc;
using pdfs.Moldels;
namespace pdfs.Repositories
{
    public class Generator
    {
        public void CreateMultiplePages(Statement statement)
        {
            int transCount = statement.Transactions.Count;
            int numberOfPagesToCreate = ((int)transCount / 28) + 1;
            List<string> fileNames = new List<string>();
            
            //var finalfilePath = "C:\\Users\\kabelos\\Desktop\\pdfTemplate\\"+statement.Customer.AccountNumber + timeStamp + ".pdf";
            Dictionary<int, List<Transaction>> groups = new Dictionary<int, List<Transaction>>();
            int i = 0;
            while (i<numberOfPagesToCreate)
            {
                List<Transaction> group = new List<Transaction>();
                groups.Add(i,group);
                i++;
            }
            int g = 0;
            foreach(Transaction trans in statement.Transactions)
            {
                groups[g].Add(trans);
                if(groups[g].Count>=28)
                {
                    g++;
                }
            }
            List<Statement> OneIntoMany = new List<Statement>();
            OneIntoMany = StatementCreator.GenerateStatementWithMultiplePages(statement.Customer, groups);
            #region variables used
            List<string> dates = new List<string>();
            List<string> raNumbers = new List<string>();
            List<string> vouchers = new List<string>();
            List<string> order_Numbers = new List<string>();
            List<string> drivers_Names = new List<string>();
            List<string> details = new List<string>();
            List<string> debits = new List<string>();
            List<string> credits = new List<string>();
            List<string> amounts = new List<string>();
            string allDates = "\n";string allRAs = "\n";string allVouchers = "\n";string allOrdersNumbers = "\n";
            string allDriverNames = "\n"; string allDetails = "\n"; string allDebits = "\n"; string allCredits = "\n"; string allAmounts = "\n";
            int page = 1;
            #endregion
            foreach (Statement statement1 in OneIntoMany)
            {
                #region population of the varables
                foreach (Transaction transaction in statement1.Transactions)
                {

                    dates.Add(transaction.Date);
                    raNumbers.Add(transaction.RaNumber);
                    vouchers.Add(transaction.Voucher);
                    order_Numbers.Add(transaction.OrderNumebr);
                    drivers_Names.Add(transaction.DriverName);
                    details.Add(transaction.Details);
                    debits.Add(transaction.Debit);
                    credits.Add(transaction.Credit);
                    amounts.Add(transaction.Amount);
                }
                #endregion
                #region Create the actual PDF file
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                timeStamp = timeStamp.Replace('-', '_');
                timeStamp = timeStamp.Replace(' ', '_');
                timeStamp = timeStamp.Replace(':', '_');
                string pdfTemplate = "C:\\Users\\kabelos\\Desktop\\pdfTemplate\\FinalTemplate.pdf";
                var filePath = "C:\\Users\\kabelos\\Desktop\\pdfTemplate\\WithMultiplePages\\" + statement1.Customer.AccountNumber + timeStamp + ".pdf"; fileNames.Add(filePath);
                PdfReader reader = new PdfReader(pdfTemplate);
                PdfStamper stamper = new PdfStamper(reader, new FileStream(filePath, FileMode.Create));
                AcroFields fields = stamper.AcroFields;
                BaseFont ubuntu = BaseFont.CreateFont(@"C:\Users\kabelos\Desktop\pdfs\pdfs\App_Data\ubuntu\Ubuntu-R.ttf", BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                BaseFont arial = BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                BaseFont arialBold = BaseFont.CreateFont(@"C:\Windows\Fonts\ARLRDBD.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);              
                List<Anchor> anchors = AnchorCreator(raNumbers);
                foreach (DictionaryEntry item in reader.AcroFields.Fields)
                {
                    if(item.Key.ToString() == "ACCOUNT_NUMBER" || item.Key.ToString() == "DATE" || item.Key.ToString() == "PAGE" ||
                        item.Key.ToString() == "TOTAL" || item.Key.ToString() == "INVOICE_DATE" || item.Key.ToString() == "VOUCHER" || item.Key.ToString() == "RA_NO" ||
                        item.Key.ToString() == "ORDER_NO" || item.Key.ToString() == "DRIVERS_NAME" || item.Key.ToString() == "DETAILS"||
                        item.Key.ToString() == "DEBIT" || item.Key.ToString() == "CREDIT" || item.Key.ToString() == "AMOUNT")
                    {
                        fields.SetFieldProperty(item.Key.ToString(), "textsize", (float)8.0, null);
                        fields.SetFieldProperty(item.Key.ToString(), "textfont", ubuntu, null);
                    }
                    else
                    {
                        fields.SetFieldProperty(item.Key.ToString(), "textsize", (float)8.0, null);
                        fields.SetFieldProperty(item.Key.ToString(), "textfont", arial, null);
                        if (item.Key.ToString() == "TOTAL")
                        {
                            fields.SetFieldProperty(item.Key.ToString(), "textsize", (float)8.0, null);
                        }
                        
                    }
                    
                }
                
                #endregion
                #region Adding Data to the PDF fields
                fields.GenerateAppearances = true;
                fields.SetField("DEBTOR_NAME", statement1.Customer.Name);
                fields.SetField("DEBTOR_ADDRESS", statement1.Customer.Address.Trim());
                fields.SetField("ATTENTION", statement1.Customer.Attn);
                fields.SetField("ACCOUNT_NUMBER", statement1.Customer.AccountNumber.Trim());
                fields.SetField("DATE", statement1.StatementDate.ToShortDateString().Trim());
                fields.SetField("PAGE", page.ToString().Trim());
                fields.SetField("CURRENT", statement1.Current.ToString().Replace(',', '.'));
                fields.SetField("30_DAYS", statement1.ThirtyDays.ToString().Replace(',', '.'));
                fields.SetField("60_DAYS", statement1.SixtyDays.ToString().Replace(',', '.'));
                fields.SetField("90_DAYS", statement1.NinetyDays.ToString().Replace(',', '.'));
                fields.SetField("120_DAYS", statement1.OneTwentyDays.ToString().Replace(',', '.'));
                fields.SetField("TOTAL", statement1.Total.ToString().Replace(',', '.'));
                fields.RemoveField("RA_NO");
                PdfContentByte canvas = stamper.GetOverContent(1);
                Rectangle hyperLinksRec = new Rectangle(90.1306f, 121.879f, 205.189f, 390.606f);
                hyperLinksRec.BorderWidth = 1.0f;
                ColumnText ct = new ColumnText(canvas);
                ct.SetSimpleColumn(95.1306f, 121.879f, 205.189f, 390.606f);
                Paragraph paragraph = new Paragraph((float)9.0,"\n");
                foreach(Anchor an in anchors)
                {
                    paragraph.Add(an);
                    paragraph.Add("\n");
                }
                ct.AddElement(paragraph);
                ct.Go();
                canvas.Rectangle(hyperLinksRec);
                
                #region set variables
                foreach (string date in dates)
                {
                    allDates = allDates + date + "\n";
                }
                foreach (string ra in raNumbers)
                {
                    allRAs = allRAs + ra + "\n";
                }
                foreach (string voucher in vouchers)
                {
                    allVouchers = allVouchers + voucher + "\n";
                }
                foreach (string orderNum in order_Numbers)
                {
                    allOrdersNumbers = allOrdersNumbers + orderNum + "\n";
                }
                foreach (string driver in drivers_Names)
                {
                    allDriverNames = allDriverNames + driver + "\n";
                }
                foreach (string detail in details)
                {
                    allDetails = allDetails + detail + "\n";
                }
                foreach (string debit in debits)
                {
                    allDebits = allDebits + debit + "\n";
                }
                foreach (string credit in credits)
                {
                    allCredits = allCredits + credit + "\n";
                }
                foreach (string amount in amounts)
                {
                    allAmounts = allAmounts + amount + "\n";
                }
                #endregion
                fields.SetField("INVOICE_DATE", allDates);
                fields.SetField("VOUCHER", allVouchers);
                fields.SetField("ORDER_NO", allOrdersNumbers);
                fields.SetField("DRIVERS_NAME", allDriverNames);
                fields.SetField("DETAILS", allDetails);
                fields.SetField("DEBIT", allDebits);
                fields.SetField("CREDIT", allCredits);
                fields.SetField("AMOUNT", allAmounts);
                #endregion
                #region The Closing of the created PDF File
                stamper.FormFlattening = true;
                stamper.SetFullCompression();
                stamper.Close();
                #endregion
                #region remove data from varables
                dates.Clear(); raNumbers.Clear(); vouchers.Clear(); order_Numbers.Clear(); drivers_Names.Clear(); details.Clear(); debits.Clear(); credits.Clear(); amounts.Clear();
                 allDates = "\n"; allRAs = "\n"; allVouchers = "\n"; allOrdersNumbers = "\n";  allDriverNames = "\n";  allDetails = "\n";  allDebits = "\n";  allCredits = "\n";  allAmounts = "\n";
                page = page + 1;
                #endregion
            }
            Merger(fileNames, statement.Customer.Name);
            foreach(string file in fileNames)
            {
                File.Delete(file);
            }

        }
        public void MakeAPDF(List<Statement> statements)
        {
            #region variables used
            List<string> dates = new List<string>();
            List<string> raNumbers = new List<string>();
            List<string> vouchers = new List<string>();
            List<string> order_Numbers = new List<string>();
            List<string> drivers_Names = new List<string>();
            List<string> details = new List<string>();
            List<string> debits = new List<string>();
            List<string> credits = new List<string>();
            List<string> amounts = new List<string>();
            string allDates = "\n";
            string allRAs = "\n";
            string allVouchers = "\n";
            string allOrdersNumbers = "\n"; string allDriverNames = "\n"; string allDetails = "\n"; string allDebits = "\n"; string allCredits = "\n"; string allAmounts = "\n";
            #endregion
            foreach (Statement st in statements)
            {
                if (st.Transactions.Count<=28)
                {
                    
                    #region population of the varables
                    foreach (Transaction transaction in st.Transactions)
                    {
                       
                        dates.Add(transaction.Date);
                        raNumbers.Add(transaction.RaNumber);
                        vouchers.Add(transaction.Voucher);
                        order_Numbers.Add(transaction.OrderNumebr);
                        drivers_Names.Add(transaction.DriverName);
                        details.Add(transaction.Details);
                        debits.Add(transaction.Debit);
                        credits.Add(transaction.Credit);
                        amounts.Add(transaction.Amount);
                    }
                    #endregion
                    #region Create the actual PDF file
                    string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    timeStamp = timeStamp.Replace('-', '_');
                    timeStamp = timeStamp.Replace(' ', '_');
                    timeStamp = timeStamp.Replace(':', '_');
                    string pdfTemplate = "C:\\Users\\kabelos\\Desktop\\pdfTemplate\\FinalTemplate.pdf";
                    var filePath = "C:\\Users\\kabelos\\Desktop\\pdfTemplate\\"+st.Customer.Name + timeStamp + ".pdf";
                    PdfReader reader = new PdfReader(pdfTemplate);
                    PdfStamper stamper = new PdfStamper(reader, new FileStream(filePath, FileMode.Create));
                    AcroFields fields = stamper.AcroFields;
                    BaseFont ubuntu = BaseFont.CreateFont(@"C:\Users\kabelos\Desktop\pdfs\pdfs\App_Data\ubuntu\Ubuntu-R.ttf", BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                    BaseFont arial = BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                    BaseFont boldArial = BaseFont.CreateFont(@"C:\Windows\Fonts\BAUHS93.ttf", BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                    List<Anchor> anchors = AnchorCreator(raNumbers);
                    foreach (DictionaryEntry item in reader.AcroFields.Fields)
                    {
                        if (item.Key.ToString() == "ACCOUNT_NUMBER" || item.Key.ToString() == "DATE" || item.Key.ToString() == "PAGE" ||
                            item.Key.ToString() == "TOTAL" || item.Key.ToString() == "INVOICE_DATE" || item.Key.ToString() == "VOUCHER" || item.Key.ToString() == "RA_NO" ||
                            item.Key.ToString() == "ORDER_NO" || item.Key.ToString() == "DRIVERS_NAME" || item.Key.ToString() == "DETAILS" ||
                            item.Key.ToString() == "DEBIT" || item.Key.ToString() == "CREDIT" || item.Key.ToString() == "AMOUNT")
                        {
                            fields.SetFieldProperty(item.Key.ToString(), "textsize", (float)8.0, null);
                            fields.SetFieldProperty(item.Key.ToString(), "textfont", ubuntu, null);
                        }
                        else
                        {
                            fields.SetFieldProperty(item.Key.ToString(), "textsize", (float)8.0, null);
                            fields.SetFieldProperty(item.Key.ToString(), "textfont", arial, null);
                            if (item.Key.ToString() == "TOTAL")
                            {
                                fields.SetFieldProperty(item.Key.ToString(), "textfont", boldArial,null);
                            }
                        }

                    }
                    #endregion
                    #region Adding Data to the PDF fields
                    fields.GenerateAppearances = true;
                    fields.SetField("DEBTOR_NAME", st.Customer.Name);
                    fields.SetField("DEBTOR_ADDRESS", st.Customer.Address.Trim());
                    fields.SetField("ATTENTION", st.Customer.Attn);
                    fields.SetField("ACCOUNT_NUMBER", st.Customer.AccountNumber.Trim());
                    fields.SetField("DATE", st.StatementDate.ToShortDateString().Trim());
                    fields.SetField("PAGE", st.statementPageNumber.ToString());
                    fields.SetField("CURRENT", st.Current.ToString().Replace(',', '.'));
                    fields.SetField("30_DAYS", st.ThirtyDays.ToString().Replace(',', '.'));
                    fields.SetField("60_DAYS", st.SixtyDays.ToString().Replace(',', '.'));
                    fields.SetField("90_DAYS", st.NinetyDays.ToString().Replace(',', '.'));
                    fields.SetField("120_DAYS", st.OneTwentyDays.ToString().Replace(',', '.'));
                    fields.SetField("TOTAL", st.Total.ToString().Replace(',', '.'));
                    fields.RemoveField("RA_NO");
                    PdfContentByte canvas = stamper.GetOverContent(1);
                    Rectangle hyperLinksRec = new Rectangle(91.1306f, 121.879f, 205.189f, 390.606f);
                    hyperLinksRec.BorderWidth = 1.0f;
                    ColumnText ct = new ColumnText(canvas);
                    ct.SetSimpleColumn(95.1306f, 121.879f, 205.189f, 390.606f);
                    Paragraph paragraph = new Paragraph((float)9.0, "\n");
                    foreach (Anchor an in anchors)
                    {
                        paragraph.Add(an);
                        paragraph.Add("\n");
                    }
                    ct.AddElement(paragraph);
                    ct.Go();
                    canvas.Rectangle(hyperLinksRec);
                    foreach (string date in dates)
                    {
                        allDates = allDates + date + "\n";
                    }
                    foreach (string ra in raNumbers)
                    {
                        allRAs = allRAs + ra + "\n";
                    }
                    foreach (string voucher in vouchers)
                    {
                        allVouchers = allVouchers + voucher + "\n";
                    }
                    foreach (string orderNum in order_Numbers)
                    {
                        allOrdersNumbers = allOrdersNumbers + orderNum + "\n";
                    }
                    foreach (string driver in drivers_Names)
                    {
                        allDriverNames = allDriverNames + driver + "\n";
                    }
                    foreach (string detail in details)
                    {
                        allDetails = allDetails + detail + "\n";
                    }
                    foreach (string debit in debits)
                    {
                        allDebits = allDebits + debit + "\n";
                    }
                    foreach (string credit in credits)
                    {
                        allCredits = allCredits + credit + "\n";
                    }
                    foreach (string amount in amounts)
                    {
                        allAmounts = allAmounts + amount + "\n";
                    }
                    fields.SetField("INVOICE_DATE", allDates);
                    fields.SetField("RA_NO",allRAs);
                    fields.SetField("VOUCHER", allVouchers);
                    fields.SetField("ORDER_NO", allOrdersNumbers);
                    fields.SetField("DRIVERS_NAME", allDriverNames);
                    fields.SetField("DETAILS", allDetails);
                    fields.SetField("DEBIT", allDebits);
                    fields.SetField("CREDIT", allCredits);
                    fields.SetField("AMOUNT", allAmounts);
                    
                    #endregion
                    #region The Closing of the created PDF File
                    stamper.FormFlattening = true;
                    stamper.SetFullCompression();
                    stamper.Close();
                    #endregion
                    dates.Clear(); raNumbers.Clear(); vouchers.Clear(); order_Numbers.Clear(); drivers_Names.Clear(); details.Clear(); debits.Clear(); credits.Clear(); amounts.Clear();
                    allDates = "\n"; allRAs = "\n"; allVouchers = "\n"; allOrdersNumbers = "\n"; allDriverNames = "\n"; allDetails = "\n"; allDebits = "\n"; allCredits = "\n"; allAmounts = "\n";

                }
                else
                {
                    CreateMultiplePages(st);
                }
                
            }
            
        }
        public static List<Anchor> AnchorCreator(List<string> allRas)
        {
            List<Anchor> anchors = new List<Anchor>();
            foreach(string ra in allRas)
            {
                string raNumber = ra.Replace("/", "%2F");
                Font link = FontFactory.GetFont("Arial", 8,Font.NORMAL, new Color(0, 0, 225));
                Anchor anchor = new Anchor(ra,link);
                anchor.Reference = "https://invoices.bcr.co.za/Home/GetInvoice?InvoiceNumber=" + raNumber;
                anchors.Add(anchor);
            }
            return anchors;
        }
        public static void Merger(List<string> pdfFiles, string compName)
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            timeStamp = timeStamp.Replace('-', '_');
            timeStamp = timeStamp.Replace(' ', '_');
            timeStamp = timeStamp.Replace(':', '_');
            Document doc = new Document();
            PdfCopy writer = new PdfCopy(doc, new FileStream("C:\\Users\\kabelos\\Desktop\\pdfTemplate\\"+compName + timeStamp + ".pdf", FileMode.Create));
            if (writer == null)
            {
                return;
            }
            doc.Open();
            foreach(string file in pdfFiles)
            {
                PdfReader reader = new PdfReader(file);
                reader.ConsolidateNamedDestinations();
                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                writer.AddPage(page);
                reader.Close();
            }
            writer.Close();
            doc.Close();
        }
           

        
    }
}