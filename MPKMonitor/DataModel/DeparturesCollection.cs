using System.Collections.ObjectModel;
using MPKMonitor.Data;

namespace MPKMonitor.DataModel
{
  public class DeparturesCollection
  {
    public DeparturesCollection(Bollard argBollard, ObservableCollection<Departures> argDirections)
    {
      this.Departures = argDirections;
      this.Bollard = argBollard;
    }

    public ObservableCollection<Departures> Departures { get; private set; }
    public Bollard Bollard { get; private set; }
  }
}
