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
using System.Data.SqlClient;


namespace ChatWinFormServerPrj
{
    public partial class Form1 : Form
    {
        TcpListener tcpListener;
        TcpClient tcpClient_;
        List<Client> clientList = new List<Client>();   //여러 client 클래스를 보관하고 있음

        List<DBClient> DBClientList = new List<DBClient>();
        string connStr = "Data Source=DESKTOP-6M21UCA;Initial Catalog=ChatDB;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tcpListener = new TcpListener(IPAddress.Any,3000);
            tcpListener.Start();
            
            // 서버 ip주소 가져와서 textbox에 넣어줌
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            for(int i=0; i<host.AddressList.Length; i++)
            {
                if(host.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    textBox1.Text = host.AddressList[i].ToString();
                    break;
                }
            }
            
            SqlConnection conn = new SqlConnection(connStr);
            string query = "select * from client";
            SqlCommand command = new SqlCommand(query, conn);
            conn.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow((IDataRecord)reader);
            }

            reader.Close();
            conn.Close();
        }
        private void ReadSingleRow(IDataRecord record)
        {
            DBClientList.Add(new DBClient(record[0].ToString(), record[1].ToString(), Convert.ToDateTime(record[2].ToString())));
        }

        class DBClient
        {
            public string id;
            string ipAddress;
            DateTime accessTime;

            public DBClient(string id, string ipAddress, DateTime accessTime) {
                this.id = id;
                this.ipAddress = ipAddress;
                this.accessTime = accessTime;
            }
        }


        private void button1_Click(object sender, EventArgs e)  //클라이언트의 접속을 기다림
        {
            Thread thread1 = new Thread(new ThreadStart(AcceptClient)); //여러명의 클라이언트로부터 계속해서 접속을 받기위해서 스레드를 사용
            thread1.IsBackground = true;
            thread1.Start();
        }
        private void AcceptClient()
        {
            while (true)
            {
                tcpClient_ = tcpListener.AcceptTcpClient();
                if (tcpClient_.Connected)
                {
                    textBox3.AppendText(((IPEndPoint)tcpClient_.Client.RemoteEndPoint).Address.ToString() + "에서 접속했습니다. \r\n");   // client의 ip주소를 가져옴
                }
                Client client = new Client(tcpClient_, textBox3, DBClientList);
                clientList.Add(client);
                client.setList(clientList);
                Thread innerThread1 = new Thread(new ThreadStart(client.process));  //클라이언트로부터 전달받은 데이터를 읽고 다시 다른 클라이언트에게 전송하기 위한 쓰레드 생성
                innerThread1.IsBackground = true;
                innerThread1.Start();
            }
            
        }

        class Client
        {
            TcpClient tcpClient;
            NetworkStream ns;
            BinaryWriter bw;
            BinaryReader br;
            int intValue;
            string strValue;

            TextBox textBox3;

            string username;
            List<Client> clientList;
            List<DBClient> DBClientList;

            public Client(TcpClient tcpClient, TextBox textBox3,List<DBClient> DBClientList)
            {
                this.tcpClient = tcpClient;
                this.textBox3 = textBox3;
                ns = tcpClient.GetStream();
                bw = new BinaryWriter(ns);
                br = new BinaryReader(ns);
                this.DBClientList = DBClientList;
            }

            public void process()   //클라이언트로부터 데이터를 읽어오고 다시 클라이언트들에게 데이터를 보내줌
            {
                if (tcpClient.Connected)
                {
                    while (true) 
                    {
                        username = br.ReadString();
                        foreach(DBClient c in DBClientList)
                        {
                            if(c.id == username)
                            {
                                ///////////////////////////////////////////////////
                            }
                        }
                    }
                        
                   
                    
                    if (clientList.Count > 1)
                    {
                        foreach(Client c in clientList)
                        {
                            c.bw.Write("========<현재 접속자>========");
                            foreach(Client c2 in clientList)
                            {
                                c.bw.Write(c2.username);
                            }
                            c.bw.Write("=========================");
                        }
                    }
                }
                    
                while (true)
                {
                    if (tcpClient.Connected)
                    {
                        if (DataReceive() == -1)    //-1인 경우 접속이 끊겼음을 의미
                            break;
                        DataSend();
                    }
                    else
                    {
                        break;
                    }
                }
                AllClose();
                //Thread.CurrentThread.Abort();
            }


            private void AllClose()
            {
                if (bw != null)
                {
                    bw.Close(); bw = null;
                }
                if (br != null)
                {
                    br.Close(); br = null;
                }
                if (ns != null)
                {
                    ns.Close(); ns = null;
                }
                if (tcpClient != null)
                {
                    tcpClient.Close(); tcpClient = null;
                }

                //접속자가 나간 경우 clientList에서 제거하고 다른 사용자에게 알림
                clientList.Remove(this);
                foreach(Client c in clientList)
                {
                    c.bw.Write(username+"님이 나가셨습니다.");
                }
            }
            private int DataReceive()
            {
                intValue = br.ReadInt32();
                if (intValue == -1) 
                {
                    return -1;
                }
                strValue = br.ReadString();
                textBox3.AppendText(username+" : "+strValue + "\r\n");
                return 0;
            }

            private void DataSend()
            {
                foreach(Client c in clientList)
                {
                    c.bw.Write(username + " : " + strValue);
                }

            }

            public void setList(List<Client> clientList) {
                this.clientList = clientList;
            }

        }

        

       

        

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            tcpListener.Stop();

        }
       


        

        
    }
}
