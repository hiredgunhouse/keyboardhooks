namespace KeyboardHooks
{
    using System;
    using System.Windows.Forms;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var autoEnable = false;
            var autoHide = false;
            foreach (var arg in args)
            {
                switch (arg.ToLower())
                {
                    case "/enable":
                        autoEnable = true;
                        break;
                    case "/hide":
                        autoHide = true;
                        break;
                }
            }

            Application.Run(new FormMain(enable: autoEnable, hide: autoHide));
        }
    }
}
