function Test-AssertTrue-Failure
{
    [PSycheTest.Test(Title = "Test Assert-True Failure")]
	param()

	Assert-True $false
}

function Test-AssertFalse-Failure
{
    [PSycheTest.Test(Title = "Test Assert-False Failure")]
	param()

	Assert-False $true
}

function Test-AssertTrue-Success
{
    [PSycheTest.Test(Title = "Test Assert-True Success")]
	param()

	Assert-True $true
}

function Test-AssertFalse-Success
{
    [PSycheTest.Test(Title = "Test Assert-False Success")]
	param()

	Assert-False $false
}
