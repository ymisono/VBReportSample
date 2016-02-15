using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Drawing
{

    /// <summary>
    /// WPF用の描画ヘルパー
    /// </summary>
    public class WpfDrawingHelper
    {
        /// <summary>
        /// 現在描画が行われているモニターのDIP（デバイス非依存ピクセル）を返す。
        /// </summary>
        public static double GetMoniterDip()
        {
            dynamic m = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
            return m.M11;
        }

        /// <summary>
        /// BitmapImageを作成する。
        /// </summary>
        public static BitmapImage CreateBitmapImage(Stream imageStream)
        {
            imageStream.Position = 0;

            dynamic bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
            bmpImage.StreamSource = imageStream;
            bmpImage.EndInit();
            //メモリリーク回避の為、フリーズする
            bmpImage.Freeze();

            return bmpImage;
        }

        public static BitmapImage CreateBitmapImage(RawImage image)
        {
            dynamic stream = image.GetContentStream();
            if (stream == null)
            {
                return null;
            }
            return CreateBitmapImage(stream);
        }

        public static string AskForImageFilePathViaDialog()
        {
            //BitmapImage returnImage = null;

            dynamic op = new Microsoft.Win32.OpenFileDialog();
            op.Title = "画像を選択してください。";
            op.Filter = "|*.jpg;*.jpeg;*.png;*.bmp;.gif|" + "JPEG形式 (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "PNG形式 (*.png)|*.png|" + "GIF形式（*.gif)|*.gif|" + "BMP形式（*.bmp)|*.bmp";
            //"全ての形式|*.*|";

            //初期位置は前のを覚えていればそれを、なければマイドキュメント
            //Dim previousPath = LocalSettings.ReadSetting(Constants.PreviousOpenFilePath)
            //If previousPath IsNot Nothing Then
            //    op.InitialDirectory = previousPath
            //Else
            //    op.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
            //End If

            //FilterIndexは1から始まる
            //op.FilterIndex = 1;
            if (op.ShowDialog() == true)
            {
                return op.FileName;
                //Dim rawImage = New RawImage(op.FileName)
                //returnImage = rawImage.CreateBitmapImage()

                //ファイルパスは覚えておく
                //LocalSettings.AddUpdateAppSettings(Constants.PreviousOpenFilePath, op.FileName)
            }

            return null;
        }
    }

}