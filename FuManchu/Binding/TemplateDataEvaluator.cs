﻿namespace FuManchu.Binding
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Provides evaluation of dynamic expressions against template data.
	/// </summary>
	public static class TemplateDataEvaluator
	{
		public static TemplateDataInfo Eval(TemplateData templateData, string expression)
		{
			return EvalComplexExpression(templateData, expression);
		}

		public static TemplateDataInfo Eval(object indexableObject, string expression)
		{
			return (indexableObject == null) ? null : EvalComplexExpression(indexableObject, expression);
		}

		private static TemplateDataInfo EvalComplexExpression(object indexableObject, string expression)
		{
			foreach (var pair in GetRightToLeftExpressions(expression))
			{
				var subExpression = pair.Left;
				var postExpression = pair.Right;

				var subTargetInfo = GetPropertyValue(indexableObject, subExpression);
				if (subTargetInfo != null)
				{
					if (string.IsNullOrWhiteSpace(postExpression))
					{
						return subTargetInfo;
					}
					if (subTargetInfo.Value != null)
					{
						var potential = EvalComplexExpression(subTargetInfo.Value, postExpression);
						if (potential != null)
						{
							return potential;
						}
					}
				}
			}

			return null;
		}

		private static IEnumerable<ExpressionPair> GetRightToLeftExpressions(string expression)
		{
			yield return new ExpressionPair(expression, string.Empty);

			var lastDot = expression.LastIndexOf('.');

			var subExpression = expression;
			var postExpression = string.Empty;

			while (lastDot > -1)
			{
				subExpression = expression.Substring(0, lastDot);
				postExpression = expression.Substring(lastDot + 1);
				yield return new ExpressionPair(subExpression, postExpression);

				lastDot = subExpression.LastIndexOf('.');
			}
		}

		private static TemplateDataInfo GetIndexPropertyValue(object indexableObject, string key)
		{
			var dict = indexableObject as IDictionary<string, object>;
			object value = null;
			bool success = false;

			if (dict != null)
			{
				success = dict.TryGetValue(key, out value);
			}
			else
			{
				var tryDelegate = TryGetValueProvider.CreateInstance(indexableObject.GetType());
				if (tryDelegate != null)
				{
					success = tryDelegate(indexableObject, key, out value);
				}
			}

			if (success)
			{
				return new TemplateDataInfo(indexableObject, value);
			}

			return null;
		}

		private static TemplateDataInfo GetPropertyValue(object container, string propertyName)
		{
			var value = GetIndexPropertyValue(container, propertyName);
			if (value != null)
			{
				return value;
			}

			var templateData = container as TemplateData;
			if (templateData != null)
			{
				container = templateData.Model;
			}

			if (container == null)
			{
				return null;
			}

			var propertyInfo = container.GetType().GetRuntimeProperty(propertyName);
			if (propertyInfo == null)
			{
				return null;
			}

			return new TemplateDataInfo(container, propertyInfo, () => propertyInfo.GetValue(container));
		}

		private struct ExpressionPair
		{
			public readonly string Left;
			public readonly string Right;

			public ExpressionPair(string left, string right)
			{
				Left = left;
				Right = right;
			}
		}
	}
}