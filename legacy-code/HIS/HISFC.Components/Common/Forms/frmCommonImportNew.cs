using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using FarPoint.Win.Spread;

namespace Neusoft.HISFC.Components.Common.Forms
{
    /// <summary>
    /// 数据导入窗体 - 支持列映射功能
    /// 兼容 .NET Framework 3.5
    /// </summary>
    public partial class frmCommonImportNew : Form
    {
        private DialogResult res = DialogResult.Cancel;

        /// <summary>
        /// 返回值
        /// </summary>
        public DialogResult Res
        {
            get { return res; }
        }

        public string TemplateFilePath { get; set; }
        public string FilePath { get; set; }

        /// <summary>
        /// 原始导入数据
        /// </summary>
        public DataTable DtImport = new DataTable();

        /// <summary>
        /// 映射后的结果数据（仅包含映射的列，按映射字段命名）
        /// </summary>
        public DataTable DtMappedResult = new DataTable();

        /// <summary>
        /// 需要映射的字段配置
        /// Key: 字段标识（如 setlId）
        /// Value: 字段显示名称（如 结算ID）
        /// </summary>
        private Dictionary<string, string> _requiredFields = new Dictionary<string, string>();

        /// <summary>
        /// 列映射关系
        /// Key: 字段标识
        /// Value: Excel中的列名
        /// </summary>
        private Dictionary<string, string> _columnMapping = new Dictionary<string, string>();

        /// <summary>
        /// 映射下拉框集合
        /// </summary>
        private Dictionary<string, ComboBox> _mappingCombos = new Dictionary<string, ComboBox>();

        // 医疗行业配色
        private Color medicalBlue = Color.FromArgb(24, 144, 255);
        private Color medicalBlueLight = Color.FromArgb(230, 244, 255);
        private Color healthGreen = Color.FromArgb(82, 196, 26);
        private Color healthGreenLight = Color.FromArgb(237, 248, 237);
        private Color warningOrange = Color.FromArgb(255, 251, 235);
        private Color dangerRed = Color.FromArgb(220, 53, 69);

        public frmCommonImportNew()
        {
            InitializeComponent();
            InitializeEventHandlers();
            InitializeUI();
            SetWebStyleGridLine(this.fpSpread1_Sheet1);
        }

        public void ChangeBtnConfirmText(string btnText) 
        {
            this.btnConfirm.Text = btnText;
        }

        /// <summary>
        /// 设置需要映射的字段
        /// </summary>
        /// <param name="fields">字段字典：Key=字段标识, Value=显示名称</param>
        public void SetRequiredFields(Dictionary<string, string> fields)
        {
            _requiredFields = fields ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// 添加需要映射的字段
        /// </summary>
        /// <param name="fieldKey">字段标识（如 setlId）</param>
        /// <param name="displayName">显示名称（如 结算ID）</param>
        public void AddRequiredField(string fieldKey, string displayName)
        {
            if (!_requiredFields.ContainsKey(fieldKey))
            {
                _requiredFields.Add(fieldKey, displayName);
            }
        }

        /// <summary>
        /// 获取映射后的值列表
        /// </summary>
        /// <param name="fieldKey">字段标识</param>
        /// <returns>该字段对应列的所有值</returns>
        public List<string> GetMappedValues(string fieldKey)
        {
            List<string> values = new List<string>();

            if (DtMappedResult != null && DtMappedResult.Columns.Contains(fieldKey))
            {
                foreach (DataRow row in DtMappedResult.Rows)
                {
                    string val = row[fieldKey] != null ? row[fieldKey].ToString() : "";
                    if (!string.IsNullOrEmpty(val))
                    {
                        values.Add(val);
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// 获取映射关系
        /// </summary>
        public Dictionary<string, string> GetColumnMapping()
        {
            return new Dictionary<string, string>(_columnMapping);
        }

        private void InitializeEventHandlers()
        {
            // 关闭按钮
            this.btnClose.Click += (s, e) => this.Close();

            // 底部按钮
            this.btnCancel.Click += (s, e) => this.Close();
            this.btnConfirm.Click += btnConfirm_Click;

            // 上传区点击
            this.pnlUpload.Click += (s, e) => SelectFile();
            this.picUpload.Click += (s, e) => SelectFile();
            this.lblUploadTitle.Click += (s, e) => SelectFile();
            this.lblUploadHint.Click += (s, e) => SelectFile();

            // 移除文件
            this.btnRemove.Click += (s, e) => ClearFile();

            // 拖放支持
            this.pnlUpload.AllowDrop = true;
            this.pnlUpload.DragEnter += PnlUpload_DragEnter;
            this.pnlUpload.DragDrop += PnlUpload_DragDrop;
            this.pnlUpload.DragLeave += PnlUpload_DragLeave;

            // 确认按钮悬停
            this.btnConfirm.MouseEnter += (s, e) => { this.btnConfirm.BackColor = Color.FromArgb(9, 109, 217); };
            this.btnConfirm.MouseLeave += (s, e) => { this.btnConfirm.BackColor = medicalBlue; };

            // 上传区悬停
            SetupUploadHover();
        }

        private void InitializeUI()
        {
            // 绘制上传区虚线边框
            this.pnlUpload.Paint += PnlUpload_Paint;

            // 文件卡片边框
            this.pnlFileCard.Paint += PnlFileCard_Paint;

            // 映射区边框
            this.pnlMapping.Paint += PnlMapping_Paint;

            // 表格容器边框
            this.pnlTable.Paint += PnlTable_Paint;

            // 底部边框
            this.pnlFooter.Paint += PnlFooter_Paint;

            // 绘制图标
            DrawUploadIcon();
            DrawFileIcon();

            // 初始化统计
            UpdateStats(0, 0);
        }

        private void PnlUpload_Paint(object sender, PaintEventArgs e)
        {
            using (Pen dashedPen = new Pen(medicalBlue, 2))
            {
                dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                dashedPen.DashPattern = new float[] { 6, 4 };
                e.Graphics.DrawRectangle(dashedPen, 1, 1,
                    pnlUpload.ClientSize.Width - 3,
                    pnlUpload.ClientSize.Height - 3);
            }
        }

        private void PnlFileCard_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(healthGreen))
            {
                e.Graphics.DrawRectangle(pen, 0, 0,
                    pnlFileCard.ClientSize.Width - 1,
                    pnlFileCard.ClientSize.Height - 1);
            }
        }

        private void PnlMapping_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(251, 191, 36)))
            {
                e.Graphics.DrawRectangle(pen, 0, 0,
                    pnlMapping.ClientSize.Width - 1,
                    pnlMapping.ClientSize.Height - 1);
            }
        }

        private void PnlTable_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(217, 217, 217)))
            {
                e.Graphics.DrawRectangle(pen, 0, 0,
                    pnlTable.ClientSize.Width - 1,
                    pnlTable.ClientSize.Height - 1);
            }
        }

        private void PnlFooter_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(230, 230, 230)))
            {
                e.Graphics.DrawLine(pen, 0, 0, pnlFooter.Width, 0);
            }
        }

        private void DrawUploadIcon()
        {
            Bitmap bmp = new Bitmap(50, 46);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.FromArgb(240, 248, 255));

                using (Pen pen = new Pen(medicalBlue, 2.5f))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                    g.DrawLine(pen, 25, 38, 25, 12);
                    g.DrawLine(pen, 25, 12, 12, 24);
                    g.DrawLine(pen, 25, 12, 38, 24);
                }
            }
            this.picUpload.Image = bmp;
        }

        private void DrawFileIcon()
        {
            Bitmap bmp = new Bitmap(30, 30);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(healthGreen);

                using (Pen pen = new Pen(Color.White, 2))
                {
                    g.DrawRectangle(pen, 7, 4, 14, 20);
                    g.DrawLine(pen, 10, 12, 18, 12);
                    g.DrawLine(pen, 10, 17, 18, 17);
                }
            }
            this.picFile.Image = bmp;
        }

        private void SetupUploadHover()
        {
            this.pnlUpload.MouseEnter += (s, e) => { this.pnlUpload.BackColor = medicalBlueLight; };
            this.pnlUpload.MouseLeave += (s, e) => { this.pnlUpload.BackColor = Color.FromArgb(240, 248, 255); };
            this.picUpload.MouseEnter += (s, e) => { this.pnlUpload.BackColor = medicalBlueLight; };
            this.picUpload.MouseLeave += (s, e) => { this.pnlUpload.BackColor = Color.FromArgb(240, 248, 255); };
            this.lblUploadTitle.MouseEnter += (s, e) => { this.pnlUpload.BackColor = medicalBlueLight; };
            this.lblUploadTitle.MouseLeave += (s, e) => { this.pnlUpload.BackColor = Color.FromArgb(240, 248, 255); };
            this.lblUploadHint.MouseEnter += (s, e) => { this.pnlUpload.BackColor = medicalBlueLight; };
            this.lblUploadHint.MouseLeave += (s, e) => { this.pnlUpload.BackColor = Color.FromArgb(240, 248, 255); };
        }

        private void PnlUpload_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsValidFileExtension(files[0]))
                {
                    e.Effect = DragDropEffects.Copy;
                    this.pnlUpload.BackColor = medicalBlueLight;
                    this.lblUploadTitle.Text = "松开鼠标上传文件";
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void PnlUpload_DragLeave(object sender, EventArgs e)
        {
            this.pnlUpload.BackColor = Color.FromArgb(240, 248, 255);
            this.lblUploadTitle.Text = "点击选择文件，或拖拽文件至此区域";
        }

        private void PnlUpload_DragDrop(object sender, DragEventArgs e)
        {
            PnlUpload_DragLeave(sender, e);
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && IsValidFileExtension(files[0]))
            {
                ProcessFile(files[0]);
            }
        }

        private bool IsValidFileExtension(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext == ".xlsx" || ext == ".xls" || ext == ".csv";
        }

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel 文件|*.xlsx;*.xls|CSV 文件|*.csv|所有支持格式|*.xlsx;*.xls;*.csv";
            openFileDialog.Title = "选择要导入的文件";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ProcessFile(openFileDialog.FileName);
            }
        }

        private void ProcessFile(string filePath)
        {
            this.FilePath = filePath;

            FileInfo fileInfo = new FileInfo(filePath);
            this.lblFileName.Text = fileInfo.Name;
            this.lblFileSize.Text = FormatFileSize(fileInfo.Length);

            // 显示文件卡片
            this.pnlFileCard.Visible = true;

            // 更新上传区提示
            this.lblUploadTitle.Text = "已选择文件，点击可重新选择";
            this.lblUploadTitle.ForeColor = healthGreen;

            // 解析数据
            ParseFileData(filePath);

            // 如果有需要映射的字段，显示映射区域
            if (_requiredFields.Count > 0 && DtImport.Columns.Count > 0)
            {
                BuildMappingUI();
                AdjustLayoutWithMapping();
            }
            else
            {
                AdjustLayoutWithoutMapping();
            }
        }

        /// <summary>
        /// 构建映射UI - 使用整齐的网格布局
        /// </summary>
        private void BuildMappingUI()
        {
            this.pnlMappingFields.Controls.Clear();
            _mappingCombos.Clear();

            // Excel列名列表（排除序号列）
            List<string> columnNames = new List<string>();
            columnNames.Add("-- 请选择 --");
            foreach (DataColumn col in DtImport.Columns)
            {
                if (col.ColumnName != "序号")
                {
                    columnNames.Add(col.ColumnName);
                }
            }

            // 布局参数
            int fieldCount = _requiredFields.Count;
            int colsPerRow = 3;  // 每行3个字段
            int rowCount = (int)Math.Ceiling(fieldCount / (double)colsPerRow);
            int fieldWidth = 310;  // 每个字段宽度
            int fieldHeight = 34;  // 每个字段高度
            int labelWidth = 80;   // 标签宽度
            int comboWidth = 220;  // 下拉框宽度
            int startX = 4;
            int startY = 4;

            // 为每个需要的字段创建映射控件
            int index = 0;
            foreach (KeyValuePair<string, string> field in _requiredFields)
            {
                int row = index / colsPerRow;
                int col = index % colsPerRow;

                int x = startX + col * fieldWidth;
                int y = startY + row * fieldHeight;

                // 字段标签
                Label lbl = new Label();
                lbl.Text = field.Value;
                lbl.Font = new Font("微软雅黑", 9.5F);
                lbl.ForeColor = Color.FromArgb(68, 68, 68);
                lbl.AutoSize = false;
                lbl.Size = new Size(labelWidth, 24);
                lbl.Location = new Point(x, y + 4);
                lbl.TextAlign = ContentAlignment.MiddleRight;
                this.pnlMappingFields.Controls.Add(lbl);

                // 下拉框
                ComboBox cmb = new ComboBox();
                cmb.Name = "cmb_" + field.Key;
                cmb.Font = new Font("微软雅黑", 9.5F);
                cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                cmb.Size = new Size(comboWidth, 24);
                cmb.Location = new Point(x + labelWidth + 4, y + 2);
                cmb.Items.AddRange(columnNames.ToArray());
                cmb.SelectedIndex = 0;

                // 尝试自动匹配
                AutoMatchColumn(cmb, field.Value, field.Key, columnNames);

                this.pnlMappingFields.Controls.Add(cmb);
                _mappingCombos.Add(field.Key, cmb);

                index++;
            }

            // 调整映射区域高度
            int fieldsHeight = rowCount * fieldHeight + 10;
            this.pnlMappingFields.Height = fieldsHeight;
            this.pnlMapping.Height = 40 + fieldsHeight;

            this.pnlMapping.Visible = true;
        }

        /// <summary>
        /// 自动匹配列名
        /// </summary>
        private void AutoMatchColumn(ComboBox cmb, string displayName, string fieldKey, List<string> columnNames)
        {
            // 尝试精确匹配（显示名或字段标识）
            for (int i = 1; i < columnNames.Count; i++)
            {
                string colName = columnNames[i];
                if (colName.Equals(displayName, StringComparison.OrdinalIgnoreCase) ||
                    colName.Equals(fieldKey, StringComparison.OrdinalIgnoreCase))
                {
                    cmb.SelectedIndex = i;
                    return;
                }
            }

            // 尝试包含匹配
            for (int i = 1; i < columnNames.Count; i++)
            {
                string colName = columnNames[i].ToLower();
                string dispLower = displayName.ToLower();
                string keyLower = fieldKey.ToLower();

                if (colName.Contains(dispLower) || dispLower.Contains(colName) ||
                    colName.Contains(keyLower) || keyLower.Contains(colName))
                {
                    cmb.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// 有映射区域时调整布局
        /// </summary>
        private void AdjustLayoutWithMapping()
        {
            this.pnlFileCard.Top = 146;
            this.pnlMapping.Top = 202;
            this.pnlMapping.Visible = true;

            int mappingBottom = this.pnlMapping.Top + this.pnlMapping.Height + 8;
            this.pnlDataHeader.Top = mappingBottom;
            this.pnlTable.Top = mappingBottom + 40;
            this.pnlTable.Height = this.pnlFooter.Top - this.pnlTable.Top - 8;
        }

        /// <summary>
        /// 无映射区域时调整布局
        /// </summary>
        private void AdjustLayoutWithoutMapping()
        {
            this.pnlFileCard.Top = 146;
            this.pnlMapping.Visible = false;
            this.pnlDataHeader.Top = 204;
            this.pnlTable.Top = 244;
            this.pnlTable.Height = this.pnlFooter.Top - 244 - 8;
        }

        private void ClearFile()
        {
            this.FilePath = null;
            this.DtImport = new DataTable();
            this.DtMappedResult = new DataTable();
            _columnMapping.Clear();

            // 隐藏文件卡片和映射区，恢复布局
            this.pnlFileCard.Visible = false;
            this.pnlMapping.Visible = false;
            this.pnlMappingFields.Controls.Clear();
            _mappingCombos.Clear();

            this.pnlDataHeader.Top = 150;
            this.pnlTable.Top = 190;
            this.pnlTable.Height = this.pnlFooter.Top - 190 - 8;

            // 恢复上传区
            this.lblUploadTitle.Text = "点击选择文件，或拖拽文件至此区域";
            this.lblUploadTitle.ForeColor = medicalBlue;

            // 清空表格
            this.fpSpread1_Sheet1.DataSource = null;
            this.fpSpread1_Sheet1.RowCount = 0;
            this.fpSpread1_Sheet1.ColumnCount = 0;

            UpdateStats(0, 0);
        }

        private void ParseFileData(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在解析文件...", false);
                Application.DoEvents();

                string ext = Path.GetExtension(filePath).ToLower();

                if (ext == ".csv")
                {
                    dataTable = ParseCsvFile(filePath);
                }
                else if (ext == ".xlsx" || ext == ".xls")
                {
                    dataTable = ParseExcelFile(filePath);
                }
                else
                {
                    throw new Exception("不支持的文件格式");
                }

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("文件为空或无法解析！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 添加序号列（在最前面）
                DataColumn rowNumCol = new DataColumn("序号", typeof(int));
                dataTable.Columns.Add(rowNumCol);
                rowNumCol.SetOrdinal(0);  // 移动到第一列

                // 填充序号
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataTable.Rows[i]["序号"] = i + 1;
                }

                // 绑定数据到表格
                this.fpSpread1_Sheet1.DataSource = dataTable;

                // 设置表头
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    this.fpSpread1_Sheet1.ColumnHeader.Cells[0, i].Text = dataTable.Columns[i].ColumnName;
                }

                // 自适应列宽
                AutoFitColumnWidths(dataTable);

                UpdateStats(dataTable.Rows.Count, dataTable.Columns.Count - 1);  // 减去序号列
                this.DtImport = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件解析失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }
        }

        private DataTable ParseExcelFile(string filePath)
        {
            DataTable dt = new DataTable();
            string ext = Path.GetExtension(filePath).ToLower();

            string connStr;
            if (ext == ".xlsx")
            {
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath +
                         ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";
            }
            else
            {
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath +
                         ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            }

            OleDbConnection conn = null;
            try
            {
                conn = new OleDbConnection(connStr);
                conn.Open();

                DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (schemaTable == null || schemaTable.Rows.Count == 0)
                {
                    throw new Exception("无法读取 Excel 工作表");
                }

                string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString();

                string sql = "SELECT * FROM [" + sheetName + "]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                if (ext == ".xlsx" && ex.Message.Contains("ACE"))
                {
                    throw new Exception("需要安装 Microsoft Access Database Engine 才能读取 .xlsx 文件。\n\n" +
                                      "或者将文件另存为 .xls 格式后重试。");
                }
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return dt;
        }

        private DataTable ParseCsvFile(string filePath)
        {
            DataTable dt = new DataTable();

            using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
            {
                string line;
                bool isFirstLine = true;

                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line.Trim()))
                        continue;

                    string[] values = ParseCsvLine(line);

                    if (isFirstLine)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            string colName = string.IsNullOrEmpty(values[i]) ? "列" + (i + 1) : values[i];
                            dt.Columns.Add(colName);
                        }
                        isFirstLine = false;
                    }
                    else
                    {
                        DataRow row = dt.NewRow();
                        for (int i = 0; i < Math.Min(values.Length, dt.Columns.Count); i++)
                        {
                            row[i] = values[i];
                        }
                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }

        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            StringBuilder current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Length = 0;
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString().Trim());
            return result.ToArray();
        }

        /// <summary>
        /// 自适应列宽 - 根据表头和数据内容调整列宽
        /// </summary>
        private void AutoFitColumnWidths(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Columns.Count == 0)
                return;

            int minWidth = 60;   // 最小列宽
            int maxWidth = 280;  // 最大列宽
            int padding = 20;    // 额外边距

            using (Graphics g = this.fpSpread1.CreateGraphics())
            {
                Font headerFont = new Font("微软雅黑", 10F, FontStyle.Bold);
                Font dataFont = new Font("微软雅黑", 10F);

                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    float maxContentWidth = 0;

                    // 计算表头宽度
                    string headerText = dataTable.Columns[col].ColumnName;
                    SizeF headerSize = g.MeasureString(headerText, headerFont);
                    maxContentWidth = headerSize.Width;

                    // 计算前20行数据的最大宽度
                    int rowsToCheck = Math.Min(dataTable.Rows.Count, 20);
                    for (int row = 0; row < rowsToCheck; row++)
                    {
                        object cellValue = dataTable.Rows[row][col];
                        string cellText = cellValue != null ? cellValue.ToString() : "";

                        if (!string.IsNullOrEmpty(cellText))
                        {
                            SizeF cellSize = g.MeasureString(cellText, dataFont);
                            if (cellSize.Width > maxContentWidth)
                            {
                                maxContentWidth = cellSize.Width;
                            }
                        }
                    }

                    // 计算最终列宽（加上边距，限制在最小和最大之间）
                    int columnWidth = (int)(maxContentWidth + padding);
                    columnWidth = Math.Max(minWidth, Math.Min(maxWidth, columnWidth));

                    // 设置列宽
                    this.fpSpread1_Sheet1.Columns[col].Width = columnWidth;
                }
            }
        }

        private void UpdateStats(int rowCount, int colCount)
        {
            this.lblRowCount.Text = "行数: " + rowCount;
            this.lblColCount.Text = "列数: " + colCount;
        }

        private string FormatFileSize(long size)
        {
            if (size < 1024)
                return size + " bytes";
            else if (size < 1024 * 1024)
                return (size / 1024.0).ToString("F2") + " KB";
            else if (size < 1024 * 1024 * 1024)
                return (size / (1024.0 * 1024)).ToString("F2") + " MB";
            else
                return (size / (1024.0 * 1024 * 1024)).ToString("F2") + " GB";
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (DtImport == null || DtImport.Rows.Count <= 0)
            {
                MessageBox.Show("请先选择并导入文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 如果有映射字段，验证并生成映射结果
            if (_requiredFields.Count > 0)
            {
                if (!ValidateMapping())
                {
                    return;
                }

                BuildMappedResult();
            }

            res = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 验证映射配置
        /// </summary>
        private bool ValidateMapping()
        {
            _columnMapping.Clear();
            List<string> unmapped = new List<string>();

            foreach (KeyValuePair<string, string> field in _requiredFields)
            {
                if (_mappingCombos.ContainsKey(field.Key))
                {
                    ComboBox cmb = _mappingCombos[field.Key];
                    if (cmb.SelectedIndex <= 0)
                    {
                        unmapped.Add(field.Value);
                    }
                    else
                    {
                        _columnMapping.Add(field.Key, cmb.SelectedItem.ToString());
                    }
                }
            }

            if (unmapped.Count > 0)
            {
                MessageBox.Show("以下字段未配置映射关系：\n\n" + string.Join("、", unmapped.ToArray()),
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SetWebStyleGridLine(SheetView fp)
        {
            // 去掉表格垂直线
            fp.VerticalGridLine = new GridLine(GridLineType.None);
            // 设置表格浅灰色水平线      
            fp.HorizontalGridLine = new GridLine(GridLineType.Flat, Color.FromArgb(153, 241, 243, 244));

            //去掉列标题的水平线
            fp.ColumnHeader.HorizontalGridLine = new GridLine(GridLineType.None);
        }

        /// <summary>
        /// 根据映射关系生成结果数据
        /// </summary>
        private void BuildMappedResult()
        {
            DtMappedResult = new DataTable();

            // 创建结果列（使用字段标识作为列名）
            foreach (string fieldKey in _columnMapping.Keys)
            {
                DtMappedResult.Columns.Add(fieldKey);
            }

            // 填充数据
            foreach (DataRow sourceRow in DtImport.Rows)
            {
                DataRow newRow = DtMappedResult.NewRow();

                foreach (KeyValuePair<string, string> mapping in _columnMapping)
                {
                    string fieldKey = mapping.Key;
                    string sourceColumn = mapping.Value;

                    if (DtImport.Columns.Contains(sourceColumn))
                    {
                        newRow[fieldKey] = sourceRow[sourceColumn];
                    }
                }

                DtMappedResult.Rows.Add(newRow);
            }
        }
    }
}
