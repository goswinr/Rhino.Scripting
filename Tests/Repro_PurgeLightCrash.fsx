// Reproduces a Rhino 9 crash observed after purging a light from a scratch document.
// This can terminate Rhino without warning. Save all work and use an empty Rhino session.
// Run via Caddis in UI mode, in two separate calls:
//   #load @"D:\Git\_Rhino_\Rhino.Scripting\Tests\Repro_PurgeLightCrash.fsx"
//   Repro_PurgeLightCrash.poisonDocument ()
//   Repro_PurgeLightCrash.createNextDocument ()
// The last call can terminate Rhino in RhinoDoc.Create. No Rhino.Scripting DLL is used.
// Rhino 9.0.26300.9401 / .NET 10.0.14: purge plus disposal reproduced twice;
// deleting instead and then creating a document succeeded. Enumeration is not required.
// A control run can pass false to closeDocument, which deletes instead of purging.
#r "RhinoCommon"

open Rhino
open Rhino.Geometry
open Rhino.DocObjects

let closeDocumentWithEnumeration purge enumerate =
    let doc = RhinoDoc.Create(null)
    printfn "created scratch document %u" doc.RuntimeSerialNumber
    let addLight x =
        let light = new Light()
        light.LightStyle <- LightStyle.WorldPoint
        light.Location <- Point3d(x, 0., 0.)
        let i = doc.Lights.Add(light)
        doc.Lights.[i].Id
    let a = addLight 0.
    let b = addLight 1.
    let obj = doc.Objects.FindId(a)
    let ok = if purge then doc.Objects.Purge(obj) else doc.Objects.Delete(obj, true)
    printfn "%s first light: %b; second %A; light table count %d" (if purge then "purged" else "deleted") ok b doc.Lights.Count
    // Match LightObjects' table iteration, observing any managed exception.
    if enumerate then
        try
            for i in 0 .. doc.Lights.Count - 1 do
                let light = doc.Lights.[i]
                printfn "slot %d deleted=%b" i light.IsDeleted
        with e -> printfn "enumeration threw %s: %s" (e.GetType().FullName) e.Message
    doc.Modified <- false
    doc.Dispose()
    printfn "disposed scratch document"

let closeDocument purge = closeDocumentWithEnumeration purge true
let poisonDocument () = closeDocumentWithEnumeration true false

let createNextDocument () =
    printfn "creating next document"
    let doc = RhinoDoc.Create(null)
    printfn "created next document %u" doc.RuntimeSerialNumber
    doc.Modified <- false
    doc.Dispose()
    printfn "disposed next document"
