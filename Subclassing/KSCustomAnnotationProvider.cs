using System;
using AlexTouch.PSPDFKit;
using System.Drawing;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSCustomAnnotationProvider : PSPDFAnnotationProvider
	{
		public KSCustomAnnotationProvider () : base()
		{
		}

		public KSCustomAnnotationProvider (IntPtr handle) : base(handle)
		{
		}

		public override PSPDFAnnotation[] AnnotationsForPage (uint page)
		{
			lock(this)
			{
				// create new note annotation and add it to the dict.
				PSPDFNoteAnnotation noteAnnotation = new PSPDFNoteAnnotation();
				noteAnnotation.Page = page;
				noteAnnotation.DocumentProvider = this.ProviderDelegate.ParentDocumentProvider();
				noteAnnotation.Contents = string.Format("Annotation from the custom annotationProvider for page {0}.", page + 1);
				
				// place it top left (PDF coordinate space starts from bottom left)
				PSPDFPageInfo pageInfo = noteAnnotation.DocumentProvider.Document.PageInfoForPage(page);
				noteAnnotation.BoundingBox = new RectangleF(100f, pageInfo.RotatedPageRect.Size.Height-100f, 32f, 32f);
				noteAnnotation.Editable = false;

				return new PSPDFAnnotation[] { noteAnnotation };
			}
		}
	}
}

