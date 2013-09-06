$initialValue = 0

function Setup
{
    [PSycheTest.TestSetup()]
	param()

    $script:initialValue = $initialValue + 10
}

function Test-First
{
    [PSycheTest.Test()]
	param()

	Assert-Equal 10 $initialValue
}

function Test-Second
{
    [PSycheTest.Test()]
	param()

	Assert-Equal 10 $initialValue
}

function Cleanup
{
    [PSycheTest.TestCleanup()]
	param()

    $script:initialValue = 20
}

