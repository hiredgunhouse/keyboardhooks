namespace KeyboardHooks
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Threading;
    using System.Windows.Forms;

    using MouseKeyboardActivityMonitor;
    using MouseKeyboardActivityMonitor.WinApi;

    using WindowsInput;

    public partial class Form1 : Form
    {
        private static int timeout;
        private static bool isDown;
        private static bool justControl;
        private static DateTime downTime;

        private KeyboardHookListener listener;

        ////private static readonly object lockObj = new object();

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(bool enable)
        {
            InitializeComponent();
            if (enable)
            {
                this.btnEnable_Click(this, new EventArgs());
            }
        }

        private void ListenerOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var mk = ModifierKeys;
            ////Log(string.Format("KeyDown - key code: {0}, Modifiers: {1}, ModifierKeys: {2}", keyEventArgs.KeyCode, keyEventArgs.Modifiers, ModifierKeys));
            Log(string.Format("KeyDown - key code: {0}, Modifiers: {1}, ModifierKeys: {2}", keyEventArgs.KeyCode, keyEventArgs.Modifiers, mk));
            ////if (keyEventArgs.Control)
            if (keyEventArgs.KeyCode == Keys.LControlKey)
            {
                ////if (keyEventArgs.Modifiers == Keys.LControlKey)
                ////if (keyEventArgs.Modifiers == Keys.None)
                ////if (ModifierKeys == Keys.ControlKey)
                if (mk == Keys.Control)
                {
                    DoControlDown();
                }
            }
            else
            {
                DoOtherDown(keyEventArgs);
            }
        }

        private void ListenerOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            Log(string.Format("KeyUp - key code: {0}, Modifiers: {1}", keyEventArgs.KeyCode, keyEventArgs.Modifiers));
            ////if (keyEventArgs.Control)
            if (keyEventArgs.KeyCode == Keys.LControlKey)
            {
                DoControlUp();
            }
        }

        private void DoControlDown()
        {
            Log("DoControlDown()");
            if (!isDown)
            {
                Log("!isDown");
                isDown = true;
                justControl = true;
                downTime = DateTime.Now;
            }
        }

        private void DoOtherDown(KeyEventArgs keyEventArgs)
        {
            Log(string.Format("DoOtherDown() - {0}", keyEventArgs.KeyCode));
            justControl = false;
        }

        private void DoControlUp()
        {
            if (isDown)
            {
                Log("DoControlUp()");
                isDown = false;
                if (justControl)
                {
                    Log("justControl");
                    var upTime = DateTime.Now;
                    if (upTime.Subtract(downTime).TotalMilliseconds <= timeout)
                    {
                        Log("within timeout");
                        // TODO send Esc 
                        InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
                        SendKeys.Send("{ESC}");
                        ////SendKeys.Send("a");
                        ////new Thread(() =>
                        ////{
                        ////    Thread.Sleep(100);
                        ////    SendKeys.Send("{ESC}");
                        ////})
                        ////.Start();
                    }
                }
            }
        }

        private void Log(string msg)
        {
            // reverse order to see newest on top
            ////lbText.Items.Add(msg);
            lbText.Items.Insert(0, msg);
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            if (btnEnable.Text == "Enable")
            {
                Enable();
                btnEnable.Text = "Disable";
            }
            else
            {
                Disable();
                btnEnable.Text = "Enable";
            }

        }

        private void Enable()
        {
            if (listener == null)
            {
                timeout = Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"]);
                listener = new MouseKeyboardActivityMonitor.KeyboardHookListener(new GlobalHooker());
                listener.KeyDown += ListenerOnKeyDown;
                listener.KeyUp += ListenerOnKeyUp;
                listener.Enabled = true;
            }
            else
            {
                listener.Stop();
            }
        }

        private void Disable()
        {
            listener.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lbText.Items.Clear();
        }
    }
}
