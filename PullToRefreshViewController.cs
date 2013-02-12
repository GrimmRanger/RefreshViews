using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FGUtil;

namespace PullToRefresh
{
	public partial class PullToRefreshViewController : UIViewController
	{
		RefreshTableView _refreshTable;
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

			_refreshScroll = new RefreshScrollView(this.View.Bounds);
			_refreshScroll.RefreshRequested += delegate(object sender, EventArgs e) {
				Console.WriteLine("Refresh Requested");
				NSTimer.CreateScheduledTimer(new TimeSpan(0, 0, 2),
				                             delegate { _refreshScroll.RefreshConcluded(); });
			};
			_refreshScroll.Delegate = new DemoScrollDelegate(_refreshScroll);
			_refreshScroll.ContentSize = new SizeF(this.View.Bounds.Width, this.View.Bounds.Height + 1);
			this.View.AddSubview(_refreshScroll);
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
					FontColor = UIColor.LightGray
				};
				return refreshView;
			}
		}
	}
}