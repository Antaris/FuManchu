namespace FuManchu.Binding
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Represents information about template data.
	/// </summary>
	public class TemplateDataInfo
	{
		private object _value;
		private Func<object> _valueAccessor;

		/// <summary>
		/// Initialises a new instance of <see cref="TemplateDataInfo"/>
		/// </summary>
		/// <param name="container">The container instance.</param>
		/// <param name="value">The value.</param>
		public TemplateDataInfo(object container, object value)
		{
			Container = container;
			_value = value;
		}

		/// <summary>
		/// Initialises a new instance of <see cref="TemplateDataInfo"/>
		/// </summary>
		/// <param name="container">The container instance.</param>
		/// <param name="propertyInfo">The property information.</param>
		/// <param name="valueAccessor">The value accessor.</param>
		public TemplateDataInfo(object container, PropertyInfo propertyInfo, Func<object> valueAccessor)
		{
			Container = container;
			PropertyInfo = propertyInfo;
			_valueAccessor = valueAccessor;
		}

		/// <summary>
		/// Gets the container instance.
		/// </summary>
		public object Container { get; private set; }

		/// <summary>
		/// Gets the property information.
		/// </summary>
		public PropertyInfo PropertyInfo { get; private set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public object Value
		{
			get
			{
				if (_valueAccessor != null)
				{
					_value = _valueAccessor();
					_valueAccessor = null;
				}

				return _value;
			}
			set
			{
				_value = value;
				_valueAccessor = null;
			}
		}
	}
}