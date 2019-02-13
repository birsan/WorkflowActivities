using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyCompany("UiPath")]
[assembly: AssemblyProduct("UiPath")]
[assembly: AssemblyCopyright("Copyright © UiPath")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: XmlnsPrefix("http://schemas.uipath.com/workflow/activities", "ui")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyTitle("UiPath.FlowProfiler.Activities")]
[assembly: AssemblyDescription("FlowProfiler Activities Pack")]
[assembly: AssemblyConfiguration("")]
[assembly: XmlnsDefinition("http://schemas.uipath.com/workflow/activities/intelligentocr", "UiPath.IntelligentOCR.Activities.Design")]
[assembly: XmlnsDefinition("http://schemas.uipath.com/workflow/activities/intelligentocr/fc", "UiPath.IntelligentOCR.Activities.Design.FC")]


// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2e738cc7-032a-43d7-8ac8-94ed3d295698")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
#if DEBUG
[assembly: AssemblyVersion("1.4.*")]
#else
[assembly: AssemblyVersion("1.4.0")]
#endif
