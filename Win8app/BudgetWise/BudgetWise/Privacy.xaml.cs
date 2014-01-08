﻿using System;
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
using Windows.System;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace BudgetWise
{
    public sealed partial class Privacy : SettingsFlyout
    {
        public Privacy()
        {
            this.InitializeComponent();
        }

        private async void LinkButton_Clicked(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://budgetwise.azurewebsites.net/"));
        }
    }
}
