using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using CalendarViewSample.Calendar;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CalendarViewSample
{
    public class SkMonthView : SKCanvasView
    {


        public static readonly BindableProperty FirstSelectedProperty =
           BindableProperty.Create(nameof(FirstSelected), typeof(CalendarObject), typeof(SkMonthView), propertyChanged: OnPropertyChanged);

        public CalendarObject FirstSelected
        {
            get => (CalendarObject)GetValue(FirstSelectedProperty);
            set => SetValue(FirstSelectedProperty, value);
        }

        public static readonly BindableProperty LastSelectedProperty =
           BindableProperty.Create(nameof(LastSelected), typeof(CalendarObject), typeof(SkMonthView), propertyChanged: OnPropertyChanged);

        public CalendarObject LastSelected
        {
            get => (CalendarObject)GetValue(LastSelectedProperty);
            set => SetValue(LastSelectedProperty, value);
        }

        public static BindableProperty FirstAvailableDateProperty =
     BindableProperty.Create(nameof(FirstAvailableDate), typeof(DateTime?), typeof(SkMonthView));

        public DateTime? FirstAvailableDate
        {
            get => (DateTime?)GetValue(FirstAvailableDateProperty);
            set => SetValue(FirstAvailableDateProperty, value);
        }

        public static BindableProperty LastAvailableDateProperty =
            BindableProperty.Create(nameof(LastAvailableDate), typeof(DateTime?), typeof(SkMonthView));

        public DateTime? LastAvailableDate
        {
            get => (DateTime?)GetValue(LastAvailableDateProperty);
            set => SetValue(LastAvailableDateProperty, value);
        }

        public static readonly BindableProperty MonthProperty =
            BindableProperty.Create(nameof(Month), typeof(DateTime), typeof(SkMonthView), DateTime.Today,
                propertyChanged: OnPropertyChanged);

        public DateTime Month
        {
            get => (DateTime)GetValue(MonthProperty);
            set => SetValue(MonthProperty, value);
        }

        static void OnPropertyChanged(BindableObject bindable, object oldVal, object newVal)
        {
            var calendarMonthView = bindable as SkMonthView;

            calendarMonthView?.InvalidateSurface();
        }

        public SkMonthView()
        {
            BackgroundColor = Color.White;
            EnableTouchEvents = true;

        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            base.OnTouch(e);

            if (e.ActionType == SKTouchAction.Released)
            {
                DrawClosestToPoint(e.Location);
            }
            e.Handled = true;

        }

        double _density => (float)DeviceDisplay.MainDisplayInfo.Density;

        float _colWidth;
        float _rowHeight;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is DateTime date)
                this.Month = date;
        }

        void DrawClosestToPoint(SKPoint point)
        {
            var col = (int)(point.X / _colWidth);
            var row = (int)((point.Y + 25 * _density) / _rowHeight);

            var day = _list.Find(item => item.col == col && item.row == row);
            if (day.day == DateTime.MinValue || day.day < FirstAvailableDate || day.day > LastAvailableDate) return;
            if (day.day != FirstSelected?.Date && day.day != LastSelected?.Date)
            {

                if (FirstSelected != null && LastSelected != null)
                {
                    FirstSelected = new CalendarObject { Date = day.day, IsSelected = true, IsFirst = true };
                    LastSelected = null;
                }
                else if (FirstSelected != null && LastSelected == null)
                {
                    if (day.day > FirstSelected.Date)
                        LastSelected = new CalendarObject { Date = day.day, IsSelected = true, IsFirst = false };
                    else
                    {
                        LastSelected = FirstSelected;
                        FirstSelected = new CalendarObject { Date = day.day, IsSelected = true, IsFirst = true };
                    }
                }
                else
                {
                    FirstSelected = new CalendarObject { Date = day.day, IsSelected = true, IsFirst = true };
                    LastSelected = null;
                }
            }

            this.InvalidateSurface();
        }
        List<(int row, int col, DateTime day)> _list;
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            base.OnPaintSurface(args);
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();
            _list = new List<(int row, int col, DateTime day)>();
            if (IsVisible)
            {

                var widthPixels = info.Width - 30;
                var heightPixels = info.Height - 30;

                var daysInMonth = DateTime.DaysInMonth(Month.Year, Month.Month);

                _colWidth = widthPixels / 7;
                _rowHeight = heightPixels / (daysInMonth < 29 && new DateTime(Month.Year, Month.Month, 1).DayOfWeek == DayOfWeek.Monday ? 6 : 7);
                if (daysInMonth > 30 && (new DateTime(Month.Year, Month.Month, 1).DayOfWeek == DayOfWeek.Saturday ||
                    new DateTime(Month.Year, Month.Month, 1).DayOfWeek == DayOfWeek.Sunday))
                    _rowHeight = heightPixels / 8;
                var row = 3;


                var paint = new SKPaint
                {
                    Color = Color.Black.ToSKColor(),
                    TextSize = (float)(17 * _density),
                    IsAntialias = true,
                    IsDither = true
                };


                SKRect textBounds = new SKRect();
                paint.MeasureText("OO", ref textBounds);

                var header = Month.ToString("MMMM, yyyy");
                header = header.First().ToString().ToUpper() + header.Substring(1);

                canvas.DrawText(header, new SKPoint(_colWidth / 4,
                        (float)(1 * _rowHeight)), new SKPaint
                        {
                            Color = Color.Black.ToSKColor(),
                            TextSize = (float)(17 * _density),
                            FakeBoldText = true,
                            IsAntialias = true,
                            IsDither = true
                        });


                var days = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames.ToList();
                var lastDay = days[0];
                days.Remove(lastDay);
                days.Add(lastDay);

                var subHeadingPaint = new SKPaint
                {
                    Color = Color.Gray.ToSKColor(),
                    TextSize = (float)(13 * _density),
                    IsAntialias = true,
                    IsDither = true
                };

                for (var i = 0; i < days.Count; i++)
                {
                    var bounds = new SKRect();
                    subHeadingPaint.MeasureText(days[i].ToUpper(), ref bounds);
                    var x = (float)(i * _colWidth);
                    x = x + _colWidth / 2 - bounds.MidX;
                    canvas.DrawText(days[i].ToUpper(), new SKPoint(x,
                       (float)(2 * _rowHeight)), subHeadingPaint);
                }

                for (var i = 1; i <= daysInMonth; i++)
                {
                    var day = new DateTime(Month.Year, Month.Month, i).Date;
                    var isAvailable =
                        day <= LastAvailableDate &&
                        day >= FirstAvailableDate;

                    var col = day.DayOfWeek == DayOfWeek.Monday ? 0 :
                        day.DayOfWeek == DayOfWeek.Tuesday ? 1 :
                        day.DayOfWeek == DayOfWeek.Wednesday ? 2 :
                        day.DayOfWeek == DayOfWeek.Thursday ? 3 :
                        day.DayOfWeek == DayOfWeek.Friday ? 4 :
                        day.DayOfWeek == DayOfWeek.Saturday ? 5 :
                        6;
                    paint.Color = isAvailable ? Color.Black.ToSKColor() : Color.Gray.ToSKColor();

                    _list.Add((row, col, day));

                    SKRect valueBounds = new SKRect();
                    paint.MeasureText(day.ToString("%d"), ref valueBounds);

                    var x = (float)(col * _colWidth);
                    var y = (float)(row * _rowHeight);
                    var rect = new SKRect((float)x, (float)(y - valueBounds.Height - (9 * _density)), (float)(_colWidth + x), (float)(y + (10 * _density)));

                    if (!(FirstSelected?.Date.Date == day.Date || LastSelected?.Date.Date == day.Date))
                    {

                        if (day.Date > FirstSelected?.Date.Date && day.Date < LastSelected?.Date.Date)
                        {
                            canvas.DrawRect(rect, new SKPaint { Color = Color.Blue.ToSKColor() });
                            paint.Color = Color.White.ToSKColor();
                            canvas.DrawText(day.ToString("%d"), new SKPoint(x + _colWidth / 2 - valueBounds.MidX, y), paint);
                        }
                        else { canvas.DrawText(day.ToString("%d"), new SKPoint(x + _colWidth / 2 - valueBounds.MidX, y), paint); }
                    }
                    else
                    {

                        if (FirstSelected != null && LastSelected != null)
                        {
                            if (FirstSelected?.Date.Date == day.Date)
                                canvas.DrawRect(new SKRect(rect.Right - rect.Width / 2, rect.Top, rect.Right, rect.Bottom), new SKPaint { Color = Color.Blue.ToSKColor() });
                            if (LastSelected?.Date.Date == day.Date)
                                canvas.DrawRect(new SKRect(rect.Left, rect.Top, rect.Left + rect.Width / 2, rect.Bottom), new SKPaint { Color = Color.Blue.ToSKColor() });

                        }


                        canvas.DrawRoundRect(new SKRoundRect(rect, rect.Height / 2, rect.Height / 2), new SKPaint { Color = Color.DarkBlue.ToSKColor() });
                        paint.Color = Color.White.ToSKColor();
                        canvas.DrawText(day.ToString("%d"), new SKPoint(x + _colWidth / 2 - valueBounds.MidX, y), paint);
                    }


                    if (i > daysInMonth - 1 || col != 6) continue;
                    row++;
                }
            }
        }
    }
}
