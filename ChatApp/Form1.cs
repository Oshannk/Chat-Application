using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace ChatApp
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;

        public Form1()
        {
            InitializeComponent();
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress address in localIP)
            {
                if(address.AddressFamily == AddressFamily.InterNetwork){
                    txtsip.Text = address.ToString();
                }
            }
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(txtsport.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void btnconnect_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint ipend = new IPEndPoint(IPAddress.Parse(txtcip.Text), int.Parse(txtcport.Text));
            try
            {
                client.Connect(ipend);
                if (client.Connected)
                {
                    chatscreen.AppendText("Connected To Server" + "\n");
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.chatscreen.Invoke(new MethodInvoker(delegate ()
                    {
                        chatscreen.AppendText("\nClient:" + recieve + "\n");
                    }));
                    recieve = "";
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(TextToSend);
                this.chatscreen.Invoke(new MethodInvoker(delegate ()
                {
                    chatscreen.AppendText("You" + TextToSend + " \n");
                }));
            }
            else
            {
                MessageBox.Show("Sending Failed");
            }
            backgroundWorker2.CancelAsync();
        }

        private void btnsend_Click(object sender, EventArgs e)
        {
            if(txtmsg.Text != "")
            {
                TextToSend = txtmsg.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            txtmsg.Text = "";
        }
    }
}
