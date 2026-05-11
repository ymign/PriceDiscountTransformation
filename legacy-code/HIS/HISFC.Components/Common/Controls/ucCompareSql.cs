using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;

namespace Neusoft.HISFC.Components.Common.Controls
{
    /// <summary>
    /// com_sql比较
    /// </summary>
    public partial class ucCompareSql : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucCompareSql()
        {
            InitializeComponent(); 
            
            this.Load += new EventHandler(frmCompareSql_Load);
            this.fpSpread1.LeaveCell += new FarPoint.Win.Spread.LeaveCellEventHandler(fpSpread1_LeaveCell);
            this.fpSpread2.LeaveCell += new FarPoint.Win.Spread.LeaveCellEventHandler(fpSpread2_LeaveCell);
        }
        
        DataSet ds = new DataSet();
        private void frmCompareSql_Load(object sender, EventArgs e)
        {
            string sql = "select id ,name from com_sql order by id ";

            Neusoft.HISFC.BizLogic.Registration.Register r = new Neusoft.HISFC.BizLogic.Registration.Register();
            r.ExecQuery(sql, ref  ds);


            this.fpSpread1_Sheet1.DataSource = ds;

            //for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            //{
            //    string name = this.fpSpread1_Sheet1.GetText(i, 1);
            //    //				name = name.Replace("\r"," ") ;
            //    //				name = name.Replace("\t"," ") ;

            //    this.fpSpread1_Sheet1.SetValue(i, 1, name, false);
            //}
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.InitialDirectory = Application.StartupPath;
            dlgOpen.Multiselect = false;
            dlgOpen.Title = "导入xml文件";
            dlgOpen.RestoreDirectory = true;
            dlgOpen.AddExtension = true;
            dlgOpen.DefaultExt = ".xml";
            dlgOpen.Filter = "xml files (*.xml)|*.xml";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                string XmlFileName = dlgOpen.FileName;

                if (this.LoadSqlFromXml(XmlFileName) == -1) return;

                this.Compare();
            }
        }

        private ArrayList alSql = new ArrayList();

        /// <summary>
        /// 从Xml加载Sql到AlSql
        /// </summary>
        /// <param name="XmlFileName"></param>
        /// <returns></returns>
        private int LoadSqlFromXml(string XmlFileName)
        {
            //ArrayList清空
            this.alSql.Clear();

            //打开文件
            System.Xml.XmlDataDocument doc = new System.Xml.XmlDataDocument();
            try
            {
                doc.Load(XmlFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }

            //将每个Sql节点的数据放到ArrayList
            XmlNodeList nodes;
            nodes = doc.SelectNodes(@"//SQL");
            try
            {
                foreach (XmlNode node in nodes)
                {
                    Neusoft.HISFC.Models.Base.Item objValue = new Neusoft.HISFC.Models.Base.Item();

                    objValue.ID = node.Attributes[0].Value.ToString();
                    string strSql = node.InnerText;

                    //strSql = strSql.Replace("\r", " ");
                    //strSql = strSql.Replace("\t", " ");
                    objValue.Name = strSql;

                    //modual
                    if (node.ParentNode == null || node.ParentNode.ParentNode == null)
                    {
                        objValue.User01 = "";
                    }
                    else
                    {
                        objValue.User01 = node.ParentNode.ParentNode.Name;
                    }

                    //type
                    if (node.ParentNode == null)
                    {
                        objValue.User02 = "";
                    }
                    else
                    {
                        objValue.User02 = node.ParentNode.Name;
                    }

                    //team
                    if (node.ParentNode == null || node.ParentNode.ParentNode == null || node.ParentNode.ParentNode.ParentNode == null)
                    {
                        objValue.User03 = "";
                    }
                    else
                    {
                        objValue.User03 = node.ParentNode.ParentNode.ParentNode.Name;
                    }

                    foreach (XmlAttribute att in node.Attributes)
                    {
                        switch (att.Name.ToLower())
                        {
                            case "input":
                                objValue.UserCode = att.Value;
                                break;
                            case "output":
                                objValue.WBCode = att.Value;
                                break;
                            case "isvalid":
                                objValue.SpellCode = att.Value;
                                break;
                            case "memo":
                                objValue.Memo = att.Value;
                                break;
                            default:
                                break;
                        }
                    }

                    this.alSql.Add(objValue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int Compare()
        {
            if (this.fpSpread2_Sheet1.RowCount > 0)
            {
                this.fpSpread2_Sheet1.Rows.Remove(0, this.fpSpread2_Sheet1.RowCount);
            }

            for (int i = this.alSql.Count - 1; i >= 0; i--)
            {
                Neusoft.HISFC.Models.Base.Item obj = (Neusoft.HISFC.Models.Base.Item)this.alSql[i];

                bool IsFound = false;

                for (int j = 0; j < this.fpSpread1_Sheet1.RowCount; j++)
                {
                    try
                    {
                        //
                        if (obj.ID == this.fpSpread1_Sheet1.GetValue(j, 0).ToString())
                        {
                            if (obj.Name == this.fpSpread1_Sheet1.GetValue(j, 1).ToString())
                            {
                                this.alSql.Remove(obj);
                            }
                            else//修改
                            {
                                this.fpSpread2_Sheet1.Rows.Add(this.fpSpread2_Sheet1.RowCount, 1);
                                int row = this.fpSpread2_Sheet1.RowCount - 1;

                                this.fpSpread2_Sheet1.SetValue(row, 0, obj.ID);
                                this.fpSpread2_Sheet1.SetValue(row, 1, obj.Name);

                                this.fpSpread2_Sheet1.Rows[row].BackColor = Color.PaleGreen;
                                this.fpSpread2_Sheet1.Rows[row].Tag = obj;
                            }

                            IsFound = true;
                            break;
                        }
                    }
                    catch
                    { }
                }

                if (IsFound == false)//新增
                {
                    this.fpSpread2_Sheet1.Rows.Add(this.fpSpread2_Sheet1.RowCount, 1);
                    int row = this.fpSpread2_Sheet1.RowCount - 1;

                    this.fpSpread2_Sheet1.SetValue(row, 0, obj.ID);
                    this.fpSpread2_Sheet1.SetValue(row, 1, obj.Name);

                    this.fpSpread2_Sheet1.Rows[row].BackColor = Color.Bisque;

                    this.fpSpread2_Sheet1.Rows[row].Tag = obj;
                }
            }

            return 0;
        }


        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (e.NewRow < 0) return;

            string sql = ds.Tables[0].Rows[e.NewRow][1].ToString();
            if (sql == null) sql = "";
            sql = sql.Replace("\t", " ");
            sql = sql.Replace("\r", " ");

            this.richTextBox1.Text = sql;

        }

        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (e.NewRow < 0) return;

            //修改,跳到相应行
            if (this.fpSpread2_Sheet1.Rows[e.NewRow].BackColor == Color.PaleGreen)
            {
                for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                {
                    if (this.fpSpread1_Sheet1.GetValue(i, 0).ToString() == this.fpSpread2_Sheet1.GetValue(e.NewRow, 0).ToString())
                    {
                        this.fpSpread1_Sheet1.ActiveRowIndex = i;
                        FarPoint.Win.Spread.LeaveCellEventArgs arg = new FarPoint.Win.Spread.LeaveCellEventArgs(null, 0, 0, i, 0);
                        this.fpSpread1_LeaveCell(null, arg);
                        break;
                    }
                }
            }
            string sql = (this.fpSpread2_Sheet1.Rows[e.NewRow].Tag as Neusoft.HISFC.Models.Base.Item).Name;

            sql = sql.Replace("\t", " ");
            sql = sql.Replace("\r", " ");
            this.richTextBox2.Text = sql;
        }

        private void button2_Click_1(object sender, System.EventArgs e)
        {

            Neusoft.HISFC.BizLogic.Registration.Register r = new Neusoft.HISFC.BizLogic.Registration.Register();

            for (int i = this.fpSpread2_Sheet1.RowCount - 1; i >= 0; i--)
            {
                if (this.fpSpread2_Sheet1.Rows[i].BackColor == Color.PaleGreen) continue;

                try
                {
                    Neusoft.HISFC.Models.Base.Item obj = (Neusoft.HISFC.Models.Base.Item)this.fpSpread2_Sheet1.Rows[i].Tag;

                    string sql = "insert into com_sql (id,name,memo,type,modual,input,output,isvalid," +
                        "team,oper_code,oper_date) values ('{0}',:a,'{1}','{2}','{3}','{4}','{5}','{6}'," +
                        "'{7}','hxw',sysdate)";

                    //保存新增
                    sql = string.Format(sql, obj.ID, obj.Memo, obj.User02, obj.User01, obj.UserCode, obj.WBCode, obj.SpellCode, obj.User03);

                    if (r.InputLong(sql, obj.Name) == -1)
                    {
                        MessageBox.Show(r.Err, i.ToString());
                        continue;
                    }

                    this.fpSpread2_Sheet1.Rows.Remove(i, 1);
                }
                catch
                { }
            }
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            //更新一条

            string sql = "insert into com_sql (id,name,memo,type,modual,input,output,isvalid," +
                "team,oper_code,oper_date) values ('{0}',:a,'{1}','{2}','{3}','{4}','{5}','{6}'," +
                "'{7}','hxw',sysdate)";

            int i = this.fpSpread2_Sheet1.ActiveRowIndex;
            if (i < 0 || this.fpSpread2_Sheet1.RowCount == 0) return;

            if (this.fpSpread2_Sheet1.Rows[i].BackColor == Color.Bisque) return;

            Neusoft.HISFC.BizLogic.Registration.Register r = new Neusoft.HISFC.BizLogic.Registration.Register();


            Neusoft.HISFC.Models.Base.Item obj = (Neusoft.HISFC.Models.Base.Item)this.fpSpread2_Sheet1.Rows[i].Tag;

            string del = "delete from com_sql where id ='" + obj.ID + "'";

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            r.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            try
            {
                if (r.ExecNoQuery(del) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(r.Err, i.ToString());
                    return;
                }

                sql = string.Format(sql, obj.ID, obj.Memo, obj.User02, obj.User01, obj.UserCode, obj.WBCode, obj.SpellCode, obj.User03);

                if (r.InputLong(sql, obj.Name) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(r.Err, i.ToString());
                    return;
                }

                Neusoft.FrameWork.Management.PublicTrans.Commit();

                this.fpSpread2_Sheet1.Rows.Remove(i, 1);

                MessageBox.Show("OK");
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
