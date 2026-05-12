using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucPoseCancleFee : UserControl
    {
        public ucPoseCancleFee()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox1.Text.Trim()))
            {
                MessageBox.Show("请选择退费方式");
            }
            if (string.IsNullOrEmpty(txtPZH.Text.Trim()))
            {
                MessageBox.Show("凭证号不能为空");
                return;
            }
            if (string.IsNullOrEmpty(txtMoney.Text.Trim()))
            {
                MessageBox.Show("金额不能为空且必须是数字");
                return;
            }
            if (DialogResult.Cancel == MessageBox.Show("请插入社保卡！", "提示", MessageBoxButtons.OKCancel))
            {
                MessageBox.Show("退费失败！");
                return ;
            }

            #region 读取pos机xml端口
            int comPort = 1;
            string comPortFileName = Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + @"Profiles\ICCardBalanceXML.xml";
            XmlDocument doc = new XmlDocument();
            StreamReader sr = new StreamReader(comPortFileName, System.Text.Encoding.UTF8);
            string cleanDown = sr.ReadToEnd();
            doc.LoadXml(cleanDown);
            sr.Close();

            XmlNode protNode = doc.SelectSingleNode("XML/PORT");
            string protStr = protNode.InnerText;
            int protTemp = 1;
            if (int.TryParse(protStr, out protTemp))
            {
                comPort = protTemp;
            }
            byte[] port = System.BitConverter.GetBytes(comPort);
            #endregion

             bool bPort = SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_SetPort(port[0]);
             if (bPort)
             {
                 if (comboBox1.Text.Trim()=="实体社保卡")
                 {
                     SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_SetTimeOut(60);
                     string para = "<A>12,0,{0},0,0,0,0,0,0,0,{1}</A>";
                     para = string.Format(para, txtPZH.Text.Trim(), txtMoney.Text.Trim());
                     string strBack = SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_Balance(para);
                     // strBack = "12,00,00,00,00,00,00";//测试用
                     string[] backDetails = strBack.Split(',');
                     if (backDetails[1] == "00" && backDetails.Length > 4)
                     {
                         MessageBox.Show("退费成功");
                     }
                     else
                     {
                         MessageBox.Show("账户退费失败！" + "-" + backDetails[3]);
                     }
                 }
                 else
                 {
                      SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_SetTimeOut(60);
                                    string para = "<A>92,{0},{1},0,0,0,0,{2}</A>";
                                    para = string.Format(para, DateTime.Now.ToString("yyyyMMddHHmmss"), txtPZH.Text.Trim(), "-" + txtMoney.Text.Trim());
                                    //para = string.Format(para, infos.JYPZH, infos.LJJYJE);
                                    string strBack = SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_Balance(para);
                                    string[] backDetails = strBack.Split(',');
                                    if (backDetails[1] == "00" && backDetails.Length > 4)
                                    {
                                        MessageBox.Show("退费成功");
                                    }
                                    else
                                    {
                                        MessageBox.Show("账户退费失败！" + "-" + backDetails[3]);
                                    }
                 }

                
             }
        }
    }
}
