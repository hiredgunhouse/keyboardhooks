using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardHookTest1
{
    using System.Configuration;
    using System.Windows.Forms;

    using MouseKeyboardActivityMonitor.WinApi;

    class Program
    {
        private static int timeout;
        private static bool isDown;
        private static bool justControl;
        private static DateTime downTime;
        private static readonly object lockObj = new object();


        static void Main(string[] args)
        {
            timeout = Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"]);
            var listener = new MouseKeyboardActivityMonitor.KeyboardHookListener(new GlobalHooker());
            listener.KeyDown += ListenerOnKeyDown;
            listener.KeyUp += ListenerOnKeyUp;
            listener.Enabled = true;

            Console.ReadLine();
        }


        private static void ListenerOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            ////if (keyEventArgs.Control)
            if (keyEventArgs.KeyCode == Keys.LControlKey)
            {
                DoControlDown();
            }
            else
            {
                DoOtherDown();
            }
        }

        private static void ListenerOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            ////if (keyEventArgs.Control)
            if (keyEventArgs.KeyCode == Keys.LControlKey)
            {
                DoControlUp();
            }
        }

        private static void DoControlDown()
        {
            if (!isDown)
            {
                isDown = true;
                justControl = true;
                downTime = DateTime.Now;
            }
        }

        private static void DoOtherDown()
        {
            justControl = false;
        }

        private static void DoControlUp()
        {
            isDown = false;
            if (justControl)
            {
                var upTime = DateTime.Now;
                if (upTime.Subtract(downTime).TotalMilliseconds <= timeout)
                {
                    // TODO send Esc 
                    SendKeys.Send("{ESC}");
                }
            }
        }
    }
}
