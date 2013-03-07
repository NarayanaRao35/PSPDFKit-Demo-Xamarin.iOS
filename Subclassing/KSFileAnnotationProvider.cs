using System;
using AlexTouch.PSPDFKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSFileAnnotationProvider : PSPDFFileAnnotationProvider
	{
		public KSFileAnnotationProvider (IntPtr handle) : base(handle)
		{
			Console.WriteLine ("KSFileAnnotationProvider (IntPtr handle)");
			this.pageAnnots = new Dictionary<uint, List<PSPDFAnnotation>> ();
		}

		public KSFileAnnotationProvider (PSPDFDocumentProvider docProv) : base(docProv)
		{
			Console.WriteLine ("KSFileAnnotationProvider (PSPDFDocumentProvider docProv)");
		}

		public override bool AddAnnotations (PSPDFAnnotation[] annotations, uint page)
		{
			Console.WriteLine ("Overridden AddAnnotations()");
			foreach (var annot in annotations)
			{
				Console.WriteLine( " {0}, Page: {1}, Absolute Page: {2}, Rect: {3}", annot.AnnotationType, annot.Page, annot.AbsolutePage, annot.Rects);
				// All added annotations are ours.
				((IKSAnnotation)annot).IsCustomAnnotation = true;
			}

			this.AddAnnotationsToCache (page, annotations);

			return true;

			// TODO: Calling base here crashes with unrecognized selector. But for our case we don't want to call base anyway.
			//return base.AddAnnotations (annotations, page);
		}

		private void AddAnnotationsToCache (uint page, params PSPDFAnnotation[] annotations )
		{
			if(annotations == null || annotations.Length <= 0)
			{
				return;
			}

			if (!this.pageAnnots.ContainsKey (page))
			{
				this.pageAnnots [page] = new List<PSPDFAnnotation> ();
			}
			this.pageAnnots [page].AddRange (annotations);
		}

		public override PSPDFAnnotation[] AnnotationsForPage (uint page)
		{
			lock (this.locker)
			{
				// Add one additional note annotation on each page.
				if (!this.pageAnnots.ContainsKey (page))
				{
					var addAnnot = new KSNoteAnnotation (false)
					{
						Page = page,
						Contents = "Custom Annot on Page " + page,
						BoundingBox = new RectangleF(278f, 406f, 32f, 32f),
						Color = UIColor.Red,
					};

					this.AddAnnotationsToCache(page, addAnnot);

					// Dim all build int annotations.
					this.pageAnnots[page].AddRange(base.AnnotationsForPage (page).Select(embeddedAnnot => { embeddedAnnot.Editable = false; embeddedAnnot.Alpha = 0.2f; return embeddedAnnot; }));
				}

				return this.pageAnnots[page].ToArray();
			}
		}

		public override void DidChangeAnnotation (PSPDFAnnotation annotation, PSPDFAnnotation originalAnnotation, NSArray keyPaths, NSDictionary options)
		{
			if(annotation is IKSAnnotation)
			{
				Console.WriteLine("Custom annotation changed: " + annotation);
			}
		}

		private object locker = new object();
		private Dictionary<uint, List<PSPDFAnnotation>> pageAnnots;

		public override bool SaveAnnotationsWithError (out NSError error)
		{
			Console.WriteLine ("Overridden SaveAnnotationsWithError()");
			//return base.SaveAnnotationsWithError (out error);
			error = null;
			return true;
		}
	}
}

