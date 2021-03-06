using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;



abstract class objectONmap  //����� ������ �� ����� �������� ����� ����������
{
    private int num;
    protected int x,y;
    protected bool life;
    protected Thread thr;
    //int constsleep = 200;                //�������� ������


    public int N { get { return num; } }
    public int X { get { return x; } }
    public int Y { get { return y; } }

    public objectONmap(int N, int X, int Y)//�����������
    {
        num = N; x = X; y = Y;
        life = true;
        thr = new Thread(new ThreadStart(Move));
    }

    abstract public void Move();

    public void Finish() { life = false; }
}

class Data : EventArgs //����� ������ ������� ��������
{
    private int n, x, y ,sleeptime;
    public int N { get { return n; } }
    public int X { get { return x; } }
    public int Y { get { return y; } }
    public int Sleeptime { get { return sleeptime; } }
    

    public Data(int N, int X, int Y ,int Sleeptime )
    { n = N; x = X; y = Y; sleeptime = Sleeptime; }
}
delegate void DelShip(Data d);//������� �������


class Port:objectONmap //����� �������� ,������ ��������� 3 �����
{
    Window w;                           //������ �� ����
    private bool free;                  //��������� �����������
    private int sleeptime;              //����� ������������ � �����
    public int Sleeptime
    {
        get { return sleeptime; }
        set { sleeptime = value; }
    }

    public event DelShip evPort;
    public Port(int N, int X, int Y, int Sleeptime, Window W)
        :base (N,X,Y)
    {
        w = W;
        sleeptime = Sleeptime;
        thr.Start();
    }

    public override void Move()
    {
        while (life)
        {
            Data d = new Data(N, X, Y ,Sleeptime);
            if (evPort != null)
                evPort(d);
            Thread.Sleep(200);
        }
    }
}

class Ship:objectONmap //����� ��������
    {
      private bool first;
      private int speed;                //�������� ������� 
      private int xPort, yPort;         //���������� ����(�����)
    private int nPort;
    private int thePortSleeptime;
      //�������� �������
      public int Speed
      {
         get {return speed;}
         set {speed=value;}
      }

      public Ship(int N, int X, int Y, int Speed)//�����������
          :base (N,X,Y)
      {
        speed = Speed;
        thr.Start();
      }

      public override void Move()
      {
        Random randomPort;
        int dx,dy,sleeped;
        randomPort = new Random();
        nPort = randomPort.Next(1, 3);
        
        while(life)
        {
           
          Console.WriteLine("����� �{0} ���������� �� ����������� �={1} Y={2} ", N,X,Y);//���������� ��� �������
          //System.Console.WriteLine("{0}   {1}   {2}          {3}", xPort, yPort, thePortSleeptime, N);//���������� ��� �������
          dx = xPort - x;
          dy = yPort - y;
          if (Math.Abs(xPort - x) < 10 && Math.Abs(yPort - y) < 10)
          {
              Console.WriteLine("����� �{0} ������� � ����",N );//���������� ��� �������
              for (sleeped = 0; sleeped != thePortSleeptime; sleeped += 200)                //�������� ��������� � �����
              {
                  if (sleeped + 200 < thePortSleeptime)
                  {
                      Console.WriteLine("����� �{0}    �������� ����������� {1}ms", N, thePortSleeptime - sleeped);//���������� ��� �������
                      Thread.Sleep(200);
                  }
              }
              //first = !first;
              
              nPort = randomPort.Next(1, 3);

          }
          x += dx / 5;
          y += dy / 5;
          Thread.Sleep(200);
        }
      }
      public void HandlerEv(Data D)
      {
          Data d = (Data)D;
          if (nPort==1&&d.N==1||nPort==2&&d.N==2||nPort==3&&d.N==3)
          { this.xPort = d.X; this.yPort = d.Y; this.thePortSleeptime = d.Sleeptime; this.nPort=d.N; }
      }
   }




//������
class Window : Form
{
  private int num;//����� �� ���
  Port port1, port2, port3;
  Ship ship,ship2;
  Font aFont = new Font("Tahoma", 12, FontStyle.Regular);

  public Window ()
  {

      this.Size = new Size(500, 500);//������ ����
      
      num = 1;//����� �� ���

      port1 = new Port(1, 20, 20, 8000, this);   //������ ����
      port1.evPort += new DelShip(this.HandlerEv);
      port2 = new Port(2, 300, 230, 2000, this); //����� ����
      port2.evPort += new DelShip(this.HandlerEv);
      port3 = new Port(1, 50, 300, 1000, this);  //������ ����
      port3.evPort += new DelShip(this.HandlerEv);
      
      ship = new Ship(104, 100, 100, 20);//������� ������ ������� � ����������� : ������ �������� ����� ������� , ������ � ,������ � , ��������� ��������
      port1.evPort += new DelShip(ship.HandlerEv );
      port2.evPort += new DelShip(ship.HandlerEv );
      port3.evPort += new DelShip(ship.HandlerEv);
      ship2 = new Ship(20, 1000, 300, 20);//������� ������ ������� � ����������� : ������ �������� ����� ������� , ������ � ,������ � , ��������� ��������
      port1.evPort += new DelShip(ship2.HandlerEv);
      port2.evPort += new DelShip(ship2.HandlerEv);
      port3.evPort += new DelShip(ship2.HandlerEv);
  }

  private void HandlerEv(Data D)
  {
      Invalidate();//������������
  }

  protected override void OnPaint(PaintEventArgs e)
  {
      
      base.OnPaint(e);
      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 0, 0)), ship.X , ship.Y, 50, 20); //���������� ����� � ������ �������  , ������������ �������1 ������� 50 ������� 20
      e.Graphics.DrawString(ship.N.ToString(), aFont, Brushes.Black, ship.X + 18, ship.Y);      //������� �� ������ �������
      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 0)), port1.X, port1.Y, 20, 20);
      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 0, 255)), port2.X, port2.Y, 20, 20); 
      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 0)), port3.X, port3.Y, 20, 20);

      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 0, 0)), ship2.X, ship2.Y, 50, 20); //���������� ����� � ������ �������  , ������������ �������1 ������� 50 ������� 20
      e.Graphics.DrawString(ship2.N.ToString(), aFont, Brushes.Black, ship2.X + 18, ship2.Y);

  }

    protected override void OnClosed(EventArgs e)
    {
        ship.Finish();
        ship2.Finish();
        port1.Finish();
        port2.Finish();
        port3.Finish();
    }
}


class Program
    {
        static void Main(string[] args)
            {
                Application.Run(new Window());
            }
    }




//Font aFont = new Font ("Tahoma",12,FontStyle.Regular);
//e.Graphics.DrawString ("�����1", aFont, Brushes.Black, 10, 30);