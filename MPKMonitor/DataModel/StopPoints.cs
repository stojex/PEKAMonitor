using System;

namespace MPKMonitor.Data
{
  public class StopPoints : IEquatable<StopPoints>
  {
    public string Symbol { get; private set; }
    public string Name { get; private set; }

    public StopPoints(String argSymbol, String argName)
    {
      this.Symbol = argSymbol;
      this.Name = argName;
    }

    public bool Equals(StopPoints other)
    {
      return Symbol == other.Symbol && Name == other.Name;
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
