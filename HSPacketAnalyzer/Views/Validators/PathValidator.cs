using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;

namespace HSPacketAnalyzer.Views.Validators
{
	[ContentProperty(nameof(DirectoryMustExist))]
	internal class PathValidator : ValidationRule
	{
		public bool DirectoryMustExist { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value is string path)
			{
				if(string.IsNullOrWhiteSpace(path))
				{
					return new ValidationResult(false, "Path is empty!");
				}

				if (!Path.IsPathRooted(path))
				{
					return new ValidationResult(false, "Path must be rooted!");
				}

				if (DirectoryMustExist && !Directory.Exists(path))
				{
					return new ValidationResult(false, "Path must point to an existing directory!");
				}

				return ValidationResult.ValidResult;
			}

			return new ValidationResult(false, "Value is not a string!");
		}
	}
}
