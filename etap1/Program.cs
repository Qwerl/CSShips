using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
    
//����������� ������� ����� ��������
class Ship
    {
      private int speed;                //�������� ������� 
      private int num;                  //����� �������-���������� ��������
      protected int x, y;               //���������� �������

      int constsleep;                   //�������� ������
      private int xPort, yPort;         //���������� ����(�����)
      protected bool life;              //������� ����� ������
      protected Thread thr;             //������ �� �����    
      //�������� �������
      public int N { get { return num; } }
      public int X { get { return x; } }
      public int Y { get { return y; } }

      public int Speed
      {
         get {return speed;}
         set {speed=value;}
      }

      public Ship(int N, int X, int Y, int Speed)
      {
        num = N; x = X; y = Y; speed = Speed;
        //������� ,�� �� ���������
        life = true;
        thr = new Thread(new ThreadStart(Move));
        //thr.Start();
      }

      //����������� ������� ������������ ��������
      public void Move()
      { 
        int dx,dy;
        while(life)
        {
          dx = xPort - x;
          dy = yPort - y;
        }
      }

      //��������� �����
      public void Finish() { life = false; }
    }

//������� ����� ��������
class Port //������ ��������� 3 �����
    {
        protected int x, y;                 //���������� �����
        private bool free = true;           //��������� �����������
        private int sleeptime;              //����� ������������ � �����
        //�������� �������
        public int Sleeptime
        {
            get { return sleeptime; }
            set { sleeptime = value; }
        }
    }


//����� ������ ������� �������� (����������� ����� ��)
class Data : EventArgs
{  
  private int n,x,y;
  public int N {get {return n;}}
  public int X {get {return x;}}
  public int Y {get {return y;}}

  public Data(int N, int X, int Y)
  { n = N; x = X; y = Y; }
}



//������� �������(��� ���� ��?)
delegate void DelEv(Data d);






//������
class Window : Form
{ 
  Ship  ship1;

  public Window ()
  {
      ship1 = new Ship(1, 2, 150, 20);
      //ship1.evShip += new DelShip(this.HandlerShip);
  }

  private void HandlerShip(Data D)
  {
      Invalidate();//������������
  }

  protected override void OnPaint(PaintEventArgs e)
  {
      
      base.OnPaint(e);
      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255,0,0)) , ship1.X, ship1.Y, 50, 20);
  }
}


class Program
    {
        static void Main(string[] args)
            {
                Application.Run(new Window());
            }
    }
