using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MapDataTools;
using NUnit.Framework;

namespace NetposaTest
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception ErrorMsg { get; set; }
    }
   public class Yun720
   {
       public Yun720()
       {
           
       }

       private string baseDir;

       public string GetBaseDir()
       {
           return this.baseDir;
       }
       public Yun720(string baseDir)
       {
           this.baseDir = baseDir;
       }
       // http://pano20.qncdn.720static.com/resource/prod/171i1da9a82/44e2awrO916/3523423/imgs/%s/l4/%0v/l4_%s_%0v_%0h.jpg

       //front, back, up, down, left, right
       char[] directory = new char[] { 'f', 'b', 'u', 'd', 'l', 'r' };
       int[] zooms = new[] {1, 2, 3, 4 };
       private string url =
           "http://pano18.qncdn.720static.com/resource/prod/af3ic822ff4/7c42f9idj46/3497304/imgs/{3}/l{0}/0{1}/l{0}_{3}_0{1}_0{2}.jpg";
       //[Test]
       //public void DownLoadFile(string tempUrl)
       //{
       //    string fileName;
       //    this.url = tempUrl;
       //    for (int zoom = 0; zoom < zooms.Length; zoom++)
       //    {
       //        var currentZoom = zooms[zoom];
       //        string directName = Path.Combine(this.basePath, currentZoom.ToString());
       //        if (!Directory.Exists(directName))
       //        {
       //            Directory.CreateDirectory(directName);
       //        }

       //        for (int i = 1; i < zooms[zoom] * 2; i++)
       //        {
       //            for (int j = 1; j < zooms[zoom] * 2; j++)
       //            {
       //                for (int k = 0; k < directory.Length; k++)
       //                {
       //                    fileName = string.Format("{2}{0}_{1}.jpg",  i - 1,j - 1, directory[k]);
       //                    this.DownLoadPic(string.Format(url, zooms[zoom], i, j, directory[k]),Path.Combine(directName,fileName));   
       //                }
       //            }
       //        }
       //    }
       //}
      // private string basePath = @"E:\Project\pano\pannellum\app\baidu\720Yun\output";

       private bool DownLoadPic(string url, string saveFile,int count = 0)
       {
           
           try
           {
               using (var client = new WebClient())
               {
                   client.DownloadFile(url, saveFile);
               }
               return true;
           }
           catch (Exception)
           {
               if (count > 3)
               {
                   return false;
               }
               count++;
               if (!DownLoadPic(url, saveFile, count))
               {
                   Console.WriteLine(url);
               }
               else
               {
                   return true;
               }
           }
           return false;
       }
       //[Test]
       //public void GenerateFile()
       //{
       //    int maxColumn = 9;
       //    int maxRow = 9;
       //    basePath = @"C:\Users\Administrator\Desktop\baidu\";

       //    for (int d = 0; d < directory.Length; d++)
       //    {
       //        using (var image = new System.Drawing.Bitmap(512 * maxColumn, 512 * maxRow))
       //        {
       //            using (Graphics g = Graphics.FromImage(image))
       //            {
       //                g.SmoothingMode = SmoothingMode.AntiAlias;
       //                for (int i = 1; i <= maxRow; i++)
       //                {
       //                    for (int j = 1; j <= maxColumn; j++)
       //                    {
       //                        g.DrawImage(Image.FromFile(Path.Combine(basePath, string.Format("4_{2}_{0}_{1}.jpg", i, j,directory[d]))),
       //                            512 * (j - 1), 512 * (i - 1), 512, 512);
       //                    }
       //                }
       //            }
       //            image.Save(Path.Combine(basePath, directory[d]+"_panno.png"));
       //        }
       //    }
       //}

       public delegate void ProcessImage(CubeImage cubeImage, string PanoId);

       public delegate void Complete();

       public event ProcessImage OnProcessImageHandler;
       public event Complete OnCompleteHandler;
       public event EventHandler<ErrorEventArgs> OnError;

       protected virtual void OnOnError(Exception errorMsg)
       {
           var handler = OnError;
           if (handler != null) handler(this, new ErrorEventArgs() { ErrorMsg = errorMsg });
       }

       protected virtual void OnOnCompleteHandler()
       {
           Complete handler = OnCompleteHandler;
           if (handler != null) handler();
       }

       protected virtual void OnOnProcessImageHandler(CubeImage cubeimage, string PanoId)
       {
           ProcessImage handler = OnProcessImageHandler;
           if (handler != null) handler(cubeimage, PanoId);
       }

       private bool CheckImage(string path)
       {
           for (int d = 0; d < directory.Length; d++)
           {
               if (!File.Exists(Path.Combine(path, this.directory[d] + ".jpg")))
               {
                   return false;
               }
           }
           return true;
       }

       public void DownLoadFile(Pano72Yun pano72Yun)
       {
           string fileName;
           string tempUrl;
           pano72Yun.SaveConfig(this.baseDir);
           var basePath = Path.Combine(this.baseDir, pano72Yun.Id);


           

           foreach (SceneItem item in pano72Yun.SceneItems)
           {
               this.DownLoadPic(item.Thumb, Path.Combine(Path.Combine(basePath, item.PanoId), "thumb.jpg"));
               item.CubeImages.Sort((image, cubeImage) =>
               {
                   return image.Level - cubeImage.Level;
               });

               var maxZoom = Math.Min(item.CubeImages[item.CubeImages.Count - 1].Level,3);
               var max = 0;
               foreach (CubeImage cubeImage in item.CubeImages)
               {
                   var currentZoom = cubeImage.Level;
                   if (maxZoom != currentZoom)
                   {
                       continue;
                   }
                   OnOnProcessImageHandler(cubeImage, item.PanoId);
                   string directName = Path.Combine(basePath, item.PanoId, currentZoom.ToString());
                   if (!Directory.Exists(directName))
                   {
                       Directory.CreateDirectory(directName);
                   }

                   if (this.CheckImage(Path.Combine(basePath, item.PanoId)))
                   {
                       continue;
                   }
                   max = Math.Min((int)Math.Pow(2, cubeImage.Level), 16);

                   for (int i = 1; i < max; i++)
                   {
                       for (int j = 1; j < max; j++)
                       {
                           for (int k = 0; k < directory.Length; k++)
                           {
                               fileName = Path.Combine(directName,string.Format("{2}{0}_{1}.jpg", i , j , directory[k]));
                               if (!File.Exists(fileName))
                               {
                                   if (! this.DownLoadPic(cubeImage.ResolveUrl(directory[k], i, j),
                                       fileName))
                                   {
                                       max = Math.Max(i, j);
                                       break;
                                   }
                               }
                           }
                       }
                   }
               }

               GenerateFile(item, Path.Combine(basePath, item.PanoId), max, maxZoom);
               OnOnCompleteHandler();
           }
       }


       public void GenerateFile(SceneItem item, string scenePath, int max, int maxZoom)
       {
           GC.Collect();
           CubeImage cubeImage = item.CubeImages[maxZoom -1];
           int otherWidth = int.Parse(cubeImage.TiledImageWidth)%item.Tilesize;
           for (int d = 0; d < directory.Length; d++)
           {
               if (File.Exists(Path.Combine(scenePath, directory[d] + ".jpg")))
               {
                   continue;
               }
               using (var image = new Bitmap(int.Parse(cubeImage.TiledImageWidth),int.Parse(cubeImage.TiledImageHeight)))
               {
                   using (Graphics g = Graphics.FromImage(image))
                   {
                       g.SmoothingMode = SmoothingMode.AntiAlias;
                        
                       for (int i = 1; i < max; i++)
                       { 
                           for (int j = 1; j < max; j++)
                           {
                               using (
                                   var img =
                                       Image.FromFile(Path.Combine(scenePath, cubeImage.Level.ToString(),
                                           string.Format("{2}{0}_{1}.jpg", i, j, directory[d]))))
                               {

                                   g.DrawImage(img, item.Tilesize*(j - 1), item.Tilesize*(i - 1),
                                       j != max - 1 ? item.Tilesize : otherWidth,
                                       i != max - 1 ? item.Tilesize : otherWidth);

                               }
                           }
                       }
                   }
                   try
                   {
                       if (directory[d] == 'u')
                       {
                           image.Save(Path.Combine(scenePath, directory[d] + "1.jpg"), ImageFormat.Jpeg);
                           image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                       }
                       if (directory[d] == 'd')
                       {
                           image.Save(Path.Combine(scenePath, directory[d] + "1.jpg"), ImageFormat.Jpeg);
                           image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                       }
                   }
                   catch (Exception e)
                   {
                       OnOnError(e);
                   }
                   image.Save(Path.Combine(scenePath, directory[d] + ".jpg"),ImageFormat.Jpeg);
               }
           }
       }
   }
}
