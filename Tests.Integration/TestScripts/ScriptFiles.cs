using System.IO;

namespace Tests.Integration.TestScripts
{
    /// <summary>
    /// Contains references to test script files.
    /// </summary>
	public static class ScriptFiles
	{
        public static readonly FileInfo HasMultipleTests = 
            new FileInfo(@".\TestScripts\HasMultipleTests.ps1");
        public static readonly FileInfo HasNonFunctionErrors = 
            new FileInfo(@".\TestScripts\HasNonFunctionErrors.ps1");
        public static readonly FileInfo HasOneTest = 
            new FileInfo(@".\TestScripts\HasOneTest.ps1");
        public static readonly FileInfo HasScriptReferences = 
            new FileInfo(@".\TestScripts\HasScriptReferences.ps1");
        public static readonly FileInfo HasSetupAndCleanup = 
            new FileInfo(@".\TestScripts\HasSetupAndCleanup.ps1");
        public static readonly FileInfo HasSkippedTest = 
            new FileInfo(@".\TestScripts\HasSkippedTest.ps1");
        public static readonly FileInfo Helpers = 
            new FileInfo(@".\TestScripts\Helpers.ps1");
	}
}