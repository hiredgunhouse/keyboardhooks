﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardHooks
{
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
            Form1 form;
            if (args.Length > 0 && args[0].ToUpper() == "/ENABLE")
            {
                form = new Form1(true);
            }
            else
            {
                form = new Form1();
            }

            Application.Run(form);
        }
    }
}
