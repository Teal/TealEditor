using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Test
{

    public static class StringUtility {
        
        public static unsafe string append(string s, int length, char c) {
            fixed(char* p = s) {
                p[length] = c;
            }
            return s;
        }


    }

	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
            //string s = "aa";
            //StringUtility.append(s, 1, 't');
            //MessageBox.Show(s);
            //unsafe {
            //    fixed(char* e = s)
            //    {
            //      //  new string('\0', 100);

            //        MessageBox.Show(s);
            //    }
            //}

            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Main());
		}
	}
}
