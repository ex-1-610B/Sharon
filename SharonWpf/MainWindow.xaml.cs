using Sharon;
using SharonWpf.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace SharonWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Settings.Default.Setting = "dsbyuif";
            Settings.Default.Save();
            left = new FeedbackController(wheelP, wheelI, wheelMin, wheelMax);
            right = new FeedbackController(wheelP, wheelI, wheelMin, wheelMax);
            middle = new FeedbackController2(armP, armI, armSpeed) { Intergreted = armInit };
            low = new FeedbackController2(armP, armI, armSpeed) { Intergreted = armInit };

            piTimer = new Timer(MoveNext, null, 1000, 20);
        }

        private const double wheelP = -0.10;
        private const double wheelI = 0.15;
        private const double wheelMax = 1;
        private const double wheelMin = -1;

        private const double wheelFullSpeed = 1;

        private const double armP = -0.1;
        private const double armI = 0.1;
        private const double armSpeed = 500;
        private const double armInit = 29283;
        private const ushort armRepeatSpeed = 30;

        private ushort middleSet = (ushort)armInit, lowSet = (ushort)armInit;

        private ushort DoubleToWheelUInt16(double value)
        {
            value = 1 - Math.Abs(value);
            value *= 1563;
            return (ushort)value;
        }

        private PIController left, right, middle, low;

        private Timer piTimer;

        private bool keyLeft, keyRight, keyUp, keyDown;

        private bool keySpace, keySpaceHandled;

        private int armState = 0;

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            TextLog.Text = "";
        }

        private void Window_KeyDown(object sender, KeyEventArgs args)
        {
            switch(args.Key)
            {
            case Key.OemPlus:
                middleSet += armRepeatSpeed;
                break;
            case Key.OemMinus:
                middleSet -= armRepeatSpeed;
                break;
            case Key.U:
            case Key.I:
                lowSet += armRepeatSpeed;
                break;
            case Key.J:
            case Key.K:
                lowSet -= armRepeatSpeed;
                break;
            }
            if(args.IsRepeat)
                return;
            switch(args.Key)
            {
            case Key.A:
                keyLeft = true;
                break;
            case Key.D:
                keyRight = true;
                break;
            case Key.W:
                keyUp = true;
                break;
            case Key.S:
                keyDown = true;
                break;
            case Key.Space:
                keySpace = true;
                keySpaceHandled = false;
                break;
            }

            if(args.Key >= Key.D0 && args.Key <= Key.D9)
            {
                armState = args.Key - Key.D0;
                SetArmByState();

            }
            if(args.Key >= Key.NumPad0 && args.Key <= Key.NumPad9)
            {
                armState = args.Key - Key.NumPad0;
                SetArmByState();
            }
            TextLog.Text += $"{args.Key} down\n";
            ScrollLog.ScrollToEnd();
        }

        private void SetArmByState()
        {
            switch(armState)
            {
            case 1:
                middleSet = Settings.Default.s1m;
                lowSet = Settings.Default.s1l;
                break;
            case 2:
                middleSet = Settings.Default.s2m;
                lowSet = Settings.Default.s2l;
                break;
            case 3:
                middleSet = Settings.Default.s3m;
                lowSet = Settings.Default.s3l;
                break;
            case 4:
                middleSet = Settings.Default.s4m;
                lowSet = Settings.Default.s4l;
                break;
            case 5:
                middleSet = Settings.Default.s5m;
                lowSet = Settings.Default.s5l;
                break;
            case 6:
                middleSet = Settings.Default.s6m;
                lowSet = Settings.Default.s6l;
                break;
            case 7:
                middleSet = Settings.Default.s7m;
                lowSet = Settings.Default.s7l;
                break;
            case 8:
                middleSet = Settings.Default.s8m;
                lowSet = Settings.Default.s8l;
                break;
            case 9:
                middleSet = Settings.Default.s9m;
                lowSet = Settings.Default.s9l;
                break;
            default:
                middleSet = (ushort)armInit;
                lowSet = (ushort)armInit;
                break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs args)
        {
            switch(args.Key)
            {
            case Key.A:
                keyLeft = false;
                break;
            case Key.D:
                keyRight = false;
                break;
            case Key.W:
                keyUp = false;
                break;
            case Key.S:
                keyDown = false;
                break;
            case Key.Space:
                keySpace = false;
                keySpaceHandled = false;
                break;
            }
            TextLog.Text += $"{args.Key} up\n";
            ScrollLog.ScrollToEnd();
        }

        private async void MoveNext(object state)
        {
            WheelPIMoveNext();
            ArmPIMoveNext();
            SetSpeedMeters();
            SetArmMeters();
            await SendControlInfo();
        }

        private void WheelPIMoveNext()
        {
            double wheelRunTurnSpeed = 0, wheelSpinTurnSpeed = 0;
            Dispatcher.Invoke(() =>
            {
                wheelRunTurnSpeed = slider_Run.Value;
                wheelSpinTurnSpeed = slider_Spin.Value;
            });
            double leftSet = 0, rightSet = 0;
            if(keyUp)
            {
                if(keyLeft)
                {
                    leftSet = wheelRunTurnSpeed;
                    rightSet = wheelFullSpeed;
                }
                else if(keyRight)
                {
                    leftSet = wheelFullSpeed;
                    rightSet = wheelRunTurnSpeed;
                }
                else
                {
                    leftSet = wheelFullSpeed;
                    rightSet = wheelFullSpeed;
                }
            }
            else if(keyDown)
            {
                if(keyLeft)
                {
                    leftSet = -wheelRunTurnSpeed;
                    rightSet = -wheelFullSpeed;
                }
                else if(keyRight)
                {
                    leftSet = -wheelFullSpeed;
                    rightSet = -wheelRunTurnSpeed;
                }
                else
                {
                    leftSet = -wheelFullSpeed;
                    rightSet = -wheelFullSpeed;
                }
            }
            else
            {
                if(keyLeft)
                {
                    leftSet = -wheelSpinTurnSpeed;
                    rightSet = wheelSpinTurnSpeed;
                }
                else if(keyRight)
                {
                    leftSet = wheelSpinTurnSpeed;
                    rightSet = -wheelSpinTurnSpeed;
                }
                else
                {
                    leftSet = 0;
                    rightSet = 0;
                }
            }


            left.MoveNext(leftSet);
            right.MoveNext(rightSet);
        }

        private void ArmPIMoveNext()
        {
            middle.MoveNext(middleSet);
            low.MoveNext(lowSet);
        }

        private void SetSpeedMeters()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    MeterSpeedLeft.Value = left.Current;
                    MeterSpeedRight.Value = right.Current;
                    MeterArmMiddle.Value = middle.Current;
                    MeterArmLow.Value = low.Current;
                });

            }
            catch(Exception)
            {
            }
        }

        private void SetArmMeters()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if(!keySpaceHandled)
                        MeterHigh.IsChecked = keySpace;
                });
            }
            catch(Exception)
            {
            }
        }

        private async Task SendControlInfo()
        {
            if(right.Current > 0)
            {
                await controller.SendValue(new Instruction(Target.Right, DoubleToWheelUInt16(right.Current)));
                await controller.SendValue(new Instruction(Target.RightForward));
            }
            else
            {
                await controller.SendValue(new Instruction(Target.Right, DoubleToWheelUInt16(right.Current)));
                await controller.SendValue(new Instruction(Target.RightBackward));
            }
            if(left.Current > 0)
            {
                await controller.SendValue(new Instruction(Target.Left, DoubleToWheelUInt16(left.Current)));
                await controller.SendValue(new Instruction(Target.LeftForward));
            }
            else
            {
                await controller.SendValue(new Instruction(Target.Left, DoubleToWheelUInt16(left.Current)));
                await controller.SendValue(new Instruction(Target.LeftBackward));
            }
            if(keySpace)
            {
                await controller.SendValue(new Instruction(Target.High, 29922));
            }
            else
            {
                await controller.SendValue(new Instruction(Target.High, 28437));
            }
            keySpaceHandled = true;

            await controller.SendValue(new Instruction(Target.Middle, (ushort)middle.Current));
            await controller.SendValue(new Instruction(Target.Low, (ushort)low.Current));
        }

        Controller controller = new Controller();
    }
}
