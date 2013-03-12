using System;
using AlexTouch.PSPDFKit;
using MonoTouch.Foundation;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSPDFDocument : PSPDFDocument
	{
		public KSPDFDocument (NSUrl url) : base(url)
		{
		}

		public override PSPDFDocumentProvider DidCreateDocumentProvider (PSPDFDocumentProvider documentProvider)
		{
			var provider = base.DidCreateDocumentProvider (documentProvider);
			Console.WriteLine (provider.AnnotationParser.FileAnnotationProvider);
			return provider;
		}
	}
}

