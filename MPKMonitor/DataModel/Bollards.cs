using System.Collections.ObjectModel;
using MPKMonitor.DataModel;

namespace MPKMonitor.Data
{
  public class Bollards
  {
    public Bollards(ObservableCollection<Directions> argDirections, Bollard argBollard)
    {
      this.OCDirections = argDirections;
      this.Bollard = argBollard;
    }
    public ObservableCollection<Directions> OCDirections { get; private set; }
    public Bollard Bollard { get; private set; }
  }
}
