using System;
using MonoTouch.UIKit;
using AlexTouch.PSPDFKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSVerticalAnnotationToolbar : UIView
	{
		public KSVerticalAnnotationToolbar (PSPDFViewController controller) : base()
		{
			this.pdfController = controller;

			this.toolbar = this.pdfController.AnnotationButtonItem.AnnotationToolbar;
			this.toolbar.Delegate = new KSVerticalAnnotationToolbarDelegate (this);
			this.BackgroundColor = UIColor.FromWhiteAlpha (0.5f, 0.8f);

			// draw button
			// TODO: PSPDFDocument.EditableAnnotationTypes is unbound!
			//if (this.pdfController.Document.EditableAnnotationTypes.ContainsObject(PSPDFAnnotationTypeStringInk)
			{
				this.drawButton = UIButton.FromType(UIButtonType.Custom);
				UIImage sketchImage = UIImage.FromBundle("PSPDFKit.bundle/sketch");
				this.drawButton.SetImage(sketchImage, UIControlState.Normal);
				this.drawButton.Frame = new RectangleF(new PointF(0, 0), sketchImage.Size);
				this.drawButton.TouchUpInside += this.drawButtonPressed;
				this.AddSubview(this.drawButton);
			}

			// TODO: PSPDFDocument.EditableAnnotationTypes is unbound!
			//if (this.pdfController.Document.EditableAnnotationTypes.ContainsObject(PSPDFAnnotationTypeStringFreeText)
			{
				this.freeTextButton = UIButton.FromType(UIButtonType.Custom);
				UIImage freeTextImage = UIImage.FromBundle("PSPDFKit.bundle/freetext");
				this.freeTextButton.SetImage(freeTextImage, UIControlState.Normal);
				this.freeTextButton.Frame = new RectangleF(new PointF(0, 0), freeTextImage.Size);
				this.freeTextButton.TouchUpInside += this.freeTextButtonPressed;
				this.AddSubview(this.freeTextButton);
			}
		}

		public PSPDFViewController pdfController;
		internal UIButton drawButton;
		internal UIButton freeTextButton;
		internal PSPDFAnnotationToolbar toolbar;

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			this.drawButton.Center = new PointF (this.Bounds.Width - this.drawButton.Bounds.Width / 2f, this.Bounds.Height / 2f - this.drawButton.Bounds.Height / 2f);
			this.freeTextButton.Center = new PointF (this.Bounds.Width - this.freeTextButton.Bounds.Width / 2f, this.Bounds.Height / 2f + this.freeTextButton.Bounds.Height / 2f);
		}

		private void freeTextButtonPressed(object sender, EventArgs args)
		{
			this.toolbar.FreeTextButtonPressed ((NSObject)sender);
		}

		private void drawButtonPressed(object sender, EventArgs args)
		{
			if (this.toolbar.ToolbarMode != PSPDFAnnotationToolbarMode.Draw)
			{
				this.pdfController.HUDViewMode = PSPDFHUDViewMode.Always;
				if (this.toolbar.Window == null)
				{
					// match style
					this.toolbar.BarStyle = this.pdfController.NavigationBarStyle;
					this.toolbar.Translucent = this.pdfController.TransparentHUD;
					this.toolbar.TintColor = this.pdfController.TintColor;
					
					// add the toolbar to the view hierarchy for color picking etc
					if (this.pdfController.NavigationController != null)
					{
						RectangleF targetRect = this.pdfController.NavigationController.NavigationBar.Frame;
						this.pdfController.NavigationController.View.InsertSubviewAbove(this.toolbar, this.pdfController.NavigationController.NavigationBar);
						this.toolbar.ShowToolbarInRect(targetRect, true);
					}
					else
					{
						RectangleF contentRect = this.pdfController.ContentRect();
						var toolbarHeight = PSPDFKitGlobal.ToolbarHeightForOrientation(this.pdfController.InterfaceOrientation);
						RectangleF targetRect = new RectangleF(contentRect.X, contentRect.Y, this.pdfController.View.Bounds.Size.Width, toolbarHeight);
						this.pdfController.View.AddSubview(this.toolbar);
						this.toolbar.ShowToolbarInRect(targetRect, true);
					}
				}
				
				// call draw mode of the toolbar
				this.toolbar.DrawButtonPressed((NSObject)sender);
			}
			else
			{
				this.pdfController.HUDViewMode = PSPDFHUDViewMode.Automatic;
				// remove toolbar
				this.toolbar.UnlockPDFController(true, true, true);
				this.toolbar.FinishDrawingAnimated(true, false);
			}
		}

	}

	public class KSVerticalAnnotationToolbarDelegate : PSPDFAnnotationToolbarDelegate
	{
		public KSVerticalAnnotationToolbarDelegate(KSVerticalAnnotationToolbar vertToolbar) : base()
		{
			this.vertToolbar = vertToolbar;
		}

		private KSVerticalAnnotationToolbar vertToolbar;

		public override void DidChangeMode (PSPDFAnnotationToolbar annotationToolbar, PSPDFAnnotationToolbarMode newMode)
		{
			if (newMode == PSPDFAnnotationToolbarMode.None &&  annotationToolbar.Window != null)
			{
				// don't show all toolbar features, hide instead.
				annotationToolbar.HideToolbar(true, () => 
				{
					annotationToolbar.RemoveFromSuperview();
				});
			}
			
			// update button selection status
			this.vertToolbar.drawButton.BackgroundColor = newMode == PSPDFAnnotationToolbarMode.Draw ? UIColor.White : UIColor.Clear;
			this.vertToolbar.freeTextButton.BackgroundColor = newMode == PSPDFAnnotationToolbarMode.FreeText ? UIColor.White : UIColor.Clear;
		}
	}
}