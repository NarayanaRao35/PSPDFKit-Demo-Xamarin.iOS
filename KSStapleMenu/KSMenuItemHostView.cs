using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace PSPDFKitDemoXamarin.iOS
{
	internal class KSMenuItemHostView : UIView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KSStapleMenu.KSMenuItemHostView"/> class.
		/// </summary>
		/// <param name="menu">the menu that is using this view</param>
		/// <param name="itemId">the item encapsulated by the host view</param>
		internal KSMenuItemHostView (KSStapleMenu menu, string itemId) : base()
		{
			this.menu = menu;
			this.ItemId = itemId;

//			this.BackgroundColor = UIColor.Blue;
//			this.Layer.BorderColor = UIColor.Yellow.CGColor;
//			this.Layer.BorderWidth = 2f;
		}

		private KSStapleMenu menu;
		public string ItemId;

		/// <summary>
		/// Expands the sub elements.
		/// </summary>
		internal void ExpandSubElements ()
		{
			float x = 0;

			if(this.menu.Mode == KSStapleMenu.STAPLEMENU_MODE.Right)
			{
				x = this.Bounds.Width - 2 * this.menu.ItemSize.Width;
			}
			else
			{
				x = this.menu.ItemSize.Width;
			}

			for(int i = 0; i < this.Subviews.Length; i++)
			{
				var subview = this.Subviews[i];
				if(i != this.menu.GetSelectedItemIndex(this.ItemId))
				{
					subview.Alpha = 1f;
					subview.Center = new PointF(x + subview.Bounds.Width / 2f, subview.Center.Y);
					if(this.menu.Mode == KSStapleMenu.STAPLEMENU_MODE.Right)
					{
						x -= this.menu.ItemSize.Width;
					}
					else
					{
						x += this.menu.ItemSize.Width;
					}
				}
			}
		}

		/// <summary>
		/// Collapses the sub elements.
		/// </summary>
		internal void CollapseSubElements()
		{
			float x = 0;
			
			if(this.menu.Mode == KSStapleMenu.STAPLEMENU_MODE.Right)
			{
				x = this.Bounds.Width - this.menu.ItemSize.Width;
			}
			else
			{
				x = 0;
			}

			for(int i = 0; i < this.Subviews.Length; i++)
			{
				var subview = this.Subviews[i];
				if(i == this.menu.GetSelectedItemIndex(this.ItemId))
				{
					subview.Alpha = 1f;
				}
				else
				{
					subview.Alpha = 0f;
				}

				if(this.menu.Mode == KSStapleMenu.STAPLEMENU_MODE.Right)
				{
					subview.Center = new PointF(x + subview.Bounds.Width / 2f, this.Bounds.Height / 2f);
				}
				else
				{
					subview.Center = new PointF(subview.Bounds.Width / 2f, this.Bounds.Height / 2f);
				}


			}
		}

		public override void LayoutSubviews ()
		{
			int selectedElementIndex = this.menu.GetSelectedItemIndex (this.ItemId);

			for(int i = 0; i < this.Subviews.Length; i++)
			{
				var subview = this.Subviews[i];
				subview.Alpha = i == selectedElementIndex ? 1f : 0f;
				//subview.Center = new PointF( this.Bounds.Width / 2f, this.Bounds.Height / 2f);
			}
		}
	}
}

