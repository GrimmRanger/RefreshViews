using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace FGUtil
{
	[Register("RefreshTableView")]
	public partial class RefreshTableView : UITableView, IRefreshDelegate
	{
		private RefreshView _refreshView;

		public new RefreshTableSource Source {
			get { return (RefreshTableSource)base.Source; }
			set 
			{
				base.Source = value;
				ConfigRefreshView();
			}
		}

		private event EventHandler _refreshRequested;
		public event EventHandler RefreshRequested {
			add 
			{
				_refreshRequested += value;
				ConfigRefreshView();
			}
			remove 
			{
				_refreshRequested -= value;
				ConfigRefreshView();
			}
		}

		private bool RefreshEnabled {
			get { return Source != null && _refreshRequested != null; }
		}

		public RefreshTableView (IntPtr handle) : base(handle) {}
		public RefreshTableView (RectangleF frame) : base(frame) {}
		public RefreshTableView () : base() {}

		private void ConfigRefreshView ()
		{
			if (RefreshEnabled) 
			{
				if (_refreshView == null)
					_refreshView = Source.RefreshView (this);

				if (_refreshView.Superview == null)
					this.AddSubview (_refreshView);
			} else 
			{
				if (_refreshView != null && _refreshView.Superview != null)
					_refreshView.RemoveFromSuperview();
			}
		}

		private void RefreshInitiated()
		{
			_refreshView.State = RefreshViewState.Refreshing;
			OnRefreshRequested();
			this.SetContentOffset(this.ContentOffset, true);
			ConductRefreshTransition();
		}

		public void RefreshConcluded ()
		{
			_refreshView.State = RefreshViewState.Idle;
			ConductRefreshTransition();
		}

		private void ConductRefreshTransition ()
		{
			UIView.BeginAnimations("Scroll");
			UIView.SetAnimationDuration(_refreshView.IsRefreshing ? 0.4 : 0.2);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			this.ContentInset = new UIEdgeInsets(_refreshView.IsRefreshing ? _refreshView.Frame.Height : 0, 0, 0, 0);
			UIView.CommitAnimations();

			this.ScrollEnabled = !_refreshView.IsRefreshing;
		}

		protected virtual void OnRefreshRequested()
		{
			EventHandler handler = _refreshRequested;

			if (handler != null)
				handler(this, EventArgs.Empty);
		}

#region
		public void DidScroll (UIScrollView scrollView)
		{
			if (RefreshEnabled && !_refreshView.IsRefreshing) 
			{
				float offset = scrollView.ContentOffset.Y;

				if (_refreshView.State != RefreshViewState.Idle && offset > -_refreshView.Frame.Height)
					_refreshView.State = RefreshViewState.Idle;
				if (_refreshView.State != RefreshViewState.Active && offset < -_refreshView.Frame.Height)
					_refreshView.State = RefreshViewState.Active;
			}
		}

		public void BeganDeceleration(UIScrollView scrollView)
		{
			if (RefreshEnabled && !_refreshView.IsRefreshing && scrollView.ContentOffset.Y < -_refreshView.Frame.Height)
				RefreshInitiated();
		}
#endregion
	}
}