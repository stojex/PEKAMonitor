using System;
using MPKMonitor.Common;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MPKMonitor.Data;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Text;
using MPKMonitor.API;
using MPKMonitor.DataModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MPKMonitor
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class StreetsPage : Page
  {
    private NavigationHelper navigationHelper;
    Bollard clickedBollard;
    public ObservableCollection<Directions> OCDirections = new ObservableCollection<Directions>();

    public StreetsPage()
    {
      this.InitializeComponent();
      this.navigationHelper = new NavigationHelper(this);
      this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
      this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
    }

    private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
    {
      //throw new NotImplementedException();
    }

    private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
    {
      progress1.IsActive = true;
      progress1.Visibility = Visibility.Visible;

      try
      {
        string pattern = (string)e.NavigationParameter;
        string jsonContent = "method=getBollardsByStreet&p0=%7B%22name%22%3A%22" + pattern + "%22%7D";

        Bollard _bollard = null;
        ObservableCollection<Bollard> OCBollard = new ObservableCollection<Bollard>();

        if (pattern != "")
        {
          tbItem.Text = pattern;
          var bollardsList = await ApiPEKA.getBollardsByStopPointAsync(jsonContent);

          int i = 0;

          foreach (Bollards item in bollardsList)
          {
            ObservableCollection<Directions> OCDepartures = new ObservableCollection<Directions>();
            OCDepartures = item.OCDirections;
            _bollard = item.Bollard;
            OCBollard.Add(_bollard);
            clickedBollard = _bollard;

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
      catch (Exception)
      {
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

      }
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
