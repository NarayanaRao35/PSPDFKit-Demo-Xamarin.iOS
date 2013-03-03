using System;
using AlexTouch.PSPDFKit;
using System.Drawing;
using MonoTouch.UIKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSExampleAnnotationViewController : PSPDFViewController
	{
		private KSVerticalAnnotationToolbar verticalToolbar;

		public KSExampleAnnotationViewController (PSPDFDocument doc) : base(doc)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// create the custom toolbar and add it on top of the HUDView.
			this.verticalToolbar = new KSVerticalAnnotationToolbar (this);
			this.verticalToolbar.Frame = new RectangleF(this.View.Bounds.Size.Width - 44f, (this.View.Bounds.Size.Height - 44f) / 2f, 1f, 1f);
			this.verticalToolbar.SizeToFit ();
			this.verticalToolbar.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleBottomMargin;
			this.HUDView.AddSubview(this.verticalToolbar);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			if(!this.IsViewLoaded)
			{
				this.verticalToolbar.pdfController = null;
				this.verticalToolbar = null;
			}
		}

		public override void setViewModeAnimated (PSPDFViewMode viewMode, bool animated)
		{
			base.setViewModeAnimated (viewMode, animated);

			// Hide vertical toolbar when switching to thumb view.
			UIView.Animate (0.25f, 0f, UIViewAnimationOptions.AllowUserInteraction, () => { this.verticalToolbar.Alpha = viewMode == PSPDFViewMode.Thumbnails ? 0f : 1f; }, null);
		}
	}
}

