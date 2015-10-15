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

    public partial class FormMain : Form
    {
        private static int timeout;
        private static bool isDown;
        private static bool justControl;
        private static DateTime downTime;

        private KeyboardHookListener listener;

        public bool AllowClosing { get; set; }

        public FormMain()
        {
            InitializeComponent();
            this.HideForm();
        }

        public void HideForm()
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void ShowForm()
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            // HA! this needs to be last or won't work as expected!!!
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        public FormMain(bool enable = false, bool hide = false)
        {
            InitializeComponent();

            if (enable)
            {
                this.btnEnable_Click(this, new EventArgs());
            }

            if (hide)
            {
                this.HideForm();
            }
        }

        private void ListenerOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var mk = ModifierKeys;
            Log(string.Format("ListenerOnKeyDown(): KeyDown - key code: {0}, Modifiers: {1}, ModifierKeys: {2}", keyEventArgs.KeyCode, keyEventArgs.Modifiers, mk));

            if (keyEventArgs.KeyCode == Keys.LControlKey)
            {
                Log("ListenerOnKeyDown(): KeyCode == LControlKey");
                if (mk == Keys.Control)
                {
                    Log("ListenerOnKeyDown(): ModifierKeys == Control");
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
            Log(string.Format("ListenerOnKeyUp(): KeyUp - key code: {0}, Modifiers: {1}", keyEventArgs.KeyCode, keyEventArgs.Modifiers));

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
            Log(string.Format("DoOtherDown(): KeyCode - {0}", keyEventArgs.KeyCode));

            justControl = false;
        }

        private void DoControlUp()
        {
            Log("DoControlUp()");

            if (isDown)
            {
                Log("DoControlUp(): isDown == true");

                isDown = false;

                if (justControl)
                {
                    Log("DoControlUp(): justControl == true");

                    var upTime = DateTime.Now;
                    if (upTime.Subtract(downTime).TotalMilliseconds <= timeout)
                    {
                        Log("DoControlUp(): within timeout");

                        Log("DoControlUp(): simulating KeyUp(CONTROL)");
                        InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
                        // SendKeys does not fork for me on polish Windows so I changed it to InputSimulator (this works for both PL and EN Windows)
                        ////SendKeys.Flush();
                        ////SendKeys.Send("{ESC}");
                        ////SendKeys.SendWait("{ESC}");
                        Log("DoControlUp(): simulating KeyPress(ESCAPE)");
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.ESCAPE);

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
            if (chkLogKeystrokes.Checked)
            {
                lbText.Items.Insert(0, msg);
            }
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowForm();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (! AllowClosing)
            {
                e.Cancel = true;
                this.HideForm();
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowForm();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AllowClosing = true;
            this.Close();
        }
    }
}
