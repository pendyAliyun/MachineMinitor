using System;
using System.Drawing;
using DevExpress.XtraCharts;

namespace MachineMonitor
{
    public static class DevChartHelp
    {
        /// <summary>
            /// 创建Series
            /// </summary>
            /// <param name="chat">ChartControl</param>
            /// <param name="seriesName">Series名字『诸如：理论电量』</param>
            /// <param name="seriesType">seriesType『枚举』</param>
            /// <param name="dataSource">数据源</param>
            /// <param name="xBindName">ChartControl的X轴绑定</param>
            /// <param name="yBindName">ChartControl的Y轴绑定</param>
        public static void CreateSeries(this ChartControl chat, string seriesName, ViewType seriesType, object dataSource, string xBindName, string yBindName)
        {
            CreateSeries(chat, seriesName, seriesType, dataSource, xBindName, yBindName, null);
        }
        /// <summary>
            /// 创建Series
            /// </summary>
            /// <param name="chat">ChartControl</param>
            /// <param name="seriesName">Series名字『诸如：理论电量』</param>
            /// <param name="seriesType">seriesType『枚举』</param>
            /// <param name="dataSource">数据源</param>
            /// <param name="xBindName">ChartControl的X轴绑定</param>
            /// <param name="yBindName">ChartControl的Y轴绑定</param>
            /// <param name="createSeriesRule">Series自定义『委托』</param>
        public static void CreateSeries(this ChartControl chat, string seriesName, ViewType seriesType, object dataSource, string xBindName, string yBindName, Action<Series> createSeriesRule)
        {
            if (chat == null)
                throw new ArgumentNullException("chat");
            if (string.IsNullOrEmpty(seriesName))
                throw new ArgumentNullException("seriesType");
            if (string.IsNullOrEmpty(xBindName))
                throw new ArgumentNullException("xBindName");
            if (string.IsNullOrEmpty(yBindName))
                throw new ArgumentNullException("yBindName");

            Series _series = new Series(seriesName, seriesType);
            _series.ArgumentScaleType = ScaleType.Qualitative;
            _series.ArgumentDataMember = xBindName;
            _series.ValueDataMembers[0] = yBindName;
            _series.Label.Visible = false; //设置系列值不显示
            
            _series.DataSource = dataSource;
            if (createSeriesRule != null)
                createSeriesRule(_series);
            chat.Series.Add(_series);
        }

        public static void SetHZTitle(ref ChartControl chartControl, string HTitle)
        {
            chartControl.Titles.Clear();                    //先清除以前的标题
            //横标题设置
            ChartTitle titles = new ChartTitle();            //声明标题
            titles.Text = HTitle;                            //名称
            titles.TextColor = Color.Black;   //颜色
            titles.Indent = 1;                                //设置距离  值越小柱状图就越大
            titles.Font = new Font("Tahoma", 10, FontStyle.Bold);//设置字体
            titles.Dock = ChartTitleDockStyle.Top;           //设置对齐方式
            titles.Indent = 0;
            titles.Alignment = StringAlignment.Center;       //居中对齐
            chartControl.Titles.Add(titles);                 //添加标题                    
        }

    }
}