using System;
using MonoTouch.UIKit;

namespace FGUtil
{
	public abstract class FGRefreshTableSource : UITableViewSource
	{
		private IRefreshDelegate _scrollDelegate;

		public FGRefreshTableSource (IRefreshDelegate scrollDelegate)
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

		public abstract FGRefreshView RefreshView(UITableView tableView);
	}
}