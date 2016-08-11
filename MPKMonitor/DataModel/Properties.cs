namespace MPKMonitor.DataModel
{
  public class Properties
  {
    /// <summary>
    /// Strafa przystanku
    /// </summary>
    public string Zone { get; private set; }
    /// <summary>
    /// Typ trasy 0 - tramwaj, 3 - autobus
    /// </summary>
    public string Route_type { get; private set; }
    /// <summary>
    /// Linie odjeżdżające z przystanku
    /// </summary>
    public string Headsigns { get; private set; }
    /// <summary>
    /// Nazwa przystanku
    /// </summary>
    public string Stop_name { get; private set; }

    /// <summary>
    /// Właściwości przystanku
    /// </summary>
    /// <param name="argZone">Strafa przystanku</param>
    /// <param name="argRoute_type">Typ trasy 0 - tramwaj, 3 - autobus</param>
    /// <param name="argHeadsigns">Linie odjeżdżające z przystanku</param>
    /// <param name="argStop_name">Nazwa przystanku</param>
    public Properties(string argZone, string argRoute_type, string argHeadsigns, string argStop_name)
    {
      this.Zone = argZone;
      this.Route_type = argRoute_type;
      this.Headsigns = argHeadsigns;
      this.Stop_name = argStop_name;
    }
  }
}
