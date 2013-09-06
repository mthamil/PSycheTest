function Test-AssertTrue-Failure
{
    [PSycheTest.Test(Title = "Test!")]
	param()

    $value = Get-False
	Assert-True $value
}

function Get-False
{
    $false
}
