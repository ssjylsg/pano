using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public partial class Yun720Control : UserControl
    {
        Yun720Tool yun720Tool = new Yun720Tool();
        private Yun720 tooYun720 = new Yun720(@"E:\Project\pano\pannellum\app\baidu\720Yun");
        private Dictionary<string,Pano72Yun> pano72Yuns = new Dictionary<string, Pano72Yun>(); 
        public Yun720Control()
        {
            InitializeComponent();
            CSharpBrowserSettings settings = new CSharpBrowserSettings()
            {
                DefaultUrl =  "https://720yun.com/"
            };
            this.textBox1.Text = "http://720yun.com/t/c8bjkrkfry2?pano_id=3407156";
            chromeWebBrowser1.Initialize(settings);
            chromeWebBrowser1.BrowserNewWindow += chromeWebBrowser1_BrowserNewWindow;
            tooYun720.OnProcessImageHandler += tooYun720_OnProcessImageHandler;
            tooYun720.OnCompleteHandler += tooYun720_OnCompleteHandler;
            this.Load += Yun720Control_Load;
            panoSource = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "720yun.json");
        }

        void Yun720Control_Load(object sender, EventArgs e)
        {
            var form = this.FindForm();
            if(form != null)
            {
                form.Closing += form_Closing;
            }
            if (File.Exists(this.panoSource))
            {
                var read = new StreamReader(this.panoSource, Encoding.UTF8);
                var content = read.ReadToEnd();
                var panoItems = JsonHelper.JsonDeserialize<List<Pano72Yun>>(content);
                read.Close();
                panoItems.ForEach(m =>
                {
                    this.pano72Yuns[m.Name] = m;
                });
                this.comboBox1.Items.AddRange(pano72Yuns.Keys.ToList().ToArray());
                if (panoItems.Count > 0)
                {
                    this.comboBox1.SelectedItem = panoItems[0].Name;
                } 
            }
        }

        private string panoSource;
        void form_Closing(object sender, CancelEventArgs e)
        {

            var itemSource = JsonHelper.JsonSerializer(pano72Yuns.Values.ToList());
            using (var stream = new FileStream(this.panoSource, FileMode.OpenOrCreate))
            {
                var buffer = System.Text.UTF8Encoding.UTF8.GetBytes(itemSource);
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        void tooYun720_OnCompleteHandler()
        {
            this.BeginInvoke(
                new MethodInvoker(
                    () => this.toolStripStatusLabel1.Text = "处理完成"));
        }

        private void tooYun720_OnProcessImageHandler(CubeImage cubeImage, string panId)
        {
            this.BeginInvoke(
                new MethodInvoker(
                    () => this.toolStripStatusLabel1.Text = string.Format("正在处理{0}第{1}数据", panId, cubeImage.Level)));
        }

        void chromeWebBrowser1_BrowserNewWindow(object sender, NewWindowEventArgs e)
        {
            Console.WriteLine(e);
        }

        private bool reload = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBox1.Text))
            {
                chromeWebBrowser1.OpenUrl(this.textBox1.Text);
                var query = this.textBox1.Text.Substring(this.textBox1.Text.LastIndexOf("/")+1);
                if (query.Split('?').Length >= 2)
                {
                    this.key.Text = query.Split('?')[0];
                    this.pano_id.Text = query.Split('?')[1].Split('=')[1];
                    var pano = yun720Tool.GetPicUrlByKey(this.key.Text, this.pano_id.Text);
                    this.dataGridView1.Tag = pano;
                    reload = true;
                    this.dataGridView1.DataSource = pano.SceneItems;
                }
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            var pano = this.dataGridView1.Tag as Pano72Yun;

            if (pano != null )
            {
                this.pano72Yuns[this.chromeWebBrowser1.Title] = pano;
                pano.Name = this.chromeWebBrowser1.Title;
                this.comboBox1.Items.Clear();
                this.comboBox1.Items.AddRange(this.pano72Yuns.Keys.ToArray());
                this.comboBox1.SelectedItem = this.chromeWebBrowser1.Title;
                new Thread(() =>
                {
                    tooYun720.DownLoadFile(pano);
                }).Start();

            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            return;
           var item = this.dataGridView1.CurrentRow.DataBoundItem as SceneItem;
            var pano = this.dataGridView1.Tag as Pano72Yun;
            if (item != null && pano != null && !reload)
            {
                this.chromeWebBrowser1.OpenUrl(string.Format(@"http://localhost:807/Project/pano/pannellum/app/cubeMap.html?path=/Project/pano/pannellum/app/baidu/720Yun/{0}/{1}/", pano.Id,item.PanoId));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.chromeWebBrowser1.OpenUrl(
                string.Format("http://localhost:807/Project/pano/pannellum/app/baidu/720Yun/{0}/index.html", this.key.Text));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.pano72Yuns.ContainsKey(comboBox1.SelectedItem.ToString()))
            {
                var pano = this.pano72Yuns[comboBox1.SelectedItem.ToString()];
                     
                this.dataGridView1.Tag = pano;
                reload = true;
                this.dataGridView1.DataSource = pano.SceneItems;

                this.key.Text = pano.Id;
                this.pano_id.Text = pano.Key;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(this.tooYun720.GetBaseDir(), this.key.Text);
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }
}
