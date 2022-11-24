﻿using PHLibrary.Arithmetic.TreeToRectangle;
using PHLibrary.ExcelExportExcelCreator;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace PHLibrary.ExcelExport
{
    /// <summary>
    /// 导出dataset
    /// </summary>
    public interface IExcelCreator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="sortSize"></param>
        /// <param name="summaryData">顶部表格二维数据</param>
        /// <param name="summaryTableBottomMargin">顶部表格和数据表格间隔行数</param>
        /// <param name="cellStyleSettings"></param>
        /// <param name="amountFormat">金额单位F0,F1,F2,F3</param>
        /// <param name="needExportImage">是否导出图片</param>
        /// <returns></returns>
        Stream Create<T1>( SheetData<T1> sheetData, SortSize sortSize, string amountFormat);
        Stream Create<T1,T2>(SheetData<T1,T2> sheetData, SortSize sortSize, string amountFormat);
        Stream Create<T1,T2,T3>(SheetData<T1,T2,T3> sheetData, SortSize sortSize, string amountFormat);

    }
}
