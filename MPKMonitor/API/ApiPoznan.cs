using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MPKMonitor.DataModel;
using Windows.Data.Json;
using Windows.Devices.Geolocation;

namespace MPKMonitor.API
{
  class ApiPoznan
  {
    private static ApiPoznan _apiPoznan = new ApiPoznan();
    private ObservableCollection<Stops> _stopPoints = new ObservableCollection<Stops>();
    private ObservableCollection<Geopoint> _geocoordinate = new ObservableCollection<Geopoint>();

    /// <summary>
    /// ObservableCollection of stop points from ApiPoznan
    /// </summary>
    public ObservableCollection<Stops> OCStops
    {
      get { return this._stopPoints; }
    }

    /// <summary>
    /// ObservableCollection of Geoocoordinate from ApiPoznan
    /// </summary>
    public ObservableCollection<Geopoint> OCGeocoordinate
    {
      get { return this._geocoordinate; }
    }

    /// <summary>
    /// Get stop points from ApiPoznan
    /// </summary>
    /// <returns>ObservableCollection of stop points</returns>
    public static async Task<IEnumerable<Stops>> GetStopsAsync()
    {
      await _apiPoznan.GetStopsDataAsync();
      return _apiPoznan.OCStops;
    }

    /// <summary>
    /// Get stop points from ApiPoznan
    /// </summary>
    /// <returns>ObservableCollection of stop points</returns>
    public static async Task<IEnumerable<Geopoint>> GetGeopointAsync()
    {
      await _apiPoznan.GetGeopointDataAsync();
      return _apiPoznan.OCGeocoordinate;
    }


    private async Task GetStopsDataAsync()
    {
      this.OCStops.Clear();
      string url = "http://www.poznan.pl/mim/plan/map_service.html?mtype=pub_transport&co=cluster";
      Uri dataUri = new Uri(url);

      HttpClient client = new HttpClient();
      var content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();
      if (responseString != "")
      {
        string jsonText = Convert.ToString(responseString);

        JsonObject jsonObject = JsonObject.Parse(jsonText);
        JsonArray jsonArray = jsonObject["features"].GetArray();

        foreach (JsonValue groupValue in jsonArray)
        {
          JsonObject groupObject = groupValue.GetObject();
          JsonObject geometry = groupObject["geometry"].GetObject();
          JsonArray cos11 = geometry["coordinates"].GetArray();

          double x = 0;
          double y = 0;
          int i = 0;

          foreach (JsonValue groupValue1 in cos11)
          {
            if (i == 0)
            {
              x = groupValue1.GetNumber();
              i = 1;
            }
            if (i == 1)
            {
              y = groupValue1.GetNumber();
            }

          }
          Geopoint iksy = new Geopoint(new BasicGeoposition()
          {
            Latitude = y,
            Longitude = x
          });
          Geometry wymiary = new Geometry(iksy, geometry["type"].GetString());

          JsonObject _properties = groupObject["properties"].GetObject();

          Properties wlasciwosci = new Properties(_properties["zone"].GetString(),
                                                  _properties["route_type"].GetString(),
                                                  _properties["headsigns"].GetString(),
                                                  _properties["stop_name"].GetString());
          Stops przystanek = new Stops(wymiary,
                                       groupObject["id"].GetString(),
                                       groupObject["type"].GetString(),
                                       wlasciwosci);

          this.OCStops.Add(przystanek);
        }
      }
      else
      {
        throw new System.Net.WebException();
      }
    }


    private async Task GetGeopointDataAsync()
    {
      this.OCGeocoordinate.Clear();
      string url = "http://www.poznan.pl/mim/plan/map_service.html?co=feature&mtype=pub_transport&id=5_10547870";
      Uri dataUri = new Uri(url);

      HttpClient client = new HttpClient();
      var content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();
      if (responseString != "")
      {
        string jsonText = Convert.ToString(responseString);

        JsonObject jsonObject = JsonObject.Parse(jsonText);
        JsonArray jsonArray = jsonObject["features"].GetArray();

        foreach (JsonValue groupValue in jsonArray)
        {
          JsonObject groupObject = groupValue.GetObject();
          JsonObject geometry = groupObject["geometry"].GetObject();
          JsonArray cos11 = geometry["coordinates"].GetArray();

          double x = 0;
          double y = 0;
          int i = 0;

          foreach (JsonValue groupValue1 in cos11)
          {

            JsonArray maciej = groupValue1.GetArray();
            foreach (JsonValue groupValue11 in maciej)
            {
              if (i == 0)
              {
                x = groupValue11.GetNumber();

              }
              if (i == 1)
              {
                y = groupValue11.GetNumber();
              }
              i = 1;
            }
            Geopoint iksy = new Geopoint(new BasicGeoposition()
            {
              Latitude = y,
              Longitude = x
            });
            i = 0;
            this.OCGeocoordinate.Add(iksy);
          }

          //Geometry wymiary = new Geometry(iksy, geometry["type"].GetString());

          //JsonObject _properties = groupObject["properties"].GetObject();

          //Properties wlasciwosci = new Properties(_properties["zone"].GetString(),
          //                                        _properties["route_type"].GetString(),
          //                                        _properties["headsigns"].GetString(),
          //                                        _properties["stop_name"].GetString());
          //Stops przystanek = new Stops(wymiary,
          //                             groupObject["id"].GetString(),
          //                             groupObject["type"].GetString(),
          //                             wlasciwosci);

          //this.OCGeocoordinate.Add(iksy);
        }
      }
      else
      {
        throw new System.Net.WebException();
      }
    }



  }
}
