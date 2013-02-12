using System;
using FGUtil;
using MonoTouch.UIKit;

namespace PullToRefresh
{
	public class DemoTableSource : RefreshTableSource
	{
		private string[] _stuff;

		public DemoTableSource (IRefreshDelegate scrollDelegate) : base(scrollDelegate)
		{
			_stuff = new string[] { "this", "that", "the other" };
		}

		public override int NumberOfSections (MonoTouch.UIKit.UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (MonoTouch.UIKit.UITableView tableview, int section)
		{
			return _stuff.Length;
		}

		public override float GetHeightForRow (MonoTouch.UIKit.UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return 40;
		}

		public override MonoTouch.UIKit.UITableViewCell GetCell (MonoTouch.UIKit.UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("cell") as UITableViewCell;

			if (cell == null)
				cell = new UITableViewCell();

			cell.TextLabel.Text = _stuff[indexPath.Row];

			return cell;
		}

		public override FGUtil.RefreshView RefreshView (MonoTouch.UIKit.UITableView tableView)
		{
			RefreshView refreshView = new RefreshView(tableView) {
				ActivityIndicatorStyle = UIActivityIndicatorViewStyle.Gray,
				FontColor = UIColor.LightGray
			};
			return refreshView;
		}
	}
}

