using System;
using System.Windows.Forms;
using Riss.Devices;
using ZDC2911Demo.IConvert;
using ZDC2911Demo.Entity;

namespace FingerPrintPro
{
    static class Program
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
             
        }
    }
}