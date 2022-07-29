using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Freya_Chat_2
{
    class Program
    {

        #region UDPSocket
        public class UDPSocket
        {
            public Socket _socket;
            private const int bufSize = 8 * 1024;
            private State state = new State();
            private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
            private AsyncCallback recv = null;

            public class State
            {
                public byte[] buffer = new byte[bufSize];
            }

            public void Server(string address, int port)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
                _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
                Receive();
            }

            public void Client(string address, int port)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.Connect(IPAddress.Parse(address), port);
                Receive();
            }

            public void Send(string text)
            {
                byte[] data = Encoding.ASCII.GetBytes(text);
                _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
                {
                    State so = (State)ar.AsyncState;
                    int bytes = _socket.EndSend(ar);
                    Console.WriteLine("Gönderildi: {1}", bytes, text);
                }, state);
            }

            public void Receive()
            {
                _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
                {
                    try
                    {
                        State so = (State)ar.AsyncState;
                        int bytess = _socket.EndReceiveFrom(ar, ref epFrom);
                        _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                        Console.WriteLine("Karşı taraf: {2}", epFrom.ToString(), bytess, Encoding.ASCII.GetString(so.buffer, 0, bytess));
                    }
                    catch { }

                }, state);
            }
        }

        #endregion

        #region Encryption
        public static string Encrypt(string clearText)
        {
            string hashx = "VOsSXlJofrPHawTjlvedDPUfE67paLvidifDrJPQtZyAOeyp6tWwZg5kGuoCb0ThkVh7XWPR3NQDClI7UB3Tg3O0jOK4fCWB2Fstv843u2E36pdNaDZVl6EHNgWHJpHpaRIo4ZX9fVi6jPiOUh2V5YQeU6wrhs5JeCLRhy3vCnEqhtJIpOx9OgqrPuQtqca9EFaGzZMzcDccdqEaQp38WZrCapfaoX2JkU7EGoPKv1jq5E7jgqFJ03ZhdQQPhEmK";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(hashx, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        #endregion

        #region Decryption

        public static string Decrypt(string cipherText)
        {
            string hashx = "VOsSXlJofrPHawTjlvedDPUfE67paLvidifDrJPQtZyAOeyp6tWwZg5kGuoCb0ThkVh7XWPR3NQDClI7UB3Tg3O0jOK4fCWB2Fstv843u2E36pdNaDZVl6EHNgWHJpHpaRIo4ZX9fVi6jPiOUh2V5YQeU6wrhs5JeCLRhy3vCnEqhtJIpOx9OgqrPuQtqca9EFaGzZMzcDccdqEaQp38WZrCapfaoX2JkU7EGoPKv1jq5E7jgqFJ03ZhdQQPhEmK";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(hashx, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion






        static void Main(string[] args)
        {

            Console.WriteLine("Konsolun hangi renk olmasını istersiniz?");

            Console.WriteLine("R = Red");
            Console.WriteLine("G = Green");
            Console.WriteLine("B = Blue");
            Console.WriteLine("C = Cyan");
            Console.WriteLine("W = White");

            string colorpicker = Console.ReadLine();

            if (colorpicker == "R")
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            else if (colorpicker == "G")
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            else if (colorpicker == "B")
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }

            else if (colorpicker == "C")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }

            else if (colorpicker == "W")
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Clear();

            Console.WriteLine("Nasıl kullanılır?" + Environment.NewLine + "Öncelikle bu sistemde bir port açın." + Environment.NewLine + "Açtığınız portu ve kendi ip adresinizi karşı tarafta bağlanmak istediğiniz ip ve port kısmına girin." + Environment.NewLine + "Aynı işlemleri diğer sistemde de yapın ve bu sistemde bağlanmak istediğiniz ip ve port kısmına yazın." + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            Console.Write("Lütfen kendi ip adresinizi girin: ");
            string ip = Console.ReadLine();

            Console.Write("Lütfen kendi portunuzu girin: ");
            string port = Console.ReadLine();

            Console.Clear();

            Console.Title = "Sistemin ip adresi: " + ip + " Sistemin portu: " + port;

            Console.Write("Lütfen bağlanmak istediğiniz ip adresini girin: ");
            string ip2 = Console.ReadLine();

            Console.Write("Lütfen bağlanmak istediğiniz portu girin: ");
            string port3 = Console.ReadLine();

            Console.Clear();

            int port4 = Convert.ToInt16(port3);

            int port2 = Convert.ToInt16(port);

            UDPSocket s = new UDPSocket();
            s.Server(ip, port2);

            int checker = 0;

            UDPSocket c = new UDPSocket();
            c.Client(ip2, port4);

            while (checker == 0)
            {

                string deger = Console.ReadLine();

                string encrypted = Encrypt(deger);

                c.Send(encrypted);

            }











        }
    }
}
