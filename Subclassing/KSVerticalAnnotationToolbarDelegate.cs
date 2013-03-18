using System;
using MonoTouch.UIKit;
using AlexTouch.PSPDFKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSVerticalAnnotationToolbarDelegate : PSPDFAnnotationToolbarDelegate
	{
		public KSVerticalAnnotationToolbarDelegate() : base()
		{
		}

		public override void DidChangeMode (PSPDFAnnotationToolbar annotationToolbar, PSPDFAnnotationToolbarMode newMode)
		{
			if (newMode == PSPDFAnnotationToolbarMode.None && annotationToolbar.Window != null)
			{
				// don't show all toolbar features, hide instead.
				annotationToolbar.HideToolbar(false, () => 
				{
					annotationToolbar.RemoveFromSuperview();
				});
			}
		}
	}
}