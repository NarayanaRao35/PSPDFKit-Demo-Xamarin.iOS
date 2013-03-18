using System;
using AlexTouch.PSPDFKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSOutlineViewControllerDelegate : PSPDFOutlineViewControllerDelegate
	{
		public KSOutlineViewControllerDelegate () : base()
		{
		}

		public KSOutlineViewControllerDelegate (IntPtr handle) : base(handle)
		{
		}

		public override bool DidTapAtElement (PSPDFOutlineViewController outlineController, PSPDFOutlineElement outlineElement)
		{
			Console.WriteLine ("DidTapAtTocElement: " + outlineElement);
			return false;
		}
	}
}

