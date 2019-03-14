using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HSPacketAnalyzer.Views.Converters
{
	/// <summary>
	/// Converter that can be used to break into the debugger for the purpose of
	/// debugging bindings.
	/// </summary>
	/// <usage>Within your XAML code, bind like this; 
	/// {Binding {}, Converter={StaticResource DebugConverter}}
	/// </usage>
	internal class DebugConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debugger.Break();
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debugger.Break();
			return value;
		}
	}
}
