using System;
using MonoTouch.UIKit;
using AlexTouch.PSPDFKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSCombinedTabBarController : UITabBarController
	{
		public KSCombinedTabBarController (PSPDFViewController controller, PSPDFDocument document) : base ()
		{
			var tocController = new PSPDFOutlineViewController (document, controller.Handle);
			var searchController = new PSPDFSearchViewController (document, controller);
			this.SetViewControllers (new UIViewController[] { tocController, searchController }, false);
		}
	}
}

