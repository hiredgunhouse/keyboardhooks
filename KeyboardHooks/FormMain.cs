namespace KeyboardHooks
{
    using System;
    using System.Configuration;
    using System.Windows.Forms;

    using MouseKeyboardActivityMonitor;
    using MouseKeyboardActivityMonitor.WinApi;

    using WindowsInput;

    public partial class FormMain : Form
    {
        private static int _timeout;
        private static bool _isDown;
        private static bool _justControl;
        private static DateTime _downTime;

        private KeyboardHookListener _listener;

        private readonly string _osVersion;

        // see here https://msdn.microsoft.com/library/windows/desktop/ms724832.aspx7 for version numbers
        ////private const string Windows10 = "10.0";
        private const string Windows10 = "6.2"; // for application not specifically targeted for Windows 8 or 10
        private const string Windows7 = "6.1";

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

            var version = Environment.OSVersion.Version;
            _osVersion = $"{version.Major}.{version.Minor}";
        }

        private void ListenerOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var mk = ModifierKeys;
            Log( $"ListenerOnKeyDown(): KeyDown - key code: {keyEventArgs.KeyCode}, Modifiers: {keyEventArgs.Modifiers}, ModifierKeys: {mk}");

            if (keyEventArgs.KeyCode == Keys.LControlKey)
            {
                Log("ListenerOnKeyDown(): KeyCode == LControlKey");

                Log($"ListenerOnKeyDown(): _osVersion == {_osVersion}");
                switch (_osVersion)
                {
                    // for Windows 10
                    case Windows10:
                        DoControlDown();
                        break;

                    // for Windows 8
                    ////case WINDOWS_8:
                    ////    // TODO
                    ////    break;

                    // for Windows 7 
                    // and others we will use the default behavior
                    // TODO: test on Windows 8 and see what the major number for it is
                    // for now I"m just interrested in making it work on Windows 7 and Windows 10 (I don't use Windows 8 and can't even test it on it)
                    case Windows7:
                    default:
                        if (mk == Keys.Control)
                        {
                            Log("ListenerOnKeyDown(): ModifierKeys == Control");
                            DoControlDown();
                        }
                        break;
                }
            }
            else
            {
                DoOtherDown(keyEventArgs);
            }
        }

        private void ListenerOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            Log($"ListenerOnKeyUp(): KeyUp - key code: {keyEventArgs.KeyCode}, Modifiers: {keyEventArgs.Modifiers}");

            if (keyEventArgs.KeyCode == Keys.LControlKey)
            {
                DoControlUp();
            }
        }

        private void DoControlDown()
        {
            Log("DoControlDown()");

            if (!_isDown)
            {
                Log("DoControlDown(): !isDown");
                _isDown = true;
                _justControl = true;
                _downTime = DateTime.Now;
            }
            else
            {
                Log("DoControlDown(): isDown");
            }
        }

        private void DoOtherDown(KeyEventArgs keyEventArgs)
        {
            Log($"DoOtherDown(): KeyCode - {keyEventArgs.KeyCode}");

            _justControl = false;
        }

        private void DoControlUp()
        {
            Log("DoControlUp()");

            if (_isDown)
            {
                Log("DoControlUp(): isDown == true");

                _isDown = false;

                if (_justControl)
                {
                    Log("DoControlUp(): justControl == true");

                    var upTime = DateTime.Now;
                    if (upTime.Subtract(_downTime).TotalMilliseconds <= _timeout)
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
                    else
                    {
                        Log("DoControlUp(): past timeout");
                    }
                }
                else
                {
                    Log("DoControlUp(): Not justControl");
                }
            }
            else
            {
                Log("DoControlUp(): Not isDown");
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
            if (_listener == null)
            {
                _timeout = Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"]);
                _listener = new MouseKeyboardActivityMonitor.KeyboardHookListener(new GlobalHooker());
                _listener.KeyDown += ListenerOnKeyDown;
                _listener.KeyUp += ListenerOnKeyUp;
                _listener.Enabled = true;
            }
            else
            {
                _listener.Stop();
            }
        }

        private void Disable()
        {
            _listener.Stop();
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
            if (!AllowClosing)
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
