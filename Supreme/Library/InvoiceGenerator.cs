using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Supreme.Models;


namespace Supreme.Library
{
    public class InvoiceGenerator
    {
        //Class Variables
        
        string orderDate = DateTime.Now.ToString("dd MMM yyyy");
        

        // for Gridview
       
        private ApplicationDbContext db = new ApplicationDbContext();


        public  Stream CreateInvoicePDF(Order order)
        {
            

            if (order == null)
            {
                return null;
            }

            var products = db.OrderProducts.Where(b => b.orderId == order.id).ToList();

            // Create a Document object
            Document document = new Document(PageSize.A4, 70, 70, 70, 70);

            //MemoryStream
            Stream PDFData = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, PDFData);

            // First, create our fonts
            var titleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
            var bodyFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            Rectangle pageSize = writer.PageSize;

            // Open the Document for writing
            document.Open();
            //Add elements to the document here

            #region Top table
            // Create the header table 

            PdfPTable headertable = new PdfPTable(3);
            
            
            headertable.HorizontalAlignment = 0;
            headertable.WidthPercentage = 100;
            headertable.SetWidths(new float[] { 4, 2, 4 });  // then set the column's __relative__ widths
            headertable.DefaultCell.Border = Rectangle.NO_BORDER;
            //headertable.DefaultCell.Border = Rectangle.BOX; //for testing
            headertable.SpacingAfter = 30;

            PdfPTable nested = new PdfPTable(1);
            //nested.HorizontalAlignment = 1;
            nested.DefaultCell.Border = Rectangle.BOX;
            PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Das Value Trading (Pvt) Ltd", bodyFont));
            nextPostCell1.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            nested.AddCell(nextPostCell1);
            PdfPCell nextPostCell2 = new PdfPCell(new Phrase("54 Kelvin road North", bodyFont));
            nextPostCell2.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            nested.AddCell(nextPostCell2);
            PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Harare", bodyFont));
            nextPostCell3.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            nested.AddCell(nextPostCell3);

            PdfPCell nextPostCell4 = new PdfPCell(new Phrase("Zimbabwe", bodyFont));
            nextPostCell4.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            nested.AddCell(nextPostCell4);

            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.Rowspan = 4;
            nesthousing.Padding = 0f;
            headertable.AddCell(nesthousing);

            headertable.AddCell("");
            PdfPCell invoiceCell = new PdfPCell(new Phrase("TAX INVOICE", titleFont));
            invoiceCell.HorizontalAlignment = 2;
            invoiceCell.Border = Rectangle.NO_BORDER;
            //invoiceCell.Indent=10
            headertable.AddCell(invoiceCell);


            
            
            PdfPCell noCell = new PdfPCell(new Phrase("Ivoice No :", bodyFont));
            noCell.HorizontalAlignment = 2;
            noCell.Border = Rectangle.NO_BORDER;
            headertable.AddCell(noCell);
            headertable.AddCell(new Phrase(order.invoiceNumber, bodyFont));
            PdfPCell dateCell = new PdfPCell(new Phrase("Date :", bodyFont));
            dateCell.HorizontalAlignment = 2;
            dateCell.Border = Rectangle.NO_BORDER;
            headertable.AddCell(dateCell);
            headertable.AddCell(new Phrase(orderDate, bodyFont));
            PdfPCell billCell = new PdfPCell(new Phrase("Bill To :", bodyFont));
            billCell.HorizontalAlignment = 2;
            billCell.Border = Rectangle.NO_BORDER;
            headertable.AddCell(billCell);
            headertable.AddCell(new Phrase(order.branch.name + "\n" + order.branch.address, bodyFont));
            document.Add(headertable);
            #endregion

            //vat table

            PdfPTable vatTable = new PdfPTable(1);
            vatTable.HorizontalAlignment = 0;
            vatTable.DefaultCell.Border = Rectangle.NO_BORDER;
            vatTable.SpacingAfter = 20;
            PdfPCell vatH = new PdfPCell(new Phrase("VAT: 10054211", boldTableFont));
            vatH.Border = Rectangle.NO_BORDER;
            vatTable.AddCell(vatH);
            document.Add(vatTable);

            #region Items Table
            //Create body table
            PdfPTable itemTable = new PdfPTable(5);
            itemTable.HorizontalAlignment = 0;
            itemTable.WidthPercentage = 100;
            itemTable.SetWidths(new float[] { 10, 40, 20, 30, 20 });  // then set the column's __relative__ widths
            itemTable.SpacingAfter = 40;
            itemTable.DefaultCell.Border = Rectangle.BOX;
            PdfPCell cell1 = new PdfPCell(new Phrase("NO", boldTableFont));
            cell1.HorizontalAlignment = 1;
            itemTable.AddCell(cell1);
            PdfPCell cell2 = new PdfPCell(new Phrase("ITEM", boldTableFont));
            cell2.HorizontalAlignment = 1;
            itemTable.AddCell(cell2);
            PdfPCell cell3 = new PdfPCell(new Phrase("QUANTITY", boldTableFont));
            cell3.HorizontalAlignment = 1;
            itemTable.AddCell(cell3);
            PdfPCell cell6 = new PdfPCell(new Phrase("CASE PRICE(USD)", boldTableFont));
            cell6.HorizontalAlignment = 1;
            itemTable.AddCell(cell6);
            PdfPCell cell4 = new PdfPCell(new Phrase("AMOUNT(USD)", boldTableFont));
            cell4.HorizontalAlignment = 1;
            itemTable.AddCell(cell4);

            double amount = 0;
            double total = 0;
            int count = 1;

            foreach (OrderProduct item in products)
            {
                amount = item.quantity * item.price;
                total +=amount;

                PdfPCell numberCell = new PdfPCell(new Phrase((count++)+"", bodyFont));
                numberCell.HorizontalAlignment = 0;
                numberCell.PaddingLeft = 10f;
                numberCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                itemTable.AddCell(numberCell);

                PdfPCell descCell = new PdfPCell(new Phrase(item.product.name, bodyFont));
                descCell.HorizontalAlignment = 0;
                descCell.PaddingLeft = 10f;
                descCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                itemTable.AddCell(descCell);

                PdfPCell qtyCell = new PdfPCell(new Phrase(item.quantity+"", bodyFont));
                qtyCell.HorizontalAlignment = 0;
                qtyCell.PaddingLeft = 10f;
                qtyCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                itemTable.AddCell(qtyCell);

                PdfPCell amtCell = new PdfPCell(new Phrase(Math.Round(item.price,2)+"", bodyFont));
                amtCell.HorizontalAlignment = 1;
                amtCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                itemTable.AddCell(amtCell);

                PdfPCell totCell = new PdfPCell(new Phrase(Math.Round(amount,2) + "", bodyFont));
                totCell.HorizontalAlignment = 1;
                totCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                itemTable.AddCell(totCell);
            }

            // Table footer
            //subtotal
            PdfPCell subtotAmtCell1 = new PdfPCell(new Phrase(""));
            subtotAmtCell1.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
            itemTable.AddCell(subtotAmtCell1);
            PdfPCell subtotAmtCell2 = new PdfPCell(new Phrase(""));
            subtotAmtCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            itemTable.AddCell(subtotAmtCell2);

            PdfPCell subtotAmtCell3 = new PdfPCell(new Phrase(""));
            subtotAmtCell3.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            itemTable.AddCell(subtotAmtCell3);

            PdfPCell subtotAmtStrCell = new PdfPCell(new Phrase("Subtotal", boldTableFont));
            subtotAmtStrCell.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            subtotAmtStrCell.HorizontalAlignment = 1;
            itemTable.AddCell(subtotAmtStrCell);

            PdfPCell subtotAmtCell = new PdfPCell(new Phrase("$" + Math.Round(total,2), boldTableFont));
            subtotAmtCell.HorizontalAlignment = 1;
            itemTable.AddCell(subtotAmtCell);

            //vat
            PdfPCell vatAmtCell1 = new PdfPCell(new Phrase(""));
            vatAmtCell1.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
            itemTable.AddCell(vatAmtCell1);
            PdfPCell vatAmtCell2 = new PdfPCell(new Phrase(""));
            vatAmtCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            itemTable.AddCell(vatAmtCell2);

            PdfPCell vatAmtCell3 = new PdfPCell(new Phrase(""));
            vatAmtCell3.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            itemTable.AddCell(vatAmtCell3);

            PdfPCell vatAmtStrCell = new PdfPCell(new Phrase("VAT", boldTableFont));
            vatAmtStrCell.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            vatAmtStrCell.HorizontalAlignment = 1;
            itemTable.AddCell(vatAmtStrCell);

            PdfPCell vatAmtCell = new PdfPCell(new Phrase("$" + Math.Round((total*0.15),2), boldTableFont));
            vatAmtCell.HorizontalAlignment = 1;
            itemTable.AddCell(vatAmtCell);



            // Total
            PdfPCell totalAmtCell1 = new PdfPCell(new Phrase(""));
            totalAmtCell1.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
            itemTable.AddCell(totalAmtCell1);
            PdfPCell totalAmtCell2 = new PdfPCell(new Phrase(""));
            totalAmtCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            itemTable.AddCell(totalAmtCell2);

            PdfPCell totalAmtCell3 = new PdfPCell(new Phrase(""));
            totalAmtCell3.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            itemTable.AddCell(totalAmtCell3);

            PdfPCell totalAmtStrCell = new PdfPCell(new Phrase("Total Amount", boldTableFont));
            totalAmtStrCell.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
            totalAmtStrCell.HorizontalAlignment = 1;
            itemTable.AddCell(totalAmtStrCell);

            PdfPCell totalAmtCell = new PdfPCell(new Phrase("$"+ Math.Round((total*1.15),2), boldTableFont));
            totalAmtCell.HorizontalAlignment = 1;
            itemTable.AddCell(totalAmtCell);

            PdfPCell cell = new PdfPCell(new Phrase("*** Please note our Bank account is USD Bank Account ***", bodyFont));
            cell.Colspan = 5;
            cell.HorizontalAlignment = 1;
            itemTable.AddCell(cell);
            document.Add(itemTable);
            #endregion

            Chunk transferBank = new Chunk(order.branch.bank.account_holder, boldTableFont);
            transferBank.SetUnderline(0.1f, -2f); //0.1 thick, -2 y-location
            document.Add(transferBank);
            document.Add(Chunk.NEWLINE);

            // Bank Account Info
            PdfPTable bottomTable = new PdfPTable(3);
            bottomTable.HorizontalAlignment = 0;
            bottomTable.TotalWidth = 300f;
            bottomTable.SetWidths(new int[] { 90, 10, 200 });
            bottomTable.LockedWidth = true;
            bottomTable.SpacingBefore = 20;
            bottomTable.DefaultCell.Border = Rectangle.NO_BORDER;
            bottomTable.AddCell(new Phrase("Account No", bodyFont));
            bottomTable.AddCell(":");
            bottomTable.AddCell(new Phrase(order.branch.bank.account_number, bodyFont));
            bottomTable.AddCell(new Phrase("Account Name", bodyFont));
            bottomTable.AddCell(":");
            bottomTable.AddCell(new Phrase(order.branch.bank.account_holder, bodyFont));
            bottomTable.AddCell(new Phrase("Branch", bodyFont));
            bottomTable.AddCell(":");
            bottomTable.AddCell(new Phrase(order.branch.bank.branch_name, bodyFont));
            bottomTable.AddCell(new Phrase("Bank", bodyFont));
            bottomTable.AddCell(":");
            bottomTable.AddCell(new Phrase(order.branch.bank.name, bodyFont));
            document.Add(bottomTable);

            //Approved by
            PdfContentByte cb = new PdfContentByte(writer);
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            cb = writer.DirectContent;
            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.SetTextMatrix(pageSize.GetLeft(300), 200);
            cb.ShowText("Approved by,");
            cb.EndText();
            //Image Singature
            /*Uri url = new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/5/56/Autograph_of_Benjamin_Franklin.svg/2000px-Autograph_of_Benjamin_Franklin.svg.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(url);
            logo.SetAbsolutePosition(pageSize.GetLeft(300), 140);
            document.Add(logo);**/

            cb = new PdfContentByte(writer);
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            cb = writer.DirectContent;
            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.SetTextMatrix(pageSize.GetLeft(70), 100);
            cb.ShowText("This is a computer generated invoice. Contact us at sales@supremebrands.co.zw for enquiries");
            cb.EndText();

            writer.CloseStream = false; //set the closestream property
                                        // Close the Document without closing the underlying stream
            document.Close();
            
            return PDFData;
        }
    }
}