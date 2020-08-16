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
using System.IO;
using Microsoft.Win32;  // ファイル選択ダイアログの名前空間を using

namespace OXIDEfactory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public int loadVideo(string video_path)
        {
            if (File.Exists(video_path) != true)
            {
                Console.WriteLine(video_path+" does not exist.");
                return -1;
            }

            MediaElementPlayView.Source = new Uri(video_path, UriKind.Relative);
            MediaElementPlayView.LoadedBehavior = MediaState.Stop;

            Console.WriteLine("loaded: " + video_path);
            return 0;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "映像ファイル (*.mp4)|*.mp4|全てのファイル (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                loadVideo(dialog.FileName);
            }
        }

        // キー入力関連
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // 動画の選択
            if (e.Key == Key.Up)
            {
                Console.WriteLine("up key");
            }
            else if (e.Key == Key.Down)
            {
                Console.WriteLine("down key");
            }

            // 動画の削除
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                Console.WriteLine("delete or backspace key");
            }

            // 動画の再生と一時停止
            if (e.Key == Key.Enter)
            {
                Console.WriteLine("enter key");
            }

            Console.WriteLine(e.Key);
        }


    }
}
