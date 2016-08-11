namespace MPKMonitor.DataModel
{
  public class BollardOrder
  {
    public double OrderNo { get; private set; }
    public string Symbol { get; private set; }
    public string Tag { get; private set; }
    public string Name { get; private set; }
    public bool MainBollard { get; private set; }

    public BollardOrder(double argOrderNo, string argSymbol, string argTag, string argName, bool argMainBollard)
    {
      this.OrderNo = argOrderNo;
      this.Symbol = argSymbol;
      this.Tag = argTag;
      this.Name = argName;
      this.MainBollard = argMainBollard;
    }
    public override string ToString()
    {
      return this.Name;
    }
  }
}
