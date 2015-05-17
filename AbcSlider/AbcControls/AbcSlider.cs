namespace AbcControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    [TemplatePart(Name = "PART_Track", Type = typeof(Track))]
    public class AbcSlider : Control
    {
        public static readonly DependencyProperty MinimumProperty;
        public static readonly DependencyProperty MaximumProperty;
        public static readonly DependencyProperty ValueProperty;

        protected const string TrackName = "PART_Track";
        private Track track;

        static AbcSlider()
        {
            AbcSlider.MinimumProperty = DependencyProperty.Register(
                "Minimum",
                typeof(double),
                typeof(AbcSlider),
                new PropertyMetadata(0.0, AbcSlider.OnMinimumChanged, AbcSlider.CoerceMinimum),
                AbcSlider.IsValidDoubleValue);

            AbcSlider.MaximumProperty = DependencyProperty.Register(
                "Maximum",
                typeof(double),
                typeof(AbcSlider),
                new PropertyMetadata(1.0, AbcSlider.OnMaximumChanged, AbcSlider.CoerceMaximum),
                AbcSlider.IsValidDoubleValue);

            AbcSlider.ValueProperty = DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(AbcSlider),
                new PropertyMetadata(0.0, AbcSlider.OnValueChanged, AbcSlider.CoerceValue),
                AbcSlider.IsValidDoubleValue);

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(AbcSlider), 
                new FrameworkPropertyMetadata(typeof(AbcSlider)));

            EventManager.RegisterClassHandler(
                typeof(AbcSlider),
                Thumb.DragDeltaEvent, 
                new DragDeltaEventHandler(AbcSlider.OnThumbDragDelta));
        }

        public double Minimum
        {
            get
            {
                return (double)this.GetValue(AbcSlider.MinimumProperty);
            }
            set
            {
                this.SetValue(AbcSlider.MinimumProperty, value);
                if (this.track != null)
                {
                    this.track.Minimum = this.Minimum;
                }
            }
        }

        public double Maximum
        {
            get
            {
                return (double)this.GetValue(AbcSlider.MaximumProperty);
            }
            set
            {
                this.SetValue(AbcSlider.MaximumProperty, value);
                if (this.track != null)
                {
                    this.track.Maximum = this.Maximum;
                }
            }
        }

        public double Value
        {
            get
            {
                return (double)this.GetValue(AbcSlider.ValueProperty);
            }
            set
            {
                this.SetValue(AbcSlider.ValueProperty, value);
                if (this.track != null)
                {
                    this.track.Value = this.Value;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var partTrack = (Track)this.GetTemplateChild(AbcSlider.TrackName);
            if (partTrack != null)
            {
                this.track = partTrack;
                this.track.Minimum = this.Minimum;
                this.track.Maximum = this.Maximum;
                this.track.Value = this.Value;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.track != null && this.track.Thumb != null && !this.track.Thumb.IsMouseOver)
            {
                var position = e.MouseDevice.GetPosition(this.track);
                var newValue = this.track.ValueFromPoint(position);
                if (AbcSlider.IsValidDoubleValue(newValue))
                {
                    this.Value = newValue;
                }

                e.Handled = true;
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected virtual void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            var thumb = (Thumb)e.OriginalSource;
            if (this.track != null && thumb == this.track.Thumb)
            {
                var newValue = this.Value + this.track.ValueFromDistance(e.HorizontalChange, e.VerticalChange);
                newValue = (double)AbcSlider.CoerceValue(this, newValue);
                if (AbcSlider.IsValidDoubleValue(newValue))
                {
                    this.Value = newValue;
                }
            }
        }

        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs args)
        {
            ((AbcSlider)sender).OnThumbDragDelta(args);
        }

        private static void OnMinimumChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var abcSlider = (AbcSlider)dependencyObject;
            abcSlider.CoerceValue(AbcSlider.ValueProperty);
            abcSlider.Minimum = (double)args.NewValue;
        }

        private static void OnMaximumChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var abcSlider = (AbcSlider)dependencyObject;
            abcSlider.CoerceValue(AbcSlider.ValueProperty);
            abcSlider.Maximum = (double)args.NewValue;
        }

        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var abcSlider = (AbcSlider)dependencyObject;
            abcSlider.Value = (double)args.NewValue;
        }

        private static object CoerceMinimum(DependencyObject dependencyObject, object value)
        {
            var maximum = ((AbcSlider)dependencyObject).Maximum;
            if ((double)value > maximum)
            {
                return maximum - 1;
            }

            return value;
        }

        private static object CoerceMaximum(DependencyObject dependencyObject, object value)
        {
            var minimum = ((AbcSlider)dependencyObject).Minimum;
            if ((double)value < minimum)
            {
                return minimum + 1;
            }

            return value;
        }

        private static object CoerceValue(DependencyObject dependencyObject, object value)
        {
            var abcSlider = (AbcSlider)dependencyObject;
            var newValue = (double)value;
            var minimum = abcSlider.Minimum;
            if (newValue < minimum)
            {
                return minimum;
            }

            var maximum = abcSlider.Maximum;
            if (newValue > maximum)
            {
                return maximum;
            }

            return value;
        }

        private static bool IsValidDoubleValue(object value)
        {
            var number = (double)value;
            var isValid = !double.IsNaN(number) && !double.IsInfinity(number);
            return isValid;
        }
    }
}
