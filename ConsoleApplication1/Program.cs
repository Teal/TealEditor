using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1 {

    class A {
        public int a { get; set; }
    }

    class Program {

        static void Main(string[] args) {
            A e = new A();
            e.a = 4;
            Console.Write(e.a);
        }
    }
}
