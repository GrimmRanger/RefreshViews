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
		FGRefreshScrollView _refreshScroll;

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

			_refreshScroll = new FGRefreshScrollView(view.Bounds);
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

		private class DemoScrollDelegate : FGRefreshScrollDelegate
		{
			public DemoScrollDelegate(IRefreshDelegate refreshDelegate) : base(refreshDelegate)
			{
			}

			public override FGUtil.FGRefreshView RefreshView (UIScrollView scrollView)
			{
				FGRefreshView refreshView = new FGRefreshView(scrollView) {
					ActivityIndicatorStyle = UIActivityIndicatorViewStyle.Gray,
					FontColor = UIColor.LightGray,
					Orientation = FGRefreshViewOrientation.Horizontal
				};
				return refreshView;
			}
		}
	}
}