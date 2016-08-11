using System;

namespace MPKMonitor.Data
{
  public class Lines : IEquatable<Lines>
  {
    public string Name { get; private set; }

    public Lines(string argName)
    {
      this.Name = argName;
    } 

    public bool Equals(Lines other)
    {
      return Name == other.Name;
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
