using System;
using Xamarin.Forms;

namespace CalendarViewSample
{
    public static class ViewExtensions
    {
        public static TView Bind<TView>(this TView view, BindableProperty targetProperty, string sourcePropertyName = ".", BindingMode mode = BindingMode.Default, IValueConverter converter = null, object converterParameter = null, string stringFormat = null, object source = null) where TView : Element
        {
            if (source != null || converterParameter != null)
                view.SetBinding(targetProperty, new Binding(
                    path: sourcePropertyName,
                    mode: mode,
                    converter: converter,
                    converterParameter: converterParameter,
                    stringFormat: stringFormat,
                    source: source
                ));
            else
                view.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
            return view;
        }
    }
}
