using System;

namespace UnityMethods.Excel
{
    public class ExcelColumnAttribute : Attribute
    {
        public string ExcelColumn { get; set; }

        public ExcelColumnAttribute(string column)
        {
            ExcelColumn = column;
        }
    }
}
