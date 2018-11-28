using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PathwayGames.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PathwayGames.Services.Excel
{
    public class ExcelService : IExcelService
    {
        public async Task<string> ExportAsync(Game game)
        {
            return await Task.Run(() => Export(game));
        }
        public string Export(Game game)
        {
            //Fix for https://github.com/OfficeDev/Open-XML-SDK/issues/221
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            //Create a new spreadsheet file - remark will overwrite existing ones with the same name!
            string fullPath = Path.Combine(Path.GetTempPath(), "results.xlsx");
           
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(fullPath, SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "CPT Game Results" };
            sheets.Append(sheet);

            workbookPart.Workbook.Save();
            //
            InsertDataIntoSheet(spreadsheetDocument, "CPT Game Results", game);
            spreadsheetDocument.Close();

            return fullPath;
        }

        private void InsertDataIntoSheet(SpreadsheetDocument spreadsheetDocument, string sheetName, Game game)
        {
            //Fix for https://github.com/OfficeDev/Open-XML-SDK/issues/221
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            // Open the document for editing       
            WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;

            //Set the sheet name on the first sheet
            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            Sheet sheet = sheets.Elements<Sheet>().FirstOrDefault();

            sheet.Name = sheetName;

            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
            // Header
            Row headerRow = sheetData.AppendChild(new Row());
            string[] headers = { "Slide number",
                                 "Slide",
                                 "Response",
                                 "Slide displayed",
                                 "Slide hidden" };
            foreach (string header in headers)
            {
                Cell cell = ConstructCell(header, CellValues.String);
                headerRow.Append(cell);
            }
            // Slides
            int slideIndex = 0;
            int maxResponseCount = 0;
            foreach (Slide slide in game.Slides)
            {
                Row dataRow = sheetData.AppendChild(new Row());
                Cell cell = ConstructCell((slideIndex+1).ToString(), CellValues.String);
                dataRow.Append(cell);
                cell = ConstructCell(slide.SlideType.ToString(), CellValues.String);
                dataRow.Append(cell);
                cell = ConstructCell(slide.ResponseOutcome.ToString(), CellValues.String);
                dataRow.Append(cell);
                cell = ConstructCell(slide.SlideDisplayed.ToString("dd/MM/yyyy HH:mm:ss.fff"), CellValues.String);
                dataRow.Append(cell);
                cell = ConstructCell(slide.SlideHidden.Value.ToString("dd/MM/yyyy HH:mm:ss.fff"), CellValues.String);
                dataRow.Append(cell);
                // Responses
                int responseIndex = 1;
                var responses = game.ButtonPresses.Where(x => x.SlideIndex == slideIndex);
                maxResponseCount = (responses.Count() > maxResponseCount) ? responses.Count() : maxResponseCount;
                foreach (ButtonPress buttonPress in responses)
                {
                    // Add data
                    cell = ConstructCell(buttonPress.ToString(), CellValues.String);
                    dataRow.Append(cell);

                    responseIndex++;
                }

                slideIndex++;
            }
            // Add dynamic headers for response
            for (int i = 1; i <= maxResponseCount; i++)
            {
                // Add header
                Cell cell = ConstructCell($"Press {i} time and coord", CellValues.String);
                headerRow.Append(cell);
            }
            workbookPart.Workbook.Save();       
        }

        private Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
    }
}
