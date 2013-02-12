using System;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.UIKit;

namespace FGUtil
{
	[Register("FGRefreshScrollView")]
	public partial class FGRefreshScrollView : UIScrollView, IRefreshDelegate
	{
		private FGRefreshView _refreshView;
		
		public new FGRefreshScrollDelegate Delegate {
			get { return (FGRefreshScrollDelegate)base.Delegate; }
			set 
			{
				base.Delegate = value;
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
			get { return Delegate != null && _refreshRequested != null; }
		}
		
		public FGRefreshScrollView (IntPtr handle) : base(handle) {}
		public FGRefreshScrollView (RectangleF frame) : base(frame) {}
		public FGRefreshScrollView () : base() {}
		
		private void ConfigRefreshView ()
		{
			if (RefreshEnabled) 
			{
				if (_refreshView == null)
					_refreshView = Delegate.RefreshView (this);
				
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
			_refreshView.State = FGRefreshViewState.Refreshing;
			OnRefreshRequested();
			this.SetContentOffset(this.ContentOffset, true);
			ConductRefreshTransition();
		}
		
		public void RefreshConcluded ()
		{
			_refreshView.State = FGRefreshViewState.Idle;
			ConductRefreshTransition();
		}
		
		private void ConductRefreshTransition ()
		{
			UIView.BeginAnimations("Scroll");
			UIView.SetAnimationDuration(_refreshView.IsRefreshing ? 0.4 : 0.2);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			float newOffset = _refreshView.IsRefreshing ? FGRefreshView.RefreshOffset : 0;
			this.ContentInset = _refreshView.Orientation == FGRefreshViewOrientation.Vertical ?
				new UIEdgeInsets(newOffset, 0, 0, 0) :
					new UIEdgeInsets(0, newOffset, 0, 0);
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
				float offset = _refreshView.Orientation == FGRefreshViewOrientation.Vertical ? 
					scrollView.ContentOffset.Y : scrollView.ContentOffset.X;
				
				if (_refreshView.State != FGRefreshViewState.Idle && offset > -FGRefreshView.RefreshOffset)
					_refreshView.State = FGRefreshViewState.Idle;
				if (_refreshView.State != FGRefreshViewState.Active && offset < -FGRefreshView.RefreshOffset)
					_refreshView.State = FGRefreshViewState.Active;
			}
		}
		
		public void BeganDeceleration(UIScrollView scrollView)
		{
			float offset = _refreshView.Orientation == FGRefreshViewOrientation.Vertical ? 
				scrollView.ContentOffset.Y : scrollView.ContentOffset.X;

			if (RefreshEnabled && !_refreshView.IsRefreshing && offset < -FGRefreshView.RefreshOffset)
				RefreshInitiated();
		}
#endregion
	}
}