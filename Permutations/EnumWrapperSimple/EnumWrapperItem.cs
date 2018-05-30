using System;

namespace Permutations.EnumWrapperSimple
{
	public class EnumWrapperItem : NotifyPropertyChangeBase
	{
		// ************************************************************************
		private EnumWrapperSource _enumWrapperSource;
		private Enum _enumValue;
		private bool _isChecked;

		// ************************************************************************
		public EnumWrapperItem(EnumWrapperSource enumWrapperSource, Enum enumValue, string name, string description, bool isInitialChecked)
		{
			_enumWrapperSource = enumWrapperSource;
			_enumValue = enumValue;
			Name = name;
			Description = description;
			_isChecked = isInitialChecked;
		}

		//// ************************************************************************
		public string Name { get; private set; }
		public string Description { get; private set; }

		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				if (value == _isChecked) return;
				_isChecked = value;
				RaisePropertyChanged();
			}
		}

		// ************************************************************************
		public Enum EnumValue
		{
			get { return _enumValue; }
		}

		// ************************************************************************
		public string DescriptionOrName
		{
			get
			{
				if (String.IsNullOrEmpty(Description))
				{
					return Name;
				}

				return Description;
			}
		}

		// ************************************************************************

	}
}
