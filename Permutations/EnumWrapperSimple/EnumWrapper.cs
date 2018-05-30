using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using HQ.Util.General;

namespace Permutations.EnumWrapperSimple
{
	// Declare an instance of this class in the ViewModel to wrap any enum
	public class EnumWrapperSource
	{
		// ******************************************************************
		private ObservableCollection<EnumWrapperItem> _items;
		private INotifyPropertyChanged _owner;
		private string _enumPropName;
		private bool _isEnumFlag = false;
		private Enum _enumValue;

		// ******************************************************************
		public IReadOnlyList<EnumWrapperItem> Items // INotifyCollectionChange will be discover through asking the interface
		{
			get { return _items; }
		}

		// ******************************************************************
		// The enum property does not need to notifyPropertyChange. The wrapper will advise.
		public EnumWrapperSource(INotifyPropertyChanged owner, [CallerMemberName] string enumPropName = null)
		{
			_owner = owner;
			_enumPropName = enumPropName;
			_enumValue = null;

			if (_owner != null && !string.IsNullOrEmpty(_enumPropName))
			{
				var pi = _owner.GetType().GetProperty(_enumPropName);
				if (pi != null)
				{
					_enumValue = pi.GetValue(_owner) as Enum;
				}
			}

			if (_enumValue == null)
			{
				throw new ArgumentException("Unable to find the enum");
			}

			var flagAttribute = _enumValue.GetType().GetCustomAttribute<FlagsAttribute>();
			if (flagAttribute != null)
			{
				_isEnumFlag = true;
			}
			
			_items = new ObservableCollection<EnumWrapperItem>();

			foreach (Enum val in System.Enum.GetValues(_enumValue.GetType()))
			{
				string name = val.ToString();
				string description = null;

				var decriptionAttribute = val.GetType().GetField(val.ToString()).GetCustomAttribute<DescriptionAttribute>();
				if (decriptionAttribute != null)
				{
					description = decriptionAttribute.Description;
				}
				else
				{
					description = name;
				}

				bool isInitialFlagOn = EnumUtil.GenericHasFlag(_enumValue, val);
				
				var enumItemWrapper = new EnumWrapperItem(this, val, name, description, isInitialFlagOn);
				_items.Add(enumItemWrapper);

				enumItemWrapper.PropertyChanged += EnumWrapperItemOnPropertyChanged;
			}
		}

		// ******************************************************************
		private bool _isCurrentlyUpdatingUserModification = false; // To prevent re-entrance. Only one check at a time is supported.
		
		private void EnumWrapperItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (! _isCurrentlyUpdatingUserModification)
			{
				_isCurrentlyUpdatingUserModification = true;

				EnumWrapperItem item = sender as EnumWrapperItem;
				if (item != null)
				{
					if (e.PropertyName == nameof(EnumWrapperItem.IsChecked))
					{
						if (_isEnumFlag)
						{
							if (item.IsChecked)
							{
								_enumValue = EnumUtil.GenericAddBits(_enumValue, item.EnumValue);
							}
							else
							{
								_enumValue = EnumUtil.GenericRemoveBits(_enumValue, item.EnumValue);
							}
						}
						else
						{
							if (item.IsChecked)
							{
								_enumValue = item.EnumValue;
							}

							foreach (var itemIter in _items)
							{
								if (!itemIter.EnumValue.Equals(item.EnumValue))
								{
									itemIter.IsChecked = false;
								}
							}
						}
					}

					try
					{
						_owner.GetType().InvokeMember(_enumPropName,
							BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.SetProperty,
							null, _owner, new[] {_enumValue});
					}
					catch (Exception ex)
					{
						Debug.Print(ex.ToString());
					}
				}
			}
			_isCurrentlyUpdatingUserModification = false;

		}

		// ******************************************************************

	}
}