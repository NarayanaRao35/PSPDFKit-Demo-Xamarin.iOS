
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using AlexTouch.PSPDFKit;
using MonoTouch.ObjCRuntime;

namespace PSPDFKitDemoXamarin.iOS
{
	public partial class KSCatalogViewController : DialogViewController
	{
		const string DevelopersGuideFileName = "DevelopersGuide.pdf";
		const string PaperExampleFileName = "amazon-dynamo-sosp2007.pdf";
		const string PSPDFCatalog = "PSPDFKit.pdf";
		const string HackerMagazineExample = "hackermonthly12.pdf";
		const string AnnotTestExample = "Annotation Test.pdf";

		public bool clearCacheNeeded;

		public KSCatalogViewController () : base (UITableViewStyle.Grouped, null)
		{
			NSUrl samplesURL = NSBundle.MainBundle.ResourceUrl.Append ("Samples", true);
			NSUrl hackerMagURL = samplesURL.Append(HackerMagazineExample, false);
			NSUrl annotTestURL = samplesURL.Append(AnnotTestExample, false);


			this.Root = new RootElement ("KSCatalogViewController")
			{
				new Section ("Full example apps", "Can be used as a template for your own apps.")
				{
					// Demonstrates always visible vertical toolbar.
					new StringElement ("PSPDFViewController playground", () =>
					{
						var doc = new PSPDFDocument(hackerMagURL);
						var controller = new KSKioskViewController(doc);
						controller.StatusBarStyleSetting = PSPDFStatusBarStyleSetting.Default;
						this.NavigationController.PushViewController(controller, true);
					}),
				},

				new Section ("Subclassing")
				{
					// Subclassing PSPDFAnnotationToolbar
					new StringElement ("Subclass annotation toolbar and drawing toolbar", () =>
					{
						var doc = new PSPDFDocument(hackerMagURL);
						var controller = new PSPDFViewController(doc);

						var barButtons = new List<PSPDFBarButtonItem>(controller.RightBarButtonItems);
						barButtons.Add(controller.AnnotationButtonItem);
						controller.RightBarButtonItems = barButtons.ToArray();
						
						var classDic = new NSMutableDictionary();
						classDic.LowlevelSetObject( new Class(typeof(KSAnnotationToolbar)).Handle, new Class(typeof(PSPDFAnnotationToolbar)).Handle);
						controller.OverrideClassNames = classDic;
						
						this.NavigationController.PushViewController(controller, true);
					}),

					// Demonstrates always visible vertical toolbar.
					new StringElement ("Vertical always-visible annotation bar", () =>
					{
						var doc = new PSPDFDocument(hackerMagURL);
						var controller = new KSExampleAnnotationViewController(doc);

						this.NavigationController.PushViewController(controller, true);
					}),

					// Tests potential binding issue when subclassing PSPDFViewController
					new StringElement ("PSPDFViewController with NULL document", () =>
					{
						var doc = new PSPDFDocument(hackerMagURL);
						var controller = new KSNoDocumentPDFViewController();
						controller.Document = doc;
						this.NavigationController.PushViewController(controller, true);
					}),

					// Demonstrates capturing bookmark set/remove.
					new StringElement ("Capture bookmarks", () =>
					{
						var doc = new PSPDFDocument(hackerMagURL);

						// Create an entry for overriding the default bookmark parser.
						var classDic = new NSMutableDictionary();
						classDic.LowlevelSetObject( new Class(typeof(KSBookmarkParser)).Handle, new Class(typeof(PSPDFBookmarkParser)).Handle);
						doc.OverrideClassNames = classDic;

						var controller = new PSPDFViewController(doc);
						controller.RightBarButtonItems = new PSPDFBarButtonItem[]
						{
							controller.BookmarkButtonItem,
							controller.SearchButtonItem,
							controller.OutlineButtonItem,
							controller.ViewModeButtonItem
						};
						this.NavigationController.PushViewController(controller, true);
					}),

					// Demonstrates custom annotation provider.
					new StringElement ("Custom Annotation Provider", () =>
					{
						var doc = new PSPDFDocument(hackerMagURL);
						doc.SetDidCreateDocumentProviderBlock(delegate(PSPDFDocumentProvider documentProvider)
						{
							documentProvider.AnnotationParser.AnnotationProviders = new NSObject[]
							{
								new KSCustomAnnotationProvider(),
								documentProvider.AnnotationParser.FileAnnotationProvider
							};
						});
					
						var controller = new PSPDFViewController(doc);
						this.NavigationController.PushViewController(controller, true);
					}),

					// Subclasses PDPFFileAnnotationProvider and injects additional annotations. 
					// This example demonstrates:
					// * Make all built in annotations (those embedded in the PDF) immutable.
					// * All annotations added by the user can be modified.
					// * Workaround for PSPDFKit bug where the text of a non-editable annotation can still be changed.
					// * Immediate callback if an annotation has been changed.
					new StringElement ("Subclass PSPDFFileAnnotationProvider", () =>
					{
						var controller = new PSPDFViewController();
						var barButtons = new List<PSPDFBarButtonItem>(controller.RightBarButtonItems);
						barButtons.Add(controller.AnnotationButtonItem);
						controller.RightBarButtonItems = barButtons.ToArray();
						controller.SetPageAnimated(2, false);

						controller.PageMode = PSPDFPageMode.Automatic;
						controller.PageTransition = PSPDFPageTransition.ScrollContinuous;
						controller.ScrollDirection = PSPDFScrollDirection.Horizontal;

						var classDic = new NSMutableDictionary();
						classDic.LowlevelSetObject(new Class(typeof(KSNoteAnnotationController)).Handle, new Class(typeof(PSPDFNoteAnnotationController)).Handle);
						controller.OverrideClassNames = classDic;

						this.NavigationController.PushViewController(controller, true);

						var doc = new KSPDFDocument(hackerMagURL);
						//var doc = new PSPDFDocument();
						//var doc = new PSPDFDocument(annotTestURL);
						
						// Create an entry for overriding the file annotation provider.
						classDic = new NSMutableDictionary();
						classDic.LowlevelSetObject( new Class(typeof(KSFileAnnotationProvider)).Handle, new Class(typeof(PSPDFFileAnnotationProvider)).Handle);
						classDic.LowlevelSetObject( new Class(typeof(KSInkAnnotation)).Handle, new Class(typeof(PSPDFInkAnnotation)).Handle);
						classDic.LowlevelSetObject( new Class(typeof(KSNoteAnnotation)).Handle, new Class(typeof(PSPDFNoteAnnotation)).Handle);
						doc.OverrideClassNames = classDic;

						controller.Document = doc;
					}),

					// Remove annotation toolbar item by setting EditableAnnotationTypes
					new StringElement ("Remove Ink from the annotation toolbar", () =>
					{
						var doc = new PSPDFDocument(hackerMagURL);
						var controller = new PSPDFViewController(doc);
						// TODO: We need NSMutableOrderedSet bound and NSOrderedSet!
						/*
						NSMutableOrderedSet annotTypes = UIDocument.EditableAnnotationTypes;
						annotTypes.RemoveObject(PSPDFAnnotationTypeStringInk);
						controller.AnnotationButtonItem.AnnotationToolbar.EditableAnnotationTypes = annotTypes;
						*/
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
