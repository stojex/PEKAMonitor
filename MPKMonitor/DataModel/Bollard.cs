namespace MPKMonitor.DataModel
{
  public class Bollard
  {
    public string Symbol { get; private set; }
    public string Tag { get; private set; }
    public string Name { get; private set; }
    public bool MainBollard { get; private set; }

    public Bollard(string argSymbol, string argTag, string argName, bool argMainBollard)
    {
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
