using System;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.UIKit;

namespace FGUtil
{
	[Register("RefreshScrollView")]
	public partial class RefreshScrollView : UIScrollView, IRefreshDelegate
	{
		private RefreshView _refreshView;
		
		public new RefreshScrollDelegate Delegate {
			get { return (RefreshScrollDelegate)base.Delegate; }
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
		
		public RefreshScrollView (IntPtr handle) : base(handle) {}
		public RefreshScrollView (RectangleF frame) : base(frame) {}
		public RefreshScrollView () : base() {}
		
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
			float newOffset = _refreshView.IsRefreshing ? RefreshView.RefreshOffset : 0;
			this.ContentInset = _refreshView.Orientation == RefreshViewOrientation.Vertical ?
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
				float offset = _refreshView.Orientation == RefreshViewOrientation.Vertical ? 
					scrollView.ContentOffset.Y : scrollView.ContentOffset.X;
				
				if (_refreshView.State != RefreshViewState.Idle && offset > -RefreshView.RefreshOffset)
					_refreshView.State = RefreshViewState.Idle;
				if (_refreshView.State != RefreshViewState.Active && offset < -RefreshView.RefreshOffset)
					_refreshView.State = RefreshViewState.Active;
			}
		}
		
		public void BeganDeceleration(UIScrollView scrollView)
		{
			float offset = _refreshView.Orientation == RefreshViewOrientation.Vertical ? 
				scrollView.ContentOffset.Y : scrollView.ContentOffset.X;

			if (RefreshEnabled && !_refreshView.IsRefreshing && offset < -RefreshView.RefreshOffset)
				RefreshInitiated();
		}
#endregion
	}
}