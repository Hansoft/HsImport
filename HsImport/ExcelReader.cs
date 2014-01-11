using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using OfficeOpenXml;


namespace HsImport
{
    class ExcelReader
    {
   public class Cell
        {
            private string value;

            public Cell(string value)
            {
                this.value = value;
            }

            public string Value
            { 
                get { return value; }
            }
        }

        public class Row
        {
            private List<Cell> cells;

            public Row()
            {
                cells = new List<Cell>();
            }

            public List<Cell> Cells
            {
                get { return cells; }
            }

            public Cell AddCell(string value)
            {
                Cell cell = new Cell(value);
                cells.Add(cell);
                return cell;
            }
        }

        private List<Row> rows;

        public ExcelReader(string fileName)
        {
            rows = new List<Row>();
            LoadExcelFile(fileName);
        }

        public List<Row> Rows
        {
            get { return rows; }
        }

        public string[] ColumnHeaders
        {
            get
            {
                string[] chs = new string[rows[0].Cells.Count];
                for (int ind=0; ind < chs.Length; ind +=1)
                    chs[ind] = rows[0].Cells[ind].Value;
                return chs;
            }
        }

        public Row AddRow()
        {
            Row row = new Row();
            rows.Add(row);
            return row;
        }

        private void LoadExcelFile(string fileName)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            ExcelPackage excelPackage = new ExcelPackage(fileInfo);
            ExcelWorksheet sheet1 = excelPackage.Workbook.Worksheets[1];
            ExcelAddressBase activeRange = sheet1.Dimension;
            if (activeRange == null)
                throw new ArgumentException("The file " + fileName + " does not contain any data.");
            for (int iRow = activeRange.Start.Row; iRow <= activeRange.End.Row; iRow += 1)
            {
                Row row = AddRow();
                for (int iCol = activeRange.Start.Column; iCol <= activeRange.End.Column; iCol += 1)
                {
                    row.AddCell(sheet1.GetValue<string>(iRow, iCol));
                }
            }
        }
    }
}

