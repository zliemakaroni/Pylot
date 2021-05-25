using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ClosedXML.Excel;
using System.Drawing;

namespace Pylot
{
    class ExcelWorker
    {
        public void SaveResults(string path, ResultsWorker rw)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Протокол");
                worksheet.Cell(1, 1).Value = "№ Протокола";
                worksheet.Cell(1, 2).Value = "Дата обследования";
                worksheet.Cell(1, 3).Value = "Фамилия Имя Отчество";
                worksheet.Cell(1, 4).Value = "Полных лет";

                worksheet.Cell(1, 5).Value = "(I) ПФО";
                worksheet.Cell(2, 6).Value = "Фазы, с";
                worksheet.Cell(3, 5).Value = "Стратегия";
                worksheet.Cell(3, 6).Value = "Восстановления";
                worksheet.Cell(3, 7).Value = "Стабилизации";
                worksheet.Cell(3, 8).Value = "Распада";

                worksheet.Cell(1, 9).Value = "(II) ППО";
                worksheet.Cell(2, 9).Value = "Время ППО, с";
                worksheet.Cell(2, 11).Value = "Фазы, с";
                worksheet.Cell(3, 10).Value = "Стратегия";
                worksheet.Cell(3, 11).Value = "Восстановления";
                worksheet.Cell(3, 12).Value = "Стабилизации";
                worksheet.Cell(3, 13).Value = "Распада";

                worksheet.Cell(1, 14).Value = "(III) КП";
                worksheet.Cell(3, 14).Value = "Стратегия";
                worksheet.Cell(1, 15).Value = "Коп";
                worksheet.Cell(1, 16).Value = "Заключение " + Environment.NewLine + "(уровень прогнозирования" + Environment.NewLine + " вероятностных событий)";


                worksheet.Cell(4, 1).Value = Results.Number;
                worksheet.Cell(4, 2).Value = Results.Date.ToString();
                worksheet.Cell(4, 3).Value = Results.Name;
                worksheet.Cell(4, 4).Value = Results.Age;
                worksheet.Cell(4, 5).Value = rw.StratBefore40Res();
                worksheet.Cell(4, 6).Value = rw.VPhaseTimeB40;
                worksheet.Cell(4, 7).Value = rw.SPhaseTimeB40;
                worksheet.Cell(4, 8).Value = rw.RPhaseTimeB40;
                worksheet.Cell(4, 9).Value = Math.Round(Results.datas.Last().x - Results.TimeToDissapeare);
                worksheet.Cell(4, 10).Value = rw.StratAfter40Res();
                worksheet.Cell(4, 11).Value = rw.VPhaseTimeA40;
                worksheet.Cell(4, 12).Value = rw.SPhaseTimeA40;
                worksheet.Cell(4, 13).Value = rw.RPhaseTimeA40;
                worksheet.Cell(4, 14).Value = rw.Strat3540Res();
                worksheet.Cell(4, 15).Value = rw.RAM;
                worksheet.Cell(4, 16).Value = rw.FinalMark();

                worksheet.Range(1, 1, 3, 16).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(1, 1, 3, 16).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(1, 1, 4, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(1, 1, 4, 16).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var worksheet2 = workbook.Worksheets.Add("Координаты");

                worksheet2.Cell(1, 1).Value = "X, с";
                worksheet2.Cell(2, 1).Value = "Y метки, мм";
                worksheet2.Cell(3, 1).Value = "Y визира, мм";
                int i = 2;
                foreach (var data in Results.datas)
                {
                    worksheet2.Cell(1, i).Value = Math.Round(data.x, 1);
                    worksheet2.Cell(2, i).Value = -Math.Round((data.yt - Results.halfHeight) * 0.26458333, 2);
                    worksheet2.Cell(3, i).Value = -Math.Round((data.yp - Results.halfHeight) * 0.26458333, 2);
                    i++;
                }

                var worksheet3 = workbook.Worksheets.Add("Графики");
                worksheet3.AddPicture(Environment.CurrentDirectory + "\\Temp\\ScreenShot.png").MoveTo(worksheet3.Cell(1, 1));
                worksheet.Columns().AdjustToContents();
                worksheet2.Columns().AdjustToContents();
                worksheet3.Columns().AdjustToContents();
                worksheet.Range("A1:A3").Merge();
                worksheet.Range("B1:B3").Merge();
                worksheet.Range("C1:C3").Merge();
                worksheet.Range("D1:D3").Merge();
                worksheet.Range("E1:H1").Merge();
                worksheet.Range("F2:H2").Merge();
                worksheet.Range("A1:A3").Merge();
                worksheet.Range("I1:M1").Merge();
                worksheet.Range("I2:I3").Merge();
                worksheet.Range("K2:M2").Merge();
                worksheet.Range("O1:O3").Merge();
                worksheet.Range("P1:P3").Merge();
                workbook.SaveAs(path + "\\№" + Results.Number + " " + Results.Name + ".xlsx");                
            }
        }
        
        public void SaveInMarks(string path, ResultsWorker rw)
        {
            
            if (!File.Exists(path))
            {
                using (var workbook = new XLWorkbook())
                {               
                    var worksheet = workbook.Worksheets.Add("Лист1");
                    worksheet.Cell(1, 1).Value = "№ Протокола";
                    worksheet.Cell(1, 2).Value = "Дата обследования";
                    worksheet.Cell(1, 3).Value = "Фамилия Имя Отчество";
                    worksheet.Cell(1, 4).Value = "Полных лет";

                    worksheet.Cell(1, 5).Value = "(I) ПФО";
                    worksheet.Cell(2, 6).Value = "Фазы, с";
                    worksheet.Cell(3, 5).Value = "Стратегия";
                    worksheet.Cell(3, 6).Value = "Восстановления";
                    worksheet.Cell(3, 7).Value = "Стабилизации";
                    worksheet.Cell(3, 8).Value = "Распада";

                    worksheet.Cell(1, 9).Value = "(II) ППО";
                    worksheet.Cell(2, 9).Value = "Время ППО, с";
                    worksheet.Cell(2, 11).Value = "Фазы, с";
                    worksheet.Cell(3, 10).Value = "Стратегия";
                    worksheet.Cell(3, 11).Value = "Восстановления";
                    worksheet.Cell(3, 12).Value = "Стабилизации";
                    worksheet.Cell(3, 13).Value = "Распада";

                    worksheet.Cell(1, 14).Value = "(III) КП";
                    worksheet.Cell(3, 14).Value = "Стратегия";
                    worksheet.Cell(1, 15).Value = "Коп";
                    worksheet.Cell(1, 16).Value = "Заключение " + Environment.NewLine + "(уровень прогнозирования" + Environment.NewLine + " вероятностных событий)";


                    worksheet.Cell(4, 1).Value = Results.Number;
                    worksheet.Cell(4, 2).Value = Results.Date.ToString();
                    worksheet.Cell(4, 3).Value = Results.Name;
                    worksheet.Cell(4, 4).Value = Results.Age;
                    worksheet.Cell(4, 5).Value = rw.StratBefore40Res();
                    worksheet.Cell(4, 6).Value = rw.VPhaseTimeB40;
                    worksheet.Cell(4, 7).Value = rw.SPhaseTimeB40;
                    worksheet.Cell(4, 8).Value = rw.RPhaseTimeB40;
                    worksheet.Cell(4, 9).Value = Math.Round(Results.datas.Last().x - Results.TimeToDissapeare);
                    worksheet.Cell(4, 10).Value = rw.StratAfter40Res();
                    worksheet.Cell(4, 11).Value = rw.VPhaseTimeA40;
                    worksheet.Cell(4, 12).Value = rw.SPhaseTimeA40;
                    worksheet.Cell(4, 13).Value = rw.RPhaseTimeA40;
                    worksheet.Cell(4, 14).Value = rw.Strat3540Res();
                    worksheet.Cell(4, 15).Value = rw.RAM;
                    worksheet.Cell(4, 16).Value = rw.FinalMark();

                    worksheet.Range(1, 1, 3, 16).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(1, 1, 3, 16).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(1, 1, 4, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(1, 1, 4, 16).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Columns().AdjustToContents();
                    worksheet.Range("A1:A3").Merge();
                    worksheet.Range("B1:B3").Merge();
                    worksheet.Range("C1:C3").Merge();
                    worksheet.Range("D1:D3").Merge();
                    worksheet.Range("E1:H1").Merge();
                    worksheet.Range("F2:H2").Merge();
                    worksheet.Range("A1:A3").Merge();
                    worksheet.Range("I1:M1").Merge();
                    worksheet.Range("I2:I3").Merge();
                    worksheet.Range("K2:M2").Merge();
                    worksheet.Range("O1:O3").Merge();
                    worksheet.Range("P1:P3").Merge();
                    workbook.SaveAs(path);
                }
            }
            else
            {
                using (var workbook = new XLWorkbook(path))
                {
                    var worksheet = workbook.Worksheets.First();
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber() + 1, 1).Value = Results.Number;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 2).Value = Results.Date.ToString();
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 3).Value = Results.Name;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 4).Value = Results.Age;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 5).Value = rw.StratBefore40Res();
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 6).Value = rw.VPhaseTimeB40;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 7).Value = rw.SPhaseTimeB40;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 8).Value = rw.RPhaseTimeB40;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 9).Value = Math.Round(Results.datas.Last().x - Results.TimeToDissapeare);
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 10).Value = rw.StratAfter40Res();
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 11).Value = rw.VPhaseTimeA40;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 12).Value = rw.SPhaseTimeA40;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 13).Value = rw.RPhaseTimeA40;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 14).Value = rw.Strat3540Res();
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 15).Value = rw.RAM;
                    worksheet.Cell(worksheet.LastRowUsed().RowNumber(), 16).Value = rw.FinalMark();

                    worksheet.LastRowUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.LastRowUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    workbook.Save();
                }
            }
        }
    }
}
