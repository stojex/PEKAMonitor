using MPKMonitor.Common;
using MPKMonitor.Data;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MPKMonitor.API;
using MPKMonitor.DataModel;

namespace MPKMonitor
{
  public sealed partial class LinesPage : Page
  {
    private readonly NavigationHelper navigationHelper;
    private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

    public LinesPage()
    {
      this.InitializeComponent();

      this.navigationHelper = new NavigationHelper(this);
      this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
      this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
    }

    public NavigationHelper NavigationHelper
    {
      get { return this.navigationHelper; }
    }

    public ObservableDictionary DefaultViewModel
    {
      get { return this.defaultViewModel; }
    }

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.
    /// This parameter is typically used to configure the page.</param>

    private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
    {
      //progress1.IsActive = true;
      //progress1.Visibility = Visibility.Visible;

      try
      {


        string pattern = (string)e.NavigationParameter;
        string jsonContent = "method=getBollardsByLine&p0=%7B%22name%22%3A%22" + pattern + "%22%7D";


        //Directions argDirections, ObservableCollection<BollardOrder> argBollard

        if (pattern != "")
        {
          var bollardsList = await ApiPEKA.getBollardsByLineAsync(jsonContent);

          int i = 0;

          foreach (BollardsOrder item in bollardsList)
          {
            ObservableCollection<BollardOrder> OCBollardOrder = item.OCBollardOrder;
            Directions Departures = item.Directions;
            //OCBollardOrder = item.OCBollardOrder;

            //Tutaj dodajemy pivotitem do pivot.

            PivotItem pvitem = new PivotItem();
            pvitem.Header = Departures.Direction;
            pvitem.Background = new SolidColorBrush(Colors.White);
            pvitem.Margin = new Thickness(0, 0, 0, 0);

            ListView listView1 = new ListView();
            string sXAML = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><Grid Margin=""5,5,5,5"">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width=""70""></ColumnDefinition>
                                <ColumnDefinition Width=""*""></ColumnDefinition>
                            </Grid.ColumnDefinitions>
                            <TextBlock Grid.Column=""0"" Margin=""10,0,0,0""
                                    Text=""{Binding OrderNo}"" Foreground=""#FF9ACD32""
                                    Pivot.SlideInAnimationGroup=""1""
                                    CommonNavigationTransitionInfo.IsStaggerElement=""True""
                                    Style=""{StaticResource ListViewItemTextBlockStyle}"" 
                                    FontSize=""21.333"" FontWeight=""Bold"" FontFamily=""Segoe WP Semibold""/>
                            <TextBlock Grid.Column=""1"" Margin=""0,0,0,0""
                                    Text=""{Binding Name}""
                                    Pivot.SlideInAnimationGroup=""1"" Foreground = ""#FF9ACD32""
                                    CommonNavigationTransitionInfo.IsStaggerElement=""True""
                                    Style=""{StaticResource ListViewItemTextBlockStyle}""
                                    FontSize=""21.333"" FontWeight=""Bold"" FontFamily=""Segoe WP Semibold""/>
                        </Grid></DataTemplate>";

            var itemsTemplate = Windows.UI.Xaml.Markup.XamlReader.Load(sXAML) as DataTemplate;
            listView1.ItemTemplate = itemsTemplate;
            listView1.ItemsSource = OCBollardOrder;
            listView1.IsItemClickEnabled = true;
            listView1.ItemClick += listView1_ItemClick;
            listView1.SelectionMode = ListViewSelectionMode.Single;
            listView1.Name = i.ToString();

            pvitem.Content = listView1;

            pivot.Items.Add(pvitem);
            i++;
            pvitem = null;
          }

          //tbItem.Text = _bollard.Name.ToString();
        }
      }
      catch (Exception)
      {
        CommonFunction.MessageBoxShow("Problem z pobraniem danych. Sprawdź połączenie internetowe. \nByć może system PEKA nie działa.");
      }
      //progress1.IsActive = false;
      //progress1.Visibility = Visibility.Collapsed;
    }

    void listView1_ItemClick(object sender, ItemClickEventArgs e)
    {
      BollardOrder item = e.ClickedItem as BollardOrder;
      string cos = item.Tag;

      if (!Frame.Navigate(typeof(ResultPage), cos))
      {
        //throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
      }
    }

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
