using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrainRing.Annotations;

namespace BrainRing
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    private int myTime;
    private const int TimeBound = 20;
    private SynchronizationContext myCtx;
    private SerialPort myPort;
    private int myRightPoints;
    private int myLeftPoints;
    private Timer myTimer;
    WMPLib.WindowsMediaPlayer myPlayer1 = new WMPLib.WindowsMediaPlayer();
    WMPLib.WindowsMediaPlayer myPlayer2 = new WMPLib.WindowsMediaPlayer();

    public int Time
    {
      get { return myTime; }
      set
      {
        if (value == myTime || value < 0) return;
        myTime = value;
        OnPropertyChanged();

        if (value == 5)
        {
          myPlayer1.controls.play();
          Task.Delay(1000).ContinueWith(_ => myPlayer1.controls.stop());
        }

        if (value == 0)
        {
          Write("e\n");
          myPlayer1.controls.play();
          Task.Delay(1500).ContinueWith(_ => myPlayer1.controls.stop());
        }
      }
    }

    public int LeftPoints
    {
      get { return myLeftPoints; }
      set
      {
        if (value == myLeftPoints) return;
        myLeftPoints = value;
        OnPropertyChanged();
      }
    }

    public int RightPoints
    {
      get { return myRightPoints; }
      set
      {
        if (value == myRightPoints) return;
        myRightPoints = value;
        OnPropertyChanged();
      }
    }

    private void Write(string s)
    {
      try
      {
        myPort.WriteLine(s);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    public MainWindow()
    {
      Time = TimeBound;
      InitializeComponent();
      myCtx = SynchronizationContext.Current;
      myPort = new SerialPort("COM4", 9600);
      myPlayer1.URL = "start.mp3";
      myPlayer1.controls.stop();
      myPlayer2.URL = "ring.mp3";
      myPlayer2.controls.stop();
      try
      {
        myPort.Open();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      myPort.DataReceived += (sender, args) =>
      {
        if (myPort.BytesToRead > 0)
        {
          string str = myPort.ReadExisting();
          if (str.Trim().Length > 0)
          {
            UseTimer(false);
              myPlayer2.controls.play();
              Task.Delay(2000).ContinueWith(_ => myPlayer2.controls.stop());
          }
        }
      };
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Escape:
          Close();
          break;

        case Key.NumPad6:
          RightPoints = RightPoints + 1;
          break;
        case Key.NumPad3:
          RightPoints = RightPoints - 1;
          break;
        case Key.NumPad4:
          LeftPoints = LeftPoints + 1;
          break;
        case Key.NumPad1:
          LeftPoints = LeftPoints - 1;
          break;
        case Key.F:
          UseTimer(false);
          Time = 99;
          Write("f");
          break;
        case Key.Q:
          UseTimer(false);
          Time = TimeBound;
          Write("q");
          break;
        case Key.S:
          UseTimer(true);
          Write("s");
          myPlayer1.controls.play();
          Task.Delay(1500).ContinueWith(_ => myPlayer1.controls.stop());
          break;

        case Key.C:
          UseTimer(true);
          Write("c");
          myPlayer1.controls.play();
          Task.Delay(1500).ContinueWith(_ => myPlayer1.controls.stop());
          break;
      }
    }

    public void UseTimer(bool start = false)
    {
      if (myTimer != null)
      {
        myTimer.Dispose();
        myTimer = null;
      }

      if (start)
      {
        myTimer = new Timer(_ =>
        {
          myCtx.Post(__ => Time--, null);
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
