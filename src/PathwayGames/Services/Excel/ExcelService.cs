using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PathwayGames.Models;
using System;
using System.IO;

namespace PathwayGames.Services.Excel
{
    public class ExcelService : IExcelService
    {
        public string Export(Game game)
        {
            //Fix for https://github.com/OfficeDev/Open-XML-SDK/issues/221
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            //Create a new spreadsheet file - remark will overwrite existing ones with the same name!
            string fullPath = Path.Combine(Path.GetTempPath(), "temp.xlsx");

            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(fullPath, SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
            sheets.Append(sheet);

            workbookPart.Workbook.Save();

            spreadsheetDocument.Close();

            return fullPath;
        }
    }
}
