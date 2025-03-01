#r "nuget: System.Windows.Forms"
#r "nuget: System.Drawing.Common"
open System


type cur = Windows.Forms.Cursor
printfn $"actual screenPoint: {Windows.Forms.Cursor.Position}"

let screenPoint =
    let cursorType = Type.GetType "System.Windows.Forms.Cursor, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    // let cursorType = Type.GetType("System.Windows.Forms.Cursor")
    match cursorType with
    | null -> failwithf "Could not load System.Windows.Forms.Cursor type"
    | t ->
        let positionProperty = t.GetProperty "Position"
        match positionProperty with
        | null -> failwithf "Could not find Position property on Cursor type"
        | p -> p.GetValue(null) :?> Drawing.Point





printfn $"screenPoint via Reflection: {screenPoint}"