// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BandOnTheRun.iOS
{
	[Register ("BandsViewController")]
	partial class BandsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UICollectionView BandsCollection { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BandsCollection != null) {
				BandsCollection.Dispose ();
				BandsCollection = null;
			}
		}
	}
}
