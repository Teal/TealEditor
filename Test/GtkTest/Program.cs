using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GtkTest {
    class Program {

        //[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool SetDllDirectory(string lpPathName);

        [STAThread]
        static void Main(string[] args) {

            //    Environment.CurrentDirectory = @"C:\Program Files (x86)\GtkSharp\2.12\bin";

          //  SetDllDirectory(@"gtk/bin");

            Application.Init();

            Window win = new Window("asdasdasdasdasd");
            win.SetSizeRequest(800, 600);

            var vBox = new VBox();
            //  win.Add (vBox);

            Button btn = new Button();

            btn.Label = "asdasdasd";
            btn.Clicked += (s, e) => {
                btn.Label += "\nClicked";
                //var md = new MessageDialog(win, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "asdasds");
                ////  md.ActionArea.Child
                //md.ShowNow();

            };
            // vBox.Add(btn);
            win.Add(btn);

            win.ShowAll();

            Application.Run();

        }

        static void AddEnvironmentPaths(IEnumerable<string> paths) {
            var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };

            string newPath = string.Join(System.IO.Path.PathSeparator.ToString(), path.Concat(paths));

            Environment.SetEnvironmentVariable("PATH", newPath);
        }
    }
}
