using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MSBandAzure.Mvvm;
using MSBandAzure.ViewModels;

namespace BandOnTheRun.iOS
{
	partial class BandsViewController : UIViewController, IUICollectionViewDataSource
	{
		MainPageViewModel _vm;
		public BandsViewController (IntPtr handle) : base (handle)
		{
			_vm = VMLocator.Instance.MainPageViewModel;
		}

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			await _vm.EnumerateBandsCmd.ExecuteAsync(null);

			// we can now update binding to the Bands colection

			BandsCollection.ReloadData ();

		}
		public nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return _vm.Bands.Count;
		}

		public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (UICollectionViewCell)collectionView.DequeueReusableCell ("BandCell", indexPath);
			return cell;
		}
	}
}
