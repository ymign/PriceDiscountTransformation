using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.Components.OutpatientFee.DB;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using Neusoft.HISFC.Components.OutpatientFee.Forms;


namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucBizStatusModify : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        private DbBizStatus _db = new DbBizStatus();

        private List<Neusoft.FrameWork.Models.NeuObject> payList = new List<Neusoft.FrameWork.Models.NeuObject>();

        public ucBizStatusModify()
        {
            InitializeComponent();
            InitSpread();

            payList = this._db.GetComDictionaryForType("PAYMODES", "");
            LoadLogData();

        }

        private void LoadLogData()
        {

            try
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("操作记录加载中...");
                Application.DoEvents();

                var data = this._db.GetLogList();

                var sheet = this.fpSheet_LogInfo;
                sheet.GrayAreaBackColor = Color.White;
                sheet.DefaultStyle.BackColor = Color.White;

                sheet.ColumnCount = 16;
                sheet.RowCount = 0;
                sheet.OperationMode = OperationMode.ReadOnly;
                sheet.Protect = true;

                var textCellType = new TextCellType();
                textCellType.ReadOnly = true;
                fpSpread1.EditModePermanent = false;

                // 统一设置列样式
                for (int c = 0; c < sheet.ColumnCount; c++)
                {
                    sheet.Columns[c].VerticalAlignment = CellVerticalAlignment.Center;
                    sheet.Columns[c].HorizontalAlignment = CellHorizontalAlignment.Center;
                    sheet.Columns[c].CellType = textCellType;
                }

                sheet.ColumnHeader.Cells[0, 0].Text = "序号";
                sheet.Columns[0].Width = 60;
                sheet.Columns[0].Visible = false;

                sheet.ColumnHeader.Cells[0, 1].Text = "业务类型";
                sheet.Columns[1].Width = 80;

                sheet.ColumnHeader.Cells[0, 2].Text = "原始主键";
                sheet.Columns[2].Width = 100;

                sheet.ColumnHeader.Cells[0, 3].Text = "发票号";
                sheet.Columns[3].Width = 120;

                sheet.ColumnHeader.Cells[0, 4].Text = "姓名";
                sheet.Columns[4].Width = 80;

                sheet.ColumnHeader.Cells[0, 5].Text = "门诊号";
                sheet.Columns[5].Width = 100;

                sheet.ColumnHeader.Cells[0, 6].Text = "证件号码";
                sheet.Columns[6].Width = 150;

                sheet.ColumnHeader.Cells[0, 7].Text = "修改类型";
                sheet.Columns[7].Width = 100;

                sheet.ColumnHeader.Cells[0, 8].Text = "修改前";
                sheet.Columns[8].Width = 150;

                sheet.ColumnHeader.Cells[0, 9].Text = "修改后";
                sheet.Columns[9].Width = 150;

                sheet.ColumnHeader.Cells[0, 10].Text = "工号";
                sheet.Columns[10].Width = 80;

                sheet.ColumnHeader.Cells[0, 11].Text = "名称";
                sheet.Columns[11].Width = 80;

                sheet.ColumnHeader.Cells[0, 12].Text = "操作时间";
                sheet.Columns[12].Width = 150;

                sheet.ColumnHeader.Cells[0, 13].Text = "操作IP";
                sheet.Columns[13].Width = 120;

                sheet.ColumnHeader.Cells[0, 14].Text = "备注";
                sheet.Columns[14].Width = 120;

                sheet.ColumnHeader.Cells[0, 15].Text = "理由";
                sheet.Columns[15].Width = 120;

                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    sheet.Rows.Add(i, 1);
                    sheet.Rows[i].Tag = item;
                    sheet.Cells[i, 0].Text = item.LogId.ToString();

                    sheet.Cells[i, 1].Text = item.BizType;
                    sheet.Cells[i, 2].Text = item.OriginPk;
                    sheet.Cells[i, 3].Text = item.InvoiceNo;
                    sheet.Cells[i, 4].Text = item.PatientName;
                    sheet.Cells[i, 5].Text = item.OutpatientId;
                    sheet.Cells[i, 6].Text = item.IdCard;

                    if (item.ItemType == "0")
                    {
                        sheet.Cells[i, 7].Text = "就诊状态";
                        sheet.Cells[i, 8].Text = item.OldValue == "0" ? "未看诊" : "已看诊";
                        sheet.Cells[i, 9].Text = item.NewValue == "0" ? "未看诊" : "已看诊";
                    }
                    else
                    {
                        sheet.Cells[i, 7].Text = "支付方式";
                        var oldPayInfo = payList.FirstOrDefault(f => f.ID == item.OldValue);
                        var oldPayName = item.OldValue;
                        if (oldPayInfo != null)
                        {
                            oldPayName = oldPayInfo.Name;
                        }

                        var newPayInfo = payList.FirstOrDefault(f => f.ID == item.NewValue);
                        var newPayName = item.NewValue;
                        if (newPayInfo != null)
                        {
                            newPayName = newPayInfo.Name;
                        }
                        sheet.Cells[i, 8].Text = oldPayName;
                        sheet.Cells[i, 9].Text = newPayName;
                    }
                    sheet.Cells[i, 10].Text = item.OperCode;
                    sheet.Cells[i, 11].Text = item.OperName;
                    sheet.Cells[i, 12].Text = item.OperDate.ToString("yyyy-MM-dd HH:mm:ss");
                    sheet.Cells[i, 13].Text = item.OperIp;
                    sheet.Cells[i, 14].Text = item.Remark;
                    sheet.Cells[i, 15].Text = item.ChangeReason;


                }


            }
            catch (Exception ex)
            {

            }
            finally
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        private void InitSpread()
        {
            var sheet = this.fpSheet_BizInfo;
            this.fpSpread1.BackColor = Color.White;
            sheet.GrayAreaBackColor = Color.White;
            sheet.DefaultStyle.BackColor = Color.White;

            sheet.ColumnCount = Enum.GetValues(typeof(BizStatusColumn)).Length;
            sheet.RowCount = 0;
            sheet.OperationMode = OperationMode.ReadOnly;
            sheet.Protect = true;

            var textCellType = new TextCellType();
            textCellType.ReadOnly = true;
            fpSpread1.EditModePermanent = false;

            // 统一设置列样式
            for (int c = 0; c < sheet.ColumnCount; c++)
            {
                sheet.Columns[c].VerticalAlignment = CellVerticalAlignment.Center;
                sheet.Columns[c].HorizontalAlignment = CellHorizontalAlignment.Center;
                sheet.Columns[c].CellType = textCellType;
            }

            // 设置列宽和列头
            SetColumnHeaders(sheet);
        }

        /// <summary>
        /// 获取列信息配置
        /// </summary>
        private ColumnInfo[] GetColumnInfo()
        {
            ColumnInfo[] columns = new ColumnInfo[]
    {
        new ColumnInfo { HeaderText = "业务类型", Width = 60, DataField = "biz_type" },
        new ColumnInfo { HeaderText = "业务状态", Width = 60, DataField = "biz_state", Formatter = this.FormatBizState },
        new ColumnInfo { HeaderText = "姓名", Width = 60, DataField = "name" },
        new ColumnInfo { HeaderText = "门诊号", Width = 80, DataField = "card_no" },
        new ColumnInfo { HeaderText = "证件号码", Width = 120, DataField = "idenno" },
        new ColumnInfo { HeaderText = "发票号", Width = 100, DataField = "invoice_no" },
        new ColumnInfo { HeaderText = "交易类型", Width = 60, DataField = "trans_type", Formatter = this.FormatTransType },
        new ColumnInfo { HeaderText = "门诊流水号", Width = 80, DataField = "clinic_no" },
        new ColumnInfo { HeaderText = "总金额", Width = 60, DataField = "tot_cost" },
        new ColumnInfo { HeaderText = "自费金额", Width = 60, DataField = "own_cost" },
        new ColumnInfo { HeaderText = "报销金额", Width = 60, DataField = "pub_cost" },
        new ColumnInfo { HeaderText = "优惠金额", Width = 60, DataField = "pay_cost" },
        new ColumnInfo { HeaderText = "支付方式", Width = 120, DataField = "pay_name" },
        new ColumnInfo { HeaderText = "操作人", Width = 100, DataField = "oper_name" },
        new ColumnInfo { HeaderText = "操作时间", Width = 150, DataField = "oper_date" },
        new ColumnInfo { HeaderText = "操作", Width = 100, DataField = "", IsAction = true }
    };
            return columns;
        }

        /// <summary>
        /// 设置列头和列宽
        /// </summary>
        private void SetColumnHeaders(SheetView sheet)
        {
            ColumnInfo[] columns = GetColumnInfo();
            for (int i = 0; i < columns.Length; i++)
            {
                sheet.Columns[i].Width = columns[i].Width;
                sheet.ColumnHeader.Cells[0, i].Text = columns[i].HeaderText;
            }
        }

        protected override int OnQuery(object sender, object neuObject)
        {
            try
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("查询中...");
                Application.DoEvents();

                var invoiceNo = this.txtInvoiceNo.Text.Trim();
                if (string.IsNullOrEmpty(invoiceNo))
                {
                    MessageBox.Show("发票号不允许为空！");
                    return -1;
                }

                var dt = _db.GetBizDataTable(invoiceNo);
                if (dt == null)
                {
                    MessageBox.Show(_db.Err);
                    return -1;
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("发票号[" + invoiceNo + "]暂未查询到任何数据!");
                    return -1;
                }

                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("加载中...");
                BindDataToFP(dt);

                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询数据出现异常：" + ex.Message);
                return -1;
            }
            finally
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }
        }

        /// <summary>
        /// 绑定数据到表格
        /// </summary>
        private void BindDataToFP(DataTable dt)
        {
            this.fpSheet_BizInfo.RowCount = 0;
            ColumnInfo[] columnInfo = GetColumnInfo();

            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                this.fpSheet_BizInfo.Rows.Add(rowIndex, 1);
                this.fpSheet_BizInfo.Rows[rowIndex].Tag = dt.Rows[rowIndex];

                DataRow dataRow = dt.Rows[rowIndex];

                for (int col = 0; col < columnInfo.Length; col++)
                {
                    ColumnInfo colInfo = columnInfo[col];

                    if (colInfo.IsAction)
                    {
                        // 操作列特殊处理
                        this.fpSheet_BizInfo.Cells[rowIndex, col].Text = "修改";
                        this.fpSheet_BizInfo.Cells[rowIndex, col].ForeColor = Color.FromArgb(106, 27, 154);
                    }
                    else if (colInfo.Formatter != null)
                    {
                        // 有格式化函数的列
                        string rawValue = dataRow[colInfo.DataField].ToString();
                        this.fpSheet_BizInfo.Cells[rowIndex, col].Text = colInfo.Formatter(rawValue);
                    }
                    else
                    {
                        // 普通列直接绑定
                        this.fpSheet_BizInfo.Cells[rowIndex, col].Text = dataRow[colInfo.DataField].ToString();
                    }
                }
            }
        }

        #region 格式化函数

        /// <summary>
        /// 格式化业务状态
        /// </summary>
        private string FormatBizState(string value)
        {
            int state;
            if (int.TryParse(value, out state))
            {
                BizState bizState = (BizState)state;
                switch (bizState)
                {
                    case BizState.NotSeen:
                        return "未看诊";
                    case BizState.Seen:
                        return "已看诊";
                    default:
                        return "未知状态";
                }
            }
            return "未知状态";
        }

        /// <summary>
        /// 格式化交易类型
        /// </summary>
        private string FormatTransType(string value)
        {
            int type;
            if (int.TryParse(value, out type))
            {
                TransType transType = (TransType)type;
                return transType == TransType.Positive ? "正交易" : "负交易";
            }
            return "负交易";
        }

        #endregion

        /// <summary>
        /// FP单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellClick(object sender, CellClickEventArgs e)
        {
            int rowIndex = e.Row;
            int columnIndex = e.Column;

            // 只处理操作列的点击
            if (columnIndex != (int)BizStatusColumn.Action)
            {
                return;
            }

            // 检查行索引是否有效
            if (rowIndex < 0 || rowIndex >= this.fpSheet_BizInfo.RowCount)
            {
                return;
            }

            // 从行Tag中获取数据
            var row = this.fpSheet_BizInfo.Rows[rowIndex].Tag as DataRow;
            if (row == null)
            {
                MessageBox.Show("无法获取行数据，无法进行修改操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 获取业务类型
            var bizType = row["biz_type"].ToString();
            var clinicNo = row["clinic_no"].ToString();
            var invoiceNo = row["invoice_no"].ToString();

            var f = new frmBizStatusModify();
            f.DataRow = row;
            f.LoadData();
            f.ShowDialog();

            if (f.DialogResult != DialogResult.OK)
            {
                return;
            }

            var listMntBizAdjustLog = f.listMntBizAdjustLog;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            foreach (var item in listMntBizAdjustLog)
            {
                var insertResult = this._db.InsertBizLog(item);
                if (!insertResult)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入数据失败:" + this._db.Err);

                    return;
                }

                if (item.BizType == "挂号")
                {
                    if (item.ItemType == "1")
                    {
                        var updateResult = this._db.UpdateRegPayType(item.OriginPk, item.InvoiceNo, row["trans_type"].ToString(), item.NewValue, item.OldValue);
                        if (!updateResult)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("更新数据异常:" + this._db.Err);
                            return;
                        }

                    }

                    if (item.ItemType == "0")
                    {
                        var updateResult = this._db.UpdateRegSeeFlag(item.OriginPk, item.NewValue);
                        if (!updateResult)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("更新数据异常:" + this._db.Err);
                            return;
                        }
                    }

                }

                if (item.BizType == "缴费")
                {
                    var updateResult = this._db.UpdateMZPayType(item.InvoiceNo, row["trans_type"].ToString(), item.NewValue, item.OldValue);
                    if (!updateResult)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("更新数据异常:" + this._db.Err);
                        return;
                    }


                }

            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            OnQuery(null, null);
        }
    }

    /// <summary>
    /// 业务状态表格列枚举
    /// </summary>
    public enum BizStatusColumn
    {
        /// <summary>业务类型</summary>
        BizType = 0,
        /// <summary>业务状态</summary>
        BizState = 1,
        /// <summary>姓名</summary>
        Name = 2,
        /// <summary>门诊号</summary>
        CardNo = 3,
        /// <summary>证件号码</summary>
        IdCardNo = 4,
        /// <summary>发票号</summary>
        InvoiceNo = 5,
        /// <summary>交易类型</summary>
        TransType = 6,
        /// <summary>门诊流水号</summary>
        ClinicNo = 7,
        /// <summary>总金额</summary>
        TotalAmount = 8,
        /// <summary>自费金额</summary>
        OwnAmount = 9,
        /// <summary>报销金额</summary>
        PublicAmount = 10,
        /// <summary>优惠金额</summary>
        DiscountAmount = 11,
        /// <summary>支付方式</summary>
        PayMethod = 12,
        /// <summary>操作人</summary>
        Operator = 13,
        /// <summary>操作时间</summary>
        OperTime = 14,
        /// <summary>操作</summary>
        Action = 15
    }

    /// <summary>
    /// 业务状态枚举
    /// </summary>
    public enum BizState
    {
        /// <summary>未看诊</summary>
        NotSeen = 0,
        /// <summary>已看诊</summary>
        Seen = 1,
        /// <summary>未知状态</summary>
        Unknown = 99
    }

    /// <summary>
    /// 交易类型枚举
    /// </summary>
    public enum TransType
    {
        /// <summary>负交易（冲销）</summary>
        Negative = 0,
        /// <summary>正交易</summary>
        Positive = 1
    }

    /// <summary>
    /// 列信息配置类
    /// </summary>
    internal class ColumnInfo
    {
        /// <summary>列头文字</summary>
        public string HeaderText { get; set; }
        /// <summary>列宽</summary>
        public int Width { get; set; }
        /// <summary>数据字段名</summary>
        public string DataField { get; set; }
        /// <summary>格式化函数</summary>
        public Func<string, string> Formatter { get; set; }
        /// <summary>是否为操作列</summary>
        public bool IsAction { get; set; }
    }


}
