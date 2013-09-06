using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Provides information about an <see cref="IVsProject"/>.
	/// </summary>
	internal class ProjectInfo : IProjectInfo
	{
		/// <summary>
		/// Initializes a new <see cref="ProjectInfo"/>.
		/// </summary>
		/// <param name="project">The project to provide information about</param>
		public ProjectInfo(IVsProject project)
		{
			_project = project;
		}

		/// <see cref="IProjectInfo.Name"/>
		public string Name
		{
			get
			{
				string name = (string)GetProjectPropertyValue(__VSHPROPID.VSHPROPID_Name);
				return name;
			}
		}

		/// <see cref="IProjectInfo.File"/>
		public FileInfo File
		{
			get
			{
				string projectFile;
				ErrorHandler.ThrowOnFailure(_project.GetMkDocument(VSConstants.VSITEMID_ROOT, out projectFile));
				return new FileInfo(projectFile);
			}
		}

		/// <see cref="IProjectInfo.IsPhysicalProject"/>
		public bool IsPhysicalProject 
		{
			get
			{
				var projectType = GetProjectPropertyValue(__VSHPROPID.VSHPROPID_TypeName);
				return projectType != null; 
			}
		}

		/// <see cref="IProjectInfo.GetProjectItems"/>
		public IEnumerable<string> GetProjectItems()
		{
			return GetProjectItems((IVsHierarchy)_project, VSConstants.VSITEMID_ROOT);
		}

		private static IEnumerable<string> GetProjectItems(IVsHierarchy hierarchyItem, uint itemId)
		{
			object propertyValue = GetPropertyValue(hierarchyItem, (int)__VSHPROPID.VSHPROPID_FirstChild, itemId);

			uint childId = GetItemId(propertyValue);
			while (childId != VSConstants.VSITEMID_NIL)
			{
				string childPath = GetCanonicalName(hierarchyItem, childId);
				yield return childPath;

				foreach (var childNodePath in GetProjectItems(hierarchyItem, childId))
					yield return childNodePath;

				propertyValue = GetPropertyValue(hierarchyItem, (int)__VSHPROPID.VSHPROPID_NextSibling, childId);
				childId = GetItemId(propertyValue);
			}
		}

		private object GetProjectPropertyValue(__VSHPROPID property)
		{
			return GetPropertyValue((IVsHierarchy)_project, (int)property, VSConstants.VSITEMID_ROOT);
		}

		private static uint GetItemId(object pvar)
		{
			if (pvar == null)
				return VSConstants.VSITEMID_NIL;
			if (pvar is int)
				return (uint)(int)pvar;
			if (pvar is uint)
				return (uint)pvar;
			if (pvar is short)
				return (uint)(short)pvar;
			if (pvar is ushort)
				return (ushort)pvar;
			if (pvar is long)
				return (uint)(long)pvar;
			return VSConstants.VSITEMID_NIL;
		}

		private static object GetPropertyValue(IVsHierarchy vsHierarchy, int propid, uint itemId)
		{
			if (itemId == VSConstants.VSITEMID_NIL)
			{
				return null;
			}

			try
			{
				object o;
				ErrorHandler.ThrowOnFailure(vsHierarchy.GetProperty(itemId, propid, out o));
				return o;
			}
			catch (NotImplementedException)
			{
				return null;
			}
			catch (COMException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
		}

		private static string GetCanonicalName(IVsHierarchy hierarchy, uint itemId)
		{
			string strRet = string.Empty;
			int hr = hierarchy.GetCanonicalName(itemId, out strRet);

			if (hr == VSConstants.E_NOTIMPL)
			{
				// Special case E_NOTIMLP to avoid perf hit to throw an exception.
				return string.Empty;
			}

			try
			{
				ErrorHandler.ThrowOnFailure(hr);
			}
			catch (COMException)
			{
				strRet = string.Empty;
			}

			// This could be in the case of S_OK, S_FALSE, etc.
			return strRet;
		}

		private readonly IVsProject _project;
	}
}