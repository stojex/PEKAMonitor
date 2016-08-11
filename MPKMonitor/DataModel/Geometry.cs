using Windows.Devices.Geolocation;

namespace MPKMonitor.DataModel
{
  /// <summary>
  /// Położenie i typ przystanku
  /// </summary>
  public class Geometry
  {
    /// <summary>
    /// Koordynaty GPS
    /// </summary>
    public Geopoint Coordinates { get; private set; }
    /// <summary>
    /// Typ - najczęściej "Point"
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Klasa punktu
    /// </summary>
    /// <param name="argCoordinates">Współrzędne GPS</param>
    /// <param name="argType">Typ punktu</param>
    public Geometry(Geopoint argCoordinates, string argType)
    {
      this.Coordinates = argCoordinates;
      this.Type = argType;
    }
  }
}
