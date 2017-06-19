using FastReport;
using GZDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace GZFrameworkDemo.ReportServer
{
    /// <summary>
    /// 报表准备，单表自定义数据源
    /// </summary>
    public class RptCommonCustomer : RptCommonBase
    {

        public delegate void BindDataSourceHandle(Report report, DataSet Data);
        /// <summary>
        /// 数据源
        /// </summary>
        private DataSet DataSource { get; set; }

        /// <summary>
        /// 存储过程名称
        /// </summary>
        private string SPName { get; set; }

        /// <summary>
        /// 存储过程参数
        /// </summary>
        private IDbParms Parms { get; set; }

        private bool ISSP { get; set; } = false;

        public void PrepareDataSource()
        {
            if (ISSP == true)
                DataSource = db.GetDataSet(SPName, Parms);
        }

        /// <summary>
        /// 自定义报表准备
        /// </summary>

        public event Action<Report> BeforePrepare;

        /// <summary>
        /// 单表数据报表 直接提供数据源
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="RptFileName"></param>
        /// <param name="DataSource"></param>
        public RptCommonCustomer(Form owner, string RptFileName, DataSet DataSource)
            : base(owner, RptFileName)
        {
            this.DataSource = DataSource;
            ISSP = false;
        }
        /// <summary>
        /// 单表数据报表 存储过程获得数据源
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="RptFileName"></param>
        /// <param name="SPName"></param>
        /// <param name="Parms"></param>
        public RptCommonCustomer(Form owner, string RptFileName, string SPName, IDbParms Parms)
            : base(owner, RptFileName)
        {
            this.SPName = SPName;
            this.Parms = Parms;
            ISSP = true;
        }

        public event BindDataSourceHandle BindDataSource;

        public override Report PrepareReport()
        {
            Report rpt = null;
            try
            {
                GZFramework.UI.Dev.WaiteServer.ShowWaite(Owner);
                rpt = LoadReport(RptFileName);

                PrepareDataSource();

                BindDataSource?.Invoke(rpt, DataSource);

                //rpt.RegisterData(DataSource, "D");

                //DataBand dataBandD1 = rpt.FindObject("Data1") as DataBand;
                //dataBandD1.DataSource = rpt.GetDataSource("D");

                UpdateRptCommandData(rpt);

                BeforePrepare?.Invoke(rpt);
            }

            catch (Exception ex)
            {
                RptSuccess = false;
                RptException = ex;
            }
            finally
            {
                GZFramework.UI.Dev.WaiteServer.CloseWaite();
                if (RptSuccess == false)
                {
                    GZFramework.UI.Dev.Common.Msg.ShowError(RptException.Message);
                    //rpt = null;
                }
                //else
                //{
                //    rpt.Prepare();//准备工作     
                //}
            }
            return rpt;
        }
    }
}
