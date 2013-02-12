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
			_refreshView.State = RefreshView.RefreshState.Refreshing;
			OnRefreshRequested();
			this.SetContentOffset(this.ContentOffset, true);
			ConductRefreshTransition();
		}
		
		public void RefreshConcluded ()
		{
			_refreshView.State = RefreshView.RefreshState.Idle;
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
				
				if (_refreshView.State != RefreshView.RefreshState.Idle && offset > -_refreshView.Frame.Height)
					_refreshView.State = RefreshView.RefreshState.Idle;
				if (_refreshView.State != RefreshView.RefreshState.Active && offset < -_refreshView.Frame.Height)
					_refreshView.State = RefreshView.RefreshState.Active;
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