using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace canberk
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessStartInfo ps_nmap = new ProcessStartInfo();
            ps_nmap.RedirectStandardOutput = true;
            ps_nmap.RedirectStandardError = true;
            ps_nmap.FileName = "C:\\Users\\Catullus0\\Desktop\\canberk\\nmap\\nmap.exe";
            ps_nmap.Arguments = "-sn 192.168.1.* -oX -";
            Process process_nmap = new Process();
            process_nmap.StartInfo = ps_nmap;
            process_nmap.Start();

            StreamReader stdout = process_nmap.StandardOutput;
            string xmlStr = stdout.ReadToEnd();

            process_nmap.WaitForExit();
            int nmap_exit_code = process_nmap.ExitCode;
            Console.WriteLine("Nmap islemi sonlandi. Cikis kodu : " + nmap_exit_code);

            var nmapXml = new XmlDocument();
            nmapXml.XmlResolver = null;
            nmapXml.LoadXml(xmlStr);

            //Console.Write(xmlStr);

            XmlTextWriter writer = new XmlTextWriter("sonuc.xml", System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;

            writer.WriteStartElement("Ips");

            XmlNodeList nodes = nmapXml.SelectNodes("nmaprun/host");
            foreach (XmlElement node in nodes)
            {
                //kod hatalı.
                //node değişkeni XMLNode olduğu zaman getattribute olmaz. XmlElement altında var. Bunu Bildir.
                //status up değilse ipleri listele demiş. 
                //burada bir gariplik var bunu sor unutma.
                //== "up" olarak değiştirdim bildirmeyi unutma
                if (node.GetAttribute("state", "status") == "up") continue;

                XmlNodeList addressV4Nodes = node.SelectNodes("descendant::address[@addrtype = 'ipv4']");
                foreach (XmlElement addressV4 in addressV4Nodes)
                {

                    writer.WriteStartElement("Ip");
                    writer.WriteAttributeString("value", addressV4.GetAttribute("addr"));
                    // writer.WriteString();
                    writer.WriteEndElement();

                }


            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();


            Console.ReadKey(true);
        }
    }
}