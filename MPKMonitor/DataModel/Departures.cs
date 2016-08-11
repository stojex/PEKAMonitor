namespace MPKMonitor.Data
{
  public class Departures
  {
    public Departures(bool argRealTime, string argMinutes, string argDirection, bool argOnStopPoint, string argDeparture, string argLine)
    {
      this.RealTime = argRealTime;
      this.Minutes = argMinutes;
      this.Direction = argDirection;
      this.OnStopPoint = argOnStopPoint;
      this.Departure = argDeparture;
      this.Line = argLine;
    }

    public bool RealTime { get; private set; }
    public string Minutes { get; private set; }
    public string Direction { get; private set; }
    public bool OnStopPoint { get; private set; }
    public string Departure { get; private set; }
    public string Line { get; private set; }

    public override string ToString()
    {
      return this.Line;
    }
  }
}
