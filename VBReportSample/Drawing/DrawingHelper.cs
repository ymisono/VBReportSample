using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Drawing
{
    static public class DrawingHelper
    {
        static public Stream RenderBitmap(Stream stream, double renderWidth, double renderHeight = 0, double renderDip = 1.0)
        {
            //streamの位置をリセット
            stream.Position = 0;

            #region スケーリング
            //元の画像の入れ先を生成
            using (var sourceImage = new Bitmap(stream))
            {
                //縦横ともに0なら怒る
                if (renderWidth == 0 && renderHeight == 0)
                {
                    throw new ArgumentException("縦横いずれもに0に指定はできません。");
                }
                //アスペクト比を維持する
                double percentHeight, percentWidth;
                if (renderWidth != 0)
                {
                    percentWidth = renderWidth / sourceImage.Width;
                }
                else
                {
                    percentWidth = sourceImage.Width * (renderHeight / sourceImage.Height);
                }
                if (renderHeight != 0)
                {
                    percentHeight = renderHeight / sourceImage.Height;
                }
                else
                {
                    percentHeight = sourceImage.Height * (renderWidth / sourceImage.Width);
                }
                var percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                int newWidth;
                int newHeight;
                //自前で必要なサイズにスケーリングする
                newWidth = (int)(sourceImage.Width * percent * renderDip);
                newHeight = (int)(sourceImage.Height * percent * renderDip);


                //スケール先の画像とストリームを用意
                using (var scaledImage = new Bitmap(newWidth, newHeight))
                {
                    scaledImage.SetResolution((float)(renderDip * 96.0), (float)(renderDip * 96.0));
                    var scaledImageStream = new MemoryStream();

                    //スケール
                    using (Graphics graphicsHandle = Graphics.FromImage(scaledImage))
                    {
                        //補完方法
                        graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //アンチエイリアス
                        graphicsHandle.SmoothingMode = SmoothingMode.HighQuality;
                        //半透明合成
                        graphicsHandle.CompositingQuality = CompositingQuality.HighQuality;
                        //ピクセルのオフセット方法
                        graphicsHandle.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphicsHandle.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
                        //保存先ストリームに保存
                        scaledImage.Save(scaledImageStream, ImageFormat.Png);
                    }
                    #endregion

                    return scaledImageStream;
                }
            }
        }

        /// <summary>
        /// 画像データより拡張子を決定する。
        /// </summary>
        public static String DetermineFileExtension(Stream plateImage)
        {
            using (var image = Image.FromStream(plateImage))
            {
                return DetermineFileExtension(image.RawFormat);
            }
        }

        /// <summary>
        /// ImageFormatから拡張子を求める。
        /// </summary>
        public static string DetermineFileExtension(ImageFormat format)
        {
            try
            {
                return ImageCodecInfo.GetImageEncoders()
                        .First(x => x.FormatID == format.Guid)
                        .FilenameExtension
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .First()
                        .Trim('*')
                        .ToLower();
            }
            catch (Exception)
            {
                return "." + format.ToString().ToLower();
            }
        }
    }
}

