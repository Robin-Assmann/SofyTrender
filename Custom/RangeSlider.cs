using System.Diagnostics;

/// <summary>
/// A Slider to select two values between 0.0 and 1.0
///
/// For Xamarin please use: https://learn.microsoft.com/en-us/xamarin/community-toolkit/views/rangeslider
/// </summary>
/// 
namespace SofyTrender.Scripts
{
    public class RangeSlider : ContentView
    {
        #region const
        private readonly static int TRACK_HEIGHT = 4;
        private readonly static int THUMB_WIDTH = 8;
        private readonly static int THUMB_RADIUS = THUMB_WIDTH / 2;
        private readonly static double DEFAULT_MIN_VALUE = 0.1;
        private readonly static double DEFAULT_MAX_VALUE = 0.9;
        private readonly static Color DEFAULT_RANGE_COLOR = Colors.LightBlue;
        private readonly static Color DEFAULT_OUT_OF_RANGE_COLOR = Colors.LightGray;
        private readonly static Color DEFAULT_THUMB_COLOR = Colors.White;
        #endregion

        /// <summary>
        /// Store column in varaible to ease width adjustements 
        /// </summary>
        #region track columns
        private ColumnDefinition LeftColumn = new ColumnDefinition { Width = GridLength.Star };
        private ColumnDefinition CenterColumn = new ColumnDefinition { Width = GridLength.Star };
        private ColumnDefinition RightColumn = new ColumnDefinition { Width = GridLength.Star };
        #endregion

        #region UI
        /// <summary>
        /// To colorize everything outsid of range (low)
        /// </summary>
        private Grid LeftTrack = new Grid
        {
            HeightRequest = TRACK_HEIGHT,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = DEFAULT_OUT_OF_RANGE_COLOR
        };

        /// <summary>
        /// To colorize everything in range
        /// </summary>
        private Grid CenterTrack = new Grid
        {
            HeightRequest = TRACK_HEIGHT,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = DEFAULT_RANGE_COLOR
        };

        /// <summary>
        /// To colorize everything outsid of range (up)
        /// </summary>
        private Grid RightTrack = new Grid
        {
            HeightRequest = TRACK_HEIGHT,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = DEFAULT_OUT_OF_RANGE_COLOR
        };

        /// <summary>
        /// Thumb for min value
        /// </summary>
        private Frame MinThumb = new Frame
        {
            BackgroundColor = DEFAULT_THUMB_COLOR,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BorderColor = DEFAULT_RANGE_COLOR,
            WidthRequest = THUMB_WIDTH,
            HeightRequest = THUMB_WIDTH * 2,
        };

        /// <summary>
        /// Thumb for max value
        /// </summary>
        private Frame MaxThumb = new Frame
        {
            BackgroundColor = DEFAULT_THUMB_COLOR,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BorderColor = DEFAULT_RANGE_COLOR,
            WidthRequest = THUMB_WIDTH,
            HeightRequest = THUMB_WIDTH * 2,
        };
        #endregion

        #region bindable properties

        #region MinValue
        /// <summary>
        /// Identifies the <see cref="MinValueProperty"/> bindable property.
        /// </summary>
        public static readonly BindableProperty MinValueProperty =
            BindableProperty.Create(nameof(MinValue),
                                    typeof(double),
                                    typeof(RangeSlider),
                                    DEFAULT_MIN_VALUE,
                                    BindingMode.TwoWay);
        /// <summary>
        /// MinValue between 0 and 1
        /// </summary>
        /// <seealso cref="MinValueProperty"/>
        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set
            {
                TranslateThumbRel(MinThumb, MinValue, value);
                SetValue(MinValueProperty, value);
            }
        }
        #endregion

        #region MaxValue
        /// <summary>
        /// Identifies the <see cref="MaxValueProperty"/> bindable property.
        /// </summary>
        public static readonly BindableProperty MaxValueProperty =
            BindableProperty.Create(nameof(MaxValue),
                                    typeof(double),
                                    typeof(RangeSlider),
                                    DEFAULT_MAX_VALUE,
                                    BindingMode.TwoWay);
        /// <summary>
        /// MaxValue  between 0 and 1
        /// </summary>
        /// <seealso cref="MaxValueProperty"/>
        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set
            {
                TranslateThumbRel(MaxThumb, MaxValue, value);
                SetValue(MaxValueProperty, value);
            }
        }
        #endregion

        #region RangeColor
        /// <summary>
        /// Identifies the <see cref="RangeColorProperty"/> bindable property.
        /// </summary>
        public static readonly BindableProperty RangeColorProperty =
            BindableProperty.Create(nameof(RangeColor),
                                    typeof(Color),
                                    typeof(RangeSlider),
                                    DEFAULT_RANGE_COLOR,
                                    BindingMode.TwoWay,
                                    propertyChanged: (bindable, oldValue, newValue) =>
                                    {
                                        if (bindable is RangeSlider slider && newValue is Color color)
                                        {
                                            slider.CenterTrack.BackgroundColor = color;
                                            slider.MinThumb.BorderColor = color;
                                            slider.MaxThumb.BorderColor = color;
                                        }
                                    });
        /// <summary>
        /// Color of range
        /// </summary>
        /// <seealso cref="RangeColorProperty"/>
        public Color RangeColor
        {
            get => (Color)GetValue(RangeColorProperty);
            set => SetValue(RangeColorProperty, value);
        }
        #endregion

        #region ThumbsColor
        /// <summary>
        /// Identifies the <see cref="ThumbsColorProperty"/> bindable property.
        /// </summary>
        public static readonly BindableProperty ThumbsColorProperty =
            BindableProperty.Create(nameof(ThumbsColor),
                                    typeof(Color),
                                    typeof(RangeSlider),
                                    DEFAULT_THUMB_COLOR,
                                    BindingMode.TwoWay,
                                    propertyChanged: (bindable, oldValue, newValue) =>
                                    {
                                        if (bindable is RangeSlider slider && newValue is Color color)
                                        {
                                            slider.MinThumb.BackgroundColor = color;
                                            slider.MaxThumb.BackgroundColor = color;
                                        }
                                    });
        /// <summary>
        /// Color of thumb
        /// </summary>
        /// <seealso cref="ThumbsColorProperty"/>
        public Color ThumbsColor
        {
            get => (Color)GetValue(ThumbsColorProperty);
            set => SetValue(ThumbsColorProperty, value);
        }
        #endregion

        #region OutOfRangeColor
        /// <summary>
        /// Identifies the <see cref="OutOfRangeColorProperty"/> bindable property.
        /// </summary>
        public static readonly BindableProperty OutOfRangeColorProperty =
            BindableProperty.Create(nameof(OutOfRangeColor),
                                    typeof(Color),
                                    typeof(RangeSlider),
                                    DEFAULT_OUT_OF_RANGE_COLOR,
                                    BindingMode.TwoWay,
                                    propertyChanged: (bindable, oldValue, newValue) =>
                                    {
                                        if (bindable is RangeSlider slider && newValue is Color color)
                                        {
                                            slider.LeftTrack.BackgroundColor = color;
                                            slider.RightTrack.BackgroundColor = color;
                                        }
                                    });
        /// <summary>
        /// Color for area outside of range
        /// </summary>
        /// <seealso cref="OutOfRangeColorProperty"/>
        public Color OutOfRangeColor
        {
            get => (Color)GetValue(OutOfRangeColorProperty);
            set => SetValue(OutOfRangeColorProperty, value);
        }
        #endregion

        #endregion

        #region engine properties
        /// <summary>
        /// Max thumb real position
        /// </summary>
        private double EffectiveMinThumbX
        {
            get
            {
                return MinThumb.X + MinThumb.TranslationX;
            }
        }

        /// <summary>
        /// Min thumb real position
        /// </summary>
        private double EffectiveMaxThumbX
        {
            get
            {
                return MaxThumb.X + MaxThumb.TranslationX;
            }
        }
        #endregion

        #region utils
        /// <summary>
        /// to ease gridlenth init from strings
        /// </summary>
        private GridLengthTypeConverter converter = new GridLengthTypeConverter();
        #endregion

        public RangeSlider()
        {
            Grid.SetColumn(LeftTrack, 0);
            Grid.SetColumn(CenterTrack, 1);
            Grid.SetColumn(RightTrack, 2);
            Grid.SetColumnSpan(MinThumb, 3);
            Grid.SetColumnSpan(MaxThumb, 3);

            Content = new Grid
            {
                ColumnDefinitions =
                {
                    LeftColumn,
                    CenterColumn,
                    RightColumn
                },
                Children =
                {
                   LeftTrack,
                   CenterTrack,
                   RightTrack,
                   MinThumb,
                   MaxThumb,
                }
            };

            //handle thumb moves via pan gesture
            double startPanXCoord = 0;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (sender, args) =>
            {
                if (sender is View thumb)
                {
                    if (args.StatusType == GestureStatus.Started)
                    {
                        startPanXCoord = thumb.TranslationX;
                    }
                    else if (args.StatusType == GestureStatus.Running)
                    {
                        TranslateThumbAbs(thumb, startPanXCoord + args.TotalX);
                        UpdateMinMaxValues();
                    }
                }
            };
            MinThumb.GestureRecognizers.Add(panGesture);
            MaxThumb.GestureRecognizers.Add(panGesture);

            //set min values
            var minReady = false;
            var maxReady = false;
            MinThumb.SizeChanged += (sender, args) =>
            {
                if (maxReady && !minReady)
                {
                    if (MaxValue > 0.5)
                    {
                        TranslateThumbRel(MaxThumb, 0.5, MaxValue);
                        TranslateThumbRel(MinThumb, 0.5, MinValue);
                    }
                    else
                    {
                        TranslateThumbRel(MinThumb, 0.5, MinValue);
                        TranslateThumbRel(MaxThumb, 0.5, MaxValue);
                    }

                }
                minReady = true;
            };

            MaxThumb.SizeChanged += (sender, args) =>
            {
                if (minReady && !maxReady)
                {
                    if (MaxValue > 0.5)
                    {
                        TranslateThumbRel(MaxThumb, 0.5, MaxValue);
                        TranslateThumbRel(MinThumb, 0.5, MinValue);
                    }
                    else
                    {
                        TranslateThumbRel(MinThumb, 0.5, MinValue);
                        TranslateThumbRel(MaxThumb, 0.5, MaxValue);
                    }
                }
                maxReady = true;
            };
        }

        /// <summary>
        /// Update Min and Max values
        /// </summary>
        private void UpdateMinMaxValues()
        {
            SetValue(MinValueProperty, (EffectiveMinThumbX + THUMB_RADIUS) / Width);
            SetValue(MaxValueProperty, (EffectiveMaxThumbX + THUMB_RADIUS) / Width);
        }

        /// <summary>
        /// Translate thumb using relatives values (old and new) (between 0 and 1)
        /// </summary>
        private void TranslateThumbRel(View thumb, double oldValue, double newValue)
        {
            var relativeDelta = newValue - Math.Max(0, Math.Min(1, oldValue));
            var absoluteDelta = relativeDelta * Width;
            TranslateThumbAbs(thumb, absoluteDelta);
        }

        /// <summary>
        /// Translate thumb using an absolute X value (in pixels)
        /// </summary>
        private void TranslateThumbAbs(View thumb, double deltaX)
        {
            var wishedX = thumb.X + deltaX;
            var otherThumbX = 0.0;
            var minBoundX = -1;
            var maxBoundX = Width - 1;
            var newX = 0.0;
            if (thumb == MinThumb)
            {
                otherThumbX = -1 + EffectiveMaxThumbX;//THUMB_WIDTH to avoid overlapping
                newX = Math.Max(minBoundX, Math.Min(Math.Min(wishedX, otherThumbX), maxBoundX));
            }
            else if (thumb == MaxThumb)
            {
                otherThumbX = 1 + EffectiveMinThumbX;//THUMB_WIDTH to avoid overlapping
                newX = Math.Min(maxBoundX, Math.Max(Math.Max(wishedX, otherThumbX), minBoundX));
            }

            thumb.TranslationX = newX - thumb.X;


            UpdateTracks();
        }

        /// <summary>
        /// Updage background tracks according to values
        /// </summary>
        private void UpdateTracks()
        {
            var leftSpace = (int)Math.Round(MinValue * 100, 0);
            var centerSpace = (int)Math.Round((MaxValue - MinValue) * 100);
            var rightSpace = (int)Math.Round((1 - MaxValue) * 100);
            LeftColumn.Width = (GridLength)converter.ConvertFromInvariantString(string.Format("{0}*", leftSpace));
            CenterColumn.Width = (GridLength)converter.ConvertFromInvariantString(string.Format("{0}*", centerSpace));
            RightColumn.Width = (GridLength)converter.ConvertFromInvariantString(string.Format("{0}*", rightSpace));
        }
    }

}

