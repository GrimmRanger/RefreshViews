using System;
using MonoTouch.UIKit;

namespace FGUtil
{
	public abstract class RefreshScrollDelegate : UIScrollViewDelegate
	{
		private IRefreshDelegate _scrollDelegate;
		
		public RefreshScrollDelegate (IRefreshDelegate scrollDelegate)
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
		
		public abstract RefreshView RefreshView(UIScrollView scrollView);
	}
	
	public interface IRefreshDelegate
	{
		void DidScroll(UIScrollView scrollView);
		void BeganDeceleration(UIScrollView scrollView);
	}
}