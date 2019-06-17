using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CalendarViewSample.Calendar;
using Xamarin.Forms;

namespace CalendarViewSample
{
    public class CalendarView : CollectionView
    {

        public static BindableProperty FirstAvailableDateProperty =
            BindableProperty.Create(nameof(FirstAvailableDate),
                typeof(DateTime?), typeof(CalendarView));

        public DateTime? FirstAvailableDate
        {
            get => (DateTime?)GetValue(FirstAvailableDateProperty);
            set => SetValue(FirstAvailableDateProperty, value);
        }

        public static BindableProperty LastAvailableDateProperty =
            BindableProperty.Create(nameof(LastAvailableDate), typeof(DateTime?),
                typeof(CalendarView));

        public DateTime? LastAvailableDate
        {
            get => (DateTime?)GetValue(LastAvailableDateProperty);
            set => SetValue(LastAvailableDateProperty, value);
        }

        public static BindableProperty FirstProperty =
            BindableProperty.Create(nameof(First),
                typeof(CalendarObject), typeof(CalendarView), defaultBindingMode: BindingMode.TwoWay);

        public CalendarObject First
        {
            get => (CalendarObject)GetValue(FirstProperty);
            set => SetValue(FirstProperty, value);
        }

        public static BindableProperty LastProperty =
            BindableProperty.Create(nameof(Last),
                typeof(CalendarObject), typeof(CalendarView), defaultBindingMode: BindingMode.TwoWay);

        public CalendarObject Last
        {
            get => (CalendarObject)GetValue(LastProperty);
            set => SetValue(LastProperty, value);
        }

        public CalendarView()
        {
            SelectionMode = SelectionMode.None;
            ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
            ItemTemplate = new DataTemplate(() => new SkMonthView() { HeightRequest = 300 }
                 .Bind(SkMonthView.FirstSelectedProperty, FirstProperty.PropertyName, BindingMode.TwoWay, source: this)
                 .Bind(SkMonthView.LastSelectedProperty, LastProperty.PropertyName, BindingMode.TwoWay, source: this)
                 .Bind(SkMonthView.BindingContextProperty, ".")
             .Bind(SkMonthView.FirstAvailableDateProperty, FirstAvailableDateProperty.PropertyName, source: this)
             .Bind(SkMonthView.LastAvailableDateProperty, LastAvailableDateProperty.PropertyName, source: this)
             );
        }
    }
}