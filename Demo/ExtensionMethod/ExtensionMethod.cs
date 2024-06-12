
using System;

namespace Demo.ExtensionMethod
{
    class DemoMethod
    {

        // Method 1
        public void M1()
        {
            Console.WriteLine("Method Name : M1");
        }

        // Method 2
        public void M2()
        {
            Console.WriteLine("Method Name : M2");
        }

        // Method 3
        public void M3()
        {
            Console.WriteLine("Method Name : M3");
        }

    }
    static class NewMethodClass
    {

        // Method 4
        public static void M4(this DemoMethod dm)
        {
            Console.WriteLine("Method Name : M4");
        }

        // Method 5
        public static void M5(this DemoMethod dm, string str)
        {
            Console.WriteLine(str);
        }
    }
    public class ExtensionMethod
    {
        public static void Main(string[] args)
        {
            DemoMethod dm = new DemoMethod();
            dm.M1();
            dm.M2();
            dm.M3();
            dm.M4();
            dm.M5("Method Name : M5");
        }
    }
}
