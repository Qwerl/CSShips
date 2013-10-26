using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Collections;



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
      private int numNextPort;
      private int speed;                //�������� ������� 
      private int xPort, yPort;         //���������� ����(�����)  
      private int nPort=3;
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
        Random randPort;
        randPort = new Random();
        numNextPort = randPort.Next(1, 4);
        //numNextPort=2;

        int dx =0, dy = 0, sleeped;
        int calc = 0;
        while(life)
        {
          //  Console.WriteLine("����� �{0} ����������� �� �����-{1}({2} , {3}) � ����-{4})", N, nPort, xPort, xPort, numNextPort);
          //Console.WriteLine("����� �{0} ���������� �� ����������� �={1} Y={2} ", N,X,Y);                                                        //���������� ��� �������
         // System.Console.WriteLine("{0}   {1}   {2}          {3}", xPort, yPort, thePortSleeptime, N);                                        //���������� ��� �������

          if (calc <= 2) //����������������������
          {
              dx = xPort - x;
              dy = yPort - y;
              calc++;
          }
                if (Math.Abs(xPort - x) < 50 && Math.Abs(yPort - y) < 50)  //��������� ������� ����������� , ��-�� ����  , ��� ����� ����� , � ���������� �������������  ���� ��� 50 ������ �������������!!
                {                                                          //��� ��������� , ��� ������ ����� ����� ��������� , ��� ������ ����������� ���������
                    calc = 0;
                    randPort = new Random();
                    numNextPort = randPort.Next(1, 4);
                   // Console.WriteLine("����� �{0} ����������� �� �����-{1} � ����-{2}  ", N, nPort, numNextPort);

                    //if (numNextPort!=3)   //1->2->3->1->2->3 � ��
                    //  numNextPort++;
                    //else
                    //  numNextPort=1;

                    //Console.WriteLine("����� �{0} ������� � ���� {1}",N , nPort );                                                                  //���������� ��� �������
                    for (sleeped = 0; sleeped != thePortSleeptime; sleeped += 200)                //�������� ��������� � �����
                    {
                        if (sleeped + 200 < thePortSleeptime)
                        {
                            //Console.WriteLine("����� �{0}    �������� ����������� {1}ms", N, thePortSleeptime - sleeped);                           //���������� ��� �������
                            Thread.Sleep(200);
                        }
                    }
                }
              x += dx * speed / 10000;
              y += dy * speed / 10000;

          Thread.Sleep(200);
        }
      }
      public void HandlerEv(Data D)
      {
          Data d = (Data)D;
          if (d.N==numNextPort)
          { this.xPort = d.X; this.yPort = d.Y; this.thePortSleeptime = d.Sleeptime; this.nPort=d.N; }
      }
   }




//������
class Window : Form
{
  private int shipCount = 0; // ������������ � DEL_ALL
  private int num;
  Port port1, port2, port3;
  //Ship ship,ship2,ship3;
  ArrayList ships;
  Button butAdd,butDel,butDelAll;
  ListBox listBox;
  Random randomSpeed;
  

  Font aFont = new Font("Tahoma", 12, FontStyle.Regular);
  

  public Window ()
  {
      randomSpeed = new Random();

      //�������� ����
      this.Size = new Size(800, 800);//������ ����
      //this.BackColor = Color.Aqua;
      this.BackgroundImage = new Bitmap("C:\\water.jpg");

      //num = 1;//����� �� ���

      //������� ������
      port1 = new Port(1, 20, 20, 2000, this);   //������ ����
      port1.evPort += new DelShip(this.HandlerEv);

      port2 = new Port(2, 720, 20, 2000, this); //����� ����
      port2.evPort += new DelShip(this.HandlerEv);

      port3 = new Port(3, 350, 650, 2000, this);  //������ ����
      port3.evPort += new DelShip(this.HandlerEv);

      //������� ��������
      ships = new ArrayList();
     
      /* ������ ��� ����������� � ADD
      ship = new Ship(1, 10, 900, 200);//������� ������ ������� � ����������� : ������ �������� ����� ������� , ������ � ,������ � , ��������� ��������
      port1.evPort += new DelShip(ship.HandlerEv );
      port2.evPort += new DelShip(ship.HandlerEv );
      port3.evPort += new DelShip(ship.HandlerEv);

      ship2 = new Ship(2, 300, 900, 500);//������� ������ ������� � ����������� : ������ �������� ����� ������� , ������ � ,������ � , ��������� ��������
      port1.evPort += new DelShip(ship2.HandlerEv);
      port2.evPort += new DelShip(ship2.HandlerEv);
      port3.evPort += new DelShip(ship2.HandlerEv);

      ship3 = new Ship(3, 900, 900, 50);//������� ������ ������� � ����������� : ������ �������� ����� ������� , ������ � ,������ � , ��������� ��������
      port1.evPort += new DelShip(ship3.HandlerEv);
      port2.evPort += new DelShip(ship3.HandlerEv);
      port3.evPort += new DelShip(ship3.HandlerEv);
      */


      //������ ������ ADD
      butAdd = new Button();

      butAdd.Location = new Point(30, 350);
      butAdd.BackColor = Color.Green;
      butAdd.Size = new Size(45, 20);
      butAdd.Text = "ADD";
      Controls.Add(butAdd);
      butAdd.Click += new EventHandler(Add);

      //������ ������ DEL
      butDel = new Button();

      butDel.Location = new Point(85, 350);
      butDel.BackColor = Color.Red;
      butDel.Size = new Size(45, 20);
      butDel.Text = "DEL";
      Controls.Add(butDel);
      butDel.Click += new EventHandler(Del);

      //������ ������ DEL_ALL
      butDelAll = new Button();

      butDelAll.BackColor = Color.DarkRed;
      butDelAll.Location = new Point(30, 375);
      butDelAll.Size = new Size(100, 40);
      butDelAll.Text = "DEL_ALL";
      Controls.Add(butDelAll);
      butDelAll.Click += new EventHandler(DelAll);

      //������ ������������� �������� ������(������������� � �������-�������� ��������)
      listBox = new ListBox();
      listBox.Location = new Point(30, 420);
      listBox.Size = new Size(100, 340);
      Controls.Add(listBox);
      


  }

  private void HandlerEv(Data D)
  {
      Invalidate();//������������
  }

    private void Add(object obj, EventArgs args)
    {
        num++;
        Ship ship = new Ship(num, 500, 500, randomSpeed.Next(100,300));
        port1.evPort += new DelShip(ship.HandlerEv);
        port2.evPort += new DelShip(ship.HandlerEv);
        port3.evPort += new DelShip(ship.HandlerEv);
        ships.Add(ship);
        listBox.Items.Add(ship.N);
    }


    private void Del(object obj, EventArgs args)
    {
        if (listBox.SelectedIndex == -1)
            MessageBox.Show("�������� ����� ���������� �������");
        else
        {
            int numSel = (int)listBox.SelectedItem;
            for (int i = 0; i < ships.Count; i++)
            {
                Ship ship = (Ship)ships[i];
                if (ship.N == numSel)
                {
                    ships.Remove(ship);
                    listBox.Items.Remove(numSel);
                    ship.Finish();
                }
            }
        }
    }

    private void DelAll(object obj, EventArgs args)
    {
        
        shipCount+= ships.Count;
        Console.WriteLine("shipCount = {0}", shipCount);
      /*  for (int i = 0, j = 1; j <= shipCount; j++)  // i ��� ������ � �������� ��������  j ��� ������ �� �������
        {
            Ship ship = (Ship)ships[i];
            ships.Remove(ship);
            ship.Finish();
            listBox.Items.Remove(j);
            Console.WriteLine("3for = {0}", j);
            
        }
     */

        for (int i = 0; i <= shipCount; i++) //������ ������
        {

            listBox.Items.Remove(i);
            //i--;
            //if (i<ships.Count)
            //{
                //Ship ship = (Ship)ships[i];
                //ships.Remove(ship);
                //ship.Finish();
        }
        //    Console.WriteLine("1for = {0}", i);
            
        

        for (int i = 0; i < ships.Count; i++)
        {

            if (i < ships.Count)
            {
                Ship ship = (Ship)ships[i];
                //listBox.Items.Remove(j);
                ships.Remove(ship);
                ship.Finish();
                //Console.WriteLine("2for = {0}", i);
                i--;                                        //������� ������������� ����
            }
            else
            {
                //Console.WriteLine("break");
                break;
            }
                
            //Console.WriteLine("3for = {0}", i);
        }

    }

    protected override void OnPaint(PaintEventArgs e)
  {
      
      base.OnPaint(e);

      Graphics g = e.Graphics;
      g.DrawImage(Image.FromFile("C:\\PORT.png"), new Point(port1.X-30, port1.Y)); 
      g.DrawImage(Image.FromFile("C:\\PORT2.png"), new Point(port2.X-50, port2.Y)); 
      g.DrawImage(Image.FromFile("C:\\PORT.png"), new Point(port3.X, port3.Y-50));
      //e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 0)), port1.X, port1.Y, 50, 50);      //����1
      //e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 0, 255)), port2.X, port2.Y, 50, 50);      //����2
      //e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 0)), port3.X, port3.Y, 50, 50);    //����3
      /*
      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 0, 0)), ship.X, ship.Y, 50, 20); //���������� ����� � ������ �������  , ������������ �������1 ������� 50 ������� 20
      e.Graphics.DrawString(ship.N.ToString(), aFont, Brushes.Black, ship.X + 18, ship.Y);      //������� �� ������ �������

      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 0, 0)), ship2.X, ship2.Y, 50, 20); //���������� ����� � ������ �������  , ������������ �������1 ������� 50 ������� 20
      e.Graphics.DrawString(ship2.N.ToString(), aFont, Brushes.Black, ship2.X + 18, ship2.Y);

      e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 0, 0)), ship3.X, ship3.Y, 50, 20); //���������� ����� � ������ �������  , ������������ �������1 ������� 50 ������� 20
      e.Graphics.DrawString(ship3.N.ToString(), aFont, Brushes.Black, ship3.X + 18, ship3.Y);
      */
      for (int i = 0; i < ships.Count; i++)
      {
          Ship ship = (Ship)ships[i];
          e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 0, 0)), ship.X, ship.Y, 50, 20); //���������� ����� � ������ �������  , ������������ �������1 ������� 50 ������� 20
          e.Graphics.DrawString(ship.N.ToString(), aFont, Brushes.Black, ship.X + 18, ship.Y);      //������� �� ������ �������
      }
  }

    protected override void OnClosed(EventArgs e)
    {
        for (int i = 0; i < ships.Count; i++)
        {
            Ship ship = (Ship)ships[i];
            ship.Finish();
        }
        port1.Finish();
        port2.Finish();
        port3.Finish();
    }
}


class Program
    {
        static void Main(string[] args)
            {
                Window w1 = new Window();
                //Window w2 = new Window();  //����� ������ ������ �������
                Application.Run(w1);
            }
    }




//Font aFont = new Font ("Tahoma",12,FontStyle.Regular);
//e.Graphics.DrawString ("�����1", aFont, Brushes.Black, 10, 30);