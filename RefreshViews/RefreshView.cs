using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace FGUtil
{
	public class RefreshView : UIView
	{
		public enum RefreshState
		{
			Idle,
			Active,
			Refreshing
		}

		private const string defaultPullText			=	"Pull to Refresh...";
		private const string defaultReleaseText			=	"Release to Refresh...";
		private const string defaultRefreshText			= 	"Refreshing...";

		private const float _defaultHeight				= 	30;
		private const float _defaultDetailWidth			= 	120;
		private const float _defaultDetailHeight		= 	20;
		private const float _indicatorDim				= 	21;

		private UIView _background;
		private UIView _indicator;
		private UILabel _detail;
		private UIImageView _arrow;
		private UIActivityIndicatorView	_activity;

		public UIView BackgroundView {
			set 
			{
				_background = value;
				
				if (_background != null && _background.Superview == null)
					this.InsertSubview(_background, 0);
			}
		}

		public UIActivityIndicatorViewStyle ActivityIndicatorStyle {
			set 
			{
				if (value != UIActivityIndicatorViewStyle.WhiteLarge)
					_activity.ActivityIndicatorViewStyle = value;
			}
		}

		private UIColor _fontColor = UIColor.White;
		public UIColor FontColor {
			get { return _fontColor; }
			set 
			{
				_fontColor = value;
				_detail.TextColor = _fontColor;
			}
		}

		private RefreshState _state;
		public RefreshState State {
			get { return _state; }
			set 
			{
				_state = value;
				Update ();
			}
		}

		public bool IsRefreshing {
			get { return _state == RefreshState.Refreshing; }
		}

		public RefreshView (UIView view) : base(new RectangleF(0, -_defaultHeight, view.Bounds.Width, _defaultHeight))
		{
			Initialize();
		}

		private void Initialize()
		{
			this.BackgroundColor = UIColor.Clear;

			_detail = new UILabel() 
			{
				Frame = new RectangleF((this.Bounds.Width - _defaultDetailWidth) / 2, 
				                       (this.Bounds.Height - _defaultDetailHeight) / 2,
				                       _defaultDetailWidth, _defaultDetailHeight),
				BackgroundColor = UIColor.Clear,
				Font = UIFont.FromName("HelveticaNeue-Medium", 10),
				TextColor = _fontColor,
				TextAlignment = UITextAlignment.Center,
				Text = defaultPullText
			};
			this.AddSubview(_detail);

			_indicator = new UIView()
			{
				Frame = new RectangleF(_detail.Frame.X - _indicatorDim - 10,
				                       (this.Bounds.Height - _indicatorDim) / 2,
				                       _indicatorDim, _indicatorDim),
				BackgroundColor = UIColor.Clear
			};
			this.AddSubview(_indicator);

			_arrow = new UIImageView(UIImage.FromBundle("RefreshViews/refreshArrow.png"));
			_indicator.AddSubview(_arrow);

			_activity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
			_activity.Hidden = true;
			_indicator.AddSubview(_activity);
		}

		public void Update ()
		{
			_activity.Hidden = !IsRefreshing;
			_arrow.Hidden = IsRefreshing;

			switch (_state) 
			{
			case RefreshState.Active:
				_detail.Text = defaultReleaseText;
				break;
			case RefreshState.Refreshing:
				_detail.Text = defaultRefreshText;
				_activity.StartAnimating();
				break;
			case RefreshState.Idle:
			default:
				_detail.Text = defaultPullText;
				_activity.StopAnimating();
				break;
			}

			AdjustFrames();
			FlipArrow();
		}

		private void AdjustFrames ()
		{
			AdjustDetailFrame();
			AdjustIndicatorFrame();
		}

		private void AdjustDetailFrame ()
		{
			if (IsRefreshing) 
			{
				_detail.SizeToFit();
				_detail.Frame = new RectangleF((this.Bounds.Width - _detail.Bounds.Width) / 2,
				                               (this.Bounds.Height - _detail.Bounds.Height) / 2,
				                               _detail.Bounds.Width, _detail.Bounds.Height);
			} 
			else 
			{
				_detail.Frame = new RectangleF((this.Bounds.Width - _defaultDetailWidth) / 2,
				                               (this.Bounds.Height - _defaultDetailHeight) / 2,
				                               _defaultDetailWidth, _defaultDetailHeight);
			}
		}

		private void AdjustIndicatorFrame()
		{
			_indicator.Frame = new RectangleF(_detail.Frame.X - _indicatorDim - 10,
			                                  (this.Bounds.Height - _indicatorDim) / 2,
			                                  _indicatorDim, _indicatorDim);
		}

		private void FlipArrow()
		{
			UIView.BeginAnimations("Rotate");
			UIView.SetAnimationDuration(0.4);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			_arrow.Transform = CGAffineTransform.MakeRotation(_state == RefreshState.Active ? (float)Math.PI : 0);
			UIView.CommitAnimations();
		}
	}
}