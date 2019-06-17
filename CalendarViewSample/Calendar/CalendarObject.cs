using System;
using System.ComponentModel;

namespace CalendarViewSample.Calendar
{
    public class CalendarObject : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }
        public bool IsSelected { get; set; }
        public bool IsAvailable { get; set; }

        public bool? IsFirst { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
