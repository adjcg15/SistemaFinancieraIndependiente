using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIClient.Utilities
{
    public class GeneratorPDFLayout
    {
        public static void GeneratePDF(string client, string creditInvoice, string plannedDate, double amount, string captureLine)
        {
            string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(downloadsFolder, "SFLayout.pdf");

            Document document = new Document(PageSize.A4, 40f, 40f, 40f, 40f);

            try
            {
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                document.Open();

                Font titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                Font labelFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                Font valueFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);

                Paragraph title = new Paragraph("Layout de Pago", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(new Paragraph(" "));

                Paragraph clientLabel = new Paragraph("Cliente:", labelFont);
                clientLabel.Alignment = Element.ALIGN_LEFT;
                document.Add(clientLabel);

                Paragraph clientValue = new Paragraph(client, valueFont);
                clientValue.Alignment = Element.ALIGN_LEFT;
                document.Add(clientValue);
                document.Add(new Paragraph(" "));
                Paragraph creditInvoiceLabel = new Paragraph("Folio del crédito:", labelFont);
                creditInvoiceLabel.Alignment = Element.ALIGN_LEFT;
                document.Add(creditInvoiceLabel);

                Paragraph creditInvoiceValue = new Paragraph(creditInvoice, valueFont);
                creditInvoiceValue.Alignment = Element.ALIGN_LEFT;
                document.Add(creditInvoiceValue);

                document.Add(new Paragraph(" "));
                Paragraph plannedDateLabel = new Paragraph("Fecha planeada de pago:", labelFont);
                plannedDateLabel.Alignment = Element.ALIGN_LEFT;
                document.Add(plannedDateLabel);

                Paragraph plannedDateValue = new Paragraph(plannedDate, valueFont);
                plannedDateValue.Alignment = Element.ALIGN_LEFT;
                document.Add(plannedDateValue);

                document.Add(new Paragraph(" "));
                Paragraph amountLabel = new Paragraph("Importe de pago:", labelFont);
                amountLabel.Alignment = Element.ALIGN_LEFT;
                document.Add(amountLabel);

                Paragraph amountValue = new Paragraph(amount.ToString("C"), valueFont);
                amountValue.Alignment = Element.ALIGN_LEFT;
                document.Add(amountValue);

                document.Add(new Paragraph(" "));

                Paragraph captureLineLabel = new Paragraph("Línea de captura:", labelFont);
                captureLineLabel.Alignment = Element.ALIGN_LEFT;
                document.Add(captureLineLabel);

                Paragraph captureLineValue = new Paragraph(captureLine, valueFont);
                captureLineValue.Alignment = Element.ALIGN_LEFT;
                document.Add(captureLineValue);
                document.Add(new Paragraph(" "));

                Paragraph generationDate = new Paragraph($"Fecha de generación del recibo: {DateTime.Now.ToString("dd/MM/yyyy")}", valueFont);
                generationDate.Alignment = Element.ALIGN_CENTER;
                document.Add(generationDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el PDF: {ex.Message}");
            }
            finally
            {
                document.Close();
            }
        }
    }
}
