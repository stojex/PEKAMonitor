using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MPKMonitor.Common;
using MPKMonitor.Data;
using MPKMonitor.DataModel;
using Windows.Data.Json;

namespace MPKMonitor.API
{
  /// <summary>
  /// Klasa pobierająca dane z systemu PEKA https://www.peka.poznan.pl/vm/
  /// </summary>
  class ApiPEKA
  {
    private static ApiPEKA _apiPEKA = new ApiPEKA();
    private ObservableCollection<Bollards> _bollards = new ObservableCollection<Bollards>();
    private ObservableCollection<BollardsOrder> _bollardsOrder = new ObservableCollection<BollardsOrder>();
    private ObservableCollection<DeparturesCollection> _departures = new ObservableCollection<DeparturesCollection>();
    private ObservableCollection<Lines> _lines = new ObservableCollection<Lines>();
    private ObservableCollection<StopPoints> _stopPoints = new ObservableCollection<StopPoints>();
    private ObservableCollection<Streets> _streets = new ObservableCollection<Streets>();

    public ObservableCollection<Bollards> OCBollards
    {
      get { return this._bollards; }
    }

    public ObservableCollection<BollardsOrder> OCBollardsOrder
    {
      get { return this._bollardsOrder; }
    }

    public ObservableCollection<DeparturesCollection> OCDepartures
    {
      get { return this._departures; }
    }

    public ObservableCollection<Lines> OCLines
    {
      get { return this._lines; }
    }

    public ObservableCollection<StopPoints> OCStopPoints
    {
      get { return this._stopPoints; }
    }

    public ObservableCollection<Streets> OCStreets
    {
      get { return this._streets; }
    }

    /// <summary>
    /// Pobierz przystanki po nazwie 
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Bollards>> getBollardsByStopPointAsync(string jsonContent)
    {
      await _apiPEKA.getBollardsByStopPointDataAsync(jsonContent);
      return _apiPEKA.OCBollards;
    }

    /// <summary>
    /// Pobierz przystanki po ulicy
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Bollards>> getBollardsByStreetAsync(string jsonContent)
    {
      await _apiPEKA.getBollardsByStreetDataAsync(jsonContent);
      return _apiPEKA.OCBollards;
    }

    /// <summary>
    /// Pobierz przystanki po linii
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<BollardsOrder>> getBollardsByLineAsync(string jsonContent)
    {
      await _apiPEKA.getBollardsByLineDataAsync(jsonContent);
      return _apiPEKA.OCBollardsOrder;
    }

    /// <summary>
    /// Pobierz odjazdy
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<DeparturesCollection>> GetDeparturesAsync(string jsonContent)
    {
      await _apiPEKA.GetDeparturesDataAsync(jsonContent);
      return _apiPEKA.OCDepartures;
    }

    /// <summary>
    /// Pobierz linie
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Lines>> GetLinesAsync(string jsonContent)
    {
      await _apiPEKA.GetLinesDataAsync(jsonContent);
      return _apiPEKA.OCLines;
    }

    /// <summary>
    /// Pobierz przystanki
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<StopPoints>> GetStopPointsAsync(string jsonContent)
    {
      await _apiPEKA.GetStopPointsDataAsync(jsonContent);
      return _apiPEKA.OCStopPoints;
    }

    /// <summary>
    /// Pobierz ulice
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Streets>> GetStreetsAsync(string jsonContent)
    {
      await _apiPEKA.GetStreetsDataAsync(jsonContent);
      return _apiPEKA.OCStreets;
    }

    private async Task getBollardsByStopPointDataAsync(string jsonContent)
    {
      this.OCBollards.Clear();
      string url = CommonFunction.GetUrl();
      Uri dataUri = new Uri(url);

      HttpClient client = new HttpClient();
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();
      if (responseString != "")
      {
        string jsonText = Convert.ToString(responseString);

      JsonObject jsonObject = JsonObject.Parse(jsonText);
      JsonObject jsonObjecd = jsonObject["success"].GetObject();
      JsonArray jsonArray = jsonObjecd["bollards"].GetArray();

      foreach (JsonValue bollardsValue in jsonArray)
      {
        JsonObject groupObject = bollardsValue.GetObject();
        ObservableCollection<Directions> OCDirections = new ObservableCollection<Directions>();

        foreach (JsonValue directionsValue in groupObject["directions"].GetArray())
        {
          JsonObject itemObject = directionsValue.GetObject();
          Directions Kierunki = new Directions(itemObject["returnVariant"].GetBoolean(),
                                               itemObject["direction"].GetString(),
                                               itemObject["lineName"].GetString());
          OCDirections.Add(Kierunki);
        }

        JsonObject bollardValue = groupObject.GetObject().GetNamedObject("bollard");
        Bollard Bolla = new Bollard(bollardValue["symbol"].GetString(),
                                    bollardValue["tag"].GetString(),
                                    bollardValue["name"].GetString(),
                                    bollardValue["mainBollard"].GetBoolean());

        Bollards BollardsItem = new Bollards(OCDirections, Bolla);
        this.OCBollards.Add(BollardsItem);
      }
    }
      else
      {
        throw new System.Net.WebException();
      }
}

    private async Task getBollardsByStreetDataAsync(string jsonContent)
    {
      this.OCBollards.Clear();
      string url = CommonFunction.GetUrl(); //GetUrl();
      Uri dataUri = new Uri(url);

      HttpClient client = new HttpClient();
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();
      if (responseString != "")
      {
        string jsonText = Convert.ToString(responseString);

      JsonObject jsonObject = JsonObject.Parse(jsonText);
      JsonObject jsonObjecd = jsonObject["success"].GetObject();
      JsonArray jsonArray = jsonObjecd["bollards"].GetArray();

      foreach (JsonValue bollardsValue in jsonArray)
      {
        JsonObject groupObject = bollardsValue.GetObject();
        ObservableCollection<Directions> OCDirections = new ObservableCollection<Directions>();

        JsonObject bollardValue = groupObject.GetObject().GetNamedObject("bollard");
        Bollard Bolla = new Bollard(bollardValue["symbol"].GetString(),
                                    bollardValue["tag"].GetString(),
                                    bollardValue["name"].GetString(),
                                    bollardValue["mainBollard"].GetBoolean());


        foreach (JsonValue directionsValue in groupObject["directions"].GetArray())
        {
          JsonObject itemObject = directionsValue.GetObject();
          Directions Kierunki = new Directions(itemObject["returnVariant"].GetBoolean(),
                                               itemObject["direction"].GetString(),
                                               itemObject["lineName"].GetString());
          OCDirections.Add(Kierunki);
        }

        Bollards BollardsItem = new Bollards(OCDirections, Bolla);
        this.OCBollards.Add(BollardsItem);
      }
      }
      else
      {
        throw new System.Net.WebException();
      }
    }

    private async Task getBollardsByLineDataAsync(string jsonContent)
    {
      this.OCBollardsOrder.Clear();
      string url = CommonFunction.GetUrl(); //GetUrl();
      Uri dataUri = new Uri(url);

      HttpClient client = new HttpClient();
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();
      if (responseString != "")
      {
        string jsonText = Convert.ToString(responseString);

      JsonObject jsonObject = JsonObject.Parse(jsonText);
      JsonObject jsonObjecd = jsonObject["success"].GetObject();
      JsonArray jsonArray = jsonObjecd["directions"].GetArray();

      foreach (JsonValue directionsValue in jsonArray)
      {
        JsonObject groupObject = directionsValue.GetObject();
        ObservableCollection<BollardOrder> OCBollardsOrder = new ObservableCollection<BollardOrder>();

        JsonObject DirectiValue = groupObject["direction"].GetObject();
        Directions Directi = new Directions(DirectiValue["returnVariant"].GetBoolean(),
                                             DirectiValue["direction"].GetString(),
                                             DirectiValue["lineName"].GetString());


        JsonArray cos = groupObject["bollards"].GetArray();
        foreach (JsonValue bollardValue in cos)
        {
          JsonObject itemObject = bollardValue.GetObject();
          BollardOrder Kierunki = new BollardOrder(itemObject["orderNo"].GetNumber(),
                                                   itemObject["symbol"].GetString(),
                                                   itemObject["tag"].GetString(),
                                                   itemObject["name"].GetString(),
                                                   itemObject["mainBollard"].GetBoolean());
          OCBollardsOrder.Add(Kierunki);
        }

        BollardsOrder BollardsItem = new BollardsOrder(Directi, OCBollardsOrder);
        this.OCBollardsOrder.Add(BollardsItem);
      }
    }
      else
      {
        throw new System.Net.WebException();
      }
}

    private async Task GetDeparturesDataAsync(string jsonContent)
    {
      this.OCDepartures.Clear();
      string url = CommonFunction.GetUrl();//GetUrl();
      Uri dataUri = new Uri(url);

      HttpClient client = new HttpClient();
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();

      if (responseString != "")
      {
        string jsonText = Convert.ToString(responseString);

        JsonObject jsonObject = JsonObject.Parse(jsonText);
        JsonObject jsonObjecd = jsonObject["success"].GetObject();

        JsonObject test = jsonObjecd.GetObject().GetNamedObject("bollard");

        Bollard Bolla = new Bollard(test["symbol"].GetString(),
                                    test["tag"].GetString(),
                                    test["name"].GetString(),
                                    test["mainBollard"].GetBoolean());

        JsonArray jsonArray = jsonObjecd["times"].GetArray();

        ObservableCollection<Departures> OCDepartures = new ObservableCollection<Departures>();
        foreach (JsonValue groupValue in jsonArray)
        {
          JsonObject groupObject1 = groupValue.GetObject();

          string tymczasowy = groupObject1["minutes"].GetNumber().ToString() + "m [" + String.Format("{0:t}", DateTime.Now.AddMinutes(groupObject1["minutes"].GetNumber())) + "]";

          Departures group = new Departures(groupObject1["realTime"].GetBoolean(),
                                            tymczasowy,
                                            groupObject1["direction"].GetString(),
                                            groupObject1["onStopPoint"].GetBoolean(),
                                            groupObject1["departure"].GetString(),
                                            groupObject1["line"].GetString());
          OCDepartures.Add(group);
        }
        DeparturesCollection DeparturesCollectionItem = new DeparturesCollection(Bolla, OCDepartures);
        this.OCDepartures.Add(DeparturesCollectionItem);
      }
      else
      {
        throw new System.Net.WebException();
      }
    }

    private async Task GetLinesDataAsync(string jsonContent)
    {
      this.OCLines.Clear();
      string url = CommonFunction.GetUrl();

      HttpClient client = new HttpClient();
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();
      if (responseString != "")
      {
        string jsonText = Convert.ToString(responseString);

        JsonObject jsonObject = JsonObject.Parse(jsonText);
        JsonArray jsonArray = jsonObject["success"].GetArray();

        foreach (JsonValue groupValue in jsonArray)
        {
          JsonObject groupObject = groupValue.GetObject();
          Lines group = new Lines(groupObject["name"].GetString());
          this.OCLines.Add(group);
        }
      }
      else
      {
        throw new System.Net.WebException();
      }
    }

    private async Task GetStopPointsDataAsync(string jsonContent)
    {
      this.OCStopPoints.Clear();
      string url = CommonFunction.GetUrl(); //GetUrl();
      Uri dataUri = new Uri(url);

      HttpClient client = new HttpClient();
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();
      if (responseString != "")
      {
        string jsonText = Convert.ToString(responseString);

        JsonObject jsonObject = JsonObject.Parse(jsonText);
        JsonArray jsonArray = jsonObject["success"].GetArray();

        foreach (JsonValue groupValue in jsonArray)
        {
          JsonObject groupObject = groupValue.GetObject();
          StopPoints group = new StopPoints(groupObject["symbol"].GetString(),
                                            groupObject["name"].GetString());
          this.OCStopPoints.Add(group);
        }
      }
      else
      {
        throw new System.Net.WebException();
      }
    }

    private async Task GetStreetsDataAsync(string jsonContent)
    {
      this.OCStreets.Clear();
      string url = CommonFunction.GetUrl(); //GetUrl();
      Uri dataUri = new Uri(url);

      HttpClient client = new HttpClient();
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");

      var response = await client.PostAsync(url, content);
      var responseString = await response.Content.ReadAsStringAsync();
      if (responseString != "")
      {

        string jsonText = Convert.ToString(responseString);

        JsonObject jsonObject = JsonObject.Parse(jsonText);
        JsonArray jsonArray = jsonObject["success"].GetArray();

        foreach (JsonValue groupValue in jsonArray)
        {
          JsonObject groupObject = groupValue.GetObject();

          Streets group = new Streets(groupObject["id"].GetNumber(),
                                      groupObject["name"].GetString());
          this.OCStreets.Add(group);
        }
      }
      else
      {
        throw new System.Net.WebException();
      }
    }

  }
}

