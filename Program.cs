﻿using System.Windows.Forms;
using System;

namespace DataGridViewControl
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Index());
        }
    }
}
