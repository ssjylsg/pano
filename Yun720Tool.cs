using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetposaTest
{
    public class SceneItem 
    {
        public SceneItem()
        {
            CubeImages = new List<CubeImage>();
        }
        public string PreViewUrl { get; set; }
        public string PanoId { get; set; }
        public int Tilesize { get; set; }
        public string Thumb { get; set; }
        public string Title { get; set; }
        public List<CubeImage> CubeImages { get; set; } 
    }

    public class Pano72Yun
    {
        [NonSerialized] 
        private string _config;

        public string Config
        {
            get { return _config; }
            set { this._config = value; }
        }

    
        public string Key { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<SceneItem> SceneItems { get; set; }

        public void SaveConfig(string basePath)
        {
            var direct = Path.Combine(basePath, Id);
            if (!Directory.Exists(direct))
            {
                Directory.CreateDirectory(direct);
            }
            if (!string.IsNullOrEmpty(this.Config))
            {
                using (var stream = new FileStream(Path.Combine(direct, "config.xml"), FileMode.OpenOrCreate))
                {
                    var buffer = System.Text.ASCIIEncoding.UTF8.GetBytes(this.Config);
                    stream.Write(buffer,0,buffer.Length);
                }
            }

            SavePannellumConfig(direct);
        }

        public void SavePannellumConfig(string basePath)
        {
            dynamic p;
            if (this.SceneItems.Count == 1)
            {
                p = new
                {
                    autoLoad = true,
                    autoRotate = true,
                    title = this.SceneItems[0].Title,
                    author = "netposa",
                    type = "cubemap",
                    basePath = string.Format("{0}/", this.SceneItems[0].PanoId),
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
            else
            {
                var dic = new Dictionary<string, dynamic>();
                this.SceneItems.ForEach(item =>
                {
                    dic[item.PanoId] = new
                    {
                        title = item.Title,
                        type = "cubemap",
                        basePath = string.Format("{0}/", item.PanoId),
                        thumb = item.Thumb,
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
                });
                p = new
                {
                    autoLoad = true,
                    autoRotate = true,
                    @default = new
                    {
                        firstScene = this.SceneItems[0].PanoId,
                        sceneFadeDuration = 2000
                    },
                    scenes = dic
                };
            }

            var content = JsonHelper.ToJson(p);
            var buffer = ASCIIEncoding.UTF8.GetBytes(content);
            using (var fileStream = new FileStream(Path.Combine(basePath, "tour.json"), FileMode.OpenOrCreate))
            {
                fileStream.Write(buffer, 0, buffer.Length);
            }
            content = CreateHtmlDemo(content);
            buffer = ASCIIEncoding.UTF8.GetBytes(content);
            using (var fileStream = new FileStream(Path.Combine(basePath, "index.html"), FileMode.OpenOrCreate))
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
            html.AppendFormat("    <title>{0}</title>                                                 ",this.Name).AppendLine();
            html.Append(
                "    <link type=\"text/css\" rel=\"Stylesheet\" href=\"/Project/pano/pannellum/src/css/pannellum.css\" />").AppendLine();
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
            html.AppendLine("<script src=\"http://cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>").AppendLine();
            html.Append(
                "    <script type=\"text/javascript\" src=\"http://cdn.bootcss.com/pannellum/2.3.2/pannellum.js\"></script>              ").AppendLine();
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
    public class CubeImage : IComparer<CubeImage>
    {
        
        public string Url { get; set; }
        public int Level { get; set; }
        public string TiledImageHeight { get; set; }
        public string TiledImageWidth { get; set; }
        public string ResolveUrl(char s,int i,int j)
        {
            string tempUrl = "";
            if (this.Url.Contains("%0v"))
            {
                tempUrl = this.Url.Replace("%s", "{0}").Replace("%0v", "{1}").Replace("%0h", "{2}");
                return  string.Format(tempUrl, s, i.ToString().PadLeft(2, '0'), j.ToString().PadLeft(2, '0'));
            }
            tempUrl = this.Url.Replace("%s", "{0}").Replace("%v", "{1}").Replace("%h", "{2}");
            return string.Format(tempUrl,s,i,j);
        }

        public int Compare(CubeImage x, CubeImage y)
        {
            return x.Level - y.Level;
        }
    }
   public class Yun720Tool
   {
       private string configUrl = "http://xml.qncdn.720static.com/@/{0}/{0}.xml";

       public Pano72Yun GetPicUrlByKey(string key, string pano_id)
       {
           var list = new List<SceneItem>();
           var content = HttpHelper.GetRequestContent(string.Format(configUrl, key));
           XmlDocument xmlDocument = new XmlDocument();
           xmlDocument.LoadXml(content);
           var scenes = xmlDocument.SelectNodes("krpano/scene");
           // 解析场景
           for (int i = 0; i < scenes.Count; i++)
           {
               var item = new SceneItem()
               {
                   PanoId = scenes[i].Attributes["pano_id"].InnerText,
                   PreViewUrl = scenes[i].SelectSingleNode("preview").Attributes["url"].InnerText
               };
               item.Tilesize = int.Parse(scenes[i].SelectSingleNode("image").Attributes["tilesize"].InnerText);
               var nodes = scenes[i].SelectSingleNode("image").SelectNodes("level");
               for (int j = 0; j < nodes.Count; j++)
               {
                   
                   var url = nodes[j].SelectSingleNode("cube").Attributes["url"].InnerText;
                   item.CubeImages.Add(new CubeImage()
                   {
                       Url = url,
                       TiledImageHeight = nodes[j].Attributes["tiledimageheight"].InnerText,
                       TiledImageWidth = nodes[j].Attributes["tiledimagewidth"].InnerText,
                       Level = int.Parse(url.Substring(url.IndexOf("/imgs/%s/") + "/imgs/%s/".Length, 2).Trim('l'))
                   });
               }
               list.Add(item);
           }
           // 解析场景thumb
           var config = xmlDocument.SelectNodes("krpano/config/thumbs/category/pano");
           var dic = new Dictionary<string, dynamic>();
           for (int i = 0; i < config.Count; i++)
           {
               dic[config[i].Attributes["pano_id"].InnerText] = new
               {
                   thumb = config[i].Attributes["thumb"].InnerText,
                   title = config[i].Attributes["title"].InnerText,
               };
           }
           list.ForEach(scene =>
           {
               scene.Thumb = dic[scene.PanoId].thumb;
               scene.Title = dic[scene.PanoId].title;
           });
           return new Pano72Yun() { Config = content, SceneItems = list, Key = pano_id, Id= key };
       }
   }
}
