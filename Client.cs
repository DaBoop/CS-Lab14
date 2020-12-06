using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace Lab14
{
    class Client
    {
        static void Main(string[] args)
        {
            var list = new int[] { 1, 2, 3 };
            var cake = new Cake("Tiramisu", 5, 5, 100, 20, 30, 20, 20);
            CustomSerializer.BinSerialize(cake, "BinCake.txt");
            CustomSerializer.BinSerialize(list, "Array.txt");
            var serializedCake = CustomSerializer.BinDeserialize(typeof(Cake), "BinCake.txt");
            var serializedArray = CustomSerializer.BinDeserialize(typeof(Array), "Array.txt", typeof(int));
            Console.WriteLine(serializedCake);
            Console.Write(serializedArray + ":");
            foreach (var elem in (Array)serializedArray)
            {
                Console.Write(elem);
            }
            Console.WriteLine();
            CustomSerializer.JSONSerialize(cake, "JsonCake.json");
            serializedCake = CustomSerializer.JSONDeserialize(typeof(Cake), "JsonCake.json");
            Console.WriteLine(serializedCake);
            

            CustomSerializer.JSONSerialize(list, "Array.json");

            CustomSerializer.XMLSerialize(cake, "XMLCake.xml");
            serializedCake = CustomSerializer.XMLDeserialize(typeof(Cake), "XMLCake.xml");
            Console.WriteLine(serializedCake);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("XMLCake.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            XmlNodeList childnodes = xRoot.SelectNodes("*");
            foreach (XmlNode n in childnodes)
                Console.WriteLine(n.OuterXml);

            Console.WriteLine();
            childnodes = xRoot.SelectNodes("price");
            foreach (XmlNode n in childnodes)
                Console.WriteLine(n.OuterXml);

            var xmldoc = XDocument.Load("XMLCake.xml");
            
            foreach (XElement elem in xmldoc.Descendants())
            {
                Console.WriteLine(elem.Value);
            }
            Console.WriteLine(xmldoc.Root);



            // Get host related information.

            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            using (Socket sock = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                sock.Connect(remoteEP);
                Console.WriteLine("Socket connected to {0}", sock.RemoteEndPoint.ToString());

                var buffer = Encoding.ASCII.GetBytes(File.ReadAllText("JsonCake.json"));
           
                int rez = sock.Send(buffer);
                Console.WriteLine($"Bytes sent: {rez}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
