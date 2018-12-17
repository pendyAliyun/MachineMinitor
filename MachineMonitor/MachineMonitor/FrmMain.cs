using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Models;
using BLL;
using DevExpress.XtraCharts;
using System.Threading;
using DevExpress.Utils;
using DevExpress.XtraTab;

namespace MachineMonitor
{
    public partial class FrmMain : Form
    {
        #region 变量对象定义及窗体初始化
        DataRow[] machineDRs;
        AIOServ aioServ = new AIOServ();
        //线程开始的时候调用的委托  
        private delegate void maxValueDelegate(int maxValue);
        //线程执行中调用的委托  
        private delegate void nowValueDelegate(int nowValue);
        private delegate void setGridDelegate(List<object> listResult);
        public FrmMain()
        {
            InitializeComponent();
            //if (DateTime.Now.Date.AddDays(-15) > Convert.ToDateTime("2018-12-04"))
            //{
            //    MessageBox.Show("试用期已过，请联系开发人员");
            //    return;
            //}
            endDate.Text = DateTime.Now.ToShortDateString().Replace("/", "-");
            startDate.Text = DateTime.Now.AddDays(-5).ToShortDateString().Replace("/", "-");
            xtraTabPage3.AutoScroll = true;
            limitAverageXTP.AutoScroll = true;
            InitData();
        }
        private void InitData()
        {
            InitProgres();
            splitContainerControl1.UseDisabledStatePainter = false;
            machineDRs = aioServ.GetMachineNo("", "");
            CboxBingdingData();
        }
        #endregion

        #region 查询按钮事件
        private void btnQuery_Click(object sender, EventArgs e)
        {
            switch (xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    LoadDataFromDB();
                    break;
                case 2:
                    LoadQingXiangXing();
                    break;
            }
        }
        #endregion

        #region 进度条相关
        private void InitProgres()
        {
            #region 进度条
            pbcProcess.Visible = false;   //暂时隐藏了这个进度条
            //设置一个最小值
            pbcProcess.Properties.Minimum = 0;
            //设置一个最大值
            pbcProcess.Properties.Maximum = 11;
            //设置步长，即每次增加的数
            pbcProcess.Properties.Step = 1;
            //设置进度条的样式
            pbcProcess.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            //当前值
            pbcProcess.Position = 0;
            //是否显示进度数据
            pbcProcess.Properties.ShowTitle = true;
            //是否显示百分比
            pbcProcess.Properties.PercentView = true;
            #endregion

            toolStripProgressBar1.Minimum = 0;

        }
        private void SetProgressValue(int maximun, int position)
        {
            maxValueDelegate maxValueDelegate = new maxValueDelegate(SetMaxValue);
            nowValueDelegate nowValueDelegate = new nowValueDelegate(SetNowValue);
            this.pbcProcess.Invoke(maxValueDelegate, maximun);
            this.pbcProcess.Invoke(nowValueDelegate, position);

            toolStripProgressBar1.ProgressBar.Invoke(maxValueDelegate, maximun);
            toolStripProgressBar1.ProgressBar.Invoke(nowValueDelegate, position);

        }
        private void SetNowValue(int position)
        {
            btnQuery.Enabled = false;
            btnQuery.Text = "加载中.";
            pbcProcess.Position = position;
            toolStripProgressBar1.Value = position;
            if (position == toolStripProgressBar1.Maximum)
            {
                btnQuery.Enabled = true;
                btnQuery.Text = "查询";
            }
            pbll.Text = (Math.Round(Convert.ToDouble((position * 1.0) / (toolStripProgressBar1.Maximum * 1.0) * 100), 2)).ToString() + "%";
        }

        private void SetMaxValue(int maximun)
        {
            pbcProcess.Properties.Maximum = maximun;
            toolStripProgressBar1.Maximum = maximun;
        }
        #endregion

        #region 导出到Excel
        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            sfd.Filter = "Excel文件(*.xls)|*.xls|Csv文件(*.csv)|*.csv|所有文件(*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.Title = "保存文件";
            sfd.FileName = DateTime.Now.GetDateTimeFormats()[99].Substring(0, 10) + "CGL";
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            gridLogList.ExportToXls(sfd.FileName);
        }


        #endregion

        #region 图表控件数据的绑定
        /// <summary>
        /// 加载设备倾向性图表
        /// </summary>
        private void LoadQingXiangXing()
        {
            List<AIOLAV> aIOLAVs;
            List<AIORAA> _dt = aioServ.GetCGLQXX<AIORAA>(startDate.DateTime, endDate.DateTime, cboxMachineNo.Text.Trim(), out aIOLAVs);
            #region 倾向性图表
            Clear_ChartSeries();
            QXXChart_A.CreateSeries("SPC_G_P1", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_G_P1");
            QXXChart_A.CreateSeries("SPC_F_P2", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_F_P2");
            QXXChart_A.CreateSeries("SPC_AS_P3", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_AS_P3");
            QXXChart_A.CreateSeries("SPC_E_P4", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_E_P4");
            QXXChart_A.CreateSeries("SPC_D_P5", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_D_P5");
            QXXChart_A.CreateSeries("SPC_C_P6", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_C_P6");
            QXXChart_A.CreateSeries("SPC_B_P9", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_B_P9");
            QXXChart_A.CreateSeries("SPC_A_P7", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_A_P7");
            QXXChart_A.CreateSeries("SPC_H_P8", ViewType.Line, (_dt.Where(p => p.indenter == "A")).ToList(), "sn", "SPC_H_P8");
            ((XYDiagram)QXXChart_A.Diagram).AxisY.Range.MinValue = minDown.Value;//Y轴区域的最小值
            ((XYDiagram)QXXChart_A.Diagram).AxisY.Range.MaxValue = maxDown.Value;
            ((XYDiagram)QXXChart_A.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)QXXChart_A.Diagram).AxisX.Range.Auto = false;
            ((XYDiagram)QXXChart_A.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)QXXChart_A.Diagram).EnableAxisXScrolling = true;
            DevChartHelp.SetHZTitle(ref QXXChart_A, "A");

            QXXChart_B.CreateSeries("SPC_G_P1", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_G_P1");
            QXXChart_B.CreateSeries("SPC_F_P2", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_F_P2");
            QXXChart_B.CreateSeries("SPC_AS_P3", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_AS_P3");
            QXXChart_B.CreateSeries("SPC_E_P4", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_E_P4");
            QXXChart_B.CreateSeries("SPC_D_P5", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_D_P5");
            QXXChart_B.CreateSeries("SPC_C_P6", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_C_P6");
            QXXChart_B.CreateSeries("SPC_B_P9", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_B_P9");
            QXXChart_B.CreateSeries("SPC_A_P7", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_A_P7");
            QXXChart_B.CreateSeries("SPC_H_P8", ViewType.Line, (_dt.Where(p => p.indenter == "B")).ToList(), "sn", "SPC_H_P8");
            ((XYDiagram)QXXChart_B.Diagram).AxisY.Range.MinValue = minDown.Value;//Y轴区域的最小值
            ((XYDiagram)QXXChart_B.Diagram).AxisY.Range.MaxValue = maxDown.Value;
            ((XYDiagram)QXXChart_B.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)QXXChart_B.Diagram).AxisX.Range.Auto = false;
            ((XYDiagram)QXXChart_B.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)QXXChart_B.Diagram).EnableAxisXScrolling = true;
            DevChartHelp.SetHZTitle(ref QXXChart_B, "B");

            QXXChart_C.CreateSeries("SPC_G_P1", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_G_P1");
            QXXChart_C.CreateSeries("SPC_F_P2", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_F_P2");
            QXXChart_C.CreateSeries("SPC_AS_P3", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_AS_P3");
            QXXChart_C.CreateSeries("SPC_E_P4", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_E_P4");
            QXXChart_C.CreateSeries("SPC_D_P5", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_D_P5");
            QXXChart_C.CreateSeries("SPC_C_P6", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_C_P6");
            QXXChart_C.CreateSeries("SPC_B_P9", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_B_P9");
            QXXChart_C.CreateSeries("SPC_A_P7", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_A_P7");
            QXXChart_C.CreateSeries("SPC_H_P8", ViewType.Line, (_dt.Where(p => p.indenter == "C")).ToList(), "sn", "SPC_H_P8");
            ((XYDiagram)QXXChart_C.Diagram).AxisY.Range.MinValue = minDown.Value;//Y轴区域的最小值
            ((XYDiagram)QXXChart_C.Diagram).AxisY.Range.MaxValue = maxDown.Value;
            ((XYDiagram)QXXChart_C.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)QXXChart_C.Diagram).AxisX.Range.Auto = false;
            ((XYDiagram)QXXChart_C.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)QXXChart_C.Diagram).EnableAxisXScrolling = true;
            DevChartHelp.SetHZTitle(ref QXXChart_C, "C");

            QXXChart_D.CreateSeries("SPC_G_P1", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_G_P1");
            QXXChart_D.CreateSeries("SPC_F_P2", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_F_P2");
            QXXChart_D.CreateSeries("SPC_AS_P3", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_AS_P3");
            QXXChart_D.CreateSeries("SPC_E_P4", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_E_P4");
            QXXChart_D.CreateSeries("SPC_D_P5", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_D_P5");
            QXXChart_D.CreateSeries("SPC_C_P6", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_C_P6");
            QXXChart_D.CreateSeries("SPC_B_P9", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_B_P9");
            QXXChart_D.CreateSeries("SPC_A_P7", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_A_P7");
            QXXChart_D.CreateSeries("SPC_H_P8", ViewType.Line, (_dt.Where(p => p.indenter == "D")).ToList(), "sn", "SPC_H_P8");
            ((XYDiagram)QXXChart_D.Diagram).AxisY.Range.MinValue = minDown.Value;//Y轴区域的最小值
            ((XYDiagram)QXXChart_D.Diagram).AxisY.Range.MaxValue = maxDown.Value;
            ((XYDiagram)QXXChart_D.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)QXXChart_D.Diagram).AxisX.Range.Auto = false;
            ((XYDiagram)QXXChart_D.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)QXXChart_D.Diagram).EnableAxisXScrolling = true;
            DevChartHelp.SetHZTitle(ref QXXChart_D, "D");

            QXXChart_E.CreateSeries("SPC_G_P1", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_G_P1");
            QXXChart_E.CreateSeries("SPC_F_P2", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_F_P2");
            QXXChart_E.CreateSeries("SPC_AS_P3", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_AS_P3");
            QXXChart_E.CreateSeries("SPC_E_P4", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_E_P4");
            QXXChart_E.CreateSeries("SPC_D_P5", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_D_P5");
            QXXChart_E.CreateSeries("SPC_C_P6", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_C_P6");
            QXXChart_E.CreateSeries("SPC_B_P9", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_B_P9");
            QXXChart_E.CreateSeries("SPC_A_P7", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_A_P7");
            QXXChart_E.CreateSeries("SPC_H_P8", ViewType.Line, (_dt.Where(p => p.indenter == "E")).ToList(), "sn", "SPC_H_P8");
            ((XYDiagram)QXXChart_E.Diagram).AxisY.Range.MinValue = minDown.Value;//Y轴区域的最小值
            ((XYDiagram)QXXChart_E.Diagram).AxisY.Range.MaxValue = maxDown.Value;
            ((XYDiagram)QXXChart_E.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)QXXChart_E.Diagram).AxisX.Range.Auto = false;
            ((XYDiagram)QXXChart_E.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)QXXChart_E.Diagram).EnableAxisXScrolling = true;
            DevChartHelp.SetHZTitle(ref QXXChart_E, "E");

            QXXChart_F.CreateSeries("SPC_G_P1", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_G_P1");
            QXXChart_F.CreateSeries("SPC_F_P2", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_F_P2");
            QXXChart_F.CreateSeries("SPC_AS_P3", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_AS_P3");
            QXXChart_F.CreateSeries("SPC_E_P4", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_E_P4");
            QXXChart_F.CreateSeries("SPC_D_P5", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_D_P5");
            QXXChart_F.CreateSeries("SPC_C_P6", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_C_P6");
            QXXChart_F.CreateSeries("SPC_B_P9", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_B_P9");
            QXXChart_F.CreateSeries("SPC_A_P7", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_A_P7");
            QXXChart_F.CreateSeries("SPC_H_P8", ViewType.Line, (_dt.Where(p => p.indenter == "F")).ToList(), "sn", "SPC_H_P8");
            ((XYDiagram)QXXChart_F.Diagram).AxisY.Range.MinValue = minDown.Value;//Y轴区域的最小值
            ((XYDiagram)QXXChart_F.Diagram).AxisY.Range.MaxValue = maxDown.Value;
            ((XYDiagram)QXXChart_F.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)QXXChart_F.Diagram).AxisX.Range.Auto = false;
            ((XYDiagram)QXXChart_F.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)QXXChart_F.Diagram).EnableAxisXScrolling = true;
            DevChartHelp.SetHZTitle(ref QXXChart_F, "F");
            #endregion

            #region 极值均值图表
            int chartLimitHeight = 130;
            double chartLimit_maxvalue = 0.4;
            chartLimit_A.CreateSeries("均值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "A")).ToList(), "sn", "agvValue");
            chartLimit_A.CreateSeries("极值", ViewType.Line, aIOLAVs.Where(p => p.indenter == "A").ToList(), "sn", "linmitVal");
            ((XYDiagram)chartLimit_A.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartLimit_A.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)chartLimit_A.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)chartLimit_A.Diagram).AxisY.Range.MaxValue = chartLimit_maxvalue;
            chartLimit_A.Height = chartLimitHeight;
            DevChartHelp.SetHZTitle(ref chartLimit_A, "A");

            chartLimit_B.CreateSeries("均值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "B")).ToList(), "sn", "agvValue");
            chartLimit_B.CreateSeries("极值", ViewType.Line, aIOLAVs.Where(p => p.indenter == "B").ToList(), "sn", "linmitVal");
            ((XYDiagram)chartLimit_B.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartLimit_B.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)chartLimit_B.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)chartLimit_B.Diagram).AxisY.Range.MaxValue = chartLimit_maxvalue;
            chartLimit_B.Height = chartLimitHeight;
            DevChartHelp.SetHZTitle(ref chartLimit_B, "B");

            chartLimit_E.CreateSeries("均值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "E")).ToList(), "sn", "agvValue");
            chartLimit_E.CreateSeries("极值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "E")).ToList(), "sn", "linmitVal");
            ((XYDiagram)chartLimit_E.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartLimit_E.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)chartLimit_E.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)chartLimit_E.Diagram).AxisY.Range.MaxValue = chartLimit_maxvalue;
            chartLimit_E.Height = chartLimitHeight;
            DevChartHelp.SetHZTitle(ref chartLimit_E, "E");

            chartLimit_C.CreateSeries("均值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "C")).ToList(), "sn", "agvValue");
            chartLimit_C.CreateSeries("极值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "C")).ToList(), "sn", "linmitVal");
            ((XYDiagram)chartLimit_C.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartLimit_C.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)chartLimit_C.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)chartLimit_C.Diagram).AxisY.Range.MaxValue = chartLimit_maxvalue;
            chartLimit_C.Height = chartLimitHeight;
            DevChartHelp.SetHZTitle(ref chartLimit_C, "C");

            chartLimit_D.CreateSeries("均值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "D")).ToList(), "sn", "agvValue");
            chartLimit_D.CreateSeries("极值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "D")).ToList(), "sn", "linmitVal");
            ((XYDiagram)chartLimit_D.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartLimit_D.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)chartLimit_D.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)chartLimit_D.Diagram).AxisY.Range.MaxValue = chartLimit_maxvalue;
            chartLimit_D.Height = chartLimitHeight;
            DevChartHelp.SetHZTitle(ref chartLimit_D, "D");

            chartLimit_F.CreateSeries("均值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "F")).ToList(), "sn", "agvValue");
            chartLimit_F.CreateSeries("极值", ViewType.Line, (aIOLAVs.Where(p => p.indenter == "F")).ToList(), "sn", "linmitVal");
            ((XYDiagram)chartLimit_F.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartLimit_F.Diagram).AxisX.Range.MaxValueInternal = 30;
            ((XYDiagram)chartLimit_F.Diagram).AxisX.Label.Visible = false;
            ((XYDiagram)chartLimit_F.Diagram).AxisY.Range.MaxValue = chartLimit_maxvalue;
            chartLimit_F.Height = chartLimitHeight;
            DevChartHelp.SetHZTitle(ref chartLimit_F, "F");

            #endregion
        }

        private void ChartBingdingData(List<AIOTOA> aIOTOAs)
        {
            machineChart.Series.Clear();
            List<Series> series = new List<Series>();
            foreach (var item in aIOTOAs)
            {
                Series serie = new Series(item.intender, ViewType.Bar);
                serie.Points.Add(new SeriesPoint(item.MachineNo, item.Quty));
                series.Add(serie);
            }
            machineChart.Series.AddRange(series.ToArray());
        }
        #endregion

        #region 鼠标移动到系列上显示相应的值
        ToolTipController toolTipController = new ToolTipController();
        private void _MouseMove(object sender, MouseEventArgs e)
        {
            ChartControl chartControl_current = null;
            ChartHitInfo hitInfo = null;
            foreach (Control c in xtraTabControl1.TabPages)
            {
                if (c is XtraTabPage && ((XtraTabPage)c).Name == "xtraTabPage3")
                {
                    foreach (Control b in c.Controls)
                    {
                        if (b is ChartControl && ((ChartControl)b).Equals(sender))
                        {
                            chartControl_current = ((ChartControl)b);
                            hitInfo = chartControl_current.CalcHitInfo(e.Location);
                        }

                    }
                }
            }
            foreach (Control c in xtraTabControl1.TabPages)
            {
                if (c is XtraTabPage && ((XtraTabPage)c).Name == "limitAverageXTP")
                {
                    foreach (Control b in c.Controls)
                    {
                        if (b is ChartControl && ((ChartControl)b).Equals(sender))
                        {
                            chartControl_current = ((ChartControl)b);
                            hitInfo = chartControl_current.CalcHitInfo(e.Location);
                        }

                    }
                }
            }
            if (hitInfo == null) return;
            StringBuilder builder = new StringBuilder();
            if (hitInfo.InDiagram)
                builder.AppendLine("In diagram");
            if (hitInfo.InNonDefaultPane)
                builder.AppendLine("In non-default pane: " + hitInfo.NonDefaultPane.Name);
            if (hitInfo.InAxis)
            {
                builder.AppendLine("In axis: " + hitInfo.Axis.Name);
                if (hitInfo.AxisLabelItem != null)
                    builder.AppendLine("  Label item: " + hitInfo.AxisLabelItem.Text);
                if (hitInfo.AxisTitle != null)
                    builder.AppendLine("  Axis title: " + hitInfo.AxisTitle.Text);
            }
            if (hitInfo.InChartTitle)
                builder.AppendLine("In chart title: " + hitInfo.ChartTitle.Text);
            if (hitInfo.InLegend)
                builder.AppendLine("In legend");
            if (hitInfo.InSeries)
                builder.AppendLine("In series: " + ((Series)hitInfo.Series).Name);
            if (hitInfo.InSeriesLabel)
            {
                builder.AppendLine("In series label");
                builder.AppendLine("  Series: " + ((Series)hitInfo.Series).Name);
            }
            if (hitInfo.SeriesPoint != null)
            {
                builder.AppendLine("  Argument: " + hitInfo.SeriesPoint.Argument);
                if (!hitInfo.SeriesPoint.IsEmpty)
                    builder.AppendLine("  Value: " + hitInfo.SeriesPoint.Values[0]);
            }
            if (builder.Length > 0)
                toolTipController.ShowHint("AIO-testing results:\n" + builder.ToString(),
                    chartControl_current.PointToScreen(e.Location));
            else
                toolTipController.HideHint();
        }
        #endregion

        #region 清空数据源图表在重新查询时调用到
        /// <summary>
        /// 清空图表在重新查询时调用到
        /// </summary>
        void Clear_ChartSeries()
        {
            QXXChart_A.Series.Clear();
            QXXChart_B.Series.Clear();
            QXXChart_C.Series.Clear();
            QXXChart_D.Series.Clear();
            QXXChart_E.Series.Clear();
            QXXChart_F.Series.Clear();

            chartLimit_A.Series.Clear();
            chartLimit_B.Series.Clear();
            chartLimit_C.Series.Clear();
            chartLimit_D.Series.Clear();
            chartLimit_E.Series.Clear();
            chartLimit_F.Series.Clear();
        }

        /// <summary>
        /// 清空GridList列表和图形表
        /// </summary>
        private void ClearGridChart()
        {
            gridLogList.DataSource = null;
            gridLogList.DataBindings.Clear();
            gridLogList.RefreshDataSource();
            gridLogList.Refresh();
            machineChart.Series.Clear();
            machineChart.RefreshData();
            machineChart.Refresh();
            Clear_ChartSeries();
        }
        #endregion

        #region 鼠标事件触发
        private void QXXChart_A_MouseMove(object sender, MouseEventArgs e)
        {
            _MouseMove(sender, e);
        }

        private void QXXChart_B_MouseMove(object sender, MouseEventArgs e)
        {
            _MouseMove(sender, e);
        }

        private void QXXChart_E_MouseMove(object sender, MouseEventArgs e)
        {
            _MouseMove(sender, e);
        }

        private void QXXChart_D_MouseMove(object sender, MouseEventArgs e)
        {
            _MouseMove(sender, e);
        }

        private void QXXChart_C_MouseMove(object sender, MouseEventArgs e)
        {
            _MouseMove(sender, e);
        }

        private void QXXChart_F_MouseMove(object sender, MouseEventArgs e)
        {
            _MouseMove(sender, e);
        }
        #endregion

        #region 下拉选择与tabcontrol切换变更事件
        private void cboxMachine_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboxMachineNo_KeyUp(null, null);
        }
        private void cboxMachineNo_KeyUp(object sender, KeyEventArgs e)
        {
            machineDRs = aioServ.GetMachineNo(cboxMachineNo.Text, cboxMachine.Text);
            CboxBingdingData();
        }

        /// <summary>
        /// 下拉框自动过滤绑定
        /// </summary>
        private void CboxBingdingData()
        {
            cboxMachineNo.Properties.Items.Clear();
            for (int i = 0; i < machineDRs.Length; i++)
            {
                cboxMachineNo.Properties.Items.Add(machineDRs[i]["AIO001"]);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            if (e.Page == xtraTabPage1 || e.Page == xtraTabPage2)
            {
                if (e.Page == xtraTabPage2) pbcProcess.Visible = true;
                LoadDataFromDB();
            }
            else if (e.Page == xtraTabPage3 || e.Page == limitAverageXTP)
            {
                pbcProcess.Hide();
                LoadQingXiangXing();
            }
        }
        #endregion

        #region 从ORACLE数据库获取CGL不良记录集
        /// <summary>
        /// 从ORACLE数据库获取CGL不良记录集
        /// </summary>
        private void LoadDataFromDB()
        {
            List<object> listResult = new List<object>();
            if ((endDate.DateTime.Date - startDate.DateTime.Date).Days > 7)
            {
                MessageBox.Show("由于数据量比较大，查询时间段天数不能超过7天");
                return;
            };
            ClearGridChart();//初始化GridList列表
            aioServ.aioServ.ReadingDb += SetProgressValue;
            IAsyncResult asyncResult = toolStripProgressBar1.ProgressBar.BeginInvoke(
                new Action(() => toolStripStatusLabel1.Text = "数据读取中..."), null);
            Thread thread = new Thread(() =>
            {
                listResult = aioServ.GetCGLNg(startDate.DateTime, endDate.DateTime, cboxMachineNo.Text.Trim());
                //使用匿名委托给Invoke传参，listResult是委托所需要的参数，可以理解为相当于给占位s传值
                this.Invoke(new setGridDelegate((s) =>
                {
                    gridLogList.DataSource = listResult[0];
                    ChartBingdingData((List<AIOTOA>)listResult[1]);
                }), listResult);
            });
            thread.Start();
        }
        #endregion

        #region 资源回收
        /// <summary>
        /// 窗体关闭时销毁掉此应用所占用的进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("确认退出，数据缓存将被清除!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                e.Cancel = false;
                this.Dispose();
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }            
        }
        #endregion


    }
}
