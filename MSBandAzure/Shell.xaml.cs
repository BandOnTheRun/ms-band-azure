using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Storage;
using MSBandAzure.Views;
using MSBandAzure.Presentation;

namespace MSBandAzure
{
    public sealed partial class Shell : Page
    {
        private ShellViewModel vm;

        public Shell()
        {
            this.InitializeComponent();
        }

        private ShellViewModel _vm = null;

        public ShellViewModel ViewModel
        {
            get
            {
                if (_vm == null)
                {
                    _vm = App.Locator.ShellViewModel;
                    _vm.MenuItems.Add(new MenuItem { Icon = "", Title = "Bands", PageType = typeof(MainPage) });
                    _vm.MenuItems.Add(new MenuItem { Icon = "", Title = "Settings", PageType = typeof(SettingsPage) });
                    _vm.SelectedMenuItem = _vm.MenuItems.First();
                }
                return _vm;
            }
        }

		public Frame RootFrame
		{
			get
			{
				return this.Frame;
			}
		}
    }
}
