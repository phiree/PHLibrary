﻿using PHLibrary.Arithmetic.TreeToRectangle;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using PHLibrary.ExcelExportExcelCreator;

namespace PHLibrary.ExcelExport
{
    /// <summary>
    ///创建表头合并的excel
    /// </summary>
    public class ExcelCreatorEPPlus : IExcelCreator
    {
        public Stream Create<T>(IList<T> data, IDictionary<string, string> propertyNameMaps = null, CellStyleSettings cellStyleSettings = null)

        {
            var dataTable = new DataTableConverter<T>().Convert(data, propertyNameMaps);
            return Create(dataTable);


        }
        public System.IO.Stream Create(DataTable dataTable, CellStyleSettings cellStyleSettings = null)
        {
            return Create(dataTable, CreateColumnTree(dataTable));
        }
        public System.IO.Stream Create(DataSet dataset, ColumnTree columnTree, CellStyleSettings cellStyleSettings = null)
        {
            return Create(FetchFrom(dataset), new List<ColumnTree> { columnTree });
        }
        public System.IO.Stream Create(DataTable dataTable, ColumnTree columnTree, CellStyleSettings cellStyleSettings = null)
        {
            return Create(new List<DataTable> { dataTable }, new List<ColumnTree> { columnTree });
        }
        public Stream Create(DataSet dataToExport, CellStyleSettings cellStyleSettings = null)
        {
            return Create(dataToExport, CreateColumnTrees(dataToExport));
        }
        public Stream Create(DataSet dataToExport, IList<ColumnTree> columnTrees, CellStyleSettings cellStyleSettings = null)
        {
            return Create(FetchFrom(dataToExport), columnTrees);

        }
        private IList<DataTable> FetchFrom(DataSet ds)
        {
            var tables = new List<DataTable>();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                tables.Add(ds.Tables[i]);
            }
            return tables;
        }
        private Stream Create(IList<DataTable> datatables, IList<ColumnTree> columnTrees,CellStyleSettings cellStyleSettings=null)
        {
            if (datatables.Count != columnTrees.Count)
            {
                throw new Exception($"表头数量[{columnTrees.Count}]和表格数量[{datatables.Count}]不一致.");
            }
            using (var excelPackage = new ExcelPackage())
            {
                for (int i = 0; i < datatables.Count; i++)// ar dataTable in dataToExport.Tables)
                {
                    var dataTable = datatables[i];
                    var columnTree = columnTrees[i];
                    var sheet = excelPackage.Workbook.Worksheets.Add(dataTable.TableName);
                    //create merged header cells
                    var headerCreateor = new ExceHeaderCreatorEPPlus(columnTree, sheet);
                    IList<string> columnFormats;
                    int headerHeight = headerCreateor.CreateHeader(out columnFormats);
                    //create body 
                    FillSheetEpplusWithLoadRange(sheet, datatables[i], headerHeight, columnFormats,cellStyleSettings);
                }
                Stream stream = new MemoryStream();
                excelPackage.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }


        private ColumnTree CreateColumnTree(DataTable table)
        {
            ColumnTree columnTree = new ColumnTree();
            var roots = new List<ColumnTreeNode>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];

                roots.Add(new ColumnTreeNode { Title = column.ColumnName });
            }
            columnTree.Roots = roots;
            return columnTree;
        }
        private List<ColumnTree> CreateColumnTrees(DataSet dataSet)
        {
            var columnTrees = new List<ColumnTree>();
            for (int ti = 0; ti < dataSet.Tables.Count; ti++)
            {
                columnTrees.Add(CreateColumnTree(dataSet.Tables[ti]));
            }
            return columnTrees;
        }
        private void FillSheetEpplusWithLoadRange(ExcelWorksheet sheet, DataTable dataTable, int startRow, IList<string> columnFormats)
        {
            FillSheetEpplusWithLoadRange(sheet, dataTable, startRow, columnFormats, null);
        }


        private void FillSheetEpplusWithLoadRange(ExcelWorksheet sheet, DataTable dataTable, int startRow, IList<string> columnFormats, CellStyleSettings cellStyleSettings)
        {

            //A workbook must have at least on cell, so lets add one... 

            int rows = dataTable.Rows.Count;
            int columns = dataTable.Columns.Count;

            //fill data
            var cells = sheet.Cells[startRow + 1, 1, rows, columns];


            cells.LoadFromDataTable(dataTable, false);
            //format
            for (int i = 0; i < columnFormats.Count; i++)// format in columnFormats)
            {
                string format = columnFormats[i];
                if (!string.IsNullOrEmpty(format))
                {

                    var columnCells = sheet.Cells[startRow + 1, i + 1,startRow+ rows, i + 1];
                    //columnCells
                    columnCells.Style.Numberformat.Format = format;

                }
            }
            //style

            var bodyCells = sheet.Cells[startRow + 1, 1, startRow + rows, columns];
            if (cellStyleSettings != null)
            {
                foreach (var cell in bodyCells)
                {
                    cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }
        }


    }

}
