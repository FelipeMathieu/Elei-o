using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Eleição
{
    class Program
    {
        static void Main(string[] args)
        {
            ControleDeProcessos.processos = new List<Processos>();

            List<Thread> threads = new List<Thread>();
            Random rdm = new Random();
            for(int i = 0; i < 5; i++)
            {
                threads.Add(new Thread(CriaProcess));
                threads[i].Start();
                Thread.Sleep(rdm.Next(1000, 3000));
            }

            Console.ReadKey();
        }

        private static void CriaProcess()
        {
            ControleDeProcessos.processos.Add(new Processos());
        }
    }
}
