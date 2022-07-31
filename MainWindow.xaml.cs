using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace batchLauncherTemplate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void UpdateTextBoxEventHandler(TextBox text_box, string data);
        public event UpdateTextBoxEventHandler UpdateTextBoxContentEvent = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        /* ボタンクリック */
        private void FolderOpenButton_Click(object sender, RoutedEventArgs e)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                RestoreDirectory = true, // 最後に選択したフォルダから始まる
                IsFolderPicker = true, // フォルダ選択モードにする
            })
            {
                if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                // FileNameで選択されたフォルダを取得する
                FolderTextbox.Text = cofd.FileName;
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var com1 = new LoadExecJob();
            com1.SetOutputFunc(BatchOutputFunc);
            com1.Run("", "");
            com1.Join();
        }

        /* コンソール出力 */
        void BatchOutputFunc(string out_txt)
        {
            /*
            if (out_txt.IndexOf("time=") > 0)
            {
                string time_tmp = out_txt.Substring(out_txt.IndexOf("time=") + 5, 8);
            }*/

            this.Dispatcher.Invoke(UpdateTextBoxContentEvent, OutputLogTextbox, out_txt);
        }

        /* デリゲート */
        void event_DataReceived2(TextBox text_box, string data)
        {
            text_box.AppendText(data + "\n");
            text_box.ScrollToEnd();
        }

        /* ロードとセーブ関連 */
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTextBoxContentEvent = new UpdateTextBoxEventHandler(event_DataReceived2);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
