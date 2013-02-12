using System;
using MonoTouch.UIKit;

namespace FGUtil
{
	public abstract class FGRefreshScrollDelegate : UIScrollViewDelegate
	{
		private IRefreshDelegate _scrollDelegate;
		
		public FGRefreshScrollDelegate (IRefreshDelegate scrollDelegate)
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
		
		public abstract FGRefreshView RefreshView(UIScrollView scrollView);
	}
	
	public interface IRefreshDelegate
	{
		void DidScroll(UIScrollView scrollView);
		void BeganDeceleration(UIScrollView scrollView);
	}
}