using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetposaTest
{
   public class GooglePano
   {
       private string url =
           "https://geo0.ggpht.com/cbk?output=tile&cb_client=earth.iv&hl=zh-CN&gl=US&panoid={0}&x={1}&y={2}&zoom=3&fover=2&nbt";

       private int maxX = 6;
       private int maxY = 3;
       private string baseDir;
       
       public GooglePano(string baseDir)
       {
           this.baseDir = baseDir;
       }
       public void DownLoad(string code)
       {
           string basePath = Path.Combine(this.baseDir, code);
           if (!Directory.Exists(basePath))
           {
               Directory.CreateDirectory(basePath);
           }
           for (int i = 0; i <= maxX; i++)
           {
               for (int j = 0; j <= maxY; j++)
               {
                   DownLoadPic(string.Format(url, code, i, j),
                       Path.Combine(basePath, string.Format("{0}_{1}.png", i, j)));
               }
           }
           WriteImage(basePath);
       }
       private void DownLoadPic(string url, string saveFile)
       {
           using (var client = new WebClient())
           {
               //client.Proxy = new WebProxy();
               client.DownloadFile(url, saveFile);
           }
       }

       private void WriteImage(string basePath)
       {
           using (var image = new System.Drawing.Bitmap(512 * (maxX + 1), 512 * (maxY +1)))
           {
               using (Graphics g = Graphics.FromImage(image))
               {
                   g.SmoothingMode = SmoothingMode.AntiAlias;
                   for (int i = 0; i < maxX; i++)
                   {
                       for (int j = 0; j < maxY; j++)
                       {
                           using (var img = Image.FromFile(Path.Combine(basePath, string.Format("{0}_{1}.png", i, j))))
                           {
                               g.DrawImage(img, 512 * j, 512 * i, 512, j != 3 ? 512:256);
                           }
                       }
                   }
               }
               image.Save(Path.Combine(basePath, "panno.png"));
           }
           // 删除图片
           for (int i = 0; i < maxX; i++)
           {
               for (int j = 0; j < maxY; j++)
               {
                   if (File.Exists(Path.Combine(basePath, string.Format("{0}_{1}.png", i, j))))
                   {
                       File.Delete(Path.Combine(basePath, string.Format("{0}_{1}.png", i, j)));
                   }
               }
           }
       }
   }
}
