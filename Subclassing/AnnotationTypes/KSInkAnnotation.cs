using System;
using AlexTouch.PSPDFKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSInkAnnotation : PSPDFInkAnnotation, IKSAnnotation
	{
		public KSInkAnnotation (bool isCustomAnnotation) : base()
		{
			this.IsCustomAnnotation = isCustomAnnotation;
		}

		public KSInkAnnotation (IntPtr handle) : base(handle)
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

