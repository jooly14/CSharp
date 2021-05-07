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

        
        static string connStr = "Data Source=DESKTOP-6M21UCA;Initial Catalog=ChatDB;Integrated Security=True";


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
            
            
        }
        

        class DBClient
        {
            public string id;
            public DateTime accessTime;

            public DBClient(string id, DateTime accessTime) {
                this.id = id;
                this.accessTime = accessTime;
            }
        }


        private void button1_Click(object sender, EventArgs e)  //클라이언트의 접속을 기다림
        {
            textBox3.AppendText("서버가 시작되었습니다.\r\n");
            Thread thread1 = new Thread(new ThreadStart(AcceptClient)); //여러명의 클라이언트로부터 계속해서 접속을 받기위해서 스레드를 사용
            thread1.IsBackground = true;
            thread1.Start();
            button1.Enabled = false;
        }
        private void AcceptClient()
        {
            while (true)
            {
                tcpClient_ = tcpListener.AcceptTcpClient();
                
                Client client = new Client(tcpClient_, textBox3, clientList);
               /* clientList.Add(client);
                client.setList(clientList);*/
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

            RichTextBox textBox3;

            string username;
            List<Client> clientList;
            List<DBClient> DBClientList;

            DateTime? existingAccessTime;

            public Client(TcpClient tcpClient, RichTextBox textBox3, List<Client> clientList)
            {
                this.tcpClient = tcpClient;
                this.textBox3 = textBox3;
                ns = tcpClient.GetStream();
                bw = new BinaryWriter(ns);
                br = new BinaryReader(ns);
                this.clientList = clientList;
                DBClientList = new List<DBClient>();
            }

            public void process()   //클라이언트로부터 데이터를 읽어오고 다시 클라이언트들에게 데이터를 보내줌
            {
                if (tcpClient.Connected)
                {
                    Boolean idchk = true;
                    while (idchk)
                    {
                      
                        idchk = false;
                        if(br.ReadInt32() == -1)    //로그인 도중에 클라이언트 프로그램 종료시 자원반환
                        {
                            ns.Close();
                            br.Close();
                            bw.Close();
                            tcpClient.Close();
                            return;
                        }
                        username = br.ReadString();
                        foreach (Client c in clientList)    // 중복 로그인 확인
                        {
                            if (c.username == username)
                            {
                                idchk = true;
                                break;
                            }
                        }

                        if (idchk)
                        {
                            bw.Write(-1);
                        }
                        else
                        {
                            bw.Write(1);
                        }
                       
                        
                    }
                    clientList.Add(this);
                    ReadContent();  //기존에 대화내용을 읽어서 로그인한 사용자에게 보내줌

                    string strIn = username + "님이 접속했습니다.";
                    string dateTimeIn = string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now); ;
                    Insert(strIn, dateTimeIn);  //db에 저장
                    strIn = strIn + " (" + dateTimeIn + ")";
                    textBox3.AppendText(strIn+"\r\n");
                    textBox3.Select(textBox3.TextLength, 0);
                    textBox3.ScrollToCaret();



                    if (clientList.Count > 1)
                    {
                        foreach(Client c in clientList)
                        {
                            if (c.username != username) {
                                c.bw.Write(strIn);  //본인을 제외한 다른 사용자에게 새로운 사용자의 접속을 알림
                            }
                            c.bw.Write("========<현재 접속자>========"); //모든 사용자에게 현재 접속자 현황을 보여줌
                            foreach(Client c2 in clientList)
                            {
                                c.bw.Write(c2.username);
                            }
                            c.bw.Write("=========================");
                        }
                    }
                    bw.Write("//STE//");    //기존 대화내용 전송의 끝을 의미 //클라이언트 화면의 스크롤 위치 조정을 위해서 사용
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

                string strOut = username + "님이 나가셨습니다.";
                string dateTimeOut = string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now); ;
                Insert(strOut, dateTimeOut);
                strOut = strOut + " (" + dateTimeOut + ")";
                foreach (Client c in clientList)
                {
                    c.bw.Write(strOut);
                }
                textBox3.AppendText(strOut+"\r\n");
                textBox3.Select(textBox3.TextLength, 0);
                textBox3.ScrollToCaret();

                ExitClient();
                

            }

            private void ExitClient()   //사용자가 나갈때 마지막 accesstime을 db에 저장
            {
                //테이블에 존재하는 아이디는 update
                //테이블에 없는 아이디는 insert
                
                UpdateAccesssTime(existingAccessTime != null? true : false);

            }
            private void UpdateAccesssTime(bool existChk)
            {
                SqlConnection conn = new SqlConnection(connStr);
                string query = "";
               
                if (existChk)
                {
                    query = "update client set accesstime = '" + string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now) + "' where id = '" + username + "'";
                }
                else
                {
                    query = "insert into client(id, accesstime) values('" + username + "','" + string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now) + "')";
                   
                }
                SqlCommand command = new SqlCommand(query, conn);
                conn.Open();

                int result = command.ExecuteNonQuery();
                conn.Close();
            }
            private void ExistChk() //db에 이미 저장된 아이디인지 확인
            {
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

                existingAccessTime = null;
                foreach (DBClient c in DBClientList)
                {
                    if(c.id == username)
                    {
                        existingAccessTime = c.accessTime;
                        break;
                    }
                }
                
            }
            private void ReadSingleRow(IDataRecord record)
            {
                DBClientList.Add(new DBClient(record[0].ToString(), DateTime.Parse(record[1].ToString())));
            }

            private int DataReceive()
            {
                intValue = br.ReadInt32();
                if (intValue == -1) 
                {
                    return -1;
                }
                strValue = br.ReadString();
                strValue = username + " : " + strValue ;
                string dateTime = string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now); ;
                Insert(strValue, dateTime);
                strValue = strValue + " (" + dateTime+")";
                textBox3.AppendText(strValue+ "\r\n");
                textBox3.Select(textBox3.TextLength, 0);
                textBox3.ScrollToCaret();
                return 0;
            }

            private void DataSend()
            {
                foreach(Client c in clientList)
                {
                    c.bw.Write(strValue);
                }

            }

            private void Insert(string str,string dateTime)
            {

                SqlConnection conn = new SqlConnection(connStr);
                string query = "insert into chatcontent(ccontent, regdate) values('"+str+ "','" + dateTime + "')";
                SqlCommand command = new SqlCommand(query, conn);

                conn.Open();

                int result = command.ExecuteNonQuery();
                conn.Close();

            }

            private void ReadContent()
            {
                SqlConnection conn = new SqlConnection(connStr);
                string query = "select * from chatcontent";
                SqlCommand command = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = command.ExecuteReader();

                ExistChk();
                bool commentchk = false;
                while (reader.Read())
                {
                    
                    if (existingAccessTime != null)
                    {
                        if (!commentchk)
                        {
                            if (DateTime.Compare(DateTime.Parse(reader[2].ToString()), existingAccessTime.Value) > 0)
                            {
                                bw.Write("");
                                bw.Write("===== 여기까지 읽으셨습니다 =====");
                                bw.Write("");
                                commentchk = true;
                            }
                        }
                    }
                    
                    ReadSingleRow2((IDataRecord)reader);
                }
                
                reader.Close();
                conn.Close();
            }
            private void ReadSingleRow2(IDataRecord record)
            {
                bw.Write(record[1].ToString()+" ("+string.Format("{0:yyyy/MM/dd HH:mm:ss}", record[2]) +")");
            }

            /*public void setList(List<Client> clientList) {
                this.clientList = clientList;
            }*/

        }

        

       

        

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            tcpListener.Stop();

        }

        
    }
}
