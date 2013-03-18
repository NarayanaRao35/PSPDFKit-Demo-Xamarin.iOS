using System;
using System.Linq;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using MonoTouch.Foundation;

namespace PSPDFKitDemoXamarin.iOS
{
	public class KSStapleMenu : UIView
	{
		public const float ANIMATION_EXPAND_SECONDS = 0.2f;
		public float ANIMATION_COLLAPSE_SECONDS = 0.2f;
		public float AUTO_CLOSE_TIMEOUT_SECS = 2.5f;

		public enum STAPLEMENU_MODE
		{
			Left,
			Right,
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KSStapleMenu.KSStapleMenu"/> class.
		/// </summary>
		/// <param name="mode">defines how the menu attaches to the parent view</param>
		/// <param name="offset">Offset in units the menu will be indented from top or left border of parent view.</param>
		/// <param name="itemSize">Size of one item.</param>
		public KSStapleMenu (STAPLEMENU_MODE mode, float offset, SizeF itemSize) : base()
		{
			this.ItemSize = itemSize;
			this.Mode = mode;
			this.Offset = offset;
			switch(this.Mode)
			{
			case STAPLEMENU_MODE.Right :
				this.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
				break;
			case STAPLEMENU_MODE.Left :
				this.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
				break;
			}

			//this.BackgroundColor = UIColor.Cyan;
		}

		/// <summary>
		/// Size of the item's elements.
		/// </summary>
		/// <value>The size of the item.</value>
		public SizeF ItemSize
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the mode.
		/// </summary>
		public STAPLEMENU_MODE Mode
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the indentation offset.
		/// </summary>
		public float Offset
		{
			get;
			private set;
		}

		/// <summary>
		/// Expands an item. This automatically collapses a previously opened item.
		/// </summary>
		/// <param name="id">Identifier of the item to expand.</param>
		/// <param name="animated">If set to <c>true</c>, animate.</param>
		public void ExpandItem(string id, bool animated)
		{
			if(string.Compare(id, this.ExpandedItemId, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return;
			}

			if(this.timer != null)
			{
				this.timer.Invalidate();
				this.timer = null;
			}

			// Collapse current open item before expanding another one.
			this.Collapse (animated);

			KSMenuItemHostView hostView = null;
			foreach(KSMenuItemHostView subview in this.Subviews)
			{
				if(string.Compare (id, subview.ItemId, StringComparison.OrdinalIgnoreCase) == 0)
				{
					hostView = subview;
					break;
				}
			}

			if(hostView == null)
			{
				return;
			};

			this.ExpandedItemId = id;

			if(animated)
			{
				UIView.Animate (ANIMATION_EXPAND_SECONDS, 0f, UIViewAnimationOptions.CurveEaseOut, delegate
				{
					hostView.ExpandSubElements ();
				},
				delegate
				{
					float newWidth = hostView.Bounds.Width;
					if(this.Mode == STAPLEMENU_MODE.Right)
					{
						this.Frame = new RectangleF(this.Superview.Bounds.Width - newWidth, this.Frame.Y, newWidth, this.Bounds.Height);
					}
					else
					{
						this.Frame = new RectangleF(0, this.Frame.X, newWidth, this.Bounds.Height);
					}
				});
			}
			else
			{
				hostView.ExpandSubElements();
				float newWidth = hostView.Bounds.Width;
				if(this.Mode == STAPLEMENU_MODE.Right)
				{
					this.Frame = new RectangleF(this.Superview.Bounds.Width - newWidth, this.Frame.Y, newWidth, this.Bounds.Height);
				}
				else
				{
					this.Frame = new RectangleF(0, this.Frame.X, newWidth, this.Bounds.Height);
				}
			}

			// Create a timer that collapses the expanded item after a while.
			this.timer = NSTimer.CreateTimer (AUTO_CLOSE_TIMEOUT_SECS, () => this.Collapse (animated));
			NSRunLoop.Current.AddTimer (this.timer, NSRunLoopMode.Default);
		}

		private NSTimer timer;

		/// <summary>
		/// Gets the current expanded item ID.
		/// </summary>
		public string ExpandedItemId
		{
			get;
			private set;
		}

		/// <summary>
		/// Collapses the current expanded item (if any).
		/// </summary>
		public void Collapse(bool animated)
		{
			if(this.timer != null)
			{
				this.timer.Invalidate();
				this.timer = null;
			}

			if(this.ExpandedItemId == null)
			{
				return;
			}

			KSMenuItemHostView hostView = null;
			foreach(KSMenuItemHostView subview in this.Subviews)
			{
				if(string.Compare (this.ExpandedItemId, subview.ItemId, StringComparison.OrdinalIgnoreCase) == 0)
				{
					hostView = subview;
					break;
				}
			}

			this.ExpandedItemId = null;
			
			if(hostView == null)
			{
				return;
			};

			if(animated)
			{
				UIView.Animate (ANIMATION_COLLAPSE_SECONDS, 0f, UIViewAnimationOptions.CurveEaseIn, delegate
				{
					hostView.CollapseSubElements ();
				},
				delegate
				{
					float newWidth = this.ItemSize.Width;
					if(this.Mode == STAPLEMENU_MODE.Right)
					{
						this.Frame = new RectangleF(this.Superview.Bounds.Width - newWidth, this.Frame.Y, newWidth, this.Bounds.Height);
					}
					else
					{
						this.Frame = new RectangleF(0, this.Frame.X, newWidth, this.Bounds.Height);
					}
				});
			}
			else
			{
				hostView.CollapseSubElements();
				float newWidth = this.ItemSize.Width;
				if(this.Mode == STAPLEMENU_MODE.Right)
				{
					this.Frame = new RectangleF(this.Superview.Bounds.Width - newWidth, this.Frame.Y, newWidth, this.Bounds.Height);
				}
				else
				{
					this.Frame = new RectangleF(0, this.Frame.X, newWidth, this.Bounds.Height);
				}
			}
		}

		/// <summary>
		/// Selects an item's (sub)element.
		/// </summary>
		/// <param name="id">Identifier of the item.</param>
		/// <param name="index">Index of the element of the item to select.</param>
		public void SelectItem(string id, int index)
		{
			Debug.Assert (id != null, "Provide item identifier!");

			id = id.ToLower ();
			if(this.selectedItemIndex.ContainsKey(id))
			{
				this.selectedItemIndex[id] = index;
			}
			else
			{
				this.selectedItemIndex.Add (id, index);
			}

			this.SetNeedsLayout ();
		}

		/// <summary>
		/// Gets the selected index of the item.
		/// </summary>
		/// <returns>The selected item index.</returns>
		/// <param name="id">Identifier.</param>
		public int GetSelectedItemIndex(string id)
		{
			if(id == null)
			{
				return 0;
			}

			id = id.ToLower ();
			if(this.selectedItemIndex.ContainsKey(id))
			{
				return this.selectedItemIndex[id];
			}

			return 0;
		}

		private Dictionary<string, int> selectedItemIndex = new Dictionary<string, int>();

		/// <summary>
		/// Adds items to the menu.
		/// </summary>
		/// <param name="itemsToAdd">Items to add.</param>
		public void AddItems(params KSStapleMenuItem[] itemsToAdd)
		{
			Debug.Assert (itemsToAdd != null, "Provide valid items for initialization!");

			if (this.items == null)
			{
				this.items = new ObservableCollection<KSStapleMenuItem> ();
			}

			this.items.CollectionChanged -= this.HandleItemsCollectionChanged;
			for(int i = 0; i < itemsToAdd.Length; ++i)
			{
				KSStapleMenuItem currentItem = itemsToAdd[i];
				this.items.Add (currentItem);

				currentItem.SizeElements(this.ItemSize);
				currentItem.OnLongPress += this.HandleItemLongPress;
				currentItem.OnSelect += this.HandleItemSelect;

				var containerView = new KSMenuItemHostView(this, currentItem.Id);

				// Make container wide enough to hold all items.
				containerView.Frame = new RectangleF(0, 0, this.ItemSize.Width * currentItem.NumberOfElements, this.ItemSize.Height);

				// Add element views to container view.
				for(int elementIndex = 0; elementIndex < currentItem.NumberOfElements; elementIndex++)
				{
					var itemView = currentItem.GetViewForIndex(elementIndex);
					containerView.AddSubview(itemView);

					if(this.Mode == STAPLEMENU_MODE.Right)
					{
						itemView.Center = new PointF(containerView.Bounds.Width - itemView.Bounds.Width /2f, containerView.Bounds.Height / 2f);
					}
					else
					{
						itemView.Center = new PointF(itemView.Bounds.Width /2f, containerView.Bounds.Height / 2f);
					}
				}

				this.AddSubview(containerView);
			}
			this.items.CollectionChanged += this.HandleItemsCollectionChanged;
		}

		private void HandleItemSelect(KSStapleMenuItem item, int index)
		{
			this.SelectItem (item.Id, index);
			this.Collapse (true);
			if(this.ItemSelected != null)
			{
				this.ItemSelected(item.Id, index);
			}
		}

		private void HandleItemLongPress(object sender, EventArgs args)
		{
			KSStapleMenuItem item = (KSStapleMenuItem)sender;
			this.ExpandItem (item.Id, true);
		}

		public delegate void ItemSelectedDelegate(string id, int index);

		/// <summary>
		/// Triggers if an item was selected.
		/// </summary>
		public event ItemSelectedDelegate ItemSelected;

		private void HandleItemsCollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			this.SetNeedsLayout ();
		}

		public override void WillMoveToSuperview (UIView newsuper)
		{
			base.WillMoveToSuperview (newsuper);
			this.SizeToFit ();

			switch(this.Mode)
			{
			case STAPLEMENU_MODE.Right :
				this.Center = new PointF(newsuper.Bounds.Width - this.Bounds.Width / 2f, this.Offset + this.Bounds.Height / 2f);
				break;

			case STAPLEMENU_MODE.Left :
				this.Center = new PointF(this.Bounds.Width / 2f, this.Offset + this.Bounds.Height / 2f);
				break;
			}
		}

		public override void SizeToFit ()
		{
			if(this.Subviews.Length == 0)
			{
				return;
			}

			this.Frame = new RectangleF (this.Frame.Location, new SizeF (this.ItemSize.Width, this.ItemSize.Height * this.ItemCount));
		}

		/// <summary>
		/// Gets the number of items in the menu.
		/// </summary>
		/// <value>The item count.</value>
		public int ItemCount
		{
			get
			{
				return this.items != null ? this.items.Count : 0;
			}
		}

		private ObservableCollection<KSStapleMenuItem> items;

		public override void LayoutSubviews ()
		{
			float y = 0;
			foreach(var subview in this.Subviews)
			{
				subview.Center = new PointF(this.Bounds.Width - subview.Bounds.Width / 2f, y + subview.Bounds.Height / 2f);
				y += subview.Bounds.Height;
			}
		}
	}
}

