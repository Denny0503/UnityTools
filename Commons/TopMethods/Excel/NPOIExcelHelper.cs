using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Data;
using System.IO;
using TopMethods.Extend;
using TopMethods.Logs;

namespace TopMethods.Excel
{
    /// <summary>
    /// NPOI操作Excel帮助类
    /// </summary>
    public static class NPOIExcelHelper
    {
        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool DataTableToExcel(DataTable table, string title, string filePath)
        {
            if (null == table || filePath.IsEmpty())
            {
                return false;
            }

            FileStream fs = null;

            try
            {
                fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                IWorkbook workBook = new HSSFWorkbook();

                ISheet sheet = workBook.CreateSheet("sheet1");

                //处理表格标题
                IRow row = sheet.CreateRow(0);
                row.CreateCell(0).SetCellValue(title);
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, table.Columns.Count - 1));
                row.Height = 800;
                row.Cells[0].CellStyle = GetCellStyle(workBook);

                //处理表格列头
                row = sheet.CreateRow(1);
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(table.Columns[i].ColumnName);
                    row.Height = 500;
                    row.Cells[i].CellStyle = GetCellStyle(workBook, "微软雅黑", 14);
                    sheet.AutoSizeColumn(i);
                }

                //处理数据内容
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    row = sheet.CreateRow(2 + i);
                    row.Height = 350;
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(table.Rows[i][j].ToString());
                        row.Cells[j].CellStyle = GetCellStyle(workBook, "微软雅黑", 12);
                        sheet.SetColumnWidth(j, 6000);
                    }
                }

                //写入数据流
                workBook.Write(fs);
                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog($"{new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}", LogLevelType.Exception, ex);
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return true;
        }

        private static ICellStyle GetCellStyle(IWorkbook workBook, string fontName = "微软雅黑", double fontSize = 17)
        {
            ICellStyle cellStyle = workBook.CreateCellStyle();
            IFont font = workBook.CreateFont();
            font.FontName = fontName;
            font.FontHeightInPoints = fontSize;
            cellStyle.SetFont(font);
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment = HorizontalAlignment.Center;
            return cellStyle;
        }
    }
}
