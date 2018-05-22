using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Eleição
{
    public static class ControleDeProcessos
    {
        public static List<Processos> processos;
        private static List<int> idsParaProcessos = new List<int>();
        private static List<string> ips = new List<string>();

        public static string PegaIp()
        {
            //IPAddress[] localIps = Dns.GetHostAddresses(Dns.GetHostName());
            //IPAddress ipLocal = localIps.Last();

            //foreach(IPAddress addr in localIps)
            //{
            //    if(addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            //    {
            //        ipLocal = addr;
            //        break;
            //    }
            //}

            if (ips.Count == 0)
            {
                string ip = "FF01::2";
                ips.Add(ip);
                return ip;
            }
            else
            {
                int ultimoValor = ips.Count + 2;
                string ip = "FF01::" + ultimoValor.ToString();
                ips.Add(ip);
                return ip;
            }

        }

        public static  bool VerificaSeTemLider()
        {
            foreach(var processo in processos)
            {
                if(processo.Lider)
                {
                    return true;
                }
            }

            return false;
        }

        public static void InsereProcessoNoControle(Processos processo)
        {
            processos.Add(processo);
        }

        public static int PegaPorta()
        {
            return (processos.Count + 5000);
        }

        public static int PegaIdParaProcesso()
        {
            Random rdm = new Random();

            if(idsParaProcessos.Count == 0)
            {
                int id = rdm.Next(1, 31);
                idsParaProcessos.Add(id);
            }
            else
            {
                int id = 0;
                while(true)
                {
                    id = rdm.Next(1, 31);
                    if(!idsParaProcessos.Contains(id))
                    {
                        idsParaProcessos.Add(id);
                        break;
                    }
                }
            }

            return idsParaProcessos.Last();
        }
    }
}
