using System;
using AlexTouch.PSPDFKit;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSNoteAnnotation : PSPDFNoteAnnotation, IKSAnnotation
	{
		public KSNoteAnnotation (bool isCustomAnnotation) : base()
		{
			this.IsCustomAnnotation = isCustomAnnotation;
		}
		
		public KSNoteAnnotation (IntPtr handle) : base(handle)
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
		#endregion
	}
}

