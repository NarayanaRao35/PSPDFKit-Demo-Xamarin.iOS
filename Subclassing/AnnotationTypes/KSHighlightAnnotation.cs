using System;
using AlexTouch.PSPDFKit;
using MonoTouch.UIKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSHighlightAnnotation : PSPDFHighlightAnnotation, IKSAnnotation
	{
		public KSHighlightAnnotation (bool isCustomAnnotation) : base()
		{
			this.IsCustomAnnotation = isCustomAnnotation;
		}

		public KSHighlightAnnotation (IntPtr handle) : base(handle)
		{
		}

		public override bool Editable
		{
			get
			{
				return this.IsCustomAnnotation ? true : false;
			}
		}



		#region IKSAnnotation implementation
		public bool IsCustomAnnotation
		{
			get;
			set;
		}
		private bool isCustomAnnotation;
		#endregion
	}
}

