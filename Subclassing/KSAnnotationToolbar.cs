using System;
using System.Linq;
using AlexTouch.PSPDFKit;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace PSPDFKitDemoXamarin.iOS
{
	/// <summary>
	/// Used to demonstrate subclassing of the annotations toolbar.
	/// </summary>
	public class KSAnnotationToolbar : PSPDFAnnotationToolbar
	{
		public KSAnnotationToolbar (IntPtr handle) : base(handle)
		{
		}

		public KSAnnotationToolbar (PSPDFViewController controller) : base(controller)
		{
		}

		public override void SetItems (UIBarButtonItem[] items, bool animated)
		{
			var customItems = new List<UIBarButtonItem> (items);

			// When setting bar items, inject a custom item.
			string customTitle = "Customized annot toolbar";
			if (customItems [0].Title != customTitle)
			{
				var demoBtn = new UIBarButtonItem (customTitle, UIBarButtonItemStyle.Plain, null);
				customItems.Insert (0, demoBtn);
			}
			base.SetItems (customItems.ToArray(), animated);
		}

		public override void DrawButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			base.DrawButtonPressed (sender);

			// Remove all buttons but the last one, which is the done button.
			var doneBtn = this.Items [this.Items.Length - 1];
			var demoBtn = new UIBarButtonItem ("Drawing toolbar", UIBarButtonItemStyle.Plain, null);

			this.Items = new UIBarButtonItem[] { demoBtn, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), doneBtn };
		}
	}
}

