//-----------------------------------------------------------------------
// <copyright file="ToolsGraphic.cs" company="Lifeprojects.de">
//     Class: ToolsGraphic
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>24.02.2022</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------


namespace PertNET.Core
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Versioning;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using EasyPrototypingNET.Graphics;

    [SupportedOSPlatform("windows")]
    public class ToolsGraphic
    {
        private static readonly object _Lock = new object();
        private static ToolsGraphic instance;


        /// <summary>
        /// Die Methode gibt eine Instanz der Klasse ToolsGraphic zurück.
        /// </summary>
        /// <returns></returns>
        public static ToolsGraphic Instance()
        {
            lock (_Lock)
            {
                if (instance == null)
                {
                    instance = new ToolsGraphic();
                }
            }

            return instance;
        }

        public static BitmapSource ResizeToBitmapSource(BitmapSource source, int width, int height)
        {
            TransformedBitmap transformBitmap = new TransformedBitmap(source,
                                                      new ScaleTransform(width / source.PixelWidth,
                                                                         height / source.PixelHeight,
                                                                         0, 0));
            return transformBitmap;
        }

        /// <summary>
        /// Obtient le thumnail de l'image source (incluant la rotation)
        /// </summary>
        /// <param name="pMediaUrl"></param>
        /// <returns></returns>
        public static BitmapSource GetThumbnail(string pMediaUrl)
        {
            ExifOrientations orientation = ExifOrientations.Normal;
            BitmapSource ret = null;
            BitmapMetadata meta = null;
            double angle = 0;

            try
            {
                BitmapFrame frame = BitmapFrame.Create(
                    new Uri(pMediaUrl),
                    BitmapCreateOptions.DelayCreation,
                    BitmapCacheOption.None);

                if (frame.Thumbnail == null)
                {
                    BitmapImage image = new BitmapImage();
                    image.DecodePixelHeight = 90; 
                    image.BeginInit();
                    image.UriSource = new Uri(pMediaUrl);
                    image.CacheOption = BitmapCacheOption.None;
                    image.CreateOptions = BitmapCreateOptions.DelayCreation;
                    image.EndInit();

                    if (image.CanFreeze)
                    {
                        image.Freeze();
                    }

                    ret = image;
                }
                else
                {
                    meta = frame.Metadata as BitmapMetadata;
                    ret = frame.Thumbnail;
                }

                if ((meta != null) && (ret != null))
                {
                    if (meta.GetQuery("/app1/ifd/{ushort=274}") != null)
                    {
                        orientation = (ExifOrientations)Enum.Parse(typeof(ExifOrientations), meta.GetQuery("/app1/ifd/{ushort=274}").ToString());
                    }

                    switch (orientation)
                    {
                        case ExifOrientations.Rotate90:
                            angle = -90;
                            break;
                        case ExifOrientations.Rotate180:
                            angle = 180;
                            break;
                        case ExifOrientations.Rotate270:
                            angle = 90;
                            break;
                    }

                    if (angle != 0)
                    {
                        ret = new TransformedBitmap(ret.Clone(), new RotateTransform(angle));
                        ret.Freeze();
                    }
                }
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
            }

            return ret;
        }

        public static BitmapImage LoadBitmapImage(string imageAbsolutePath,
                                                 BitmapCacheOption bitmapCacheOption = BitmapCacheOption.None)
        {
            BitmapImage image = new BitmapImage();

            if (File.Exists(imageAbsolutePath) == true)
            {
                image.BeginInit();
                image.CacheOption = bitmapCacheOption;
                image.UriSource = new Uri(imageAbsolutePath);
                image.EndInit();
            }

            return image;
        }

        public static int HexColorToInt(string pHexColor)
        {
            int result = 0;
            result = Convert.ToInt32(pHexColor.Substring(1), 16);

            return result;
        }

        public static System.Drawing.Color IntToColor(int pIntColor)
        {
            System.Drawing.Color result = System.Drawing.Color.FromArgb(pIntColor);
            return result;
        }

        public static string IntToHexColor(int pIntColor)
        {
            string result = string.Empty;
            if (pIntColor == 0 || pIntColor == 16777215)
            {
                result = "00FFFFFF";
            }
            else
            {
                result = System.Drawing.Color.FromArgb(pIntColor).Name;
            }

            return string.Format("#{0}", result);
        }

        public static byte[] ImageToByteArray(System.Drawing.Image pBitmapSource, System.Drawing.Imaging.ImageFormat pImageFormat)
        {
            MemoryStream memStream = new MemoryStream();
            pBitmapSource.Save(memStream, pImageFormat);
            memStream.Flush();
            return memStream.ToArray();
        }

        public static byte[] BitmapSorceToByteArray(BitmapSource pBitmapSource)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(pBitmapSource));
            encoder.QualityLevel = 100;
            byte[] bit = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(pBitmapSource));
                encoder.Save(stream);
                bit = stream.ToArray();
                stream.Close();
            }

            return bit;
        }

        public static Bitmap ByteArrayToBitmap(byte[] pSource)
        {
            ImageConverter ic = new ImageConverter();
            System.Drawing.Image img = (System.Drawing.Image)ic.ConvertFrom(pSource);
            Bitmap bitmap = new Bitmap(img);
            return bitmap;
        }

        public static BitmapSource ByteArrayToBitmapSource(byte[] pSource)
        {
            var stream = new MemoryStream(pSource);
            return System.Windows.Media.Imaging.BitmapFrame.Create(stream);
        }

        public static System.Drawing.Image LoadImageNoLock(string path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path)); 
            return System.Drawing.Image.FromStream(ms);
        }

        public static BitmapImage ImageSourceToBitmapImage(ImageSource pImageSource)
        {
            BitmapImage retVal = null;

            retVal = new BitmapImage(new Uri(pImageSource.ToString()));

            return retVal;
        }

        public static BitmapFrame ResizedToBitmapFrame(ImageSource source, int width, int height, int margin)
        {
            var rect = new System.Windows.Rect(margin, margin, width - (margin * 2), height - (margin * 2));

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawDrawing(group);
            }

            var resizedImage = new RenderTargetBitmap(width, height,  96, 96, PixelFormats.Default); 
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }
    }
}