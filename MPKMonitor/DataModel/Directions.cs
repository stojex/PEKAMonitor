namespace MPKMonitor.Data
{
  public class Directions
  {
    public bool ReturnVariant { get; private set; }
    public string Direction { get; private set; }
    public string LineName { get; private set; }
    public Directions(bool argReturnVariant, string argDirection, string argLineName)
    {
      this.ReturnVariant = argReturnVariant;
      this.Direction = argDirection;
      this.LineName = argLineName;
    }

    public override string ToString()
    {
      return this.LineName;
    }
  }
}
