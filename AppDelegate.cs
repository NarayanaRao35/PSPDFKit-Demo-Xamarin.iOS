using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AlexTouch.PSPDFKit;

namespace PSPDFKitDemoXamarin.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public UIWindow window;
		public KSCatalogViewController catalogController;
		public PSPDFNavigationController navController;

		public static bool PSIsIpad = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			this.window = new UIWindow (UIScreen.MainScreen.Bounds);
			
//			string appVersion = NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleShortVersionString").ToString ();
//			Console.WriteLine("Starting Catalog Example {0} with {1}", appVersion, PSPDFKitGlobal.VersionString);
			
			this.window = new UIWindow(UIScreen.MainScreen.Bounds);
			
			this.navController = new PSPDFNavigationController();
			this.catalogController = new KSCatalogViewController ();
			this.navController.PushViewController (this.catalogController, false);
			this.window.RootViewController = this.navController;
			this.window.MakeKeyAndVisible ();

			return true;
		}
	}
}

