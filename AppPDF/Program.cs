using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.StyledXmlParser.Node;
using System.Text.RegularExpressions;
using iText.StyledXmlParser.Jsoup.Select;

class Program
{
    static void Main()
    {
        // Obtener la carpeta donde está el ejecutable
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        string patron_fecha = @"\d{2}/\d{2}/\d{4}";

        string[] pdfFiles            = Directory.GetFiles(currentDirectory, "*.pdf");
        string fechaemision          = "FecharecepcióndelaNúmerodemuestra:";
        string procedencia           = "Procedencia:";
        string puntotoma             = "Puntodetoma:";
        string resultadoleg          = "Nodetectadaenelvolumen";
        int texto_fechalengh         = fechaemision.Length;
        int texto_procedencialength  = procedencia.Length;
        int texto_puntotomalength    = puntotoma.Length;
        int texto_resultadoleglength = resultadoleg.Length;

        string nombre_final_pdf      = "";
        string separador             = "_";
        string character_separador   = "(a)";


        if (pdfFiles.Length == 0)
        {
            Console.WriteLine("No se encontraron archivos PDF en la carpeta.");
            return;
        }

        // Procesar cada PDF encontrado
        foreach (string pdfFile in pdfFiles)
        {
            Console.WriteLine($"Leyendo archivo: {Path.GetFileName(pdfFile)}");

            using (PdfReader pdfReader = new PdfReader(pdfFile))
            using (PdfDocument pdfDoc  = new PdfDocument(pdfReader))
            {
                string text = "";
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    text += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));

                }

                text = Regex.Replace(text, @"\s+", "");

                MatchCollection fechasencontradas = Regex.Matches(text, patron_fecha);

                if (fechasencontradas.Count > 0)
                {
                    Console.WriteLine("Fechas encontradas");


                    // Hay que cambiar el formato del string de como está a 
                    // yyyymmdd
                    nombre_final_pdf += fechasencontradas[1].Value + separador;

                    if (text.Contains(procedencia))
                    {
                        int final = -1;
                        final = text.IndexOf(character_separador);
                        if (final != -1)
                        {
                            // Principio de la cadena
                            // Procedencia:""
                            int principio       = text.IndexOf(procedencia) + texto_procedencialength;
                            string subtext      = text.Substring(text.IndexOf(procedencia));

                            MatchCollection matchescharacter = Regex.Matches(subtext, character_separador);
                            Match matches2 = Regex.Match(subtext, character_separador);

                            string procedencias = text.Substring(text.IndexOf(procedencia) + texto_procedencialength, matches2.Index);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Fallo al encontrar la procedencia");
                    }
                }
                else
                {
                    Console.WriteLine("Fallo al encontrar las fechas de emisión\n");
                }


                Console.WriteLine(text);
            }


            nombre_final_pdf = "";
            Console.WriteLine("\n-----------------------------\n");
        }
    }
}
