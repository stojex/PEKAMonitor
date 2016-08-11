using System;

namespace MPKMonitor.Data
{
  public class Streets : IEquatable<Streets>
  {
    public double Id { get; private set; }
    public string Name { get; private set; }

    public Streets(double argId, string argName)
    {
      this.Id = argId;
      this.Name = argName;
    }

    public bool Equals(Streets other)
    {
      return Id == other.Id && Name == other.Name;
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
