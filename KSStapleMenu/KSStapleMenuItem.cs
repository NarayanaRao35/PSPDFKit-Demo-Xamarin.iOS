using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace PSPDFKitDemoXamarin.iOS
{
	/// <summary>
	/// Represents one item in the menu. Each item can consist of several sub elements.
	/// </summary>
	public class KSStapleMenuItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KSStapleMenu.KSStapleMenuItem"/> class.
		/// </summary>
		/// <param name="id">Identifier of the item's first element</param>
		/// <param name="image">Image of the first sub element</param>
		public KSStapleMenuItem (string id, UIImage image)
			: this(id, image, null, 0, UIColor.Clear)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KSStapleMenu.KSStapleMenuItem"/> class.
		/// </summary>
		/// <param name="id">Identifier of the item's first element</param>
		/// <param name="image">Image of the first sub element</param>
		/// <param name="title">Optional title of the first sub element</param>
		/// <param name="titleFontSize">Optional title font size</param>
		/// <param name="titleColor">Optional color of the title</param>
		public KSStapleMenuItem (string id, UIImage image, string title, float titleFontSize, UIColor titleColor)
		{
			this.Id = id;
			this.elements = new List<UIButton> ();
			this.AddElement (image, title, titleFontSize, titleColor);
		}

		/// <summary>
		/// Gets the identifier of the item.
		/// </summary>
		/// <value>The identifier.</value>
		public string Id
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the view for a specific sub element.
		/// </summary>
		/// <returns>The view for index.</returns>
		/// <param name="index">Index.</param>
		public UIView GetViewForIndex(int index)
		{
			if(this.elements.Count <= 0 || this.elements.Count < index)
			{
				return null;
			}
			return this.elements[index];
		}

		/// <summary>
		/// Gets the toal number of elements in this item.
		/// </summary>
		/// <value>The number of elements.</value>
		public int NumberOfElements
		{
			get
			{
				return this.elements == null ? 0 : this.elements.Count;
			}
		}


		private List<UIButton> elements;

		/// <summary>
		/// Resizes the item's elements.
		/// </summary>
		/// <param name="size">Size.</param>
		public void SizeElements(SizeF size)
		{
			foreach(var button in this.elements)
			{
				button.Frame = new RectangleF (new PointF (0, 0), size);
//				button.Layer.BorderColor = UIColor.Black.CGColor;
//				button.Layer.BorderWidth = 2f;

				UILabel label = null;
				if(button.Subviews.Length > 1)
				{
					label = (UILabel)button.Subviews[1];
					label.Center = new PointF(button.Bounds.Width / 2f, button.Bounds.Bottom - label.Bounds.Height / 2f);
				}
			}
		}

		/// <summary>
		/// Adds a sub element to this item. Each item can handle a staple of of sub elements.
		/// </summary>
		/// <returns>The element to add</returns>
		/// <param name="image">Image of the sub element</param>
		public int AddElement(UIImage image)
		{
			return this.AddElement (image, null, 0, UIColor.Clear);
		}

		/// <summary>
		/// Adds a sub element to this item. Each item can handle a staple of of sub elements.
		/// </summary>
		/// <returns>The element to add</returns>
		/// <param name="image">Image of the sub element</param>
		/// <param name="title">Optional title</param>
		/// <param name="titleFontSize">Optional title font size</param>
		/// <param name="titleColor">Optional color of the title</param>
		public int AddElement(UIImage image, string title, float titleFontSize, UIColor titleColor)
		{
			var button = UIButton.FromType (UIButtonType.Custom);
			button.SetImage (image, UIControlState.Normal);
			button.ContentMode = UIViewContentMode.Center;

			this.elements.Add (button);

			var longPressGest = new UILongPressGestureRecognizer ( () => 
			{
				if (this.OnLongPress != null)
				{
					this.OnLongPress (this, null);
				}
			});
			longPressGest.MinimumPressDuration = 0.4f;
			button.AddGestureRecognizer (longPressGest);

			if(title != null)
			{
				var label = new UILabel()
				{
					Text = title,
					Font = UIFont.SystemFontOfSize(titleFontSize),
					BackgroundColor = UIColor.Clear,
					TextColor = titleColor,
					AdjustsFontSizeToFitWidth = false,
					AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin
				};
				label.SizeToFit();
				button.AddSubview(label);
			}

			//button.BackgroundColor = UIColor.Green;

			// Return index of the element.
			var index = this.elements.Count - 1;

			button.TouchUpInside += (object sender, EventArgs e) => this.OnSelect(this, index);

			return index;
		}

		public delegate void SelectHandler(KSStapleMenuItem item, int index);

		public delegate void LongPressHandler(object sender, EventArgs args);
	
		/// <summary>
		/// Attach to get informed about long presses on the item.
		/// </summary>
		public event LongPressHandler OnLongPress;

		/// <summary>
		/// Attach to get informed about selection of an element of the item.
		/// </summary>
		public event SelectHandler OnSelect;
	}
}

