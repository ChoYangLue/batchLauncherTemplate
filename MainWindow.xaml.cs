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
using System.Threading;
using Microsoft.Win32;          // ファイル選択ダイアログの名前空間を using
using SharpDX.DirectInput;      // ゲームパッド入力

namespace OXIDEfactory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserInput user_inp;

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
            if (e.Key == System.Windows.Input.Key.Up)
            {
                Console.WriteLine("up key");
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                Console.WriteLine("down key");
            }

            // 動画の削除
            if (e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back)
            {
                Console.WriteLine("delete or backspace key");
            }

            // 動画の再生と一時停止
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Console.WriteLine("enter key");
            }

            Console.WriteLine(e.Key);
        }

        private void Window_PadUpdate(JoystickState jState)
        {
            // 挙動確認用：押されたキーをタイトルバーに表示する
            // アナログスティックの左右軸
            bool inputX = true;
            if (jState.X > 300)
            {
                Console.WriteLine("入力キー：→");
            }
            else if (jState.X < -300)
            {
                Console.WriteLine("入力キー：←");
            }
            else
            {
                inputX = false;
            }
            // アナログスティックの上下軸
            bool inputY = true;
            if (jState.Y > 300)
            {
                Console.WriteLine("入力キー：↓");
            }
            else if (jState.Y < -300)
            {
                Console.WriteLine("入力キー：↑");
            }
            else
            {
                inputY = false;
            }
            // 未入力だった場合
            if (!inputX && !inputY)
            {
                //Text = "入力キー：";
            }

            // ボタン判定
            if (jState.Buttons[0])
            {
                // 1番ボタンが押されたらフォームが閉じるように処理
                //Close();
                Console.WriteLine("Buttons[0]");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            user_inp = new UserInput(true);
            user_inp.SetPadUpdateFunc(Window_PadUpdate);
            user_inp.StartThread();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // PadThreadFuncメソッドを終了
            user_inp.StopThread();
        }
    }
}
