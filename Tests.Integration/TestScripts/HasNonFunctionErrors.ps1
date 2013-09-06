Assert-True $false

function Test-ShouldPass1
{
    [PSycheTest.Test()]
	param()

	Assert-True true
}

function Test-ShouldPass2
{
    [PSycheTest.Test()]
	param()

	Assert-True $true
}