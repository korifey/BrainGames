using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
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
using MelodyGuess.Annotations;
using Path = System.IO.Path;

namespace MelodyGuess
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
    WMPLib.WindowsMediaPlayer myMelodyPlayer = new WMPLib.WindowsMediaPlayer();
    WMPLib.WindowsMediaPlayer myBellPlayer = new WMPLib.WindowsMediaPlayer();

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



    public ObservableCollection<MelodyGridElement> Elements { get; set; } 

    public MainWindow()
    {
      //Elements.Add(new MelodyGridElement(0, 0));
      const string mainDir = "data";
      const int n = 4;
      Elements = new ObservableCollection<MelodyGridElement>();

      int i = 0;
      foreach (var dir in Directory.EnumerateDirectories(mainDir))
      {      
        Elements.Add(new Category(i, 0, new DirectoryInfo(dir).Name));
        int j = 0;
        foreach (var f in Directory.EnumerateFiles(dir).Where(_ => _.EndsWith(".mp3")))
        {
          Elements.Add(new Note(i, j+1, f));
          if (++j == n) break;
        }
        if (++i == n) break;
      }


      InitializeComponent();

      myCtx = SynchronizationContext.Current;
      myPort = new SerialPort("COM3", 9600);
      myMelodyPlayer.controls.stop();
      myBellPlayer.URL = "ring.mp3";
      myBellPlayer.controls.stop();
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
          string str = myPort.ReadExisting().Trim();
          if (str == "4")
          {
            myMelodyPlayer.controls.pause();
            myBellPlayer.controls.play();
            Task.Delay(2000).ContinueWith(_ => myBellPlayer.controls.stop());
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

        case Key.L:
          RightPoints = RightPoints + 1;
          break;
        case Key.K:
          RightPoints = RightPoints - 1;
          break;
        case Key.O:
          LeftPoints = LeftPoints + 1;
          break;
        case Key.I:
          LeftPoints = LeftPoints - 1;
          break;
        case Key.F:
          Write("f");
          myMelodyPlayer.controls.stop();
          myBellPlayer.controls.stop();
          break;
        case Key.M:
          myBellPlayer.controls.stop();
          const string melody = "main.mp3";
          myMelodyPlayer.controls.stop();
          myMelodyPlayer.URL = melody;
          myMelodyPlayer.controls.play();
          break;
        case Key.Q:
          Write("q");
          break;
        case Key.E:
          Write("e");
          myMelodyPlayer.controls.stop();
          break;
        case Key.C:
          Write("c");
          myMelodyPlayer.controls.play();
          break;
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


    private void OnLeftButton(object sender, MouseButtonEventArgs e)
    {
      var img = sender as Image;
      if (img == null) return;

      var note = (Note) img.Tag;
      if (!note.On) return;

      note.On = false;
      myMelodyPlayer.URL = note.Url;
      myMelodyPlayer.controls.play();
      Write("qs");
    }

    private void OnRightButton(object sender, MouseButtonEventArgs e)
    {
      var img = sender as Image;
      if (img == null) return;

      var note = (Note)img.Tag;
      if (note.On) return;

      note.On = true;
      myMelodyPlayer.controls.stop();
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
