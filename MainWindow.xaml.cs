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
        private bool pad_update_thread_flag = true;
        private Joystick pad_handle;

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

        // Joystick入力関連
        public void PadInit()
        {
            // 入力周りの初期化
            DirectInput dinput = new DirectInput();
            if (dinput != null)
            {
                // 使用するゲームパッドのID
                var joystickGuid = Guid.Empty;
                // ゲームパッドからゲームパッドを取得する
                if (joystickGuid == Guid.Empty)
                {
                    foreach (DeviceInstance device in dinput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                    {
                        joystickGuid = device.InstanceGuid;
                        break;
                    }
                }
                // ジョイスティックからゲームパッドを取得する
                if (joystickGuid == Guid.Empty)
                {
                    foreach (DeviceInstance device in dinput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    {
                        joystickGuid = device.InstanceGuid;
                        break;
                    }
                }
                // 見つかった場合
                if (joystickGuid != Guid.Empty)
                {
                    // パッド入力周りの初期化
                    pad_handle = new Joystick(dinput, joystickGuid);
                    if (pad_handle != null)
                    {
                        // バッファサイズを指定
                        pad_handle.Properties.BufferSize = 128;

                        // 相対軸・絶対軸の最小値と最大値を
                        // 指定した値の範囲に設定する
                        foreach (DeviceObjectInstance deviceObject in pad_handle.GetObjects())
                        {
                            switch (deviceObject.ObjectId.Flags)
                            {
                                case DeviceObjectTypeFlags.Axis:
                                // 絶対軸or相対軸
                                case DeviceObjectTypeFlags.AbsoluteAxis:
                                // 絶対軸
                                case DeviceObjectTypeFlags.RelativeAxis:
                                    // 相対軸
                                    var ir = pad_handle.GetObjectPropertiesById(deviceObject.ObjectId);
                                    if (ir != null)
                                    {
                                        try
                                        {
                                            ir.Range = new InputRange(-1000, 1000);
                                        }
                                        catch (Exception) { }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void UpdateForPad()
        {
            // フォームにフォーカスが無い場合、処理終了
            //if (!Focused) { return; }
            // 初期化が出来ていない場合、処理終了
            if (pad_handle == null) { return; }

            // キャプチャするデバイスを取得
            pad_handle.Acquire();
            pad_handle.Poll();

            // ゲームパッドのデータ取得
            var jState = pad_handle.GetCurrentState();
            // 取得できない場合、処理終了
            if (jState == null) { return; }

            // 以下の処理は挙動確認用

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

        // 別スレッドで実行するメソッド
        private void PadThreadFunc()
        {
            while (pad_update_thread_flag)
            {
                UpdateForPad();

                // CPUがフル稼働しないようにFPSの制限をかける
                // ※簡易的に、おおよそ秒間30フレーム程度に制限
                Thread.Sleep(32);
            }

            Console.WriteLine("JoyStickThread End.");
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PadInit();

            // PadThreadFuncメソッドを別のスレッドで実行するThreadオブジェクトを作成する
            Thread t = new Thread(new ThreadStart(PadThreadFunc));
            t.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // PadThreadFuncメソッドを終了
            pad_update_thread_flag = false;
        }
    }
}
