using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
    
//����������� ������� ����� ��������
abstract class Ships
    {
      private int speed;
      private int num;
      protected int x, y;
      int constsleep;                                                     //int dx,dy;
      //�������� �������
      public int N { get { return num; } }
      public int X { get { return x; } }
      public int Y { get { return y; } }
      public int Speed
      {
         get {return speed;}
         set {speed=value;}
      }
    }
//����������� ������� ������ ��������
abstract class Port1
    {
        protected int x, y;
        private bool free = true;
        private int sleeptime;
        //�������� �������
        public int Sleeptime
        {
            get { return sleeptime; }
            set { sleeptime = value; }
        }
    }

abstract class Port2
{
    protected int x, y; //�������� ���� ��� ���� ����� ������
    private bool free = true;
    private int sleeptime;
    //�������� �������
    public int Sleeptime
    {
        get { return sleeptime; }
        set { sleeptime = value; }
    }
}

abstract class Port3
{
    protected int x, y; //�������� ���� ��� ���� ����� ������
    private bool free = true;
    private int sleeptime;
    //�������� �������
    public int Sleeptime
    {
        get { return sleeptime; }
        set { sleeptime = value; }
    }
}

    
      









    class Program
    {
        static void Main(string[] args)
            {
                Application.Run(new Windows());
            }
    }
