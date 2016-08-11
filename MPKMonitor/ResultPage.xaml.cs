using MPKMonitor.Common;
using MPKMonitor.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MPKMonitor.API;
using MPKMonitor.DataModel;

namespace MPKMonitor
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class ResultPage : Page
  {
    private NavigationHelper navigationHelper;
    private ObservableDictionary defaultViewModel = new ObservableDictionary();
    string arg = "";
    Bollard _bollard = null;
    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

    ObservableCollection<Directions> OCDirections = new ObservableCollection<Directions>();
    int timercount = 14;


    public ResultPage()
    {
      this.InitializeComponent();

      this.navigationHelper = new NavigationHelper(this);
      this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
      this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

      DispatcherTimer dispatcherTimer = new DispatcherTimer();
      dispatcherTimer.Tick += dispatcherTimer_Tick;
      dispatcherTimer.Interval = new TimeSpan(0, 0, timercount+1);
      dispatcherTimer.Start();

      DispatcherTimer dispatcherTimer2 = new DispatcherTimer();
      dispatcherTimer2.Tick += dispatcherTimer2_Tick;
      dispatcherTimer2.Interval = new TimeSpan(0, 0, 1);
      dispatcherTimer2.Start();
    }
    
    
    private void dispatcherTimer_Tick(object sender, object e)
    {
      LoadAndRefreshData();
      timercount = 14; 
    }
 
    private void dispatcherTimer2_Tick(object sender, object e)
    {
      tbAutmaticRefresh.Text = "Automatyczne odświeżenie za " + timercount.ToString() + " s.";
      timercount--;
    }


    /// <summary>
    /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
    /// </summary>
    public NavigationHelper NavigationHelper
    {
      get { return this.navigationHelper; }
    }

    /// <summary>
    /// Gets the view model for this <see cref="Page"/>.
    /// This can be changed to a strongly typed view model.
    /// </summary>
    public ObservableDictionary DefaultViewModel
    {
      get { return this.defaultViewModel; }
    }

    /// <summary>
    /// Populates the page with content passed during navigation.  Any saved state is also
    /// provided when recreating a page from a prior session.
    /// </summary>
    /// <param name="sender">
    /// The source of the event; typically <see cref="NavigationHelper"/>
    /// </param>
    /// <param name="e">Event data that provides both the navigation parameter passed to
    /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
    /// a dictionary of state preserved by this page during an earlier
    /// session.  The state will be null the first time a page is visited.</param>
    private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
    {
      arg = (string)e.NavigationParameter;

      if ((App.Current as App).favouriteBollard.ContainsKey(arg))
      {
        btDelFromFavourite.Visibility = Visibility.Visible;
      }
      else
      {
        btAddToFavourite.Visibility = Visibility.Visible;
      }

      LoadAndRefreshData();

    }

    private void abbRefreshManual_Click(object sender, RoutedEventArgs e)
    {
      LoadAndRefreshData();
    }

    private async void LoadAndRefreshData()
    {
      progress1.IsActive = true;
      progress1.Visibility = Visibility.Visible;

      string pattern = Uri.EscapeDataString(arg);
      string jsonContent = "method=getTimes&p0=%7B%22symbol%22%3A%22" + pattern + "%22%7D";

      if (pattern != "")
      {
        try
        {
          var bollardsList = await ApiPEKA.GetDeparturesAsync(jsonContent);
          //string test = bollardsList.ToString();

          ObservableCollection<Departures> OCDepartures = new ObservableCollection<Departures>();

          foreach (DeparturesCollection item in bollardsList)
          {
            OCDepartures = item.Departures;
            _bollard = item.Bollard;
          }

          tbTitle.Text = _bollard.Name.ToString();
          lvResults.ItemsSource = OCDepartures;
        }
        catch (Exception ex)
        {
          if (ex is System.Net.WebException)
          {
            CommonFunction.MessageBoxShow("Problem z pobraniem danych. Sprawdź połączenie internetowe. \nByć może system PEKA nie działa.");
          }
          else
          {
            CommonFunction.SendException(ex, "jssonContent: " + jsonContent);
          }
        }
      }
      progress1.IsActive = false;
      progress1.Visibility = Visibility.Collapsed;
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

    #region NavigationHelper registration

    /// <summary>
    /// The methods provided in this section are simply used to allow
    /// NavigationHelper to respond to the page's navigation methods.
    /// <para>
    /// Page specific logic should be placed in event handlers for the  
    /// <see cref="NavigationHelper.LoadState"/>
    /// and <see cref="NavigationHelper.SaveState"/>.
    /// The navigation parameter is available in the LoadState method 
    /// in addition to page state preserved during an earlier session.
    /// </para>
    /// </summary>
    /// <param name="e">Provides data for navigation methods and event
    /// handlers that cannot cancel the navigation request.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      this.navigationHelper.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.navigationHelper.OnNavigatedFrom(e);
    }

    #endregion

    private void abbDelFromFavourite_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (arg != "")
        {
          (App.Current as App).favouriteBollard.Remove(arg);

          btDelFromFavourite.Visibility = Visibility.Collapsed;
          btAddToFavourite.Visibility = Visibility.Visible;
          StoreFavouriteList();
        }
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, "arg: " + arg);
      }
    }

    private void abbAddToFavourite_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (arg != "" && _bollard != null)
        {
          (App.Current as App).favouriteBollard.Add(arg, _bollard.Name.ToString());

          btAddToFavourite.Visibility = Visibility.Collapsed;
          btDelFromFavourite.Visibility = Visibility.Visible;
          StoreFavouriteList();
        }
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, "_bollard.Name: " + _bollard.Name.ToString());
      }
    }

    private async void StoreFavouriteList()
    {
      string zapis = "";
      try
      {
        StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.ReplaceExisting);
        
        var d = (App.Current as App).favouriteBollard;
        foreach (KeyValuePair<string, string> pair in d)
        {
          zapis += pair.Key + "\t" + pair.Value + "\n";
        }
        await FileIO.WriteTextAsync(sampleFile, zapis);
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, zapis);
      }  
    }
  }
}
