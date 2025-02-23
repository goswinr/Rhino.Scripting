#r "nuget: FSharp.Core, 6.0.7"// dependency of Rhino.Scripting
#r "nuget: FsEx, 0.16.0" // dependency of Rhino.Scripting
#r "nuget: Rhino.Scripting, 0.8.2"

using rs = Rhino.Scripting.RhinoScriptSyntax;

var pt =  rs.AddPoint(2.0, 2.0, 2.0);
rs.ObjectLayer(pt, "some new layer", true) ; // createLayerIfMissing=true

var ob =  rs.GetObject("Select an Object");
rs.ObjectColor(ob, System.Drawing.Color.Blue);


System.Console.WriteLine("Hello Rhino.Scripting");






