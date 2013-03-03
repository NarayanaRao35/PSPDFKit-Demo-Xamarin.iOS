using System;
using MonoTouch.UIKit;
using AlexTouch.PSPDFKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSVerticalAnnotationToolbar : UIView
	{
		public const float BUTTON_WIDTH = 44f;
		public const float BUTTON_HEIGHT = 44f;

		private int numButtons;

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
				this.drawButton.Frame = new RectangleF(new PointF(0, 0), new SizeF(BUTTON_WIDTH, BUTTON_HEIGHT));
				this.drawButton.TouchUpInside += this.drawButtonPressed;
				this.AddSubview(this.drawButton);
				this.numButtons++;
			}

			// TODO: PSPDFDocument.EditableAnnotationTypes is unbound!
			//if (this.pdfController.Document.EditableAnnotationTypes.ContainsObject(PSPDFAnnotationTypeStringFreeText)
			{
				this.freeTextButton = UIButton.FromType(UIButtonType.Custom);
				UIImage freeTextImage = UIImage.FromBundle("PSPDFKit.bundle/freetext");
				this.freeTextButton.SetImage(freeTextImage, UIControlState.Normal);
				this.freeTextButton.Frame = new RectangleF(new PointF(0, 0), new SizeF(BUTTON_WIDTH, BUTTON_HEIGHT));
				this.freeTextButton.TouchUpInside += this.freeTextButtonPressed;
				this.AddSubview(this.freeTextButton);
				this.numButtons++;
			}

			// TODO: PSPDFDocument.EditableAnnotationTypes is unbound!
			//if (this.pdfController.Document.EditableAnnotationTypes.ContainsObject(PSPDFAnnotationTypeStringNote)
			{
				this.noteButton = UIButton.FromType(UIButtonType.Custom);
				UIImage noteImage = UIImage.FromBundle("PSPDFKit.bundle/note");
				this.noteButton.SetImage(noteImage, UIControlState.Normal);
				this.noteButton.Frame = new RectangleF(new PointF(0, 0), new SizeF(BUTTON_WIDTH, BUTTON_HEIGHT));
				this.noteButton.TouchUpInside += this.noteButtonPressed;
				this.AddSubview(this.noteButton);
				this.numButtons++;
			}

			// TODO: PSPDFDocument.EditableAnnotationTypes is unbound!
			//if (this.pdfController.Document.EditableAnnotationTypes.ContainsObject(PSPDFAnnotationTypeStringHighlight)
			{
				// Highlight is a bit problematic: once highlight mode has been activated...how to leave it again?
				this.highlightButton = UIButton.FromType(UIButtonType.Custom);
				UIImage highlightImage = UIImage.FromBundle("PSPDFKit.bundle/highlight");
				this.highlightButton.SetImage(highlightImage, UIControlState.Normal);
				this.highlightButton.Frame = new RectangleF(new PointF(0, 0), new SizeF(BUTTON_WIDTH, BUTTON_HEIGHT));
				this.highlightButton.TouchUpInside += this.highlightButtonPressed;
				this.AddSubview(this.highlightButton);
				this.numButtons++;
			}
		}

		public override SizeF SizeThatFits (SizeF size)
		{
			return base.SizeThatFits (size);
		}

		public override void SizeToFit ()
		{
			this.Frame = new RectangleF (this.Frame.Location, new SizeF (BUTTON_WIDTH, BUTTON_HEIGHT * this.numButtons));
		}

		public PSPDFViewController pdfController;
		internal UIButton drawButton;
		internal UIButton freeTextButton;
		internal UIButton highlightButton;
		internal UIButton noteButton;
		internal PSPDFAnnotationToolbar toolbar;

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			float centerX = this.Bounds.Width - BUTTON_WIDTH / 2f;
			float centerY = BUTTON_HEIGHT / 2f;

			if(this.drawButton != null)
			{
				this.drawButton.Center = new PointF (centerX, centerY);
				centerY += BUTTON_HEIGHT;
			}

			if(this.freeTextButton != null)
			{
				this.freeTextButton.Center = new PointF (centerX, centerY);
				centerY += BUTTON_HEIGHT;
			}

			if(this.noteButton != null)
			{
				this.noteButton.Center = new PointF (centerX, centerY);
				centerY += BUTTON_HEIGHT;
			}

			if(this.highlightButton != null)
			{
				this.highlightButton.Center = new PointF (centerX, centerY);
				centerY += BUTTON_HEIGHT;
			}
		}

		private void noteButtonPressed(object sender, EventArgs args)
		{
			this.toolbar.NoteButtonPressed((NSObject)sender);
		}

		private void highlightButtonPressed(object sender, EventArgs args)
		{
			this.toolbar.HighlightButtonPressed((NSObject)sender);
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