
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using AlexTouch.PSPDFKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public partial class KSCatalogViewController : DialogViewController
	{
		const string DevelopersGuideFileName = "DevelopersGuide.pdf";
		const string PaperExampleFileName = "amazon-dynamo-sosp2007.pdf";
		const string PSPDFCatalog = "PSPDFKit.pdf";
		const string HackerMagazineExample = "hackermonthly12.pdf";

		public bool clearCacheNeeded;

		public KSCatalogViewController () : base (UITableViewStyle.Grouped, null)
		{
			NSUrl samplesURL = NSBundle.MainBundle.ResourceUrl.Append ("Samples", true);
			NSUrl hackerMagURL = samplesURL.Append(HackerMagazineExample, false);


			this.Root = new RootElement ("KSCatalogViewController")
			{
				new Section ("Subclassing")
				{
					new StringElement ("Vertical always-visible annotation bar", () =>
					{
						var doc = new PSPDFDocument(hackerMagURL);
						var controller = new KSExampleAnnotationViewController(doc);
						this.NavigationController.PushViewController(controller, true);
					})
				}
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// Restore state as it was before.
			this.NavigationController.SetNavigationBarHidden(false, animated);
			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, animated);
			UIApplication.SharedApplication.SetStatusBarHidden(false, animated ? UIStatusBarAnimation.Fade : UIStatusBarAnimation.None);
			PSPDFKitGlobal.PSPDFFixNavigationBarForNavigationControllerAnimated(this.NavigationController, false);
			this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			
			// clear cache (for night mode)
			if (this.clearCacheNeeded)
			{
				this.clearCacheNeeded = false;
				PSPDFCache.SharedCache.ClearCache();
			}
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			PSPDFKitGlobal.PSPDFFixNavigationBarForNavigationControllerAnimated(this.NavigationController, animated);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return AppDelegate.PSIsIpad ? true : toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown;
		}
	}
}
