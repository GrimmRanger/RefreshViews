using System;
using MonoTouch.UIKit;

namespace FGUtil
{
	public abstract class RefreshTableSource : UITableViewSource
	{
		private IScrollDelegate _scrollDelegate;

		public RefreshTableSource (IScrollDelegate scrollDelegate)
		{
			_scrollDelegate = scrollDelegate;
		}

		public override void Scrolled (UIScrollView scrollView)
		{
			_scrollDelegate.DidScroll(scrollView);
		}

		public override void DecelerationStarted (UIScrollView scrollView)
		{
			_scrollDelegate.BeganDeceleration(scrollView);
		}

		public abstract RefreshView RefreshView(UITableView tableView);
	}

	public interface IScrollDelegate
	{
		void DidScroll(UIScrollView scrollView);
		void BeganDeceleration(UIScrollView scrollView);
	}
}

