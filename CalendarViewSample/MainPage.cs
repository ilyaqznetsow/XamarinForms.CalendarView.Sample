using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CalendarViewSample
{
    public class MainPage : ContentPage
    {
        List<DateTime> ItemsCollection { get; set; }
        public MainPage()
        {
            ItemsCollection = Enumerable.Range(-12, 12)
              .Select(DateTime.Now.AddMonths)
              .ToList();

            Content = new StackLayout
            {
                Padding = new Thickness(0,20,0,20),
                Spacing = 20,
                Children = {
                new Label{Text="Calendar View Sample"
                },
                new CalendarView()
                {ItemsSource = ItemsCollection,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand}
                }
            };
        }

    }
}

