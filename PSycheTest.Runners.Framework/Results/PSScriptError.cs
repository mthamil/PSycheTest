using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PSycheTest.Runners.Framework.Utilities.Text;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// Contains information about an error in a PowerShell script.
	/// </summary>
	public class PSScriptError : ScriptError
	{
		/// <summary>
		/// Initializes a new <see cref="PSScriptError"/>.
		/// </summary>
		/// <param name="errorRecord">A PowerShell script error record</param>
		/// <param name="testScriptFile">The source script file</param>
		public PSScriptError(IErrorRecord errorRecord, FileInfo testScriptFile)
		{
			_errorRecord = errorRecord;
			_testScriptFile = testScriptFile;
			_scriptStackTrace = errorRecord.ScriptStackTrace
				.Lines()
				.Where(line => !String.IsNullOrWhiteSpace(line))
				.Select(line => new ScriptStackFrame(line))
				.ToList();
		}

		/// <see cref="ScriptError.StackTrace"/>
		public override string Message
		{
			get { return _errorRecord.Exception.Message; }
		}

		/// <see cref="ScriptError.StackTrace"/>
		public override string StackTrace
		{
			get
			{
				var formattedStackTrace = _scriptStackTrace.Select(stackFrame => FormatScriptStackFrame(stackFrame, _testScriptFile));
				return  String.Format("{1}{0}{2}",
				                     Environment.NewLine,
									 FilterStackTrace(base.StackTrace),
				                     String.Join(Environment.NewLine, formattedStackTrace));
			}
		}

		private static string FormatScriptStackFrame(ScriptStackFrame stackFrame, FileInfo testScriptFile)
		{
			var file = stackFrame.File ?? testScriptFile;
			return String.Format(StackFrameFormat,
								 file.Name,
								 String.IsNullOrEmpty(stackFrame.Function) ? string.Empty : String.Format(":{0}()", stackFrame.Function),
								 file.FullName,
			                     stackFrame.Line);
		}

		/// <see cref="ScriptError.FilterStackTrace"/>
		protected override string FilterStackTrace(string stackTrace)
		{
			var filteredStack = stackTrace
				.Lines()
				.TakeWhile(line => !_stackFilterPrefixes.Any(line.StartsWith));

			return String.Join(Environment.NewLine, filteredStack);
		}

		/// <see cref="ScriptError.Exception"/>
		public override Exception Exception
		{
			get { return _errorRecord.Exception; }
		}

		private readonly IErrorRecord _errorRecord;
		private readonly FileInfo _testScriptFile;
		private readonly IEnumerable<ScriptStackFrame> _scriptStackTrace;

		private const string StackFrameFormat = "at {0}{1} in {2}:line {3}";	// 0 = file name, 1 = function name, 2 = full file path, 3 = line number

		private static readonly IEnumerable<string> _stackFilterPrefixes = new List<string>
		{
			String.Format("at {0}.Assert", typeof(AssertionCmdletBase).Namespace) 
		};

		private class ScriptStackFrame
		{
			public ScriptStackFrame(string stackFrame)
			{
				int commaIndex = stackFrame.IndexOf(",");
				var functionName = stackFrame.Substring(3, commaIndex - 3);
				Function = functionName == ScriptBlockPlaceHolder ? null : functionName;

				int lineDelimiterIndex = stackFrame.IndexOf(": line ");
				var fileName = stackFrame.Substring(commaIndex + 1, lineDelimiterIndex - commaIndex - 1).Trim();
				File = fileName == NoFilePlaceholder ? null : new FileInfo(fileName);

				Line = Int32.Parse(stackFrame.Substring(lineDelimiterIndex + 7));
			}

			/// <summary>
			/// The name of the function or null if error did not occur in a function.
			/// </summary>
			public string Function { get; private set; }

			/// <summary>
			/// The script file or null if the file is not available.
			/// </summary>
			public FileInfo File { get; private set; }

			/// <summary>
			/// The line number.
			/// </summary>
			public int Line { get; private set; }

			private const string ScriptBlockPlaceHolder = "<ScriptBlock>";
			private const string NoFilePlaceholder = "<No file>";
		}
	}
}