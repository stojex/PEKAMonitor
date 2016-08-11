using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MPKMonitor.API;
using MPKMonitor.Common;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace MPKMonitor
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Maps : Page
  {
    private NavigationHelper navigationHelper;

    public Maps()
    {
      this.InitializeComponent();
      this.navigationHelper = new NavigationHelper(this);
      this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
      this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
    }
    Geolocator geolocator;

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.
    /// This parameter is typically used to configure the page.</param>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      mcMap.MapServiceToken = "431cbeb0-923b-4fbe-9a64-01052749dd11";
      this.navigationHelper.OnNavigatedTo(e);


      MapPolyline line = new MapPolyline();
      line.StrokeColor = Colors.Red;
      line.StrokeThickness = 5;
      var punkty = await ApiPoznan.GetGeopointAsync();

      List<BasicGeoposition> PosList = new List<BasicGeoposition>();
      foreach (var item in punkty)
      {
        PosList.Add(new BasicGeoposition()
        {
          Latitude = item.Position.Latitude,
          Longitude = item.Position.Longitude
        });
      }
      line.Path = new Geopath(PosList);

      mcMap.MapElements.Add(line);


      var streets = await ApiPoznan.GetStopsAsync();
      foreach (var item in streets)
      {
        var jayway1 = new Geopoint(new BasicGeoposition() { Latitude = item.Geometry.Coordinates.Position.Latitude, Longitude = item.Geometry.Coordinates.Position.Longitude });
        var youPin1 = CreatePin(item.Properties.Stop_name, item.Properties.Route_type, item.Id);
        mcMap.Children.Add(youPin1);
        MapControl.SetLocation(youPin1, jayway1);
        MapControl.SetNormalizedAnchorPoint(youPin1, new Point(0.5, 0.5));

      }





      //foreach (var item in streets)
      //{
      //  mcMap.MapElements.Add(CreateIconToMap(item.Properties.Stop_name, item.Properties.Route_type, item.Geometry.Coordinates.Position.Latitude, item.Geometry.Coordinates.Position.Longitude));
      //}
      //-------------------------


      //MapPolyline line = new MapPolyline();
      //line.StrokeColor = Colors.Red;
      //line.StrokeThickness = 10;
      //line.Path.Add(new GeoCoordinate(47.6602, -122.098358));
      //line.Path.Add(new GeoCoordinate(47.561482, -122.071544));
      //MyMap.MapElements.Add(line);

      //-------------------------
      //var jayway = new Geopoint(new BasicGeoposition() { Latitude = 52.4385179, Longitude = 16.93107195 });
      //var youPin = CreatePin("test", "3", "");
      //mcMap.Children.Add(youPin);
      //MapControl.SetLocation(youPin, jayway);
      //MapControl.SetNormalizedAnchorPoint(youPin, new Point(0.0, 1.0));
      //await mcMap.TrySetViewAsync(jayway, 15, 0, 0, MapAnimationKind.Bow);

    }

    private DependencyObject CreatePin(string tag, string typ, string id)
    {
      //Creating a Grid element.
      var myGrid = new Grid();
      myGrid.RowDefinitions.Add(new RowDefinition());
      myGrid.RowDefinitions.Add(new RowDefinition());
      myGrid.Background = new SolidColorBrush(Colors.Transparent);

      var cos = new TextBlock();
      cos.Text = tag;
      cos.Tag = id;
      cos.Foreground = new SolidColorBrush(Colors.Black);
      cos.SetValue(Grid.RowProperty, 0);
      myGrid.Children.Add(cos);

      Image obrazek = new Image();

      if (typ == "3")
      {
        obrazek.Source = new BitmapImage(new Uri("ms-appx:///Icons/autobus.png", UriKind.Absolute));
      }
      else
      {
        obrazek.Source = new BitmapImage(new Uri("ms-appx:///Icons/tramwaj.png", UriKind.Absolute));
      }
      obrazek.Height = 16;
      obrazek.Width = 16;
      obrazek.Tag = id;
      obrazek.SetValue(Grid.RowProperty, 1);
      myGrid.Children.Add(obrazek);

      return myGrid;


      #region oryginalny kod
      ////Creating a Grid element.
      //var myGrid = new Grid();
      //myGrid.RowDefinitions.Add(new RowDefinition());
      //myGrid.RowDefinitions.Add(new RowDefinition());
      //myGrid.Background = new SolidColorBrush(Colors.Transparent);

      ////Creating a Rectangle
      //var myRectangle = new Rectangle { Fill = new SolidColorBrush(Colors.Black), Height = 20, Width = 20 };
      //myRectangle.SetValue(Grid.RowProperty, 0);
      //myRectangle.SetValue(Grid.ColumnProperty, 0);
      //myRectangle.Tag = "PSAM02";

      ////Adding the Rectangle to the Grid
      //myGrid.Children.Add(myRectangle);

      ////Creating a Polygon
      //var myPolygon = new Polygon()
      //{
      //  Points = new PointCollection() { new Point(2, 0), new Point(22, 0), new Point(2, 40) },
      //  Stroke = new SolidColorBrush(Colors.Black),
      //  Fill = new SolidColorBrush(Colors.Black)
      //};
      //myPolygon.SetValue(Grid.RowProperty, 1);
      //myPolygon.SetValue(Grid.ColumnProperty, 0);

      ////Adding the Polygon to the Grid
      //myGrid.Children.Add(myPolygon);
      //return myGrid;

      #endregion




    }

    private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
    {
      //arg = (Coordinates)e.NavigationParameter;

      geolocator = new Geolocator();
      geolocator.DesiredAccuracyInMeters = 10;

      try
      {
        //Getting Current Location
        Geoposition geoposition = await geolocator.GetGeopositionAsync(
            maximumAge: TimeSpan.FromMinutes(5),
            timeout: TimeSpan.FromSeconds(10));

        MapIcon mapIcon = new MapIcon();
        // Locate your MapIcon  
        mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Icons/refresh.png"));
        // Show above the MapIcon  
        mapIcon.Title = "Twoja lokalizacja";
        // Setting up MapIcon location  
        mapIcon.Location = new Geopoint(new BasicGeoposition()
        {
          Latitude = geoposition.Coordinate.Point.Position.Latitude,
          Longitude = geoposition.Coordinate.Point.Position.Longitude
        });
        // Positon of the MapIcon  
        mapIcon.NormalizedAnchorPoint = new Point(0.0, 0.0);


        mcMap.MapElements.Add(mapIcon);
        // Showing in the Map  

        //mcMap.MapElements.Add(CreateIconToMap("PSAM01", 0, 52.43851790, 16.93107195));
        //mcMap.MapElements.Add(CreateIconToMap("PSAM02", 0, 52.43907816, 16.93037079));


        await mcMap.TrySetViewAsync(mapIcon.Location, 10D, 0, 0, MapAnimationKind.Bow);

      }
      catch (UnauthorizedAccessException)
      {
        //MessageBox("Location service is turned off!");
      }
    }

    /// <summary>
    /// Create MapIcon object to add to MapControl
    /// </summary>
    /// <param name="Title">Title of point</param>
    /// <param name="ImageType">Type of image 3-autobus, 0-tramwaj</param>
    /// <param name="Lat">GPS Latitude</param>
    /// <param name="Lon">GPS Longitude</param>
    /// <returns>MapIcon object</returns>
    MapIcon CreateIconToMap(string Title, string ImageType, double Lat, double Lon)
    {
      MapIcon mapIcon = new MapIcon();

      if (ImageType == "3")
      {
        mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Icons/autobus.png"));
      }
      else
      {
        mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Icons/tramwaj.png"));
      }
      // Show above the MapIcon  
      mapIcon.Title = Title;
      // Setting up MapIcon location  
      mapIcon.Location = new Geopoint(new BasicGeoposition()
      {
        Latitude = Lat,
        Longitude = Lon
      });
      // Positon of the MapIcon  
      mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
      return mapIcon;
    }

    /// <summary>
    /// Preserves state associated with this page in case the application is suspended or the
    /// page is discarded from the navigation cache.  Values must conform to the serialization
    /// requirements of <see cref="SuspensionManager.SessionState"/>.
    /// </summary>
    /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
    /// <param name="e">Event data that provides an empty dictionary to be populated with
    /// serializable state.</param>
    private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
    {
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.navigationHelper.OnNavigatedFrom(e);
    }

    public NavigationHelper NavigationHelper
    {
      get { return this.navigationHelper; }
    }

    private void mcMap_Tapped(object sender, TappedRoutedEventArgs e)
    {
      //var itemId = (sender as MapControl);
      var itemId = e.OriginalSource;
      var cos = itemId as Image;
      if (cos != null)
      {
        var dupa = cos.Tag;

        if (!Frame.Navigate(typeof(ResultPage), dupa))
        {

        }
      }


      //CommonFunction.MessageBoxShow("Klikłem");
    }

  }
}