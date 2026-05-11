using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class HolographicViewURL:Neusoft.Emr.DoctorStation.Port.IPlugin,System.ComponentModel.INotifyPropertyChanged,IDisposable
    {
        #region IPlugin 成员

        public IList<Neusoft.Emr.DoctorStation.Port.IAction> Actions
        {
            get;
            set;
           
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Disposed;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.Disposed +=  value;
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.Disposed -= value;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public Neusoft.Emr.DoctorStation.Port.IHost Host
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Neusoft.Emr.DoctorStation.Model.TreeItem MenuItem
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnActionsInitialized()
        {
        }

        public void OnDisposing(Neusoft.Emr.DoctorStation.Port.DisposingEventArgs e)
        {
           
        }

        public void OnHostInitialized()
        {
            this.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        public Neusoft.Emr.DoctorStation.Port.OperationModel OperationModel
        {
            get;
            set;
        }


        public string Text
        {

            get;
            set;
        }

        #endregion

        #region INotifyPropertyChanged 成员

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                this.PropertyChanged += value;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                this.PropertyChanged -= value;
            }
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (this.Disposed!= null)
            {
                this.Disposed(this, EventArgs.Empty);
            }
        }

       

        #endregion

      

        public void Execute()
        {
            string text = @"http://172.16.61.194:8082/pacsimage/showstudiesbypatientidandhospitalname.action?hosptialname=&patientid={0}&={1}&";
            string hisInpatientNo = this.Host.InPatientInfo.HisInpatientNo;
            string patientNo = this.Host.InPatientInfo.PatientNo;
            if (string.IsNullOrEmpty(hisInpatientNo))
            {
                MessageBox.Show("调取失败，请联系信息科！");
            }
            else
            {
                text = string.Format(text, patientNo, hisInpatientNo);
                System.Diagnostics.Process.Start("chrome.exe",text);
            }
        }


      

       
    }
}
