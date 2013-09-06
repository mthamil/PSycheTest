. .\Helpers.ps1

function Test-ScriptReference
{
    [PSycheTest.Test()]
	param()

	Assert-Equal "Hello, World!" $(Get-TestString)
}

