using BudgetWise.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BudgetWise
{
    public class WishListItem
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
 
        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }
    }
    
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class WishList : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private MobileServiceCollection<WishListItem, WishListItem> items;
        private IMobileServiceTable<WishListItem> wishListTable = App.MobileService.GetTable<WishListItem>();

        public WishList()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        private async void InsertWishListItem(WishListItem wishListItem)
        {
            try
            {
                await wishListTable.InsertAsync(wishListItem);
                items.Add(wishListItem);
            }
            catch (Exception)
            {

            }
        }

        private async void UpdateCheckedWishListItem(WishListItem item)
        {
            try
            {
                await wishListTable.UpdateAsync(item);
            }
            catch (InvalidOperationException)
            {

            }
        }

        private async void RefreshWishListItems()
        {
            try
            {
              //  await Authenticate();

                items = await wishListTable.ToCollectionAsync();
                ListItems.ItemsSource = items;
            }
            catch (InvalidOperationException)
            {

            }
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await Authenticate();
            RefreshWishListItems();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            await Authenticate();
            if (item.Text != "")
            {
                loginText.Text = "";
                var wishListItem = new WishListItem { Text = item.Text };
                InsertWishListItem(wishListItem);
                RefreshWishListItems();
            }
            else
            {
                loginText.Text = "Please enter an item";
            }
        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            WishListItem item = cb.DataContext as WishListItem;
            UpdateCheckedWishListItem(item);

            await wishListTable.DeleteAsync(item);
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        /*    if (user == null)
            {
                loginText.Text = "You are not logged in!";
                signIn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            { */
                RefreshWishListItems();
         //   }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private MobileServiceUser user;
        public async System.Threading.Tasks.Task Authenticate()
        {
            while (user == null)
            {
                string message;
                try
                {
                    user = await App.MobileService
                        .LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount, true);
                    message =
                        //string.Format("You are now logged in - {0}", user.UserId);
                        string.Format("You are now logged in");
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }


                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }

        }

        private async void SignInButton_Clicked(object sender, RoutedEventArgs e)
        {
            await Authenticate();
            loginText.Text = "";
            signIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            RefreshWishListItems();
        }
    }
}
