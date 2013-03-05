using System;
using AlexTouch.PSPDFKit;
using System.Drawing;
using MonoTouch.UIKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSNoDocumentPDFViewController : PSPDFViewController
	{
		// This is the actual binding test. Calling plain base() without parameters used to crash when subclassing PSPDFViewController.
		public KSNoDocumentPDFViewController () : base()
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			var alert = new UIAlertView ("Subclassing", "This is a simple subclass of PSPDFViewController, testing the late setting of PSPDFDocument.", null, "OK");
			alert.Show ();
		}
	}
}

