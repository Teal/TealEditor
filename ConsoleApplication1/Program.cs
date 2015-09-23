using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1 {

    class A {
        public int a { get; set; }

        public event Func<bool> b;

        public bool dd() { 
            return b();
        }

    }

    class Program {

        static void Main(string[] args) {
            A e = new A();
            e.b += E_f;
            e.b += E_b;
            Console.Write(e.dd());
        }

        private static bool E_b() {
            return true;
        }
        private static bool E_f() {
            return false;
        }
    }
}
