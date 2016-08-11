using System.Collections.ObjectModel;
using MPKMonitor.Data;

namespace MPKMonitor.DataModel
{
  public class BollardsOrder
  {
    public BollardsOrder(Directions argDirections, ObservableCollection<BollardOrder> argBollard)
    {
      this.Directions = argDirections;
      this.OCBollardOrder = argBollard;
    }
    public ObservableCollection<BollardOrder> OCBollardOrder { get; private set; }
    public Directions Directions { get; private set; }
  }
}
