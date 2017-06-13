using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MapDataTools;

namespace NetposaTest
{
    public class PanoItem
    {
        public string Code { get; set; }
        public string RoadName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Coord Coord { get; set; }
        public Object MoveDir { get; set; }
        public Object NorthDir { get; set; }
        public Object Pitch { get; set; }
        public Object Heading { get; set; }
        public List<string> Panos { get; set; }
        /// <summary>
        /// 计算百度街景初始方向
        /// </summary>
        /// <returns></returns>
        public double GetYaw()
        {
            var north = double.Parse(this.NorthDir.ToString()) + double.Parse(this.MoveDir.ToString());
            if (north > 180)
            {
                north = -(360 - north + 90);
            }
            return north;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        public void SaveConfig(string directory)
        {
            var config = new
            {
                autoLoad = true,
                autoRotate = true,
                type = "cubemap",
                pitch = Pitch,
              //  minPitch = -34,
                yaw = GetYaw(),
                fov= 100,
                northOffset = 0,
                basePath = "",
                cubeMap = new string[]
                {
                    "l.jpg",
                    "f.jpg",
                    "r.jpg",
                    "b.jpg",
                    "u.jpg",
                    "d.jpg"
                }
            };

            var content = JsonHelper.ToJson(config);
            var buffer = System.Text.ASCIIEncoding.UTF8.GetBytes(content);
            using (
                var fileStream = new FileStream(Path.Combine(directory, Code, "tour.json"),
                    FileMode.OpenOrCreate))
            {
                fileStream.Write(buffer, 0, buffer.Length);
            }

            content = CreateHtmlDemo(content);
            buffer = ASCIIEncoding.UTF8.GetBytes(content);
            using (var fileStream = new FileStream(Path.Combine(directory, Code, "index.html"), FileMode.OpenOrCreate))
            {
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }

        private string CreateHtmlDemo(string tour)
        {
            var html = new StringBuilder();

            html.Append("            <!DOCTYPE HTML>");
            html.Append("<html>                                                                                 ").AppendLine();
            html.Append("                                                                                       ").AppendLine();
            html.Append("<head>                                                                                 ").AppendLine();
            html.Append("    <meta charset=\"utf-8\">                                                           ").AppendLine();
            html.Append("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">         ").AppendLine();
            html.AppendFormat("    <title>{0}</title>                                             ",this.RoadName).AppendLine();
            html.Append(
                "    <link type=\"text/css\" rel=\"Stylesheet\" href=\"http://cdn.bootcss.com/pannellum/2.3.2/pannellum.css\" />").AppendLine();
 
            html.Append("</head>                                                                                ").AppendLine();
            html.Append("                                                                                       ").AppendLine();
            html.Append("<body>                                                                                 ").AppendLine();
            html.Append(
                "    <div id=\"container\" style=\"left: 0px;bottom:0px;position: absolute;\">                     ").AppendLine();
            html.Append("        <noscript>                                                                     ").AppendLine();
            html.Append("            <div class=\"pnlm-info-box\">                                              ").AppendLine();
            html.Append("                <p>Javascript is required to view this panorama.                       ").AppendLine();
            html.Append("                    <br>(It could be worse; you could need a plugin.)</p>              ").AppendLine();
            html.Append("            </div>                                                                     ").AppendLine();
            html.Append("        </noscript>                                                                    ").AppendLine();
            html.Append("    </div>                                                                             ").AppendLine();
            html.AppendLine("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>").AppendLine();
            html.Append(
                "    <script type=\"text/javascript\" src=\"//cdn.bootcss.com/pannellum/2.3.2/pannellum.js\"></script>              ").AppendLine();
            html.Append("    <script type=\"text/javascript\">                                                  ").AppendLine();
            html.Append("var opt = " + tour + ";").AppendLine();
            html.Append(" var viewer = pannellum.viewer('container', opt);").AppendLine();
            html.Append(" if (opt.scenes) {       ").AppendLine();
            html.Append("var div = document.createElement('div');     ").AppendLine();
            html.Append(
                "  div.style = \"text-align: center;position: absolute; z-index: 3001; overflow: hidden; opacity: 1; cursor: default; pointer-events: auto; background: none rgba(0, 0, 0, 0.3); border-style: solid; border-color: rgb(0, 0, 0); border-width: 0px; border-radius: 0px; width: 1440px; height: 140px; transform: translateZ(1e+12px) translate(0px, 670px) translate(-720px, 50px) rotate(0deg) translate(720px, -50px);\"").AppendLine();
            html.Append("  $(\"#container\").append($(div));                    ").AppendLine().AppendLine();
            html.Append("  for (var sence in opt.scenes) {                      ").AppendLine();
            html.Append("      var img = document.createElement('img');         ").AppendLine();
            html.Append("      img.src = sence + \"/thumb.jpg\";                ").AppendLine();
            html.Append("      img.style.backgroundImage = 'url('+img.src+')';  ").AppendLine();
            html.Append("      img.width = 80;                                  ").AppendLine();
            html.Append("      img.id = sence;                                  ").AppendLine();
            html.Append("      img.title = opt.scenes[sence].title;             ").AppendLine();
            html.Append("      img.height = 80;                                 ").AppendLine();
            html.Append("      img.style.zIndex = 3001                          ").AppendLine();
            html.Append("      img.style.cursor = 'default';                    ").AppendLine();
            html.Append("      img.style.marginLeft = '20px'                    ").AppendLine();
            html.Append("                                                       ").AppendLine();
            html.Append("      $(div).append(img);                              ").AppendLine();
            html.Append("  }                                                    ").AppendLine();
            html.Append("  $('img',$(div)).click(function(){                    ").AppendLine();
            html.Append("  	viewer.loadScene($(this)[0].id)                     ").AppendLine();
            html.Append("  })                                                   ").AppendLine();
            html.Append(" }                                                     ").AppendLine();
            html.Append("    </script>").AppendLine();
            html.Append("</body>").AppendLine();
            html.Append("").AppendLine();
            html.Append("</html>").AppendLine();
            return html.ToString();
        }
    }

    public class PanoTool
    {
        private string codeUrl =
            "http://pcsv0.map.bdimg.com/?udt=20161226&qt=qsdata&x={0}&y={1}&l=16&action=0&mode=day&t=1496371919247";

        private string panno = "http://pcsv1.map.bdimg.com/?qt=pdata&sid={0}&pos=0_0&z=1&udt=20161226";
        private string multiRes = "http://pcsv0.map.bdimg.com/?qt=pdata&sid={0}&pos={1}_{2}&z=4&udt=20161226";

        private string panoInfo =
            "http://pcsv0.map.bdimg.com/?udt={1}&qt=sdata&pc=1&sid={0}&t={2}";

        public PanoTool()
        {

        }

        private void ResolvePanoInfo(PanoItem item)
        {
            string url = string.Format(this.panoInfo, item.Code, DateTime.Now.ToString("YY-MMM-DD"), DateTime.Now.Ticks);
            var content = HttpHelper.GetRequestContent(url);
            var dic = JsonHelper.JsonDeserialize<Dictionary<string, object>>(content);
            dic = (Dictionary<string, object>) (((ArrayList) dic["content"])[0]);

            item.MoveDir = dic["MoveDir"];
            item.NorthDir = dic["NorthDir"];
            item.Pitch = dic["Pitch"];
            item.Heading = dic["Heading"];

            var list = dic["Roads"] as ArrayList;
            if (list != null && list.Count > 0)
            {
                dic = list[0] as Dictionary<string, object>;
                if (dic != null)
                {
                    list = dic["Panos"] as ArrayList;
                    if (list != null && list.Count != 0)
                    {
                       item.Panos = new List<string>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            dic = (Dictionary<string, object>) list[i];
                            item.Panos.Add(string.Join("&", new[] { dic["PID"], dic["X"], dic["Y"] }));
                        }
                    }
                }
            }
        }

        public PanoTool(string basePath)
        {
            this.directory = basePath;
        }

        public PanoItem GetPannoCode(int x, int y)
        {
            var url = string.Format(codeUrl, x, y);
            var content = JsonHelper.JsonDeserialize<Dictionary<string, object>>(HttpHelper.GetRequestContent(url));
            if (((Dictionary<string, object>) (content["result"]))["error"].ToString() == "0")
            {
                var dic = ((Dictionary<string, object>) content["content"]);
                PanoItem item = new PanoItem()
                {
                    X = (int)dic["x"],
                    Y = (int)dic["y"]
                };
                item.Code = dic["id"].ToString();
                item.RoadName = dic["RoadName"].ToString();

                ResolvePanoInfo(item);

                return item;
            }
            return null;
        }

        public void GeneratePano(PanoItem item)
        {
            string code = item.Code;
            DownLoadPano(code);
            WriteImage();
            // RunCmd(code);
            var path = Path.Combine(this.directory, item.Code);
            CreateCubic(Path.Combine(path, "panno.png"), path);
            item.SaveConfig(this.directory);
        }

         

        private string directory;
        private void DownLoadPano(string code)
        {
            this.basePath = Path.Combine(this.directory, code);
            if (!Directory.Exists(this.basePath))
            {
                Directory.CreateDirectory(this.basePath);
            }
            
            this.DownLoadPic(string.Format(panno, code), string.Format("preview.png"));

            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxColumn; j++)
                {
                    this.DownLoadPic(string.Format(multiRes, code, i, j), string.Format("{0}_{1}.png", i, j));
                }
            }
        }

        private string basePath = @"C:\Users\Administrator\Desktop\baidu";

        private void DownLoadPic(string url, string saveFile)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, Path.Combine(basePath, saveFile));
            }
        }

        private int maxRow = 4;
        private int maxColumn = 8;

        private void WriteImage()
        {
            using (var image = new System.Drawing.Bitmap(512*maxColumn, 512*maxRow))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    for (int i = 0; i < maxRow; i++)
                    {
                        for (int j = 0; j < maxColumn; j++)
                        {
                            using(var img = Image.FromFile(Path.Combine(basePath, string.Format("{0}_{1}.png", i, j))))
                            {
                                g.DrawImage(img,512*j, 512*i, 512, 512);
                            }
                        }
                    }
                }
                image.Save(Path.Combine(basePath, "panno.png"));
            }
            // 删除图片
            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxColumn; j++)
                {
                    if (File.Exists(Path.Combine(basePath, string.Format("{0}_{1}.png", i, j))))
                    {
                        File.Delete(Path.Combine(basePath, string.Format("{0}_{1}.png", i, j)));
                    }
                }
            }
        }
        // cube image 配置
        /// <summary>
        ///  "l.jpg",
            //"f.jpg",
            //"r.jpg",
            //"b.jpg",
            //"u.jpg",
            //"d.jpg"
        /// </summary>
        /// <param name="code"></param>
        private void RunCmd(string code)
        {
            string cmdExe;
            string cmdStr;
            try
            {
                cmdExe = @"E:\Project\pano\pannellum\utils\multires\generate.py";
                cmdStr =
                    string.Format(
                        @"{1}\{0}\panno.png -o {1}\{0}",
                        code,this.directory);
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format(@"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit");

                    myPro.StandardInput.WriteLine(str);
                    myPro.StandardInput.AutoFlush = true;
                    
                   // myPro.WaitForExit();
                }
            }
            catch
            {

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="origFilename">全景图片</param>
        /// <param name="output">输出位置</param>
        /// <param name="nonaPath">nona位置</param>
        public void CreateCubic(string origFilename, String output,string nonaPath = "C:/Program Files/Hugin/bin/nona.exe")
        {
            int tileSize = 512;
            var img = Image.FromFile(origFilename);
            double origWidth = img.Width;
            double origHeight = img.Height;
            img.Dispose();
            int cubeSize = 8*(int) (origWidth/Math.PI/8);
            int levels = (int) (Math.Ceiling((Math.Log((float) (cubeSize)/tileSize, 2)))) + 1;
            var faceLetters = new char[] {'f', 'b', 'u', 'd', 'l', 'r'};
            var text = new List<string>();
            text.Add("p E0 R0 f0 h" + cubeSize + " n\"TIFF_m\" u0 v90 w" + cubeSize);
            text.Add("m g1 i0 m2 p0.00784314");
            text.Add("i a0 b0 c0 d0 e0 f4 h" + origHeight + " n\"" + origFilename + "\" p0 r0 v360 w" + origWidth +
                        " y0");
            text.Add("i a0 b0 c0 d0 e0 f4 h" + origHeight + " n\"" + origFilename + "\" p0 r0 v360 w" + origWidth +
                        " y180");
            text.Add("i a0 b0 c0 d0 e0 f4 h" + origHeight + " n\"" + origFilename + "\" p-90 r0 v360 w" + origWidth +
                        " y0");
            text.Add("i a0 b0 c0 d0 e0 f4 h" + origHeight + " n\"" + origFilename + "\" p90 r0 v360 w" + origWidth +
                        " y0");
            text.Add("i a0 b0 c0 d0 e0 f4 h" + origHeight + " n\"" + origFilename + "\" p0 r0 v360 w" + origWidth +
                        " y90");
            text.Add("i a0 b0 c0 d0 e0 f4 h" + origHeight + " n\"" + origFilename + "\" p0 r0 v360 w" + origWidth +
                        " y-90");
            text.Add("v");
            text.Add("*");
             
            using (var stream = new FileStream(Path.Combine(output, "cubic.pto"), FileMode.OpenOrCreate))
            {
                using (var write = new StreamWriter(stream))
                {
                    text.ForEach(write.WriteLine);
                }
            }
            
            var cmdStr = string.Format(@"-o {0}\face {0}\cubic.pto", output);
            
            using (var myPro = new Process())
            {
                myPro.StartInfo.FileName = "cmd.exe";
                myPro.StartInfo.UseShellExecute = false;
                myPro.StartInfo.RedirectStandardInput = true;
                myPro.StartInfo.RedirectStandardOutput = true;
                myPro.StartInfo.RedirectStandardError = true;
                myPro.StartInfo.CreateNoWindow = true;
                myPro.Start();
                //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                string str = string.Format(@"""{0}"" {1} {2}", nonaPath, cmdStr, "&exit");

                myPro.StandardInput.WriteLine(str);
                myPro.StandardInput.AutoFlush = true;

                myPro.WaitForExit();
            }
            var faces = new string[]
            {
                "face0000.tif", "face0001.tif", "face0002.tif", "face0003.tif", "face0004.tif", "face0005.tif"
            };
            Graphics g;
            Image newImage;
            for (int i = 0; i < faceLetters.Length; i++)
            {
                using (newImage = new Bitmap(1024, 1024))
                {
                    using (img = Image.FromFile(Path.Combine(output, faces[i])))
                    {
                        using (g = Graphics.FromImage(newImage))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.CompositingQuality = CompositingQuality.HighQuality;

                            g.DrawImage(img, 0, 0, 1024, 1024);
                        }
                    }
                    if (faceLetters[i] == 'u')
                    {
                        newImage.RotateFlip(RotateFlipType.Rotate90FlipXY);
                    }
                    if (faceLetters[i] == 'd')
                    {
                        newImage.RotateFlip(RotateFlipType.Rotate270FlipY);
                    }
                    newImage.Save(Path.Combine(output, faceLetters[i] + ".jpg"),ImageFormat.Jpeg);
                }
            }

            faces.ToList().ForEach(f =>
            {
                File.Delete(Path.Combine(output,f));
            });
            File.Delete(Path.Combine(output, "cubic.pto"));

            var config = new
            {
                autoLoad = true,
                autoRotate = true,
                type = "cubemap",
                // pitch= pitch,
                minPitch = -30,
                //  yaw= yaw,
                // fov= fov,
                //  northOffset= northOffset,
                basePath = "",
                cubeMap = new string[]
                {
                    "l.jpg",
                    "f.jpg",
                    "r.jpg",
                    "b.jpg",
                    "u.jpg",
                    "d.jpg"
                }
            };
        }

        public void DownloadBaiduRoomData(Extent extent)
        {
            string uidUrl = "http://pcsv0.map.bdimg.com/?qt=ck&tid=23686_7882_19_1033";
            string scapeUrl = "http://pcsv0.map.bdimg.com/scape/?udt=201400606&qt=idata&iid=a15545aaed33ff9f5fbe8646";
        }
    }
}
