using System.ComponentModel;
using System.Runtime.CompilerServices;
using MelodyGuess.Annotations;

namespace MelodyGuess
{
  public abstract class MelodyGridElement : INotifyPropertyChanged
  {
    protected MelodyGridElement(int row, int column)
    {
      Row = row;
      Column = column;
    }

    public int Row { get; set; }
    public int Column { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public class Category : MelodyGridElement
  {
    public string Name { get; set; }

    public Category(int row, int column, string name) : base(row, column)
    {
      Name = name;
    }
  }

  public class Note : MelodyGridElement
  {
    private bool myOn;

    public bool On
    {
      get { return myOn; }
      set { myOn = value; OnPropertyChanged();}
    }

    public string Url { get; set; }

    public Note(int row, int column, string url) : base(row, column)
    {
      Url = url;
      On = true;
    }
  }
}