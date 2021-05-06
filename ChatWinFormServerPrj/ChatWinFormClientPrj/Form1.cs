using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatWinFormClientPrj
{
    public partial class Form1 : Form
    {

        NetworkStream ns;
        BinaryReader br;
        BinaryWriter bw;
        TcpClient tcpClient;

        int idx = -1;
        bool notReadConfirm = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)  //접속 버튼
        {
            try
            {
                tcpClient = new TcpClient(textBox1.Text, 3000);     //서버 ip주소를 입력하여 3000포트로 접속
            }
            catch(SocketException ex)
            {
                MessageBox.Show("서버가 작동하지 않습니다.");
                this.Close();
            }
            
            if(tcpClient.Connected)
            {
                ns = tcpClient.GetStream();
                bw = new BinaryWriter(ns);
                br = new BinaryReader(ns);
                textBox2.AppendText("서버 접속 성공\r\n");
                button1.Enabled = false;
                textBox4.Focus();
                if (textBox4.TextLength > 0)
                {
                    button3.Enabled = true;
                }
            }
            else
            {
                textBox2.AppendText("서버 접속 실패\r\n");
            }

        }

        private void ReadFnc()  //쓰레드 메서드
        {
            
            while (true)
            {
                string txt = br.ReadString();
                if(txt == "//STE//")
                {
                    if(idx == -1)
                    {
                        idx = textBox2.TextLength;
                    }
                    textBox2.Select(idx, 0);
                    textBox2.ScrollToCaret();
                    notReadConfirm = true;
                }
                else
                {
                    
                    textBox2.AppendText(txt + "\r\n");   //서버로부터 데이터를 받을때마다 textbox2에 append함
                    
                    if (!txt.Contains(" : "))
                    {
                        textBox2.Select(textBox2.Text.LastIndexOf(txt), txt.Length);
                        textBox2.SelectionColor = Color.Gray;
                    }
                    else
                    {
                        if(txt.Contains(textBox4.Text + " :"))
                        {
                            textBox2.Select(textBox2.Text.LastIndexOf(textBox4.Text + " :"), (textBox4.Text + " :").Length);
                            textBox2.SelectionColor = Color.Yellow;
                        }
                        
                        
                    }


                    if (!notReadConfirm)
                    {
                        if (txt == "===== 여기까지 읽으셨습니다 =====")
                        {
                            idx = textBox2.Text.LastIndexOf("===== 여기까지 읽으셨습니다 =====");
                        }
                    }
                    else
                    {
                        if (!textBox2.ContainsFocus)
                        {
                            idx = textBox2.TextLength;
                            textBox2.Select(idx, 0);
                            textBox2.ScrollToCaret();
                        }
                        
                    }
                    
                    
                }
                

                
            }

            
        }
        private void button2_Click(object sender, EventArgs e)  //보내기 버튼
        {
            
            bw.Write(1);    //정상 신호
            bw.Write(textBox3.Text);    //작성한 글을 서버로 전송
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(bw != null)
            {
                bw.Write(-1);   //연결이 끊겼음을 보내는 신호
                bw.Close();
            }
            if(br != null)
                br.Close();
            if(ns != null)
                ns.Close();
            if(tcpClient!=null)
                tcpClient.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tcpClient!=null && tcpClient.Connected)
            {
                int ivalue = 0;
                bw.Write(1);
                bw.Write(textBox4.Text);    //이름 보내기
                if ((ivalue = br.ReadInt32()) == -1)
                {
                    MessageBox.Show("이미 접속 중인 아이디입니다.");
                    textBox4.Focus();
                    textBox4.SelectAll();
                }
                else
                {
                    MessageBox.Show(textBox4.Text+"로 접속되었습니다.");
                    button2.Enabled = true;
                    button3.Enabled = false;
                    textBox3.Focus();
                    Thread thread1 = new Thread(new ThreadStart(ReadFnc));  //서버에서 오는 데이터를 읽어들이기 위해서 쓰레드 생성
                    thread1.IsBackground = true;
                    thread1.Start();
                }
            }
            else
            {
                MessageBox.Show("서버 접속을 먼저 진행하세요");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length > 0)
            {
                if(tcpClient!=null)
                    button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
            }
        }

        private void textBox2_VScroll(object sender, EventArgs e)
        {
            
        }

    }
}
