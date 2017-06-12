using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapDataTools;
using Sashulin;

namespace NetposaTest
{
    [ComVisible(true)]
    public partial class BaiduUserControl : UserControl
    {
        private PanoTool tool = new PanoTool(@"E:\Project\pano\pannellum\app\baidu");
        private string panoSource;
        private WebBrowser mapBrowser;
        private BindingList<PanoItem> bindingSource;
        //http://lbsyun.baidu.com/index.php?title=coordinate
        private string baiduUrl =
            "http://api.map.baidu.com/geoconv/v1/?coords={0},{1}&from=1&to=6&ak=zeTqFBtbo8P9b5YeeepZouxd";
        public BaiduUserControl()
        {
            InitializeComponent();
            mapBrowser = new System.Windows.Forms.WebBrowser();
            if (!this.DesignMode)
            {

                mapBrowser.Dock = DockStyle.Fill;
                mapBrowser.ObjectForScripting = this;
                mapBrowser.ScrollBarsEnabled = false;
                mapBrowser.ScrollBarsEnabled = false;
                this.splitContainer1.Panel1.Controls.Add(mapBrowser);
                mapBrowser.Navigate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "webmap/index_baidu.html"));
            }
            CSharpBrowserSettings settings = new CSharpBrowserSettings();
            chromeWebBrowser1.Initialize(settings);
            panoSource = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "pano.json");
            this.Load += PanoFrom_Load;
            bindingSource = new BindingList<PanoItem>();

            dataGridView1.DataSource = bindingSource;
            this.dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }
 
        void PanoFrom_Load(object sender, EventArgs e)
        {
            if (File.Exists(this.panoSource))
            {
                var read = new StreamReader(this.panoSource, Encoding.UTF8);
                var content = read.ReadToEnd();
                var panoItems = JsonHelper.JsonDeserialize<List<PanoItem>>(content);
                read.Close();

                panoItems.ForEach(m =>
                {
                    bindingSource.Add(m);
                });
            }
            var frm = this.FindForm();
            if (frm != null)
            {
                frm.Closed += PanoFrom_FormClosed;
            }
        }

        void PanoFrom_FormClosed(object sender, EventArgs e)
        {
            var items = this.bindingSource.ToList();
            var itemSource = JsonHelper.JsonSerializer(items);
            using (var stream = new FileStream(this.panoSource, FileMode.OpenOrCreate))
            {
                var buffer = System.Text.UTF8Encoding.UTF8.GetBytes(itemSource);
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        private void DealBaiduCoor(string url, Action<int, int> callback)
        {
            using (var client = new WebClient())
            {
                var e = client.DownloadString(new Uri(url));
                var dic = JsonHelper.JsonDeserialize<Dictionary<string, object>>(e);
                if (dic["status"].ToString() == "0")
                {
                    dic = (Dictionary<string, object>)((ArrayList)dic["result"])[0];
                    callback((int)double.Parse(dic["x"].ToString()) + 36, (int)double.Parse(dic["y"].ToString()));
                }
            }
        }

        private void SetCenter(Coord coord)
        {
            mapBrowser.Document.InvokeScript("setCenter", new object[] { coord.lon, coord.lat });
        }

        public void Click(double x1, double y1, int x, int y)
        {

            y = y - 23000;
            var pano = tool.GetPannoCode(x, y);
            toolStripStatusLabel1.Text = "请求数据中...";
            if (pano != null)
            {


                new Thread(() =>
                {
                    tool.GeneratePano(pano);
                    pano.Coord = new Coord(x1, y1);
                    if (this.IsDisposed)
                    {
                        return;
                    }
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (this.IsDisposed)
                        {
                            return;
                        }
                        bindingSource.Add(pano);
                        toolStripStatusLabel1.Text = pano.RoadName + "全景数据处理完成";
                        PanoFrom_FormClosed(null, null);
                    }));
                }).Start();

            }
            else
            {
                toolStripStatusLabel1.Text = "无全景数据";
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var item = this.dataGridView1.CurrentRow.DataBoundItem as PanoItem;
            if (item != null)
            {
                var code = item.Code;
                toolStripStatusLabel1.Text = item.RoadName;
                this.SetCenter(item.Coord);
                //var yaw = (double.Parse(item.MoveDir.ToString()) - double.Parse(item.NorthDir.ToString()));
                //var opt = string.Format("code={4}&pitch={0}&yaw={1}&fov={2}&northOffset={3}", item.Pitch, yaw,
                //    100, 0, code);
                chromeWebBrowser1.OpenUrl(
                    string.Format("http://localhost:807/Project/pano/pannellum/app/baidu/{0}/index.html", code));
            }
        }

    }
}
