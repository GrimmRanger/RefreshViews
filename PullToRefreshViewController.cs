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

		public PullToRefreshViewController () : base ("PullToRefreshViewController", null)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			_refreshTable = new RefreshTableView(this.View.Bounds);
			_refreshTable.RefreshRequested += delegate(object sender, EventArgs e) {
				Console.WriteLine("Refresh Requested");
				NSTimer.CreateScheduledTimer(new TimeSpan(0, 0, 2),
				                             delegate { _refreshTable.RefreshConcluded(); });
			};
			_refreshTable.Source = new DemoTableSource(_refreshTable);
			this.View.AddSubview(_refreshTable);
		}
	}
}

