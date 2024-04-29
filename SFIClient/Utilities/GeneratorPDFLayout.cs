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

                document.Add(new Paragraph($"Nombre del cliente: {client}"));
                document.Add(new Paragraph($"Folio del crédito: {creditInvoice}"));
                document.Add(new Paragraph($"Fecha planeada de pago: {plannedDate}"));
                document.Add(new Paragraph($"Importe de pago: {amount:C}"));
                document.Add(new Paragraph($"Línea de captura: {captureLine}"));
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
