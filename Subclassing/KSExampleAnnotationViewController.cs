using System;
using AlexTouch.PSPDFKit;
using System.Drawing;
using MonoTouch.UIKit;
using System.Reflection;
using MonoTouch.Foundation;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSExampleAnnotationViewController : PSPDFViewController
	{
		private KSStapleMenu verticalToolbar;

		public KSExampleAnnotationViewController (PSPDFDocument doc) : base(doc)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// create the custom toolbar and add it on top of the HUDView.
			this.verticalToolbar = new KSStapleMenu(KSStapleMenu.STAPLEMENU_MODE.Right, 80f, new SizeF(80f, 80f));
			this.verticalToolbar.BackgroundColor = UIColor.White;
			
			var inkItem = new KSStapleMenuItem("INK", UIImage.FromBundle("ResourceData/VerticalToolbar/annot-ink-0"), "Red", 12f, UIColor.Black);
			inkItem.AddElement( UIImage.FromBundle("ResourceData/VerticalToolbar/annot-ink-1"), "Green", 12f, UIColor.Black);
			inkItem.AddElement( UIImage.FromBundle("ResourceData/VerticalToolbar/annot-ink-2"), "Blue", 12f, UIColor.Black);

			var noteItem = new KSStapleMenuItem("NOTE", UIImage.FromBundle("ResourceData/VerticalToolbar/annot-note"), "Note", 12f, UIColor.Black);

			var freetextItem = new KSStapleMenuItem("FREETEXT", UIImage.FromBundle("ResourceData/VerticalToolbar/annot-freetext"), "Freetext", 12f, UIColor.Black);
			
			var hilightItem = new KSStapleMenuItem("HIGHLIGHT", UIImage.FromBundle("ResourceData/VerticalToolbar/annot-highlight-0"), "Red", 12f, UIColor.Black);
			hilightItem.AddElement( UIImage.FromBundle("ResourceData/VerticalToolbar/annot-highlight-1"), "Green", 12f, UIColor.Black);
			hilightItem.AddElement( UIImage.FromBundle("ResourceData/VerticalToolbar/annot-highlight-2"), "Blue", 12f, UIColor.Black);

			this.verticalToolbar.AddItems(inkItem, noteItem, freetextItem, hilightItem);
			this.verticalToolbar.ItemSelected += HandleAnnotationToolbarItemSelected;
			this.HUDView.AddSubview(this.verticalToolbar);
			
			// Setup our own annotation toolbar delegate that hides the annotation toolbar.
			this.AnnotationButtonItem.AnnotationToolbar.Delegate = new KSVerticalAnnotationToolbarDelegate();

			this.AnnotationButtonItem.AnnotationToolbar.HideAfterDrawingDidFinish = true;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.annotListeners = new NSObject[2];
			this.annotListeners[0] = NSNotificationCenter.DefaultCenter.AddObserver ("PSPDFAnnotationAddedNotification", this.HandleAnnotationAdded);
			this.annotListeners[1] = NSNotificationCenter.DefaultCenter.AddObserver ("PSPDFAnnotationChangedNotification", this.HandleAnnotationChanged);
		}

		private NSObject[] annotListeners;

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObservers (this.annotListeners);
		}

		private void HandleAnnotationAdded(NSNotification notif)
		{
			Console.WriteLine ("Annotation added");
			if(notif.Object is PSPDFHighlightAnnotation)
			{
				((PSPDFHighlightAnnotation)notif.Object).Color = defaultHighlightColor;
			}

			// Show annotations toolbar.
			UIView.Animate(0.3f, () => { this.verticalToolbar.Alpha = 1f; });

			// Show scrobble bar.
			this.SetScrobbleBarEnabled (true, true);

			var toolbar = this.AnnotationButtonItem.AnnotationToolbar;
			if(toolbar.ToolbarMode == PSPDFAnnotationToolbarMode.Draw)
			{
				toolbar.ToolbarMode = PSPDFAnnotationToolbarMode.None;
				toolbar.FinishDrawingAnimated(false, true);
				return;
			}

			this.HUDViewMode = PSPDFHUDViewMode.Automatic;
			this.HUDVisible = true;
			toolbar.ToolbarMode = PSPDFAnnotationToolbarMode.None;
		}

		private void HandleAnnotationChanged(NSObject notif)
		{
			Console.WriteLine ("Annotation changed");
		}

		private UIColor defaultHighlightColor = UIColor.Clear;

		/// <summary>
		/// Gets called if an element from the annotations toolbar has been selected.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="index">Index.</param>
		private void HandleAnnotationToolbarItemSelected (string id, int index)
		{
			this.SetScrobbleBarEnabled (false, true);
			// Show annotations toolbar.
			UIView.Animate(0.3f, () => { this.verticalToolbar.Alpha = 0f; });

			var toolbar = this.AnnotationButtonItem.AnnotationToolbar;

			switch(id)
			{
			case "NOTE":
			{
				toolbar.NoteButtonPressed(this);
			}
			break;

			case "FREETEXT":
			{
				toolbar.FreeTextButtonPressed(this);
			}
			break;

			case "HIGHLIGHT":
			{
				switch(index)
				{
				case 0:
					defaultHighlightColor = UIColor.Red;
					break;
				case 1:
					defaultHighlightColor = UIColor.Green;
					break;
				case 2:
					defaultHighlightColor = UIColor.Blue;
					break;
				}
				toolbar.HighlightButtonPressed(this);
			}
			break;

			case "INK" :
			{
				switch(index)
				{
				case 0:
					toolbar.DrawColor = UIColor.Red;
					break;
				case 1:
					toolbar.DrawColor = UIColor.Green;
					break;
				case 2:
					toolbar.DrawColor = UIColor.Blue;
					break;
				}
				if (toolbar.ToolbarMode != PSPDFAnnotationToolbarMode.Draw)
				{
					this.HUDViewMode = PSPDFHUDViewMode.Always;
					if (toolbar.Window == null)
					{
						// match style
						toolbar.BarStyle = this.NavigationBarStyle;
						toolbar.Translucent = this.TransparentHUD;
						toolbar.TintColor = this.TintColor;

						
						// add the toolbar to the view hierarchy for color picking etc
						if (this.NavigationController != null)
						{
							RectangleF targetRect = this.NavigationController.NavigationBar.Frame;
							this.NavigationController.View.InsertSubviewAbove(toolbar, this.NavigationController.NavigationBar);
							toolbar.ShowToolbarInRect(targetRect, true);
						}
						else
						{
							RectangleF contentRect = this.ContentRect();
							var toolbarHeight = PSPDFKitGlobal.ToolbarHeightForOrientation(this.InterfaceOrientation);
							RectangleF targetRect = new RectangleF(contentRect.X, contentRect.Y, this.View.Bounds.Size.Width, toolbarHeight);
							this.View.AddSubview(toolbar);
							toolbar.ShowToolbarInRect(targetRect, true);
						}
					}
					
					// call draw mode of the toolbar
					toolbar.DrawButtonPressed(this);
				}
			}
				break;
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

