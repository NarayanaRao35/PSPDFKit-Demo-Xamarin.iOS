using System;
using System.Linq;
using AlexTouch.PSPDFKit;
using MonoTouch.UIKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSNoteAnnotationController : PSPDFNoteAnnotationController
	{
		public KSNoteAnnotationController (IntPtr handle) : base(handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			UITextView txtView = this.View.Subviews.FirstOrDefault (oSub => oSub is UITextView) as UITextView;
			// Never allow editing built in annotations.
			txtView.Editable = false;

			if(this.Annotation is IKSAnnotation)
			{
				var customAnnotation = (IKSAnnotation)this.Annotation;
				if(customAnnotation.IsCustomAnnotation)
				{
					txtView.Editable = true;
				}
			}
		}

		public override bool ShowColorAndIconOptions
		{
			get
			{
				return false;
			}
		}

		#region PSPDFStyleable protocol
		public override bool ShouldTintAlertView
		{
			get
			{
				return this._ShouldTintAlertView;
			}
			set
			{
				this._ShouldTintAlertView = value;
			}
		}
		private bool _ShouldTintAlertView;

		private UIColor _TintColor;
		public override UIColor TintColor
		{
			get
			{
				return _TintColor;
			}
			set
			{
				this._TintColor = value;
			}
		}
		
		private UIBarStyle _BarStyle;
		public override UIBarStyle BarStyle 
		{
			get {
				return _BarStyle;
			}
			set {
				_BarStyle = value;
			}
		}
		
		private bool _IsBarTranslucent;
		public override bool IsBarTranslucent {
			get {
				return _IsBarTranslucent;
			}
			set {
				_IsBarTranslucent = value;
			}
		}
		
		private bool _IsInPopover;
		public override bool IsInPopover {
			get {
				return _IsInPopover;
			}
			set {
				_IsInPopover = value;
			}
		}
		#endregion
	}
}

