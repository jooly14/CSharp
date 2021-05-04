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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)  //접속 버튼
        {
            tcpClient = new TcpClient(textBox1.Text, 3000);     //서버 ip주소를 입력하여 3000포트로 접속
            if(tcpClient.Connected)
            {
                ns = tcpClient.GetStream();
                bw = new BinaryWriter(ns);
                br = new BinaryReader(ns);
                textBox2.AppendText("서버 접속 성공\r\n");
                bw.Write(textBox4.Text);    //이름 보내기
                Thread thread1 = new Thread(new ThreadStart(ReadFnc));  //서버에서 오는 데이터를 읽어들이기 위해서 쓰레드 생성
                thread1.IsBackground = true;
                thread1.Start();
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
                textBox2.AppendText(br.ReadString()+ "\r\n");   //서버로부터 데이터를 받을때마다 textbox2에 append함
            }
        }
        private void button2_Click(object sender, EventArgs e)  //보내기 버튼
        {
            bw.Write(1);    //정상 신호
            bw.Write(textBox3.Text);    //작성한 글을 서버로 전송
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bw.Write(-1);   //연결이 끊겼음을 보내는 신호
            bw.Close();
            br.Close();
            ns.Close();
            tcpClient.Close();
        }

        
    }
}
