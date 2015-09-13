using AIMS.Libraries.Scripting.ScriptControl.Project;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace AIMS.Libraries.Scripting.ScriptControl.Converter
{
	internal class ConversionEventHandler : ITypeLibImporterNotifySink
	{
		private TlbImp Parent = null;

		public ConversionEventHandler(TlbImp parent)
		{
			this.Parent = parent;
		}

		public void ReportEvent(ImporterEventKind eventKind, int eventCode, string eventMsg)
		{
			this.Parent.InvokeReportEvent(new ReportEventEventArgs(eventKind, eventCode, eventMsg));
		}

		public Assembly ResolveRef(object typeLib)
		{
			ITypeLib tLib = typeLib as ITypeLib;
			Assembly result;
			try
			{
				string tLibname = Marshal.GetTypeLibName(tLib);
				IntPtr ppTlibAttri = IntPtr.Zero;
				tLib.GetLibAttr(out ppTlibAttri);
				string guid = ((System.Runtime.InteropServices.ComTypes.TYPELIBATTR)Marshal.PtrToStructure(ppTlibAttri, typeof(System.Runtime.InteropServices.ComTypes.TYPELIBATTR))).guid.ToString();
				RegistryKey typeLibsKey = Registry.ClassesRoot.OpenSubKey("TypeLib\\{" + guid + "}");
				TypeLibrary typeLibrary = TypeLibrary.Create(typeLibsKey);
				TlbImp importer = new TlbImp(this.Parent.References);
				string outputFolder = Path.GetDirectoryName(this.Parent.AsmPath);
				string interopFileName = Path.Combine(outputFolder, "Interop." + typeLibrary.Name + ".dll");
				string asmPath = interopFileName;
				ResolveRefEventArgs t = new ResolveRefEventArgs("Reference Interop '" + Path.GetFileName(asmPath) + "' created succesfully.");
				this.Parent.InvokeResolveRef(t);
				this.Parent.References.Add(new ComReferenceProjectItem(ScriptControl.GetProject(), typeLibrary));
				result = importer.Import(interopFileName, typeLibrary.Path, typeLibrary.Name, this.Parent);
			}
			catch (Exception Ex)
			{
				ResolveRefEventArgs t = new ResolveRefEventArgs(Ex.Message);
				this.Parent.InvokeResolveRef(t);
				result = null;
			}
			finally
			{
				Marshal.ReleaseComObject(tLib);
			}
			return result;
		}
	}
}
