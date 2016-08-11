namespace MPKMonitor.DataModel
{
  /// <summary>
  /// Klasa przystanków pobieranych z Poznań API
  /// http://www.poznan.pl/mim/plan/map_service.html?mtype=pub_transport&co=cluster
  /// </summary>
  public class Stops
  {
    /// <summary>
    /// Położenie i typ przystanku
    /// </summary>
    public Geometry Geometry { get; private set; }
    /// <summary>
    /// Identyfikator przystanku
    /// </summary>
    public string Id { get; private set; }
    /// <summary>
    /// Zawsze przychodzi Feature - podejrzewam, że do przyszłego wykorzytania
    /// </summary>
    public string Type { get; private set; }
    /// <summary>
    /// Właściwości przystanku
    /// </summary>
    public Properties Properties { get; private set; }

    /// <summary>
    /// Klasa przystanków pobieranych z Poznań API
    /// </summary>
    /// <param name="argGeometry">Położenie i typ przystanku</param>
    /// <param name="argId">Identyfikator przystanku</param>
    /// <param name="argType">Zawsze przychodzi Feature - podejrzewam, że do przyszłego wykorzytania</param>
    /// <param name="argProperties">Właściwości przystanku</param>
    public Stops(Geometry argGeometry, string argId, string argType, Properties argProperties)
    {
      this.Geometry = argGeometry;
      this.Id = argId;
      this.Type = argType;
      this.Properties = argProperties;
    }
  }
}
