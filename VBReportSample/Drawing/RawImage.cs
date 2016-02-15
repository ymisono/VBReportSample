
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace Drawing
{
    public class RawImage : IDisposable
    {
        /// <summary>
        /// 画像本体
        /// </summary>

        private MemoryStream _contentInStream;
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 画像のバイト列
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 画像の拡張子
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 空の基本コンストラクタ
        /// </summary>
        public RawImage()
        {
        }

        /// <summary>
        /// ファイルパスから開く
        /// </summary>
        public RawImage(string imageFilePath)
        {
            //最初は絶対に空
            _contentInStream = new MemoryStream();

            using (var fs = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                _contentInStream.SetLength(fs.Length);
                fs.Read(_contentInStream.GetBuffer(), 0, Convert.ToInt32(fs.Length));
            }

            //ロードしたデータから拡張子を求める
            Extension = DrawingHelper.DetermineFileExtension(_contentInStream);
        }

        public Stream GetContentStream()
        {
            //空ならNULLを返す
            if (_contentInStream.Length != 0)
            {
                return _contentInStream;
            }
            else {
                return null;
            }
        }

        #region "IDisposable Support"
        // 重複する呼び出しを検出するには
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    _contentInStream.Dispose();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
            }
            this.disposedValue = true;
        }

        // TODO: 上の Dispose(ByVal disposing As Boolean) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
        //Protected Overrides Sub Finalize()
        //    ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

        // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(disposing As Boolean) に記述します。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }

}

