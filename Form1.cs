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
            thread = new Thread(Server);
            thread.Start();
        }

        private void Server()
        {
            server = null;
            try
            {
                IPAddress ip;
                Int32 port = 13000;
                try
                {
                    ip = IPAddress.Parse(File.ReadAllText(@"C:\\Windows\\fingertechts.ini"));
                }catch(Exception e)
                {
                    ip = null;

                    MessageBox.Show("Não foi possível encontrar fingertechts.ini, adicione o arquivo e reinicie o programa");
                   

                }

                if (ip == null) { setStatus("falha ao inicializar, fingertech.ini não encontrado"); return; }
                setlabel(lb_ip, ip.ToString());
                server = new TcpListener(ip, port);
                server.Start();
                setStatus("Serviço Iniciado");
                Byte[] bytes = new Byte[15000];
                String data = null;

                while (true) 
                {
                   
                    String digital = null;
                    TcpClient client = server.AcceptTcpClient();               

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
                        setStatus("Não foi possível Capturar digital, Dispositivo não encontrado");
                        digital = "";
                    }
                    else
                    {
                        setStatus("ok");
                    }

                    //Converter para array de byte
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(digital);
                    //envia resposta de volta
                    stream.Write(msg, 0, msg.Length);
                    setlabel(lb_ip_req, client.Client.RemoteEndPoint.ToString());
                    client.Close();
                   
                }
            }
            catch (SocketException e)
            {
               setStatus("Não foi possível Capturar digital, verifique conexão");

            }    
        }


        public void setlabel(Label label, String text)
        {
            label.BeginInvoke(new Action(() =>
            {
                label.Text = text;
            }

                ));
        }


        public void setStatus(String text)
        {
            lb_status.BeginInvoke(new Action(() =>
            {
                lb_status.Text = text;
            }));
        }
         private  void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (server != null) server.Stop();
            thread.Abort();

            
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                    this.ShowInTaskbar = false;
                }
            }
        }


        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void Lb_ip_req_Click(object sender, EventArgs e)
        {

        }

  
    }
}
