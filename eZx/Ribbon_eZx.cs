﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using eZx_API.Entities;
using eZstd.Miscellaneous;
using eZx.Database;
using eZx.RibbonHandler;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.VisualBasic;
using Application = Microsoft.Office.Interop.Excel.Application;
using Office = Microsoft.Office.Core;

namespace eZx
{
    public partial class Ribbon_eZx
    {
        #region   ---  Declarations & Definitions

        #region   ---  Fields

        /// <summary>
        /// 此Application中所有的数据库工作表
        /// </summary>
        private List<DataSheet> F_DbSheets = new List<DataSheet>();

        #endregion

        #region   ---  Properties

        private DataSheet F_ActiveDataSheet;

        /// <summary> 此Application中的活动数据库。 </summary>
        /// <remarks>如果当前活动的Excel工作表是一个符合格式的数据库工作表，
        /// 则此属性指向此对应的数据库对象，否则，返回Nothing。</remarks>
        public DataSheet ActiveDatabaseSheet
        {
            get { return this.F_ActiveDataSheet; }
            set
            {
                F_ActiveDataSheet = value;
                if (value == null) // 说明此Worksheet不能成功地构成一个数据库格式
                {
                    this.btnEditDatabase.Enabled = false;
                    this.btnConstructDatabase.Enabled = true;
                }
                else // 说明此Worksheet 符合数据库格式
                {
                    this.btnEditDatabase.Enabled = true;
                    this.btnConstructDatabase.Enabled = true;
                }
            }
        }

        #endregion

        #region   ---  Fields

        /// <summary>
        /// 当前正在运行的Excel程序
        /// </summary>
        /// <remarks></remarks>
        private Application _app;

        /// <summary>
        /// 用来临时保存数据的工作簿
        /// </summary>
        /// <remarks>此工作簿用来保存各种临时数据，比如从图表中提取出来的数据情况</remarks>
        private Workbook _tempWkbk;

        /// <summary>
        /// 用来临时保存数据的工作簿的文件路径
        /// </summary>
        /// <remarks>此工作簿位于桌面上的“tempData.xlsx”</remarks>
        private Char path_Tempwkbk;

        // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

        /// <summary> 供各项命令使用的第一个基本参数，此字段值会由TextChange事件而自动修改。 </summary>
        private string Para1;

        /// <summary> 供各项命令使用的第二个基本参数 </summary>
        private string Para2;

        /// <summary> 供各项命令使用的第三个基本参数 </summary>
        private string Para3;

        #endregion

        #endregion

        /// <summary> 构造函数 </summary>
        public void Ribbon_zfy_Load(Object sender, RibbonUIEventArgs e)
        {
            _app = Globals.ThisAddIn.Application;
            _app.SheetActivate += this.ExcelApp_SheetActivate;
            Para1 = EditBox_p1.Text;
            Para2 = EditBox_p2.Text;
            Para3 = EditBox_p3.Text;
        }

        #region   ---  数据库 ---

        /// <summary> 显示工作表中的UsedRange的范围 </summary>
        public void btn_DataRange_Click(object sender, RibbonControlEventArgs e)
        {
            Range rg = _app.ActiveSheet.UsedRange;
            rg.Select();
            // .Value = .Value '这一操作会将单元`格中的公式转化为对应的值，而且，将#DIV/0!、#VALUE!等错误转换为Integer.MinValue
        }

        /// <summary> 显示工作表中的UsedRange的范围 </summary>
        public void ButtonValue_Click(object sender, RibbonControlEventArgs e)
        {
            Range rg = _app.Selection;
            rg.Formula = rg.Formula; //这一操作会将单元格中的公式转化为对应的值，而且，将#DIV/0!、#VALUE!等错误转换为Integer.MinValue
        }

        /// <summary>
        /// 准备构造一个数据库
        /// </summary>
        /// <remarks></remarks>
        public void btnConstructDatabase_Click(Object sender, RibbonControlEventArgs e)
        {
            // -------------------------- 对当前工作表的信息进行处理 --------------------------
            // 此工作表是否曾经是一个数据库
            Worksheet sht = _app.ActiveSheet;
            DataSheet CorrespondingDatasheet = CorrespondingInCollection(sht, this.F_DbSheets);
            try
            {
                if (CorrespondingDatasheet != null)
                {
                    // 说明此工作表是包含在当前的数据库集合中的，它曾经是一个数据库，但是可能在进行修改后，已经不符合数据库规范了。
                    // ------------ 构造数据库 --------------
                    CorrespondingDatasheet = ConstructDatabase(); //将刷新后的数据库更新到集合中的元素中
                    this.ActiveDatabaseSheet = CorrespondingDatasheet;
                }
                else
                {
                    // 说明此工作表并不在数据库集合中，但是它可能是一个数据库。
                    // ------------ 构造数据库 --------------
                    this.ActiveDatabaseSheet = ConstructDatabase();
                    if (this.ActiveDatabaseSheet != null)
                    {
                        this.F_DbSheets.Add(this.ActiveDatabaseSheet);
                    }
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("当前工作表不符合数据库格式。", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ActiveDatabaseSheet = null;
            }
        }

        /// <summary>
        /// 构造数据库
        /// </summary>
        /// <remarks></remarks>
        private DataSheet ConstructDatabase()
        {
            DataSheet dtSheet = default(DataSheet);
            //
            Form_ConstructDatabase frm = new Form_ConstructDatabase(_app.ActiveSheet, true);
            dtSheet = frm.ShowDialog();
            //
            return dtSheet;
        }

        /// <summary>
        /// 准备编辑数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void btnEditDatabase_Click(object sender, RibbonControlEventArgs e)
        {
            DataSheet dtSheet;
            //
            Form_ConstructDatabase frm = new Form_ConstructDatabase(_app.ActiveSheet, false,
                this.ActiveDatabaseSheet);
            dtSheet = frm.ShowDialog();
            //
        }

        /// <summary>
        /// 找出某工作表在数据库集合中所对应的那一项，如果没有对应项，则返回Nothing
        /// </summary>
        /// <param name="DataSheet">要进行匹配的Excel工作表</param>
        /// <param name="DatasheetCollection">要进行搜索的数据库集合。</param>
        private DataSheet CorrespondingInCollection(Worksheet DataSheet, List<DataSheet> DatasheetCollection)
        {
            DataSheet dtSheet = null;
            foreach (DataSheet dbSheet in this.F_DbSheets)
            {
                if (ExcelFunction.SheetCompare(dbSheet.WorkSheet, DataSheet))
                {
                    dtSheet = dbSheet;
                    break;
                }
            }
            return dtSheet;
        }

        #endregion

        #region   ---  图表 ---

        public void btn_XYExchange_Click(object sender, RibbonControlEventArgs e)
        {
            var ch = ChartHandler.GetUniqueInstance(_app.ActiveChart);
            ch.XYExchange();
        }

        /// <summary>
        /// 提取图表中的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void btn_ExtractDataFromChart_Click(object sender, RibbonControlEventArgs e)
        {
            var ch = ChartHandler.GetUniqueInstance(_app.ActiveChart);
            ch.ExtractDataFromChart();
        }

        #endregion

        #region   ---  数据处理 ---

        /// <summary> 进行数据的重新排列 </summary>
        public void btnReArrange_Click(object sender, RibbonControlEventArgs e)
        {
            // ---------------------------- 确定Range的有效范围 ------------------------------------------
            Worksheet sht = _app.ActiveSheet;
            Range rgData = _app.Selection;
            rgData = rgData.Areas[1];
            Range firstCell = default(Range); // 有效区间中的左上角第一个单元
            Range bottomCell = default(Range); // 有效区间中的左下角的那个单元
            Range rbcell = default(Range); // 有效区间中的右下角的那个单元
            int SortedId = 0;
            double interval = 0;
            string[] strInterval_Id = EditBox_ReArrangeIntervalId.Text.Split(',');
            double.TryParse(strInterval_Id[0], out interval);
            int.TryParse(strInterval_Id[1], out SortedId);
            int startRow;
            //
            rbcell = rgData.Ex_CornerCell(CornerIndex.BottomRight);
            bottomCell = rgData.Cells[rgData.Rows.Count, SortedId];
            firstCell = rgData.Cells[1, 1];
            if (bottomCell.Value == null)
            {
                bottomCell = bottomCell.End[XlDirection.xlUp];
            }
            if (firstCell.Value == null)
            {
                firstCell = firstCell.End[XlDirection.xlDown];
            }
            rgData = sht.Range[firstCell, sht.Cells[bottomCell.Row, rbcell.Column]];
            startRow = Convert.ToInt32(rgData.Cells[1, 1].Row);

            // ------------------------------------- 提取参数 -------------------------------------
            Range rgIdColumn = rgData.Columns[SortedId];
            double startData = 0;
            double endData = 0;
            try
            {
                startData = double.Parse(EditBox_ReArrangeStart.Text);
            }
            catch (Exception ex)
            {
                try
                {
                    startData = DateTime.Parse(EditBox_ReArrangeStart.Text).ToOADate();
                }
                catch (Exception)
                {
                    try
                    {
                        startData = Convert.ToDouble(_app.WorksheetFunction.Min(rgIdColumn));
                        EditBox_ReArrangeStart.Text = Convert.ToString(startData);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("指定的数据列中的数据不能进行排序！" + "\r\n" +
                                        ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            try
            {
                endData = double.Parse(EditBox_ReArrangeEnd.Text);
            }
            catch (Exception ex)
            {
                try
                {
                    endData = DateTime.Parse(EditBox_ReArrangeEnd.Text).ToOADate();
                }
                catch (Exception)
                {
                    try
                    {
                        endData = Convert.ToDouble(_app.WorksheetFunction.Max(rgIdColumn));
                        EditBox_ReArrangeEnd.Text = Convert.ToString(endData);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("指定的数据列中的数据不能进行排序！" + "\r\n" +
                                        ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            //
            // 检查参数的正确性
            if (endData <= startData || interval == 0 || interval > endData - startData || SortedId == 0 ||
                SortedId > rgData.Columns.Count)
            {
                MessageBox.Show("指定的参数不正确！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 检查数据的有效性
            object[,] Value = rgData.Value2;

            SortedList<double, int> v_row = new SortedList<double, int>(); //每一个key代表此标志列的实际数据，对应的value代表此数据在指定的区间内的局部行号
            int r = 0;
            try
            {
                object v = null;
                for (r = 1; r <= Value.Length - 1; r++)
                {
                    v = Value[r, SortedId];
                    if ((v != null) && string.Compare("", v.ToString().Trim()) != 0)
                    {
                        v_row.Add((double)v, r);
                    }
                }
            }
            catch (Exception ex)
            {
                Range c = rgData.Cells[r, SortedId];
                MessageBox.Show("单元格 " + c.Address + " 的数据不符合规范，请检查。" + "\r\n" +
                                ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                c.Activate();
                return;
            }

            // ------------------------------------------ 开始重新排列数据 ------------------------------------------
            int RowsCount = 0;
            int ColsCount = rgData.Columns.Count;

            RowsCount = (int)(Math.Floor((endData - startData) / interval) + 1);

            object[,] arrResult = new object[RowsCount - 1 + 1, ColsCount - 1 + 1];
            var arrKey = v_row.Keys;
            var arrValue = v_row.Values;
            int valueRow = 0;
            for (r = 0; r <= RowsCount - 1; r++)
            {
                int sourceR = (int)arrKey.IndexOf(startData + r * interval);
                if (sourceR >= 0)
                {
                    valueRow = Convert.ToInt32(arrValue[sourceR]); // 指定的数据在Excel区间中的行号
                    for (int c = 0; c <= ColsCount - 1; c++)
                    {
                        arrResult[r, c] = Value[valueRow, c + 1];
                    }
                }
            }
            // 将排列完成后的结果放置回Excel单元格中

            Range rgResult = sht.Range[firstCell, firstCell.Offset[RowsCount - 1, ColsCount - 1]];
            rgResult.Value = arrResult;
            rgResult.Columns[SortedId].NumberFormatLocal = rgData.Cells[1, SortedId].NumberFormatLocal; // 还原这一列的数值格式
            rgResult.Select();
        }

        /// <summary>
        /// 消除空行
        /// </summary>
        public void btnShrink_Click(object sender, RibbonControlEventArgs e)
        {
            Range rgData = _app.Selection;
            rgData = rgData.Areas[1];
            int colsCount = rgData.Columns.Count;
            Worksheet sht = rgData.Worksheet;

            // 确定要以选择的区域中的哪一列作为排序列
            int sortedId = 0;
            if (colsCount == 1)
            {
                sortedId = 1;
            }
            else
            {
                if (int.TryParse(Para1, out sortedId))
                {
                    if (sortedId == 0 || sortedId > colsCount)
                    {
                        MessageBox.Show("指定的数据列的值超出选择的区域范围", "出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("参数 P1 不能转为整数值", "出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            //
            Range firstCell = default(Range); // 有效区间中的右下角的那个单元
            Range bottomCell = default(Range);
            Range rbcell = default(Range);
            int startRow;
            rbcell = rgData.Ex_CornerCell(CornerIndex.BottomRight);
            bottomCell = rgData.Cells[rgData.Rows.Count, sortedId];
            firstCell = rgData.Cells[1, 1];
            if (bottomCell.Value == null)
            {
                bottomCell = bottomCell.End[XlDirection.xlUp];
            }
            if (firstCell.Value == null)
            {
                firstCell = firstCell.End[XlDirection.xlDown];
            }
            rgData = sht.Range[firstCell, sht.Cells[bottomCell.Row, rbcell.Column]];
            startRow = Convert.ToInt32(rgData.Cells[1, 1].Row);
            //

            int rowsCount = rgData.Rows.Count;
            object[,] arrData = new object[rowsCount - 1 + 1, colsCount - 1 + 1];
            //
            object[,] Value = rgData.Value;

            //
            object v = null;
            int DataRows = 0; // 当前数据行
            for (int r = 1; r <= rowsCount; r++)
            {
                v = Value[r, sortedId];
                if ((v != null) && string.Compare("", v.ToString().Trim()) != 0)
                {
                    for (int c = 0; c <= colsCount - 1; c++)
                    {
                        arrData[DataRows, c] = Value[r, c + 1];
                    }
                    DataRows++;
                }
            }

            // 将处理完成后的结果放置回Excel单元格中
            Range rgResult = sht.Range[firstCell, firstCell.Offset[DataRows - 1, colsCount - 1]];
            object[,] arrResult = new object[DataRows - 1 + 1, colsCount - 1 + 1]; // 剔除无用的数据，而保留非空行
            for (int r = 0; r <= DataRows - 1; r++)
            {
                for (int c = 0; c <= colsCount - 1; c++)
                {
                    arrResult[r, c] = arrData[r, c];
                }
            }
            rgResult.Value = arrResult;
            rgResult.Select();
        }

        /// <summary> 数据重排 </summary>
        /// <remarks>  请在P1中输入新的行数，P2中输入新的列数。
        /// 在进行重排时，全先将所有的数据排成一列，然后再进行重排。</remarks>
        public void DataReshape(object sender, RibbonControlEventArgs e)
        {
            Range rg = _app.Selection;
            Range startCell = rg.Cells[1, 1];
            object[,] Value = rg.Areas[1].Value;
            //
            UInt32 row = 0;
            UInt32 col = 0;
            bool blnDeleteNull = false;
            try
            {
                row = uint.Parse(Para1);
                col = uint.Parse(Para2);
                blnDeleteNull = (Para3 != null) && (string.Compare(Para3.ToString(), "False", false) != 0);
                if (row == 0 || col == 0)
                {
                    throw new ArgumentOutOfRangeException("Col 或 Row", "行或列的数值不能为零。");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("P1或者P2不能转换为数值");
                return;
            }
            //
            UInt32 ValidDataCount = 0; // 所有数据中，有效的数据的个数
            //将数据由二维表格转换为一维向量，其中只有前面的ValidDataCount个数据是有效的
            object[] arrData = GetDataListFromTable(Value, blnDeleteNull, ref ValidDataCount);
            object[,] NewShape = new object[row - 1 + 1, col - 1 + 1];
            UInt32 RowIndex = 0;
            UInt32 ColIndex = 0;
            for (UInt32 i = 1; i <= ValidDataCount; i++)
            {
                ColIndex = (UInt32)Math.Ceiling((double)i / row);
                RowIndex = i - (ColIndex - 1) * row;
                if (i <= row * col)
                {
                    NewShape[RowIndex - 1, ColIndex - 1] = arrData[i - 1];
                }
                else // 考虑到源表格中的有效数据的个数大于目标表格中的元素个数的情况
                {
                    break;
                }
            }
            // 将重排的数据写入Excel表格中
            Range DataRg = startCell.Resize[row, col];
            DataRg.Value = NewShape;
            DataRg.Select();
        }

        /// <summary>
        /// 将Excel中的二维表格数据转换为一个向量
        /// </summary>
        /// <param name="Table">要进行数据转换的二维表格</param>
        /// <param name="DeleteNull">是否要删除每一列结尾处的多个空数据。</param>
        /// <param name="ValidDataCount">返回的向量中的有效数据的个数，如果DeleteNull的值为False，则其值与二维表格Table中的元素个数相同。</param>
        /// <returns>一个向量，其中的元素个数与Table中的元素个数相同，但是只有 ValidDataCount 个有效数据</returns>
        /// <remarks></remarks>
        private object[] GetDataListFromTable(object[,] Table, bool DeleteNull, ref UInt32 ValidDataCount)
        {
            int Count = Table.Length;
            object[] arrData = new object[Count - 1 + 1];
            UInt32 RowCount = (UInt32)(Table.GetUpperBound(0) - Table.GetLowerBound(0) + 1);
            UInt32 ColCount = (UInt32)(Table.GetUpperBound(1) - Table.GetLowerBound(1) + 1);
            //Information.UBound((System.Array) Table, 2) - Information.LBound((System.Array) Table, 2) + 1;
            //
            if (DeleteNull)
            {
                object v = null;
                UInt32 startIndex = 0; // 对于某一列数据而言，其中第一行的数据在转换后的一维向量中的Index
                UInt32 valueIndex = 0; // 当前要写入的数据在一维向量中的Index
                UInt32 ValidDataCountInCol = 0; // 本列中有效数据的个数
                for (UInt32 col = 1; col <= ColCount; col++)
                {
                    for (UInt32 row = 1; row <= RowCount; row++)
                    {
                        // 一次处理一列数据
                        v = Table[row, col];
                        valueIndex = startIndex + row - 1;
                        arrData[valueIndex] = Table[row, col]; // 先将这一列的所有数据写入向量中
                        if (v != null)
                        {
                            ValidDataCountInCol = row;
                        }
                    }
                    startIndex += ValidDataCountInCol;
                }
                ValidDataCount = startIndex; //
            }
            else
            {
                UInt32 valueIndex = 0;
                for (UInt32 row = 1; row <= RowCount; row++)
                {
                    for (UInt32 col = 1; col <= ColCount; col++)
                    {
                        valueIndex = RowCount * (col - 1) + row;
                        arrData[valueIndex - 1] = Table[row, col];
                    }
                }
                ValidDataCount = RowCount * ColCount;
            }
            //
            return arrData;
        }

        public void ButtonTranspose_Click(object sender, RibbonControlEventArgs e)
        {
            // ---------------------------- 确定Range的有效范围 ------------------------------------------
            Application app = _app;
            Worksheet sht = _app.ActiveSheet;
            Range rgData = _app.Selection;
            //
            var tspValues = new List<object>();
            var tspRg = new List<Range>(); // 用来记录每一个小Area在转置后的范围，用来精确赋值
            Range tspUnionRange = default(Range); //用来记录记录每一个小Area在转置并Union后的范围，用来作界面选择
            foreach (Range rgArea in rgData.Areas)
            {
                // 提取每一个小区域的转置后的数值
                if (rgArea.Cells.Count == 1)
                {
                    tspValues.Add(rgArea.Value);
                }
                else
                {
                    // 如果 Area 中只有一个单元格，则 Transpose 会将这个单元格中的 Nothing 转换为 0.0
                    tspValues.Add(app.WorksheetFunction.Transpose(rgArea));
                }
                //
                Range tRg = rgArea.Ex_Transpose();
                tspRg.Add(tRg);
                //
                if (tspUnionRange == null)
                {
                    tspUnionRange = tRg;
                }
                else
                {
                    // 注意每一次Union并不是一定都会增加一个Area
                    tspUnionRange = app.Union(tspUnionRange, tRg);
                }
            }

            //
            rgData.Clear();

            // 对转置后的区域进行赋值并选中
            for (int i = 0; i <= tspRg.Count - 1; i++)
            {
                tspRg[i].Value = tspValues[i];
            }

            // 对转置后的区域进行赋值并选中
            tspUnionRange.Select();
        }

        /// <summary> 保持Range 区域的左上角不变，对整个区域进行行列转转置 </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private Range Transpose(Range range)
        {
            var sht = range.Worksheet;
            Application app = sht.Application;
            Range transposedRange = default(Range);
            foreach (Range rgArea in range.Areas)
            {
                Range tRg = rgArea.Ex_Transpose();
                if (transposedRange == null)
                {
                    transposedRange = tRg;
                }
                else
                {
                    transposedRange = app.Union(transposedRange, tRg);
                }
            }
            return transposedRange;
        }


        /// <summary> 数据点缩减 </summary>
        private void button_SpeedMode_Click(object sender, RibbonControlEventArgs e)
        {
            Application app = Globals.ThisAddIn.Application;
            FormSpeedModeHandler f = FormSpeedModeHandler.GetUniqueInstance(app);
            f.Show(null);
        }

        /// <summary> 插值算法集 </summary>
        private void button_Interpolations_Click(object sender, RibbonControlEventArgs e)
        {
            Application app = Globals.ThisAddIn.Application;
            FormInterpolations f = FormInterpolations.GetUniqueInstance(app);
            f.Show(null);
        }

        #endregion

        #region   ---  事件处理 ---

        /// <summary>
        ///  激活一个新的工作表
        /// </summary>
        private void ExcelApp_SheetActivate(object sender)
        {
            Worksheet sheet = _app.ActiveSheet;
            this.ActiveDatabaseSheet = CorrespondingInCollection(sheet, this.F_DbSheets);
        }

        public void EditBox_p1_TextChanged(object sender, RibbonControlEventArgs e)
        {
            string strText = EditBox_p1.Text;
            if (string.IsNullOrEmpty(strText))
            {
                Para1 = null;
            }
            else
            {
                Para1 = strText;
            }
        }

        public void EditBox_p2_TextChanged(object sender, RibbonControlEventArgs e)
        {
            string strText = EditBox_p2.Text;
            if (string.IsNullOrEmpty(strText))
            {
                Para2 = null;
            }
            else
            {
                Para2 = strText;
            }
        }

        public void EditBox_p3_TextChanged(object sender, RibbonControlEventArgs e)
        {
            string strText = EditBox_p2.Text;
            if (string.IsNullOrEmpty(strText))
            {
                Para3 = null;
            }
            else
            {
                Para3 = strText;
            }
        }




        #endregion

    }
}