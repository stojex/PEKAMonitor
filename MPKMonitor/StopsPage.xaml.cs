using MPKMonitor.Common;
using MPKMonitor.Data;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MPKMonitor.API;
using MPKMonitor.DataModel;

namespace MPKMonitor
{
  public sealed partial class StopsPage : Page
  {
    private readonly NavigationHelper navigationHelper;
    private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
    Bollard clickedBollard;

    public StopsPage()
    {
      this.InitializeComponent();

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

    private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
    {
      progress1.IsActive = true;
      progress1.Visibility = Visibility.Visible;
      try
      {
        string pattern = (string)e.NavigationParameter;
        string jsonContent = "method=getBollardsByStopPoint&p0=%7B%22name%22%3A%22" + pattern + "%22%7D";

        Bollard _bollard = null;
        ObservableCollection<Bollard> OCBollard = new ObservableCollection<Bollard>();
        if (pattern != "")
        {
          var bollardsList = await ApiPEKA.getBollardsByStopPointAsync(jsonContent);

          //int ilosc = OCDepartures.Count;

          

          int i = 0;

          foreach (Bollards item in bollardsList)
          {
            ObservableCollection<Directions> OCDepartures = new ObservableCollection<Directions>();
            OCDepartures = item.OCDirections;
            _bollard = item.Bollard;
            OCBollard.Add(_bollard);
            clickedBollard = _bollard;

            tbItem.Text = _bollard.Name;

            //Button btBollard = new Button();

            string btContent = _bollard.Name;
            string btTag = _bollard.Tag;
            string btDirection = "";

            foreach (Directions Directionitem in OCDepartures)
            {
              btDirection += Directionitem.LineName + " -> " + Directionitem.Direction + ", ";
            }

            string sXAML = @"<Button Margin=""0,0,0,0"" VerticalAlignment=""Stretch"" HorizontalAlignment=""Stretch"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" Name=""btBollard";
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
                <TextBlock Grid.Column=""0"" Grid.Row=""0"" Text=""";
            sXAML += btContent;
            sXAML += @""" Style =""{ThemeResource ListViewItemContentTextBlockStyle}"" HorizontalAlignment=""Stretch"" FontSize=""20"" FontWeight=""Bold"" />
                  <TextBlock Grid.Column=""1"" Grid.Row =""0"" Text=""";
            sXAML += btTag;
            sXAML += @""" HorizontalAlignment=""Right"" FontSize=""20"" FontWeight =""Bold"" Style=""{ThemeResource ListViewItemContentTextBlockStyle}"" />
                <TextBlock Grid.Column=""0"" Grid.ColumnSpan=""2"" Grid.Row= ""1"" TextWrapping=""WrapWholeWords"" Style=""{ThemeResource ListViewItemContentTextBlockStyle}"" Foreground=""White"" FontSize=""18"" FontWeight=""Light"" Text=""";
            sXAML += btDirection;
            sXAML += @""" HorizontalAlignment=""Left"" /></Grid></Button>";

            object itemsTemplate = Windows.UI.Xaml.Markup.XamlReader.Load(sXAML) as Button;
            var tmp = itemsTemplate as Button;
            tmp.Click += btBollard_Click;

            spItemResult.Children.Add(tmp as Button);
            i++;
          }
        }
      }
      catch (Exception ex)
      {
        string cos = ex.InnerException.ToString();
        CommonFunction.MessageBoxShow("Problem z pobraniem danych. Sprawdź połączenie internetowe. \nByć może system PEKA nie działa.");
      }
      progress1.IsActive = false;
      progress1.Visibility = Visibility.Collapsed;
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
    /// page is discarded from the navigation cache.  Values must conform to the serialization
    /// requirements of <see cref="SuspensionManager.SessionState"/>.
    /// </summary>
    /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
    /// <param name="e">Event data that provides an empty dictionary to be populated with
    /// serializable state.</param>
    private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
    {
      // TODO: Save the unique state of the page here.
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
  }
}