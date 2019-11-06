using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rdpServiceSocket
{
    public partial class Form1 : Form
    {

        TcpListener server;
        utilsNitgen utils = new utilsNitgen();
        Thread thread;

        public Form1()
        {

            InitializeComponent();
            thread =new Thread(Server);
            thread.Start();
        }

        private void Server()
        {


            server = null;
            try
            {

                Int32 port = 13000;
                IPAddress ip = IPAddress.Parse(File.ReadAllText(@"C:\\Windows\\fingertechts.ini"));
                server = new TcpListener(ip, port);
                server.Start();
                Byte[] bytes = new Byte[15000];
                String data = null;

                while (true)
                {
                   

                    String digital = null;
                    TcpClient client = server.AcceptTcpClient();
                    //Console.WriteLine("Connected!");

                    data = null;
                    NetworkStream stream = client.GetStream();
                    int i = stream.Read(bytes, 0, bytes.Length);
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                  
                        switch (data)
                        {
                            case "0":
                                digital = utils.Enroll();
                                break;
                            case "1":
                                digital = utils.Capturar();
                                break;
                        }
                    if (digital == null)
                    {
                        lb_status.BeginInvoke(new Action(() =>
                        {
                            lb_status.Text = "Não foi possível Capturar digital, verifique conexão";
                        }));
                        digital = "";
                    }



                    
                    //Converter para array de byte
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(digital);
                    //envia resposta de volta
                    stream.Write(msg, 0, msg.Length);
                    client.Close();


                   
                }
            }
            catch (SocketException e)
            {
                lb_status.Text = "Não foi possível Capturar digital, verifique conexão";


               
            }           



        }



         private  void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            server.Stop();
            thread.Abort();

            
        }
        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }
    }
}
