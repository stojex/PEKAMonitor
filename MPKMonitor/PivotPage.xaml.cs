using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MPKMonitor.API;
using MPKMonitor.Common;
using MPKMonitor.Data;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MPKMonitor
{
  public sealed partial class PivotPage : Page
  {
    private readonly NavigationHelper navigationHelper;
    private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
    private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

    ObservableCollection<StopPoints> OCStopPoints = new ObservableCollection<StopPoints>();
    ObservableCollection<StopPoints> OCHistoryStopPoints = new ObservableCollection<StopPoints>();
    ObservableCollection<Lines> OCHistoryLines = new ObservableCollection<Lines>();
    ObservableCollection<Streets> OCHistoryStreets = new ObservableCollection<Streets>();

    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

    public PivotPage()
    {
      this.InitializeComponent();

      this.NavigationCacheMode = NavigationCacheMode.Required;

      this.navigationHelper = new NavigationHelper(this);
      this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
      this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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
    /// Populates the page with content passed during navigation. Any saved state is also
    /// provided when recreating a page from a prior session.
    /// </summary>
    /// <param name="sender">
    /// The source of the event; typically <see cref="NavigationHelper"/>.
    /// </param>
    /// <param name="e">Event data that provides both the navigation parameter passed to
    /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
    /// a dictionary of state preserved by this page during an earlier
    /// session. The state will be null the first time a page is visited.</param>
    private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
    {
      labelHistoria.Visibility = Visibility.Visible;
      labelpiStreetsHistoria.Visibility = Visibility.Visible;
      labelpiLinesHistoria.Visibility = Visibility.Visible;
      await ReadFavouriteList();
      await ReadHistoryStopList();
      await ReadHistoryLines();
      await ReadHistoryStreets();

      var favouriteBollardList = (App.Current as App).favouriteBollard;

      if (favouriteBollardList.Count > 0)
      {
        tbClearFavouriteList.Visibility = Visibility.Collapsed;
        //TODO: Należy dodać TextBlock tbClearFavouriteList żeby dodawał się po stronie kodu, ponieważ poniższa komenda usunie go z ekranu.
        spFavouriteList.Children.Clear();
        int i = 0;
        foreach (KeyValuePair<string, string> pair in favouriteBollardList)
        {

          string btContent = pair.Value;
          string btTag = pair.Key;
          string btDirection = "";

          //foreach (Directions Directionitem in OCDepartures)
          //{
          //  btDirection += Directionitem.LineName + " -> " + Directionitem.Direction + ", ";
          //}

          string sXAML = @"<Button Margin=""10,0,10,0"" VerticalAlignment=""Stretch"" HorizontalAlignment=""Stretch"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" Name=""btBollard";
          sXAML += i.ToString();
          sXAML += @""" Tag=""";
          sXAML += btTag;
          sXAML += @""" Background=""YellowGreen"" Foreground=""White"" HorizontalContentAlignment=""Stretch"" VerticalContentAlignment=""Stretch"" >
                <Grid Margin=""0,0,0,0"" HorizontalAlignment=""Stretch"" VerticalAlignment=""Stretch"">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition />
                    <ColumnDefinition />
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition />
                    <RowDefinition />
                </Grid.RowDefinitions>
                <TextBlock Grid.Column=""0"" Grid.Row=""0"" Width=""500*"" Text=""";
          sXAML += btContent;
          sXAML += @""" Style=""{ThemeResource ListViewItemContentTextBlockStyle}"" HorizontalAlignment=""Left"" FontSize=""20"" FontWeight=""Bold"" />
                  <TextBlock Grid.Column=""1"" Grid.Row =""0"" Width=""100"" Text=""";
          sXAML += btTag;
          sXAML += @""" HorizontalAlignment=""Right"" FontSize=""20"" FontWeight=""Bold"" Style=""{ThemeResource ListViewItemContentTextBlockStyle}"" />
                <TextBlock Grid.Column=""0"" Grid.ColumnSpan=""2"" Grid.Row= ""1"" TextWrapping=""WrapWholeWords"" Style=""{ThemeResource ListViewItemContentTextBlockStyle}"" Foreground=""White"" FontSize=""18"" FontWeight=""Light"" Text=""";
          sXAML += btDirection;
          sXAML += @""" HorizontalAlignment=""Left"" /></Grid></Button>";

          object itemsTemplate = Windows.UI.Xaml.Markup.XamlReader.Load(sXAML) as Button;
          var tmp = itemsTemplate as Button;
          tmp.Click += btBollard_Click;

          spFavouriteList.Children.Add(tmp as Button);
          i++;

        }
      }
      else
      {
        tbClearFavouriteList.Visibility = Visibility.Visible;
      }


      try
      {
        lvStops.ItemsSource = null;
        lvStops.ItemsSource = new ObservableCollection<StopPoints>(OCHistoryStopPoints.Reverse());
        lvLine.ItemsSource = null;
        lvLine.ItemsSource = new ObservableCollection<Lines>(OCHistoryLines.Reverse());
        lvStreets.ItemsSource = null;
        lvStreets.ItemsSource = new ObservableCollection<Streets>(OCHistoryStreets.Reverse()); ;
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, "");
      }

    }

    void btBollard_Click(object sender, RoutedEventArgs e)
    {
      var itemId = (sender as Button).Tag;

      if (!Frame.Navigate(typeof(ResultPage), itemId))
      {
        //throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
      }
    }
    /// <summary>
    /// Preserves state associated with this page in case the application is suspended or the
    /// page is discarded from the navigation cache. Values must conform to the serialization
    /// requirements of <see cref="SuspensionManager.SessionState"/>.
    /// </summary>
    /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
    /// <param name="e">Event data that provides an empty dictionary to be populated with
    /// serializable state.</param>
    private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
    {
      // TODO: Save the unique state of the page here.
    }

    /// <summary>
    /// Adds an item to the list when the app bar button is clicked.
    /// </summary>
    //private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
    //{
    //  string groupName = this.pivot.SelectedIndex == 0 ? FirstGroupName : SecondGroupName;
    //  var group = this.DefaultViewModel[groupName] as SampleDataGroup;
    //  var nextItemId = group.Items.Count + 1;
    //  var newItem = new SampleDataItem(
    //      string.Format(CultureInfo.InvariantCulture, "Group-{0}-Item-{1}", this.pivot.SelectedIndex + 1, nextItemId),
    //      string.Format(CultureInfo.CurrentCulture, this.resourceLoader.GetString("NewItemTitle"), nextItemId),
    //      string.Empty,
    //      string.Empty,
    //      this.resourceLoader.GetString("NewItemDescription"),
    //      string.Empty);

    //  group.Items.Add(newItem);

    //  // Scroll the new item into view.
    //  var container = this.pivot.ContainerFromIndex(this.pivot.SelectedIndex) as ContentControl;
    //  var listView = container.ContentTemplateRoot as ListView;
    //  listView.ScrollIntoView(newItem, ScrollIntoViewAlignment.Leading);
    //}

    /// <summary>
    /// Invoked when an item within a section is clicked.
    /// </summary>
    private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
    {
      // Navigate to the appropriate destination page, configuring the new page
      // by passing required information as a navigation parameter
      //var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
      //if (!Frame.Navigate(typeof(ItemPage), itemId))
      //{
      //    throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
      //}
    }

    /// <summary>
    /// Loads the content for the second pivot item when it is scrolled into view.
    /// </summary>
    //private async void SecondPivot_Loaded(object sender, RoutedEventArgs e)
    //{
    //    //var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-2");
    //    //this.DefaultViewModel[SecondGroupName] = sampleDataGroup;

    //    //string pattern = "opolska";
    //    //string jsonContent = "method=getStopPoints&p0=%7B%22pattern%22%3A%22" + pattern + "%22%7D";
    //    //var stopPoints = await StopPointsSource.GetStopPointsAsync(jsonContent);

    //    //this.DefaultViewModel["OCStopPointsL"] = stopPoints;


    //}

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


    private async void tbStopsSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      labelHistoria.Visibility = Visibility.Collapsed;

      string pattern = tbStopsSearch.Text.ToLower();
      string jsonContent = "";
      if (pattern != "")
      {
        pattern = CommonFunction.RepleaceSpecialCharacter(pattern);
        try
        {
          lvStops.ItemsSource = null;
          jsonContent = "method=getStopPoints&p0=%7B%22pattern%22%3A%22" + pattern + "%22%7D";
          var stopPoints = await ApiPEKA.GetStopPointsAsync(jsonContent);
          lvStops.ItemsSource = stopPoints;
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
    }

    private async void tbLineSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      labelpiLinesHistoria.Visibility = Visibility.Collapsed;
      string pattern = tbLineSearch.Text.ToLower();
      string jsonContent = "";
      if (pattern != "")
      {
        pattern = CommonFunction.RepleaceSpecialCharacter(pattern);
        try
        {
          lvLine.ItemsSource = null;
          //TODO: Przenieść treść method= do ApiPEKA, pozostawiając jako parametr wywołania sam pattern. We wszystkich wystąpieniach
          jsonContent = "method=getLines&p0=%7B%22pattern%22%3A%22" + pattern + "%22%7D";
          var lines = await ApiPEKA.GetLinesAsync(jsonContent);
          lvLine.ItemsSource = lines;
        }
        catch (Exception ex)
        {
          if (ex is System.Net.WebException)
          {
            CommonFunction.MessageBoxShow("Problem z pobraniem danych. Sprawdź połączenie internetowe. \nByć może system PEKA nie działa.");
          }
          else
          {
            CommonFunction.SendException(ex, "jsonContent: " + jsonContent);
          }
        }
      }
    }

    private async void tbStreetsSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      labelpiStreetsHistoria.Visibility = Visibility.Collapsed;
      string pattern = tbStreetsSearch.Text.ToLower();
      string jsonContent = "";
      if (pattern != "")
      {
        pattern = CommonFunction.RepleaceSpecialCharacter(pattern);
        try
        {
          lvStreets.ItemsSource = null;
          jsonContent = "method=getStreets&p0=%7B%22pattern%22%3A%22" + pattern + "%22%7D";
          var streets = await ApiPEKA.GetStreetsAsync(jsonContent);
          lvStreets.ItemsSource = streets;
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
    }

    private void lvStops_ItemClick(object sender, ItemClickEventArgs e)
    {
      var itemId = ((StopPoints)e.ClickedItem).Name;
      var itemSymbol = ((StopPoints)e.ClickedItem).Symbol;

      addHistoryStop(itemSymbol.ToString(), itemId.ToString());

      if (!Frame.Navigate(typeof(StopsPage), itemId))
      {
        throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
      }
    }

    private void lvLines_ItemClick(object sender, ItemClickEventArgs e)
    {
      var itemId = ((Lines)e.ClickedItem).Name;

      addHistoryLines(itemId.ToString());

      if (!Frame.Navigate(typeof(LinesPage), itemId))
      {
        throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
      }
    }

    private void lvStreets_ItemClick(object sender, ItemClickEventArgs e)
    {
      var itemId = ((Streets)e.ClickedItem).Name;
      var itemSymbol = ((Streets)e.ClickedItem).Id;

      addHistoryStreets(itemSymbol, itemId.ToString());
      if (!Frame.Navigate(typeof(StreetsPage), itemId))
      {
        throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
      }
    }


    /// <summary>
    /// Read Favourite Bollard list from textfile
    /// </summary>
    private async Task ReadFavouriteList()
    {
      (App.Current as App).favouriteBollard.Clear();
      try
      {
        StorageFile sampleFile = await localFolder.GetFileAsync("dataFile.txt");
        string d = await FileIO.ReadTextAsync(sampleFile);
        string[] lines = d.Split(new string[] { "\t", "\n" }, StringSplitOptions.None);

        for (int i = 0; i < lines.Count() - 1; i += 2)
        {
          string test1 = lines[i];
          string test2 = lines[i + 1];
          (App.Current as App).favouriteBollard.Add(lines[i], lines[i + 1]);
        }
      }
      catch (Exception ex)
      {
        //CommonFunction.SendException(ex, "");
      }
    }


    private async Task ReadHistoryStopList()
    {
      OCHistoryStopPoints.Clear();
      try
      {
        StorageFile sampleFile = await localFolder.GetFileAsync("historyStop.txt");
        string d = await FileIO.ReadTextAsync(sampleFile);
        string[] lines = d.Split(new string[] { "\t", "\n" }, StringSplitOptions.None);

        for (int i = 0; i < lines.Count() - 1; i += 2)
        {
          OCHistoryStopPoints.Add(new StopPoints(lines[i], lines[i + 1]));
        }
      }
      catch (Exception ex)
      {
        //CommonFunction.SendException(ex, "");
      }

    }

    private async void StoreHistoryStopList()
    {
      string zapis = "";
      try
      {
        StorageFile sampleFile = await localFolder.CreateFileAsync("historyStop.txt", CreationCollisionOption.ReplaceExisting);

        foreach (var pair in OCHistoryStopPoints)
        {
          zapis += pair.Symbol + "\t" + pair.Name + "\n";
        }
        await FileIO.WriteTextAsync(sampleFile, zapis);
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, zapis);
      }
    }

    private void addHistoryStop(string key, string value)
    {
      try
      {
        if (key != "" && value != null)
        {
          if (OCHistoryStopPoints.Contains(new StopPoints(key, value)))
          {
            OCHistoryStopPoints.Remove(new StopPoints(key, value));
          }

          OCHistoryStopPoints.Add(new StopPoints(key, value));
          StoreHistoryStopList();
        }
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, "");
      }
    }

    //****************************************************************************************//

    private async Task ReadHistoryLines()
    {
      OCHistoryLines.Clear();
      try
      {
        StorageFile sampleFile = await localFolder.GetFileAsync("historyLines.txt");
        string d = await FileIO.ReadTextAsync(sampleFile);
        string[] lines = d.Split(new string[] { "\n" }, StringSplitOptions.None);

        for (int i = 0; i < lines.Count() - 1; i++)
        {
          OCHistoryLines.Add(new Lines(lines[i]));
        }
      }
      catch (Exception ex)
      {
        //CommonFunction.SendException(ex, "");
      }

    }

    private async void StoreHistoryLines()
    {
      string zapis = "";
      try
      {
        StorageFile sampleFile = await localFolder.CreateFileAsync("historyLines.txt", CreationCollisionOption.ReplaceExisting);

        foreach (var pair in OCHistoryLines)
        {
          zapis += pair.Name + "\n";
        }
        await FileIO.WriteTextAsync(sampleFile, zapis);
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, zapis);
      }
    }

    private void addHistoryLines(string key)
    {
      try
      {
        if (key != "")
        {
          if (OCHistoryLines.Contains(new Lines(key)))
          {
            OCHistoryLines.Remove(new Lines(key));
          }

          OCHistoryLines.Add(new Lines(key));
          StoreHistoryLines();
        }
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, "");
      }
    }

    //****************************************************************************************//

    private async Task ReadHistoryStreets()
    {
      OCHistoryStreets.Clear();
      try
      {
        StorageFile sampleFile = await localFolder.GetFileAsync("historyStreets.txt");
        string d = await FileIO.ReadTextAsync(sampleFile);
        string[] lines = d.Split(new string[] { "\t", "\n" }, StringSplitOptions.None);

        for (int i = 0; i < lines.Count() - 1; i += 2)
        {
          OCHistoryStreets.Add(new Streets(Convert.ToDouble(lines[i]), lines[i + 1]));
        }
      }
      catch (Exception ex)
      {
        //CommonFunction.SendException(ex, "");
      }

    }

    private async void StoreHistoryStreets()
    {
      string zapis = "";
      try
      {
        StorageFile sampleFile = await localFolder.CreateFileAsync("historyStreets.txt", CreationCollisionOption.ReplaceExisting);

        foreach (var pair in OCHistoryStreets)
        {
          zapis += pair.Id + "\t" + pair.Name + "\n";
        }
        await FileIO.WriteTextAsync(sampleFile, zapis);
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, zapis);
      }
    }

    private void addHistoryStreets(double id, string key)
    {
      try
      {
        if (key != "")
        {
          if (OCHistoryStreets.Contains(new Streets(id, key)))
          {
            OCHistoryStreets.Remove(new Streets(id, key));
          }

          OCHistoryStreets.Add(new Streets(id, key));
          StoreHistoryStreets();
        }
      }
      catch (Exception ex)
      {
        CommonFunction.SendException(ex, "");
      }
    }

    //private async void btAPIPoznanGetStops_Click(object sender, RoutedEventArgs e)
    //{
    //  //var streets = await StopsSource.GetStopsAsync();
    //  //var q = streets.Where(X => X.Id == "PSAM01").FirstOrDefault();

    //  //DataModel.Geometry dupa = q.Geometry;
    //  //Coordinates pipa = dupa.Coordinates;

    //  if (!Frame.Navigate(typeof(Maps)))
    //  {
    //    throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
    //  }

    //}
  }
}
