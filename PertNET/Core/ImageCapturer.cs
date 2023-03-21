//-----------------------------------------------------------------------
// <copyright file="ImageCapturer.cs" company="Lifeprojects.de">
//     Class: ImageCapturer
//     Copyright © Lifeprojects.de 2023
// </copyright>
//
// <Framework>4.8</Framework>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>13.01.2023 15:15:14</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class ImageCapturer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageCapturer"/> class.
        /// </summary>
        public ImageCapturer()
        {
        }

        public static void SaveToBmp(FrameworkElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        public static void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                   (int)visual.ActualWidth,
                   (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }
    }
}
