using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Neusoft.HISFC.Components.Common.Forms
{
    public partial class frmCommonImport : Form
    {
        private DialogResult res = DialogResult.Cancel;
        // <summary>
        /// 返回值
        /// </summary>
        public DialogResult Res
        {
            get { return res; }
            //set { res = value; }
        }

        public string TemplateFilePath { get; set; }

        public string FilePath { get; set; }

        public DataTable DtImport = new DataTable();

        public frmCommonImport()
        {
            InitializeComponent();
            InitializeCustomUI();
            //this.btnSelectFile.Click += btnSelectFile_Click;
            this.btnClose.Click += btnClose_Click;
        }

        private void InitializeCustomUI()
        {
            this.panelFileUploadBox.Paint += (sender, e) =>
            {
                // 绘制虚线边框
                using (Pen dashedPen = new Pen(Color.FromArgb(0, 123, 255)) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(dashedPen, 0, 0, panelFileUploadBox.ClientSize.Width - 1, panelFileUploadBox.ClientSize.Height - 1);
                }
            };

            // 点击上传框时打开文件对话框
            panelFileUploadBox.Click += (sender, e) => OpenFileDialog(panelFileUploadBox);

            // 允许拖放
            panelFileUploadBox.DragEnter += (sender, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            };

            panelFileUploadBox.DragDrop += (sender, e) =>
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    MessageBox.Show("文件已上传!");
                    return;
                }
            };

        }

        // 打开文件对话框
        private void OpenFileDialog(Panel fileUploadBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel 文件|*.xlsx|CSV 文件|*.csv",
                Title = "选择上传的文件"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = openFileDialog.FileName;
                //FileInfo fileInfo = new FileInfo(FilePath);
                this.lblFilePath.Text = FilePath;
                this.lblFilePath.ForeColor = Color.FromArgb(30, 159, 255); ;
                // 确保标签居中
                CenterLabel(fileUploadBox, this.lblFilePath);
            }
        }

        private void CenterLabel(Panel fileUploadBox, Label label)
        {
            // 计算 X 轴居中位置，保持 Y 轴不变
            label.Location = new Point(
                (fileUploadBox.Width - label.Width) / 2, // 水平居中
                label.Location.Y // 保持原有 Y 坐标不变
            );
        }


        private void label4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog(this.panelFileUploadBox);
        }

        public void ImportExcelToDataGridView(string filePath)
        {
            var dataTable = new DataTable();
            try
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("解析中...", false);
                Application.DoEvents();

                // 获取文件信息
                FileInfo fileInfo = new FileInfo(filePath);
                string fileName = fileInfo.Name;
                long fileSize = fileInfo.Length;
                string fileExtension = fileInfo.Extension;

                string fileSizeFormatted = FormatFileSize(fileSize);


                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0);
                    IRow headerRow = sheet.GetRow(0);
                    for (int col = 0; col < headerRow.LastCellNum; col++)
                    {
                        if (headerRow.GetCell(col) == null)
                        {
                            dataTable.Columns.Add("未知列名[" + col.ToString() + "]");
                        }
                        else
                        {
                            dataTable.Columns.Add(headerRow.GetCell(col).ToString());
                        }

                    }

                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        IRow dataRow = sheet.GetRow(row);
                        DataRow dataTableRow = dataTable.NewRow();

                        for (int col = 0; col < dataRow.LastCellNum; col++)
                        {

                            if (dataRow.GetCell(col) == null)
                            {
                                dataTableRow[col] = string.Empty;
                            }
                            else
                            {
                                dataTableRow[col] = dataRow.GetCell(col).ToString();
                            }


                        }

                        dataTable.Rows.Add(dataTableRow);
                    }



                }
                this.fpSpread1_Sheet1.DataSource = dataTable;
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    this.fpSpread1_Sheet1.ColumnHeader.Cells[0, i].Text = dataTable.Columns[i].ColumnName;
                }

                //文件信息：xxx行   xxx列   格式：xlsx
                this.lblFileMsg.Text = "文件名称：" + fileName + "   大小：" + fileSizeFormatted + "   " + dataTable.Rows.Count + "行   " + dataTable.Columns.Count + "列   格式：" + fileExtension;

                DtImport = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("解析数据出现异常:" + ex.Message);
                return;
            }
            finally
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }
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

        private void btnParseData_Click(object sender, EventArgs e)
        {
            ImportExcelToDataGridView(FilePath);
        }

        private void btnDownloadTemplate_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(TemplateFilePath))
            {
                MessageBox.Show("暂未配置模板，无法下载！");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = "template.xlsx", // 默认文件名
                Filter = "Excel文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string savePath = saveFileDialog.FileName;

                try
                {

                    if (File.Exists(TemplateFilePath))
                    {
                        File.Copy(TemplateFilePath, savePath);
                        MessageBox.Show("模板下载成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("模板文件不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("下载失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (DtImport == null || DtImport.Rows.Count <= 0)
            {
                MessageBox.Show("数据源为空,无法提交！");
                return;
            }

            res = DialogResult.OK;
            this.Close();
        }


    }
}
