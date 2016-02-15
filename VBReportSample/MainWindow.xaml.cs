using AdvanceSoftware.VBReport8;
using Drawing;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace VBReportSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateTestImage_Click(object sender, RoutedEventArgs e)
        {
            var imagePath = AppDomain.CurrentDomain.BaseDirectory + "amslertest.png";

            CreateImage(imagePath);
        }

        private void CreateOwnImage_Click(object sender, RoutedEventArgs e)
        {
            var imagePath = WpfDrawingHelper.AskForImageFilePathViaDialog();

            CreateImage(imagePath);
        }

        private void CreateImage(String imagePath)
        {
            if (imagePath != null)
            {
                //メモリ上に展開
                var rawImage = new RawImage(imagePath);
                //Bitmapインスタンスを作成
                var originalImage = new Bitmap(rawImage.GetContentStream());

                //サムネ
                imageContainer.Source = WpfDrawingHelper.CreateBitmapImage(rawImage);

                //VBReportのインスタンス
                using (var cellReportLocal = new AdvanceSoftware.VBReport8.CellReport())
                {
                    //エラーハンドラ
                    cellReportLocal.Error += (object lsender, AdvanceSoftware.VBReport8.ReportErrorEventArgs le) =>
                    {
                        MessageBox.Show("cellReport1エラー：[" + System.Enum.GetName(typeof(AdvanceSoftware.VBReport8.ErrorNo), le.ErrorNo) + "]");
                    };

                    var sheetPath = AppDomain.CurrentDomain.BaseDirectory + "sample.xlsx";
                    cellReportLocal.FileName = sheetPath;
                    cellReportLocal.Report.Start();
                    cellReportLocal.Report.File();
                    cellReportLocal.Page.Start();

                    //ピクセルで指定
                    cellReportLocal.ScaleMode = ScaleMode.Pixel;

                    cellReportLocal.Cell("B4").Drawing.AddImage(imagePath, originalImage.Width, originalImage.Height);

                    cellReportLocal.Page.End();
                    cellReportLocal.Report.End();
                    cellReportLocal.Report.SaveAs("result_sample.xlsx", ExcelVersion.ver2013);

#if DEBUG
                    //作成したシートを開く
                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\result_sample.xlsx");
#endif
                }
            }
        }

        private void TryToOpenVBReportFromWrongPath_Click(object sender, RoutedEventArgs e)
        {
            //VBReportのインスタンス
            using (var cellReportLocal = new AdvanceSoftware.VBReport8.CellReport())
            {
                //エラーハンドラ

                cellReportLocal.Error += (object lsender, AdvanceSoftware.VBReport8.ReportErrorEventArgs le) =>
                {
                    MessageBox.Show("cellReport1エラー：[" + System.Enum.GetName(typeof(AdvanceSoftware.VBReport8.ErrorNo), le.ErrorNo) + "]");
                };


                try
                {
                    //間違ったファイル名で開こうとする。
                    cellReportLocal.FileName = AppDomain.CurrentDomain.BaseDirectory + "fdjafjdkajfjdkajfkdjkfjksfsafkjfkas.xlsx";

                    cellReportLocal.Report.Start();
                    cellReportLocal.Report.File();
                    cellReportLocal.Page.Start();

                    cellReportLocal.Page.End();
                    cellReportLocal.Report.End();
                }
                catch(NullReferenceException)
                {
                    MessageBox.Show("NullReferenceExceptionがVBReportから投げられました。");
                    //再スローする
                    throw;
                }

            }
        }
    }
}
