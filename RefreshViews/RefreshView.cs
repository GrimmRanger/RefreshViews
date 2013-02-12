using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace FGUtil
{
	public enum RefreshViewOrientation
	{
		Vertical,
		Horizontal
	}

	public enum RefreshViewState
	{
		Idle,
		Active,
		Refreshing
	}

	public class RefreshView : UIView
	{
		private const string defaultPullText			=	"Pull to Refresh...";
		private const string defaultReleaseText			=	"Release to Refresh...";
		private const string defaultRefreshText			= 	"Refreshing...";

		private const float _defaultDetailWidth			= 	120;
		private const float _defaultDetailHeight		= 	20;
		private const float _indicatorDim				= 	21;
		public const float RefreshOffset				= 	30;

		private SizeF _superviewDims;
		private UIView _background;
		private UIView _indicator;
		private UILabel _detail;
		private UIImageView _arrow;
		private UIActivityIndicatorView	_activity;

		private RefreshViewOrientation _orientation;
		public RefreshViewOrientation Orientation
		{
			get { return _orientation; }
			set
			{
				_orientation = value;
				AdjustFrame();
				AdjustFrames();
				FlipArrow();
			}
		}
		
		private RefreshViewState _state;
		public RefreshViewState State {
			get { return _state; }
			set 
			{
				_state = value;
				Update ();
			}
		}

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

		public bool IsRefreshing {
			get { return _state == RefreshViewState.Refreshing; }
		}

		public RefreshView (UIView view) : base(new RectangleF(0, -RefreshOffset, view.Bounds.Width, RefreshOffset))
		{
			_superviewDims = view.Frame.Size;
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
			if (Orientation == RefreshViewOrientation.Vertical)
				UpdateDetail();
			UpdateIndicator();

			AdjustFrames();
			FlipArrow();
		}

		private void UpdateDetail()
		{
			switch (_state) 
			{
			case RefreshViewState.Active:
				_detail.Text = defaultReleaseText;
				break;
			case RefreshViewState.Refreshing:
				_detail.Text = defaultRefreshText;
				break;
			case RefreshViewState.Idle:
			default:
				_detail.Text = defaultPullText;
				break;
			}
		}

		private void UpdateIndicator()
		{
			_activity.Hidden = !IsRefreshing;
			_arrow.Hidden = IsRefreshing;

			switch (_state) 
			{
			case RefreshViewState.Refreshing:
				_activity.StartAnimating();
				break;
			case RefreshViewState.Idle:
			default:
				_activity.StopAnimating();
				break;
			}
		}

		private void AdjustFrame()
		{
			if (Orientation == RefreshViewOrientation.Vertical)
			{
				this.Frame = new RectangleF(0, -RefreshOffset, _superviewDims.Width, RefreshOffset);
			}
			else 
			{
				this.Frame = new RectangleF(-RefreshOffset, 0, RefreshOffset, _superviewDims.Height);
			}
		}

		private void AdjustFrames()
		{
			AdjustDetailFrame();
			AdjustIndicatorFrame();
		}

		private void AdjustDetailFrame ()
		{
			if (Orientation == RefreshViewOrientation.Vertical)
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
			else
			{
				_detail.Frame = RectangleF.Empty;
			}
		}

		private void AdjustIndicatorFrame()
		{
			if (Orientation == RefreshViewOrientation.Vertical)
			{
				_indicator.Frame = new RectangleF(_detail.Frame.X - _indicatorDim - 10,
				                                  (this.Bounds.Height - _indicatorDim) / 2,
				                                  _indicatorDim, _indicatorDim);
			}
			else
			{
				_indicator.Frame = new RectangleF((this.Bounds.Width - _indicatorDim) / 2,
				                                  (this.Bounds.Height - _indicatorDim) / 2,
				                                  _indicatorDim, _indicatorDim);
			}
		}

		private void FlipArrow()
		{
			UIView.BeginAnimations("Rotate");
			UIView.SetAnimationDuration(0.4);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			float rad = Orientation == RefreshViewOrientation.Vertical ?
				_state == RefreshViewState.Active ? (float)Math.PI : 0 :
					_state == RefreshViewState.Active ? (float)Math.PI/2 : 3*(float)Math.PI/2;
			_arrow.Transform = CGAffineTransform.MakeRotation(rad);
			UIView.CommitAnimations();
		}
	}
}