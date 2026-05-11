using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    public partial class ucPanelStatus : UserControl
    {
        private Color lineColor = Color.Silver;
        private int lineThickness = 1;

        private string qtyValue = "1";
        private string qtyText = "数量描述";

        // 分别设置左、上、右、下
        private Padding paddingLocation = new Padding(0, 3, 0, 0);

        public Padding PaddingLocation
        {
            get { return paddingLocation; }
            set
            {
                if (paddingLocation == value) return;
                paddingLocation = value;
                if (this.panelBig.Padding != null)
                    this.panelBig.Padding = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("外观")]
        [Description("数量Value")]
        [DefaultValue("1")]
        public string QtyValue
        {
            get { return qtyValue; }
            set
            {
                if (qtyValue == value) return;
                qtyValue = value;
                if (this.lblValue != null)
                    this.lblValue.Text = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("外观")]
        [Description("数量Text")]
        [DefaultValue("数量描述")]
        public string QtyText
        {
            get { return qtyText; }
            set
            {
                if (qtyText == value) return;
                qtyText = value;
                if (this.lblText != null)
                    this.lblText.Text = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("外观")]
        [Description("线条颜色")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color LineColor
        {
            get { return lineColor; }
            set
            {
                if (lineColor == value) return;
                lineColor = value;
                if (this.panelBig != null)
                    this.panelBig.BackColor = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("外观")]
        [Description("线条粗细（像素）")]
        [DefaultValue(1)]
        public int LineThickness
        {
            get { return lineThickness; }
            set
            {
                int v = (value < 1) ? 1 : value;
                if (lineThickness == v) return;
                lineThickness = v;
                if (this.panelBig != null)
                    this.panelBig.Height = lineThickness; // 竖线则改为 Width
                this.Invalidate();
            }
        }

        public ucPanelStatus()
        {
            InitializeComponent();

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.panelBig, 16);
            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.panelSmall, 16);



            // 初始同步，设计器/运行时都能看到默认效果
            if (this.panelBig != null)
            {
                this.panelBig.BackColor = this.lineColor;
                this.panelBig.Height = this.lineThickness; // 竖线则改为 Width
            }
        }
    }
}
