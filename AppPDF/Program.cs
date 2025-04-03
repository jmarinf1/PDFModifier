using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
/*
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Jsoup.Select;
*/
using iTextSharp.text.pdf;

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
        string caracter_separador    = @"\(a\)";
        


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
                text = Regex.Replace(text, @"\s+", "");

                AcroFields campos = pdfReader.AcroFields;

                bool permiteCopiar = (pdfReader.Permissions & PdfWriter.ALLOW_COPY) != 0;
                Console.WriteLine("Permite copiar texto: " + permiteCopiar);


                MatchCollection fechasencontradas = Regex.Matches(text, patron_fecha);
                //Console.WriteLine(text);
                if (fechasencontradas.Count > 0)
                {
                    // Hay que cambiar el formato del string de como está a 
                    // yyyymmdd
                    string formatosalida = "yyMMdd";
                    string formatoentrada = "dd/mm/yyyy";
                    nombre_final_pdf += DateTime.ParseExact(fechasencontradas[1].Value, formatoentrada, CultureInfo.InvariantCulture).ToString(formatosalida);

                    if (text.Contains(procedencia))
                    {
                        int final = -1;
                        final     = text.IndexOf(caracter_separador);
                        if (final != -1)
                        {
                            // Principio de la cadena
                            // Procedencia:""
                            int principio  = text.IndexOf(procedencia) + texto_procedencialength;
                            string subtext = text.Substring(text.IndexOf(procedencia));

                            MatchCollection matchescharacter = Regex.Matches(subtext, caracter_separador);
                            Match matches2 = Regex.Match(subtext, caracter_separador);

                            //Console.WriteLine(subtext);

                            string procedencia_substring = subtext.Substring(texto_procedencialength, matches2.Index);

                            //Console.WriteLine("procedencia substring: " + subtext[matches2.Index +1]);
                                                      

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
            }


            nombre_final_pdf = "";
            Console.WriteLine("\n-----------------------------\n");
        }
    }
}
