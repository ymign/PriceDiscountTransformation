namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class tvICDCategory
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (this.hsAllICD != null)
                {
                    this.hsAllICD.Clear();
                    this.hsAllICD = null;
                }

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(tvICDCategory));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // groupImageList
            // 
            this.groupImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("groupImageList.ImageStream")));
            this.groupImageList.Images.SetKeyName(0, "group1.ICO");
            this.groupImageList.Images.SetKeyName(1, "group2.ICO");
            this.groupImageList.Images.SetKeyName(2, "group3.ico");
            this.groupImageList.Images.SetKeyName(3, "group4.ico");
            this.groupImageList.Images.SetKeyName(4, "group5.ico");
            // 
            // deptImageList
            // 
            this.deptImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("deptImageList.ImageStream")));
            this.deptImageList.Images.SetKeyName(0, "b1.ico");
            this.deptImageList.Images.SetKeyName(1, "r1.ico");
            this.deptImageList.Images.SetKeyName(2, "dir_close.bmp");
            this.deptImageList.Images.SetKeyName(3, "dir_open.bmp");
            this.deptImageList.Images.SetKeyName(4, "G2 Folder Grey.ico");
            this.deptImageList.Images.SetKeyName(5, "G2 Folder Blue.ico");
            this.deptImageList.Images.SetKeyName(6, "access.cpl#110_7.ico");
            this.deptImageList.Images.SetKeyName(7, "ie4power.ico");
            this.deptImageList.Images.SetKeyName(8, "doctor.ico");
            this.deptImageList.Images.SetKeyName(9, "doctor_zr.ico");
            this.deptImageList.Images.SetKeyName(10, "121.GIF");
            this.deptImageList.Images.SetKeyName(11, "安排.ico");
            this.deptImageList.Images.SetKeyName(12, "导入.ico");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "b1.ico");
            this.imageList1.Images.SetKeyName(1, "r1.ico");
            this.imageList1.Images.SetKeyName(2, "dir_close.bmp");
            this.imageList1.Images.SetKeyName(3, "dir_open.bmp");
            this.imageList1.Images.SetKeyName(4, "G2 Folder Grey.ico");
            this.imageList1.Images.SetKeyName(5, "G2 Folder Blue.ico");
            this.imageList1.Images.SetKeyName(6, "ie4.0buf.ico");
            this.imageList1.Images.SetKeyName(7, "ie4power.ico");
            this.imageList1.Images.SetKeyName(8, "doctor.ico");
            this.imageList1.Images.SetKeyName(9, "doctor_zr.ico");
            this.imageList1.Images.SetKeyName(10, "121.GIF");
            this.imageList1.Images.SetKeyName(11, "安排.ico");
            this.imageList1.Images.SetKeyName(12, "导入.ico");
            // 
            // tvICDCategory
            // 
            this.AllowDrop = true;
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.ImageList imageList1;

    }
}
