function Test-ShouldBeSkipped
{
    [PSycheTest.Test(Title = "Should be skipped", SkipBecause = "I said so.")]
	param()

	Assert-True $false
}

function Test-OneEqualsOne
{
    [PSycheTest.Test(Title = "Test 1 = 1")]
	param()

	Assert-True (1 -eq 1)
    Assert-Equal 1 1
}
