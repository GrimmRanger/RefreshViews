using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FGUtil;

namespace PullToRefresh
{
	public partial class PullToRefreshViewController : UIViewController
	{
//		RefreshTableView _refreshTable;
		RefreshScrollView _refreshScroll;

		public PullToRefreshViewController () : base ("PullToRefreshViewController", null)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
//			_refreshTable = new RefreshTableView(this.View.Bounds);
//			_refreshTable.RefreshRequested += delegate(object sender, EventArgs e) {
//				Console.WriteLine("Refresh Requested");
//				NSTimer.CreateScheduledTimer(new TimeSpan(0, 0, 2),
//				                             delegate { _refreshTable.RefreshConcluded(); });
//			};
//			_refreshTable.Source = new DemoTableSource(_refreshTable);
//			this.View.AddSubview(_refreshTable);

			UIView view = new UIView(new RectangleF(0, 40, this.View.Bounds.Width, 100));
			view.BackgroundColor = UIColor.Red;

			_refreshScroll = new RefreshScrollView(view.Bounds);
			_refreshScroll.RefreshRequested += delegate(object sender, EventArgs e) {
				Console.WriteLine("Refresh Requested");
				NSTimer.CreateScheduledTimer(new TimeSpan(0, 0, 2),
				                             delegate { _refreshScroll.RefreshConcluded(); });
			};
			_refreshScroll.Delegate = new DemoScrollDelegate(_refreshScroll);
			_refreshScroll.ContentSize = new SizeF(view.Bounds.Width + 1, view.Bounds.Height);
			view.AddSubview(_refreshScroll);
			this.View.AddSubview(view);
		}

		private class DemoScrollDelegate : RefreshScrollDelegate
		{
			public DemoScrollDelegate(IRefreshDelegate refreshDelegate) : base(refreshDelegate)
			{
			}

			public override FGUtil.RefreshView RefreshView (UIScrollView scrollView)
			{
				RefreshView refreshView = new RefreshView(scrollView) {
					ActivityIndicatorStyle = UIActivityIndicatorViewStyle.Gray,
					FontColor = UIColor.LightGray,
					Orientation = RefreshViewOrientation.Horizontal
				};
				return refreshView;
			}
		}
	}
}