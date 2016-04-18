﻿namespace FuManchu.Tests.Operators
{
	using Xunit;

	/// <summary>
	/// Provides tests for the <see cref="NegatedEqualityOperator"/>
	/// </summary>
	public class NegatedEqualityOperatorFacts : OperatorFactsBase
	{
		public NegatedEqualityOperatorFacts() : base(new NegatedEqualityOperator()) { }

		[Fact]
		public void PassingTwoNullValues_ShouldBeFalse()
		{
			RunTest(null, null, false);
		}

		[Fact]
		public void PassingOneNullValue_ShouldBeTrue()
		{
			RunTest(true, null, true);
		}

		[Fact]
		public void PassingSameValues_ShouldBeFalse()
		{
			RunTest(true, true, false);
		}

		[Fact]
		public void PassingDifferentValues_ShouldBeTrue()
		{
			RunTest(true, false, true);
		}

		[Fact]
		public void PassingDifferentTypes_WillUseTypeConversion()
		{
			RunTest(true, "true", false);
		}

		[Fact]
		public void PassingDifferentTypes_WithNonConvertableValues_ShouldBeTrue()
		{
			RunTest(true, "hello", true);
		}
	}
}