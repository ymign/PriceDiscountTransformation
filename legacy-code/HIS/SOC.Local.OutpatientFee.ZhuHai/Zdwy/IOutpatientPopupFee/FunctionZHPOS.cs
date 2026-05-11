using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Windows.Forms;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientPopupFee
{
    /// <summary>
    /// 珠海医保POS终端机接口
    /// </summary>
    class FunctionZHPOS
    {
        #region ZHSB.DLL调用

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="nport"></param>
        /// <param name="speed"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [DllImport("ZHSB.dll", SetLastError = true)]
        private static extern int OpenSerialPort(int nport, int speed, ref String format);

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        [DllImport("ZHSB.dll", SetLastError = true)]
        private static extern int CloseSerialPort();

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="TransType">交易类型 (1,读  2,写 3,黑名单更新 4,清算)</param>
        /// <param name="ToTerminal"></param>
        /// <param name="ReqLen"></param>
        /// <param name="FrmTominal"></param>
        /// <param name="RepLen"></param>
        /// <param name="TimeOut"></param>
        /// <returns></returns>
        [DllImport("ZHSB.dll", SetLastError = true)]
        private static extern int ExchangeData(char transType, ref char[] toTerminal, uint reqLen, ref char[] frmTominal, ref uint repLen, uint TimeOut);

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [DllImport("ZHSB.dll", SetLastError = true)]
        private static extern int AscToBcd(ref char[] src, ref char[] dest, long len);

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [DllImport("ZHSB.dll", SetLastError = true)]
        private static extern int BcdToAsc(ref char[] src, ref char[] dest, long len);

        #endregion

        #region 获取参数

        /// <summary>
        /// 参数路径
        /// </summary>
        private string proFileName = Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + @"Profile\珠海医保卡POS机参数.xml";

        /// <summary>
        /// 端口号
        /// </summary>
        private int portNum;

        /// <summary>
        /// 速度
        /// </summary>
        private int speed;

        /// <summary>
        /// 格式
        /// </summary>
        private string format = string.Empty;

        /// <summary>
        /// 超时时间
        /// </summary>
        private int timeOut;

        /// <summary>
        /// 获取本地参数
        /// </summary>
        public int GetParam()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                StreamReader sr = new StreamReader(proFileName, System.Text.Encoding.Default);
                string cleanDown = sr.ReadToEnd();
                doc.LoadXml(cleanDown);
                sr.Close();

                XmlNode paramNode = doc.SelectSingleNode("/POS参数");
                int.TryParse(paramNode.ChildNodes[0].Attributes["PortNum"].Value, out this.portNum);
                int.TryParse(paramNode.ChildNodes[0].Attributes["BaudRate"].Value, out this.speed);
                this.format = paramNode.ChildNodes[0].Attributes["Format"].Value.Trim();
                int.TryParse(paramNode.ChildNodes[0].Attributes["TimeOut"].Value, out this.timeOut);
                return 1;
            }
            catch (Exception objEx)
            {
                MessageBox.Show("珠海医保卡POS机参数获取失败!请手动扣费珠海医保卡!" + objEx.Message);
                return -1;
            }

        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        public int OpenlPort()
        {
            if (OpenSerialPort(this.portNum, this.speed, ref format) != 0)
            {
                MessageBox.Show("POS机操作,打开串口: COM" + this.portNum + " 失败!请手动扣费珠海医保卡!");
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        public int Closelport()
        {
            if (CloseSerialPort() != 0)
            {
                MessageBox.Show("POS机操作,关闭串口失败!请手动扣费珠海医保卡!");
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 读取卡信息
        /// </summary>
        /// <returns></returns>
        public int ReadMCCardInfo()
        {
            char transType = '1';  //读
            char[] sendPara = new Char[0];             //传入参数
            uint sendLength = 0;                       //传入查收长度
            char[] receivePara= new char[500];         //传出参数
            uint receiveLength = 0;                    //传出参数长度
            uint to = uint.Parse(timeOut.ToString());                         //超时

            for (int i = 0; i < 500; i++)
            {
                receivePara[i] = ' ';
            }

            int returnValue = ExchangeData(transType, ref sendPara, sendLength, ref receivePara, ref receiveLength, to);

            //查询不成功
            if (returnValue != 0)
            {
                //if(ll_rtn==
            }

            //查询成功
            return returnValue;

        }


        #endregion



    }
}
