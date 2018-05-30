using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HQ.Util.General
{
	public static class EnumUtil
	{
		// ******************************************************************
		// EO: Extracted from Andrew Smith article on "Accessing Enum members in Xaml"
		// Return null if no description attibute provided
		public static string GetEnumDescription(Object enumValue)
		{
			return GetEnumDescription(enumValue, false);
		}

		// ******************************************************************
		public static bool IsEnumHasFlagAttribute(Object enumValue)
		{
			return IsEnumHasFlagAttribute(enumValue.GetType());
		}

		// ******************************************************************
		public static bool IsEnumHasFlagAttribute(Type enumType)
		{
			object[] flags = enumType.GetCustomAttributes(typeof(FlagsAttribute), false);
			if (flags.Length == 0)
			{
				return false;
			}

			return true;
		}

		// ******************************************************************
		const string enumSeparator = " | ";

		/// <summary>
		/// Get any enum attribute description in this order (enum with Flags attribute supported):
		/// 1 - From attribute "LocalizableDescriptionAttribute" where it is searching up the resources
		/// 2 - From attribute "DescriptionAttribute" where it is searching up the direct string
		/// 3 - The name of the attribute
		/// </summary>
		/// <param name="enumValue"></param>
		/// <param name="returnToStringIfNotFound"></param>
		/// <returns></returns>
		public static string GetEnumDescription(Object enumValue, bool returnToStringIfNotFound)
		{
			string description = "";

			if (IsEnumHasFlagAttribute(enumValue))
			{
				bool firstValue = true;
				foreach (object val in Enum.GetValues(enumValue.GetType()))
				{
					if (((int)enumValue != 0 && ((int)val == 0)))
					{
						continue;
					}

					if ((((int)enumValue) & ((int)val)).Equals((int)val))
					{
						if (!firstValue)
						{
							description += enumSeparator;
						}
						else
						{
							firstValue = false;
						}

						object[] attribs = null;
						attribs =
							val.GetType()
								.GetField(val.ToString())
								.GetCustomAttributes(typeof(LocalizableDescriptionAttribute), false);
						if (attribs.Length > 0)
						{
							description += ((LocalizableDescriptionAttribute)attribs[0]).Description;
							continue;
						}

						attribs =
							val.GetType()
								.GetField(val.ToString())
								.GetCustomAttributes(typeof(DescriptionAttribute), false);
						if (attribs.Length > 0)
						{
							attribs =
								val.GetType()
									.GetField(val.ToString())
									.GetCustomAttributes(typeof(DescriptionAttribute), false);
							description += ((DescriptionAttribute)attribs[0]).Description;
						}
						else
						{
							description += Enum.Format(val.GetType(), val, "G");
						}
					}
				}
			}
			else
			{
				foreach (object val in Enum.GetValues(enumValue.GetType()))
				{
					if (enumValue.Equals(val))
					{
						object[] attribs = null;
						attribs =
							val.GetType()
								.GetField(val.ToString())
								.GetCustomAttributes(typeof(LocalizableDescriptionAttribute), false);
						if (attribs.Length > 0)
						{
							description = ((LocalizableDescriptionAttribute)attribs[0]).Description;
							break;
						}

						attribs =
							val.GetType()
								.GetField(val.ToString())
								.GetCustomAttributes(typeof(DescriptionAttribute), false);
						if (attribs.Length > 0)
						{
							description = ((DescriptionAttribute)attribs[0]).Description;
							break;
						}

						description = Enum.Format(val.GetType(), val, "G");

						break;
					}
				}
			}

			return description;
		}

		// **************************************************************
		/// <summary>
		/// Works for enum with and without FlagsAttribute. Work in conjunction with ConvertEnumValueToString (the opposite)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumValueName"></param>
		/// <returns></returns>
		public static T ConvertStringToEnumValue<T>(string enumValueName) where T : struct
		{
			return (T)Enum.Parse(typeof(T), enumValueName);
		}

		// **************************************************************
		/// <summary>
		/// Works for enum with and without FlagsAttribute. Work in conjunction with ConvertStringToEnumValue (the opposite)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumValueName"></param>
		/// <returns></returns>
		public static string ConvertEnumValueToString<T>(T enumValue)
		{
			return Enum.Format(typeof(T), enumValue, "G");
		}

		//// **************************************************************
		//public static bool HasAnyFlags(object t1, object t2)
		//{
		//    if (t1.GetType().IsEnum && t2.GetType().IsEnum)
		//    {
		//        return (((uint)t1) & ((uint)t2)) > 0;
		//    }

		//    throw new ArgumentException("Paremeters should be enum values");
		//}

		// **************************************************************
		public static bool HasAnyFlags<T>(T value, T lookingForFlag) where T : struct
		{
			int intValue = (int)(object)value;
			int intLookingForFlag = (int)(object)lookingForFlag;
			return (intValue & intLookingForFlag) > 0;
		}

		// **************************************************************
		/// <summary>
		/// Return an enumerators of input flag(s)
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetFlags<T>(this T input)
		{
			foreach (Enum value in Enum.GetValues(input.GetType()))
			{
				if ((int)(object)value != 0) // Just in case somebody has defined an enum with 0.
				{
					if (((Enum)(object)input).HasFlag(value))
						yield return (T)(object)value;
				}
			}
		}

		// **************************************************************
		public static IEnumerable<T> GetFlags<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		// **************************************************************
		/// <summary>
		/// Generic method to remove bit(s) from the first value. Will perform an OR.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="bitsToAdd"></param>
		/// <returns></returns>
		public static Enum GenericAddBits(Enum a, Enum bitsToAdd)
		{
			// consider adding argument validation here

			if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
				return (Enum)Enum.ToObject(a.GetType(), Convert.ToInt64(a) | Convert.ToInt64(bitsToAdd));
			else
				return (Enum)Enum.ToObject(a.GetType(), Convert.ToUInt64(a) | Convert.ToUInt64(bitsToAdd));
		}

		// **************************************************************
		/// <summary>
		/// Generic method to remove bit(s) from the first value. Will perform an AND ~.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="bitToRemove"></param>
		/// <returns></returns>
		public static Enum GenericRemoveBits(Enum a, Enum bitToRemove)
		{
			// consider adding argument validation here

			if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
				return (Enum)Enum.ToObject(a.GetType(), Convert.ToInt64(a) & ~ Convert.ToInt64(bitToRemove));
			else
				return (Enum)Enum.ToObject(a.GetType(), Convert.ToUInt64(a) & ~ Convert.ToUInt64(bitToRemove));
		}

		// **************************************************************
		public static bool GenericHasFlag(Enum a, Enum flag)
		{
			if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
				return (Convert.ToInt64(a) & Convert.ToInt64(flag)) > 0;

			return (Convert.ToUInt64(a) & Convert.ToUInt64(flag)) > 0;
		}

		// **************************************************************
	}
}
