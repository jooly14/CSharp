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

        //기존에 존재하는 대화내용을 가져오는 데 사용
        int idx = -1;
        bool notReadConfirm = false;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;    //보내기 버튼 사용 제한
            button3.Enabled = false;    //로그인 버튼 사용 제한
            textBox1.Focus();           //서버 아이피 주소 적는 텍스트 박스에 포커스
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
                button1.Enabled = false;    //서버 접속 버튼 사용 제한
                textBox1.ReadOnly = true;   //서버 주소 입력 칸 사용 제한
                textBox4.Focus();           //아이디 입력 칸에 포커스
                if (textBox4.TextLength > 0)    //서버 접속 전에 아이디를 미리 입력해둔 경우
                {
                    button3.Enabled = true;     //로그인 버튼 활성화
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
                if(txt == "//STE//")    //기존에 있던 대화내용 가져올 때 마지막 라인임을 표시
                {
                    if(idx == -1)       //모든 내용이 모두 다 본 내용인 경우
                    {
                        idx = textBox2.TextLength;  //마지막 라인에 스크롤을 위치시킴
                    }
                    textBox2.Select(idx, 0);
                    textBox2.ScrollToCaret();
                    notReadConfirm = true;
                }
                else
                {
                    
                    textBox2.AppendText(txt + "\r\n");   //서버로부터 데이터를 받을때마다 textbox2에 append함
                    
                    if (!txt.Contains(" : "))           //대화 내용이 아닌 단순 알림의 경우 회색으로 표시
                    {
                        textBox2.Select(textBox2.Text.LastIndexOf(txt), txt.Length);
                        textBox2.SelectionColor = Color.Gray;
                    }
                    else
                    {
                        if(txt.Contains(textBox4.Text + " :"))      //본인이 작성한 글인 경우 파란색으로 아이디를 표시
                        {
                            textBox2.Select(textBox2.Text.LastIndexOf(textBox4.Text + " :"), (textBox4.Text + " :").Length);
                            textBox2.SelectionColor = Color.Blue;
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
                        if (!textBox2.ContainsFocus)    //대화내용을 확인하고 있는 중에 새로운 문자가 왔다고 다른 데로 포커스가 가지 않게 하기 위해서
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
            textBox3.Text = "";
            textBox3.Focus() ;
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
                    textBox4.ReadOnly = true;
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

        private void textBox4_TextChanged(object sender, EventArgs e)   //아이디 입력칸에 한 자 이상 작성해야 로그인버튼 활성화
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

        private void textBox3_KeyUp(object sender, KeyEventArgs e)  //대화 내용입력칸에서 엔터키를 누르면 전송 버튼 클릭 이벤트 발생
        {
            if(e.KeyCode == Keys.Enter)
            {
                button2_Click(sender, e);
            }
        }
    }
}
