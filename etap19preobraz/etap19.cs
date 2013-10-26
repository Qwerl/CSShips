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
    public int constsleep = 30;                //�������� ������


    public int N { get { return num; } }
    public int X { get { return x; } }
    public int Y { get { return y; } }

    public objectONmap(int N, int X, int Y)//�����������
    {
        num = N; x = X; y = Y;
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
    //Window w;                           //������ �� ����
    private bool free;                  //��������� �����������
    private int sleeptime;              //����� ������������ � �����
    public int Sleeptime
    {
        get { return sleeptime; }
        set { sleeptime = value; }
    }

    public event DelShip evPort;
    public Port(int N, int X, int Y, int Sleeptime)
        :base (N,X,Y)
    {
        
        sleeptime = Sleeptime;
       //thr.Start();
    }

    public override void Move()
    {
        Data d = new Data(N, X, Y, Sleeptime);
        //    if (evPort != null)
          //      evPort(d);
        while (life)
        {
        //    Data d = new Data(N, X, Y ,Sleeptime);
            if (evPort != null)
                evPort(d);
        //    Thread.Sleep(constsleep);
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
    private Color colorTheSHIP;
      
    public Color ShipColor 
    {
        get { return colorTheSHIP;  }
        set { colorTheSHIP = value; }
    }
    //�������� �������
    public int Speed
    {
        get {return speed;}
        set {speed=value;}
    }

    public Ship(int N, int X, int Y, int Speed, Color ShipColor)//�����������
        :base (N,X,Y)
    {
        life = true;
        thr = new Thread(new ThreadStart(Move));
        speed = Speed;
        colorTheSHIP = ShipColor;
        thr.Start();
    }

    public override void Move()
    {
        Random randPort;
        randPort = new Random();
        numNextPort = randPort.Next(0, 3);
        //numNextPort=2;

        int dx =0, dy = 0, sleeped;
        int calc = 0;
        while(life)
        {
            //Console.WriteLine("����� �{0} ����������� �� �����-{1}({2} , {3}) � ����-{4})", N, nPort, xPort, xPort, numNextPort);                 //���������� ��� �������
            //Console.WriteLine("����� �{0} ���������� �� ����������� �={1} Y={2} ", N,X,Y);                                                        //���������� ��� �������
            //Console.WriteLine("{0}   {1}   {2}          {3}", xPort, yPort, thePortSleeptime, N);                                                 //���������� ��� �������

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
                numNextPort = randPort.Next(0,3);
                   // Console.WriteLine("����� �{0} ����������� �� �����-{1} � ����-{2}  ", N, nPort, numNextPort);

                    //if (numNextPort!=3)   //1->2->3->1->2->3 � ��
                    //  numNextPort++;
                    //else
                    //  numNextPort=1;

                    //Console.WriteLine("����� �{0} ������� � ���� {1}",N , nPort );                                                                  //���������� ��� �������
                for (sleeped = 0; sleeped != thePortSleeptime; sleeped += constsleep)                //�������� ��������� � �����
                {
                    if (sleeped + constsleep < thePortSleeptime)
                    {
                        //Console.WriteLine("����� �{0}    �������� ����������� {1}ms", N, thePortSleeptime - sleeped);                           //���������� ��� �������
                        Thread.Sleep(constsleep);
                    }
                }
            }
            x += dx * speed / 10000;
            y += dy * speed / 10000;
            Thread.Sleep(constsleep);
        }
    }   

    public void HandlerEv(Data D)
    {
        Data d = (Data)D;
        if (d.N==numNextPort)
        { 
            this.xPort = d.X;
            this.yPort = d.Y;
            this.thePortSleeptime = d.Sleeptime;
            this.nPort=d.N; 
        }
    }
}

class Window : Form //������
{
    private int shipCount = 0;              //������������ � DEL_ALL � DEL, �������� ����������� ����������� �������� , ������� ���� ������� �� ������
    private int numShip;                        //���������� ��������
    //Port port1, port2, port3;               
    ArrayList ships,ports;                        //������ ��������
    Button butAdd,butDel,butDelAll;         //��������
    ListBox listBox;                        //����� ������
    Random randomSpeed;                     //�������� ����� ��� ������ ������� ��������� ��������
    Font aFont = new Font("Tahoma", 12, FontStyle.Regular);
  

    public Window ()
    {
        randomSpeed = new Random();
        //�������� ����
        this.Size = new Size(800, 800);//������ ����
        //this.BackColor = Color.Aqua;  //������� ������ , ������ ��� ��������
        this.BackgroundImage = new Bitmap("C:\\wate2.jpg"); //������ � �����
        

        //this.BackgroundImage = new Bitmap(Ships.Properties.Resources.water);   //�������� ������
        
        //������� ������
        ports = new ArrayList();
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                int x = 20, y = 20;
                Port port = new Port(i, x, y, 2100);
                port.evPort += new DelShip(this.HandlerEv);
                ports.Add(port);
            }
            if (i == 1)
            {
                int x = 720, y = 20;
                Port port = new Port(i, x, y, 2100);
                port.evPort += new DelShip(this.HandlerEv);
                ports.Add(port);
            }
            if (i == 2)
            {
                int x = 350, y = 650;
                Port port = new Port(i, x, y, 2100);
                port.evPort += new DelShip(this.HandlerEv);
                ports.Add(port);
            }
            //Port port = new Port(i, x , y, 2000);
        }
        /*
        port1 = new Port(1, 20, 20, 2000);
        port1.evPort += new DelShip(this.HandlerEv);

        port2 = new Port(2, 720, 20, 2000);
        port2.evPort += new DelShip(this.HandlerEv);

        port3 = new Port(3, 350, 650, 2000);
        port3.evPort += new DelShip(this.HandlerEv);
        */

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
        listBox.MouseClick += new MouseEventHandler(ListBox);
    }

    private void HandlerEv(Data D)
    {
        Invalidate();//������������
    }

    private void ListBox(object obj, EventArgs args)
    {
        //Console.WriteLine("� ���");
        int numSel = (int)listBox.SelectedItem;
        for (int i = 0; i < ships.Count; i++)
        {
            Ship ship = (Ship)ships[i];
            if (ship.N == numSel)
            {
                Console.WriteLine("�������� ������� � {0} = {1}",ship.N , ship.Speed);
                ship.ShipColor = Color.DarkCyan;
            }
        }
    }

    private void Add(object obj, EventArgs args)
    {
        numShip++;
        int randomSpeedColor = randomSpeed.Next(100, 500);
        Ship ship = new Ship(numShip, 500, 500, randomSpeedColor, Color.FromArgb(randomSpeedColor / 2, 0, 0));
        for (int i = 0; i < ports.Count; i++)
        {
            Port port = (Port)ports[i];
            port.evPort += new DelShip(ship.HandlerEv);
        }
        /*
        port1.evPort += new DelShip(ship.HandlerEv);
        port2.evPort += new DelShip(ship.HandlerEv);
        port3.evPort += new DelShip(ship.HandlerEv);
        */
        ships.Add(ship);
        listBox.Items.Add(ship.N);
    }

    private void Del(object obj, EventArgs args)
    {
        if (listBox.Items.Count == 0)
            MessageBox.Show("������� �������� ������ ���� �������", "������");
        else
        {
            if (listBox.SelectedIndex == -1)
                MessageBox.Show("�������� ����� ���������� ������� �� ������", "������");
            else
            {
                int numSel = (int)listBox.SelectedItem;
                for (int i = 0; i < ships.Count; i++)
                {
                    Ship ship = (Ship)ships[i];
                    if (ship.N == numSel)
                    {
                        shipCount++;
                        ships.Remove(ship);
                        listBox.Items.Remove(numSel);
                        ship.Finish();
                    }
                }
            }
        }
    }

    private void DelAll(object obj, EventArgs args)
    {
        if (ships.Count == 0)
            MessageBox.Show("�� ������� �� ������ �������", "������");
        else
        {
            shipCount += ships.Count;
            Console.WriteLine("shipCount = {0}", shipCount);
                                                                                                    /*for (int i = 0, j = 1; j <= shipCount; j++)  // i ��� ������ � �������� ��������  j ��� ������ �� �������
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



            for (int i = 0; i < ships.Count; i++) //������ ��������    ������� ������:���� ��������� , ���� ships.Count �� ������ ����
            {

                if (i < ships.Count)
                {
                    Ship ship = (Ship)ships[i];
                    ships.Remove(ship);
                    ship.Finish();
                    //Console.WriteLine("2for = {0}", i);
                    i--;
                }
                else
                {
                                                                                                    //Console.WriteLine("break");
                    break;//�� ����� , ����� �� ���� ������ ������� � ���� ���������� , ������� ���� ��������
                }
                //Console.WriteLine("3for = {0}", i);
            }
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        for (int i = 0; i < ports.Count; i++)
        {
            Port port = (Port)ports[i];
            if (port.N == 0)
            {
                g.DrawImage(Image.FromFile("C:\\PORTprozrachn_TEN2.jpg"), new Point(port.X - 30, port.Y));
                //Console.WriteLine("narisoval 1");
            }
            if (port.N == 1)
            {
                g.DrawImage(Image.FromFile("C:\\PORTprozrachn_TEN2.jpg"), new Point(port.X - 50, port.Y));
                //Console.WriteLine("narisoval 2"); 
            }
            if (port.N == 2)
            {
                g.DrawImage(Image.FromFile("C:\\PORTprozrachn_TEN2.jpg"), new Point(port.X, port.Y - 50));
                //Console.WriteLine("narisoval 3");
            }
        }
        /* 
        g.DrawImage(Ships.Properties.Resources.PORTprozrachn_TEN_, new Point(port1.X - 30, port.Y));
        g.DrawImage(Ships.Properties.Resources.PORTprozrachn_TEN_, new Point(port2.X - 50, port2.Y));
        g.DrawImage(Ships.Properties.Resources.PORTprozrachn_TEN_, new Point(port3.X, port3.Y - 50));
        */

        /* ������ �� ������� ����� �������� ��� ����� ������ , ���� ��� �� ������ ������ �������
        //e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 0)), port1.X, port1.Y, 50, 50);      //����1
        //e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 0, 255)), port2.X, port2.Y, 50, 50);      //����2
        //e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 0)), port3.X, port3.Y, 50, 50);    //����3
          
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
            e.Graphics.FillEllipse(new SolidBrush(ship.ShipColor), ship.X, ship.Y, 50, 20);           //���������� ����� �:  1)������ ������� (������� ������� �� ��������) 2.3)������������ ������� 4)������� 50 5)������� 20
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
        for (int i=0 ; i< ports.Count;i++)
        {
            Port port = (Port)ports[i];
            port.Finish();
        }
        //port3.Finish();
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