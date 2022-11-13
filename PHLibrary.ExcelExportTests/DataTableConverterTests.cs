﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.ExcelExportExcelCreator;
using PHLibrary.Reflection.ArrayValuesToInstance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PHLibrary.ExcelExportExcelCreator.Tests
{
    [TestClass()]
    public class DataTableConverterTests
    {
        public DataTableConverterTests()
        {

        }
        [TestMethod()]
        public void ConvertTestWithDescription()
        {
            var studentList = new List<Student> { new Student { Name = "yf" } };
            DataTableConverter<Student> converter = new DataTableConverter<Student>();
            var dataTable = converter.Convert(studentList, null);
            Assert.AreEqual("姓名", dataTable.Columns[1].ColumnName);
            Assert.AreEqual("Age", dataTable.Columns[0].ColumnName);
        }
        [TestMethod()]
        public void ConvertTestWithDescriptionWithoutOrder()
        {
            var studentList = new List<Student2> { new Student2 { Name = "yf" } };
            DataTableConverter<Student2> converter = new DataTableConverter<Student2>();
            var dataTable = converter.Convert(studentList, null);
            Assert.AreEqual("姓名", dataTable.Columns[0].ColumnName);
            Assert.AreEqual("Age", dataTable.Columns[1].ColumnName);
        }
        
        
         
        

        public class Student
        {
            [PropertyOrder(3)]
            [System.ComponentModel.DisplayName("姓名")]
            public string Name { get; set; }
            [PropertyOrder(1)]
            public int Age { get; set; }
        }
        public class Student2
        {

            [System.ComponentModel.DisplayName("姓名")]
            public string Name { get; set; }

            public int Age { get; set; }
        }

        [TestMethod()]
        public void ConvertToTwoDimentioanlTest()
        {  
            var table=CreateTable();
          
            DataTableConverter<Student> converter = new DataTableConverter<Student>();
            
            var newTable = converter.ConvertToTwoDimentioanl(table, new TwoDimensionalDefine("尺码","SizeGuid","数量"),Sort);

            Assert.AreEqual(6, newTable.Columns.Count);
            Assert.AreEqual("春装001", newTable.Rows[0][0]);
            Assert.AreEqual("红色", newTable.Rows[0][1]);
            Assert.AreEqual("2", newTable.Rows[0][2]);
            Assert.AreEqual("1", newTable.Rows[0][3]);
            Assert.AreEqual("3", newTable.Rows[0][4]);
            Assert.AreEqual("蓝色", newTable.Rows[1][1]);
            Assert.AreEqual("5", newTable.Rows[1][2]);
            Assert.AreEqual("4", newTable.Rows[1][3]);
            Assert.AreEqual("6", newTable.Rows[1][5]);
        }
      
        private System.Data.DataTable CreateTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("品名", typeof(string)));
            dataTable.Columns.Add(new DataColumn("颜色", typeof(string)));
            dataTable.Columns.Add(new DataColumn("尺码", typeof(string)));
            dataTable.Columns.Add(new DataColumn("数量", typeof(int)));
            dataTable.Rows.Add("春装001", "红色", "L", 1);
            dataTable.Rows.Add("春装001", "红色", "M", 2);
            dataTable.Rows.Add("春装001", "红色", "XL", 3);
            dataTable.Rows.Add("春装001", "蓝色", "L", 4);
            dataTable.Rows.Add("春装001", "蓝色", "M", 5);
            dataTable.Rows.Add("春装001", "蓝色", "XXL", 6);
            /*二维表：
            品名，  颜色      L M XL XXL
            春001  红色      1 2 3
            春001  蓝色      4 5     6
           
            */
            return dataTable;
        }
        private IList<string> Sort(IList<TwoDimensionalX> columns) { 
            return new List<string> { "M", "L", "XL", "XXL" };
            }
        private IList<string> Sort(IList<Tuple<string,string>> columnsWithGuid)
        {
            return new List<string> { "M", "L", "XL", "XXL" };
        }
    }
}