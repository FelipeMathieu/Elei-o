using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Eleição
{
    public class Processos
    {
        private int id;
        private int porta;
        private IPAddress ip;
        private UdpClient udpClient;
        private IPEndPoint ipe;
        private Thread multThread;
        private bool lider = false;

        private String strData = "";
        private String Ret = "";
        private ASCIIEncoding ASCII = new ASCIIEncoding();
        private Byte[] data = new byte[] { };

        private char[] greetings = new char[] { };

        public Processos()
        {
            this.ConectaProcesso();
        }

        public Processos(int id, int porta, IPAddress ip)
        {
            this.id = id;
            this.porta = porta;
            this.ip = ip;
            this.ConectaProcesso();
        }

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public int Porta
        {
            get
            {
                return this.porta;
            }
            set
            {
                this.porta = value;
            }
        }
        public IPAddress Ip
        {
            get
            {
                return this.ip;
            }
            set
            {
                this.ip = value;
            }
        }
        public bool Lider
        {
            get
            {
                return this.lider;
            }
            set
            {
                this.lider = value;
            }
        }

        private void ConectaProcesso()
        {
            if (this.InsereProcessoNoControle())
            {
                Random rdm = new Random();
                Thread.Sleep(rdm.Next(1000, 5000));

                this.MandaRecebe("Lider?");
            }
        }

        private void MandaRecebe(string mensagem)
        {
            this.greetings = mensagem.ToCharArray();

            Random rdm = new Random();

            Thread.Sleep(rdm.Next(1000, 5000));

            Console.WriteLine(new string(this.greetings));
            this.udpClient.Send(this.GetByteArray(greetings), greetings.Length, this.ipe);

            this.multThread = new Thread(this.ReceiveUntilStop);
            this.multThread.Start();
        }

        private void PerguntaPorLider()
        {
            this.MandaRecebe("Lider?");
        }

        private void RecebidadeMensagemDoLider(bool recebi)
        {
            if(!recebi && !ControleDeProcessos.VerificaSeTemLider())
            {
                this.lider = true;
                Console.WriteLine("Processo " + this.id.ToString() + " virou lider.");
            }
            else
            {
                if(ControleDeProcessos.VerificaSeTemLider())
                {
                    this.lider = false;
                    Console.WriteLine("Processo " + this.id.ToString() + " nao virou lider.");
                }
                else
                {
                    this.lider = true;
                    Console.WriteLine("Processo " + this.id.ToString() + " virou lider.");
                }
            }
        }

        private void ReceiveUntilStop()
        {
            IPAddress m_GrpAddr = IPAddress.Parse("FF01::1");

            UdpClient receiver = new UdpClient(AddressFamily.InterNetworkV6);
            receiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.IPv6Any, 2000);
            receiver.ExclusiveAddressUse = false;
            receiver.Client.Bind(endpoint);
            receiver.JoinMulticastGroup(m_GrpAddr);

            while (true)
            {
                this.data = receiver.Receive(ref endpoint);
                this.strData = this.ASCII.GetString(this.data);
                this.Ret += this.strData + "\n";

                Console.WriteLine("Processo " + this.id.ToString() + ", recebeu a mensagem: " + this.Ret);

                if(this.Ret.Equals("Lider?") && this.lider)
                {
                    this.MandaRecebe(this.id.ToString());
                }
                else
                {
                    this.MandaRecebe("Nao sou lider!");
                }

                if(this.Ret.Length == 1)
                {
                    this.RecebidadeMensagemDoLider(true);
                }
                else
                {
                    this.RecebidadeMensagemDoLider(false);
                }

                this.Ret = "";
            }
        }

        private Byte[] GetByteArray(Char[] ChArray)
        {
            Byte[] Ret = new Byte[ChArray.Length];
            for (int i = 0; i < ChArray.Length; i++)
                Ret[i] = (Byte)ChArray[i];
            return Ret;
        }

        private int PegaId()
        {
            return ControleDeProcessos.PegaIdParaProcesso();
        }

        private IPAddress PegaIp()
        {
            return IPAddress.Parse(ControleDeProcessos.PegaIp());
        }

        private int PegaPorta()
        {
            return ControleDeProcessos.PegaPorta();
        }

        private bool InsereProcessoNoControle()
        {
            try
            {
                this.id = this.PegaId();
                this.ip = this.PegaIp();
                this.porta = PegaPorta();

                IPAddress m_GrpAddr = IPAddress.Parse("FF01::1");

                this.udpClient = new UdpClient(this.porta, AddressFamily.InterNetworkV6)
                {
                    EnableBroadcast = true
                };
                this.udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                IPv6MulticastOption ipv6MulticastOption = new IPv6MulticastOption(m_GrpAddr);

                IPAddress group = ipv6MulticastOption.Group;
                long interfaceIndex = ipv6MulticastOption.InterfaceIndex;

                IPv6MulticastOption ipv6MulticastOption2 = new IPv6MulticastOption(group, interfaceIndex);

                group = ipv6MulticastOption2.Group;
                interfaceIndex = ipv6MulticastOption2.InterfaceIndex;

                this.udpClient.JoinMulticastGroup((int)interfaceIndex, group);
                this.ipe = new IPEndPoint(m_GrpAddr, 2000);

                ControleDeProcessos.InsereProcessoNoControle(this);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
