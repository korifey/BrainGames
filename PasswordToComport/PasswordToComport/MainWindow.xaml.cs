using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
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

namespace PasswordToComport
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private SerialPort myPort;

    public MainWindow()
    {
      InitializeComponent();
      Password.Focus();
      string[] cmdln = Environment.GetCommandLineArgs();
      var port = "COM" + ((cmdln != null && cmdln.Length > 1) ? cmdln[1] : "3");
      Console.WriteLine("Connecting to port: {0}", port);
      myPort = new SerialPort(port, 9600);
      try
      {
        myPort.Open();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
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

    private void Password_OnKeyDown(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Enter:
          if (Password.Text.ToLower() == "spring")
          {
            Write("x");
          } else if (Password.Text.ToLower() == "jetexit")
          {
            Close();
          }
          break;
      }
    }

    private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
    {
//      switch (e.Key)
//      {
//        case Key.Escape:
//          Close();
//          break;
//      }
    }
  }
}
