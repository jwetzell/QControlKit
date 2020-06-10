using System;
using System.Threading;
using QSharp;

namespace qControlDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("qControlDemo Started");
            QBrowser browser = new QBrowser();
            Thread.Sleep(100000000);
        }
    }
}
