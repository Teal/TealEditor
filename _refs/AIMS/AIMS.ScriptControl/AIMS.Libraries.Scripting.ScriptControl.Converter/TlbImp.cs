using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace AIMS.Libraries.Scripting.ScriptControl.Converter
{
	public class TlbImp
	{
		public enum REGKIND
		{
			REGKIND_DEFAULT,
			REGKIND_REGISTER,
			REGKIND_NONE
		}

		private enum RegKind
		{
			RegKind_Default,
			RegKind_Register,
			RegKind_None
		}

		public string AsmPath = "";

		public ArrayList References;

		public event EventHandler<ReportEventEventArgs> ReportEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ReportEvent = (EventHandler<ReportEventEventArgs>)Delegate.Combine(this.ReportEvent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ReportEvent = (EventHandler<ReportEventEventArgs>)Delegate.Remove(this.ReportEvent, value);
			}
		}

		public event EventHandler<ResolveRefEventArgs> ResolveRef
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ResolveRef = (EventHandler<ResolveRefEventArgs>)Delegate.Combine(this.ResolveRef, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ResolveRef = (EventHandler<ResolveRefEventArgs>)Delegate.Remove(this.ResolveRef, value);
			}
		}

		[DllImport("oleaut32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
		private static extern void LoadTypeLibEx(string strTypeLibName, TlbImp.RegKind regKind, out ITypeLib typeLib);

		protected virtual void OnReportEvent(ReportEventEventArgs e)
		{
			if (this.ReportEvent != null)
			{
				this.ReportEvent(this, e);
			}
		}

		protected virtual void OnResolveRef(ResolveRefEventArgs e)
		{
			if (this.ResolveRef != null)
			{
				this.ResolveRef(this, e);
			}
		}

		public void InvokeResolveRef(ResolveRefEventArgs e)
		{
			this.OnResolveRef(e);
		}

		public void InvokeReportEvent(ReportEventEventArgs e)
		{
			this.OnReportEvent(e);
		}

		public TlbImp(ArrayList references)
		{
			this.References = references;
		}

		public Assembly Import(string InteropFileName, string path, string name)
		{
			return this.Import(InteropFileName, path, name, null);
		}

		public Assembly Import(string InteropFileName, string path, string name, TlbImp parent)
		{
			this.AsmPath = Path.GetDirectoryName(InteropFileName);
			ITypeLib typeLib;
			TlbImp.LoadTypeLibEx(path, TlbImp.RegKind.RegKind_None, out typeLib);
			Assembly result;
			if (typeLib == null)
			{
				result = null;
			}
			else
			{
				TypeLibConverter converter = new TypeLibConverter();
				ConversionEventHandler eventHandler = new ConversionEventHandler((parent == null) ? this : parent);
				AssemblyBuilder asm = converter.ConvertTypeLibToAssembly(typeLib, Path.GetFileName(InteropFileName), TypeLibImporterFlags.None, eventHandler, null, null, name, null);
				string outputFolder = Path.GetDirectoryName(InteropFileName);
				string interopFName = Path.Combine(outputFolder, "Interop." + name + ".dll");
				asm.Save(Path.GetFileName(interopFName));
				Marshal.ReleaseComObject(typeLib);
				result = asm;
			}
			return result;
		}
	}
}
