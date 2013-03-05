using System;
using AlexTouch.PSPDFKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSBookmarkParser : PSPDFBookmarkParser
	{
		public KSBookmarkParser (IntPtr handle) : base(handle)
		{
		}

		public override bool RemoveBookmarkForPage (uint page)
		{
			Console.WriteLine("Remove Bookmark: {0}", page);
			return base.RemoveBookmarkForPage (page);
		}

		public override bool AddBookmarkForPage (uint page)
		{
			Console.WriteLine("Add Bookmark: {0}", page);
			return base.AddBookmarkForPage (page);
		}
	}
}

