// Tests for Src/Scripting_Light.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Light"

let vec x y z = Vector3d(x, y, z)
let red = Drawing.Color.FromArgb(255, 0, 0)
let rgb (c:Drawing.Color) = (int c.R, int c.G, int c.B)
let lightGeo (id:Guid) = (doc().Objects.FindId id).Geometry :?> Light
let spot () = rs.AddSpotLight(pt 0. 0. 0., 5., pt 0. 0. 10.)
let rect () = rs.AddRectangularLight(pt 0. 0. 0., pt 4. 0. 0., pt 0. 3. 0.)

// ------------------------------- creating -------------------------------

test "AddPointLight" (fun () ->
    let id = rs.AddPointLight(pt 1. 2. 3.)
    ok "is a light" (rs.IsLight id)
    ok "is a point light" (rs.IsPointLight id)
    nearPt "location" (pt 1. 2. 3.) (rs.LightLocation id)
    ok "enabled by default" (rs.IsLightEnabled id)
    )

test "AddDirectionalLight" (fun () ->
    let id = rs.AddDirectionalLight(pt 0. 0. 10., pt 0. 0. 0.)
    ok "is a directional light" (rs.IsDirectionalLight id)
    nearPt "location is the start point" (pt 0. 0. 10.) (rs.LightLocation id)
    nearVec "direction is end minus start" (vec 0. 0. -10.) (rs.LightDirection id)
    )

test "AddSpotLight" (fun () ->
    let id = spot ()
    ok "is a spot light" (rs.IsSpotLight id)
    nearPt "location is the apex" (pt 0. 0. 10.) (rs.LightLocation id)
    nearVec "direction goes from the apex to the origin" (vec 0. 0. -10.) (rs.LightDirection id)
    near "radius of the cone" 5. (rs.SpotLightRadius id)
    near "default hardness" 0.5 (rs.SpotLightHardness id)
    let negative = rs.AddSpotLight(pt 0. 0. 0., -3., pt 0. 0. 10.)
    near "a negative radius becomes 1" 1. (rs.SpotLightRadius negative)
    )

test "AddRectangularLight" (fun () ->
    let id = rect ()
    ok "is a rectangular light" (rs.IsRectangularLight id)
    nearPt "location is the origin" (pt 0. 0. 0.) (rs.LightLocation id)
    let plane, length, width = rs.RectangularLightPlane id
    near "the plane is the XY plane (its normal is the cross product of the height and the width vector)" 1. (abs plane.Normal.Z)
    near "first length (the height vector)" 3. length
    near "second length (the width vector)" 4. width
    )

test "AddLinearLight" (fun () ->
    let id = rs.AddLinearLight(pt 0. 0. 0., pt 10. 0. 0., 0.2)
    ok "is a linear light" (rs.IsLinearLight id)
    nearPt "location is the start point" (pt 0. 0. 0.) (rs.LightLocation id)
    nearVec "direction" (vec 10. 0. 0.) (rs.LightDirection id)
    let g = lightGeo id
    near "given width" 0.2 g.Width.Length
    let limited = rs.AddLinearLight(pt 0. 0. 0., pt 10. 0. 0., 5.)
    near "width is limited to one twentieth of the length" 0.5 (lightGeo limited).Width.Length
    let zero = rs.AddLinearLight(pt 0. 0. 0., pt 10. 0. 0., 0.)
    near "an explicit zero width is kept" 0. (lightGeo zero).Width.Length
    // default width: half an inch in model units, limited to 1/20 of the length
    rs.UnitSystem 8
    let def = rs.AddLinearLight(pt 0. 0. 0., pt 100. 0. 0.)
    near "default width is half an inch (inch model units)" 0.5 (lightGeo def).Width.Length
    let short = rs.AddLinearLight(pt 0. 0. 0., pt 2. 0. 0.)
    near "default width is limited to 1/20 of the length" 0.1 (lightGeo short).Width.Length
    rs.UnitSystem 2
    let mm = rs.AddLinearLight(pt 0. 0. 0., pt 1000. 0. 0.)
    near "default width in millimeters" 12.7 (lightGeo mm).Width.Length
    )

// ------------------------------- lists and tests -------------------------------

test "LightCount" (fun () ->
    let n = rs.LightCount()
    rs.AddPointLight(pt 0. 0. 0.) |> ignore
    rs.AddPointLight(pt 1. 0. 0.) |> ignore
    eq "two more lights" (n + 2) (rs.LightCount())
    )

test "LightObjects" (fun () ->
    let a = rs.AddPointLight(pt 0. 0. 0.)
    let b = spot ()
    let ids = rs.LightObjects()
    ok "contains both lights" (ids.Contains a && ids.Contains b)
    rs.DeleteObject a |> ignore
    ok "deleted lights are not listed" (not ((rs.LightObjects()).Contains a))
    )

test "IsLight" (fun () ->
    let l = rs.AddPointLight(pt 0. 0. 0.)
    ok "light" (rs.IsLight l)
    ok "point is no light" (not (rs.IsLight (rs.AddPoint(0., 0., 0.))))
    ok "unknown id" (not (rs.IsLight (Guid.NewGuid())))
    ok "light geometry" (rs.IsLight (lightGeo l :> GeometryBase))
    ok "point geometry" (not (rs.IsLight (Point(pt 0. 0. 0.) :> GeometryBase)))
    )

/// Checks that exactly the light kind in the list is true
let kindTest (name:string) (byId: Guid -> bool) (byGeo: GeometryBase -> bool) (make: unit -> Guid) =
    let id = make ()
    ok (name + ": by id") (byId id)
    ok (name + ": by geometry") (byGeo (lightGeo id :> GeometryBase))
    let others = [ rs.AddPointLight(pt 0. 0. 0.); rs.AddDirectionalLight(pt 0. 0. 1., pt 0. 0. 0.); spot (); rect (); rs.AddLinearLight(pt 0. 0. 0., pt 5. 0. 0.) ]
    let sameKind = others |> List.filter byId
    ok (name + ": exactly one of the five other lights has this kind or none") (sameKind.Length <= 1)
    ok (name + ": a point object is false") (not (byId (rs.AddPoint(0., 0., 0.))))
    ok (name + ": an unknown id is false") (not (byId (Guid.NewGuid())))
    ok (name + ": point geometry is false") (not (byGeo (Point(pt 0. 0. 0.) :> GeometryBase)))
    ok (name + ": null geometry is false") (not (byGeo null))

test "IsPointLight" (fun () ->
    kindTest "point light" (fun id -> rs.IsPointLight id) (fun g -> rs.IsPointLight g) (fun () -> rs.AddPointLight(pt 0. 0. 0.))
    ok "spot light is no point light" (not (rs.IsPointLight (spot ())))
    )
test "IsDirectionalLight" (fun () ->
    kindTest "directional light" (fun id -> rs.IsDirectionalLight id) (fun g -> rs.IsDirectionalLight g) (fun () -> rs.AddDirectionalLight(pt 0. 0. 1., pt 0. 0. 0.))
    ok "point light is no directional light" (not (rs.IsDirectionalLight (rs.AddPointLight(pt 0. 0. 0.))))
    )
test "IsSpotLight" (fun () ->
    kindTest "spot light" (fun id -> rs.IsSpotLight id) (fun g -> rs.IsSpotLight g) spot
    ok "point light is no spot light" (not (rs.IsSpotLight (rs.AddPointLight(pt 0. 0. 0.))))
    )
test "IsRectangularLight" (fun () ->
    kindTest "rectangular light" (fun id -> rs.IsRectangularLight id) (fun g -> rs.IsRectangularLight g) rect
    ok "point light is no rectangular light" (not (rs.IsRectangularLight (rs.AddPointLight(pt 0. 0. 0.))))
    )
test "IsLinearLight" (fun () ->
    kindTest "linear light" (fun id -> rs.IsLinearLight id) (fun g -> rs.IsLinearLight g) (fun () -> rs.AddLinearLight(pt 0. 0. 0., pt 5. 0. 0.))
    ok "point light is no linear light" (not (rs.IsLinearLight (rs.AddPointLight(pt 0. 0. 0.))))
    )

test "IsLightReference" (fun () ->
    let l = rs.AddPointLight(pt 0. 0. 0.)
    ok "a light of the document is no reference" (not (rs.IsLightReference l))
    throwsRs "unknown id" (fun () -> rs.IsLightReference (Guid.NewGuid()))
    )

// ------------------------------- properties -------------------------------

test "EnableLight" (fun () ->
    let a = rs.AddPointLight(pt 0. 0. 0.)
    let b = rs.AddPointLight(pt 1. 0. 0.)
    ok "enabled by default" (rs.EnableLight a)
    rs.EnableLight(a, false)
    ok "disabled" (not (rs.EnableLight a))
    ok "IsLightEnabled agrees" (not (rs.IsLightEnabled a))
    rs.EnableLight(a, true)
    ok "enabled again" (rs.EnableLight a)
    rs.EnableLight([ a; b ], false)
    ok "multiple: first disabled" (not (rs.EnableLight a))
    ok "multiple: second disabled" (not (rs.EnableLight b))
    ok "from geometry" (not (rs.EnableLight (lightGeo a)))
    throws "unknown id, get" (fun () -> rs.EnableLight (Guid.NewGuid()))
    throws "unknown id, set" (fun () -> rs.EnableLight(Guid.NewGuid(), true))
    throws "not a light" (fun () -> rs.EnableLight (rs.AddPoint(0., 0., 0.)))
    )

test "IsLightEnabled" (fun () ->
    let a = rs.AddPointLight(pt 0. 0. 0.)
    ok "enabled" (rs.IsLightEnabled a)
    ok "enabled, geometry" (rs.IsLightEnabled (lightGeo a))
    rs.EnableLight(a, false)
    ok "disabled" (not (rs.IsLightEnabled a))
    ok "disabled, geometry" (not (rs.IsLightEnabled (lightGeo a)))
    throws "unknown id" (fun () -> rs.IsLightEnabled (Guid.NewGuid()))
    )

test "LightColor" (fun () ->
    let a = rs.AddPointLight(pt 0. 0. 0.)
    let b = rs.AddPointLight(pt 1. 0. 0.)
    rs.LightColor(a, red)
    eq "color round trip" (rgb red) (rgb (rs.LightColor a))
    eq "from geometry" (rgb red) (rgb (rs.LightColor (lightGeo a)))
    rs.LightColor([ a; b ], Drawing.Color.FromArgb(0, 0, 255))
    eq "multiple: first" (0, 0, 255) (rgb (rs.LightColor a))
    eq "multiple: second" (0, 0, 255) (rgb (rs.LightColor b))
    throws "unknown id, get" (fun () -> rs.LightColor (Guid.NewGuid()))
    throws "unknown id, set" (fun () -> rs.LightColor(Guid.NewGuid(), red))
    )

test "LightDirection" (fun () ->
    let a = spot ()
    let b = spot ()
    rs.LightDirection(a, vec 1. 0. 0.)
    nearVec "direction round trip" (vec 1. 0. 0.) (rs.LightDirection a)
    nearVec "from geometry" (vec 1. 0. 0.) (rs.LightDirection (lightGeo a))
    rs.LightDirection([ a; b ], vec 0. 1. 0.)
    nearVec "multiple: first" (vec 0. 1. 0.) (rs.LightDirection a)
    nearVec "multiple: second" (vec 0. 1. 0.) (rs.LightDirection b)
    throws "unknown id, get" (fun () -> rs.LightDirection (Guid.NewGuid()))
    throws "unknown id, set" (fun () -> rs.LightDirection(Guid.NewGuid(), vec 1. 0. 0.))
    )

test "LightLocation" (fun () ->
    let a = rs.AddPointLight(pt 0. 0. 0.)
    let b = rs.AddPointLight(pt 1. 0. 0.)
    rs.LightLocation(a, pt 5. 6. 7.)
    nearPt "location round trip" (pt 5. 6. 7.) (rs.LightLocation a)
    nearPt "from geometry" (pt 5. 6. 7.) (rs.LightLocation (lightGeo a))
    rs.LightLocation([ a; b ], pt 1. 1. 1.)
    nearPt "multiple: first" (pt 1. 1. 1.) (rs.LightLocation a)
    nearPt "multiple: second" (pt 1. 1. 1.) (rs.LightLocation b)
    throws "unknown id, get" (fun () -> rs.LightLocation (Guid.NewGuid()))
    throws "unknown id, set" (fun () -> rs.LightLocation(Guid.NewGuid(), pt 0. 0. 0.))
    )

test "LightName" (fun () ->
    let a = rs.AddPointLight(pt 0. 0. 0.)
    let b = rs.AddPointLight(pt 1. 0. 0.)
    rs.LightName(a, "my light")
    eq "name round trip" "my light" (rs.LightName a)
    eq "from geometry" "my light" (rs.LightName (lightGeo a))
    rs.LightName([ a; b ], "both")
    eq "multiple: first" "both" (rs.LightName a)
    eq "multiple: second" "both" (rs.LightName b)
    throws "unknown id, get" (fun () -> rs.LightName (Guid.NewGuid()))
    throws "unknown id, set" (fun () -> rs.LightName(Guid.NewGuid(), "x"))
    )

test "RectangularLightPlane" (fun () ->
    let id = rect ()
    let plane, length, width = rs.RectangularLightPlane id
    nearPt "plane origin" (pt 0. 0. 0.) plane.Origin
    near "length" 3. length
    near "width" 4. width
    let p2, l2, w2 = rs.RectangularLightPlane (lightGeo id)
    near "from geometry: length" l2 length
    near "from geometry: width" w2 width
    throwsRs "not a rectangular light" (fun () -> rs.RectangularLightPlane (rs.AddPointLight(pt 0. 0. 0.)))
    throws "unknown id" (fun () -> rs.RectangularLightPlane (Guid.NewGuid()))
    )

test "SpotLightHardness" (fun () ->
    let a = spot ()
    let b = spot ()
    rs.SpotLightHardness(a, 0.8)
    near "hardness round trip" 0.8 (rs.SpotLightHardness a)
    near "from geometry" 0.8 (rs.SpotLightHardness (lightGeo a))
    rs.SpotLightHardness([ a; b ], 0.2)
    near "multiple: first" 0.2 (rs.SpotLightHardness a)
    near "multiple: second" 0.2 (rs.SpotLightHardness b)
    let p = rs.AddPointLight(pt 0. 0. 0.)
    throwsRs "get on a point light" (fun () -> rs.SpotLightHardness p)
    throwsRs "set on a point light" (fun () -> rs.SpotLightHardness(p, 0.5))
    throwsRs "multiple set with a point light" (fun () -> rs.SpotLightHardness([ a; p ], 0.5))
    throwsRs "from geometry of a point light" (fun () -> rs.SpotLightHardness (lightGeo p))
    )

test "SpotLightRadius" (fun () ->
    let a = spot ()
    let b = spot ()
    rs.SpotLightRadius(a, 2.)
    near "radius round trip" 2. (rs.SpotLightRadius a)
    near "from geometry" 2. (rs.SpotLightRadius (lightGeo a))
    rs.SpotLightRadius([ a; b ], 3.)
    near "multiple: first" 3. (rs.SpotLightRadius a)
    near "multiple: second" 3. (rs.SpotLightRadius b)
    let p = rs.AddPointLight(pt 0. 0. 0.)
    throwsRs "get on a point light" (fun () -> rs.SpotLightRadius p)
    throwsRs "set on a point light" (fun () -> rs.SpotLightRadius(p, 1.))
    throwsRs "multiple set with a point light" (fun () -> rs.SpotLightRadius([ a; p ], 1.))
    throwsRs "from geometry of a point light" (fun () -> rs.SpotLightRadius (lightGeo p))
    )

test "SpotLightShadowIntensity" (fun () ->
    let a = spot ()
    let b = spot ()
    rs.SpotLightShadowIntensity(a, 0.7)
    near "intensity round trip" 0.7 (rs.SpotLightShadowIntensity a)
    near "from geometry" 0.7 (rs.SpotLightShadowIntensity (lightGeo a))
    rs.SpotLightShadowIntensity([ a; b ], 0.1)
    near "multiple: first" 0.1 (rs.SpotLightShadowIntensity a)
    near "multiple: second" 0.1 (rs.SpotLightShadowIntensity b)
    let p = rs.AddPointLight(pt 0. 0. 0.)
    throwsRs "get on a point light" (fun () -> rs.SpotLightShadowIntensity p)
    throwsRs "set on a point light" (fun () -> rs.SpotLightShadowIntensity(p, 0.5))
    throwsRs "multiple set with a point light" (fun () -> rs.SpotLightShadowIntensity([ a; p ], 0.5))
    throwsRs "from geometry of a point light" (fun () -> rs.SpotLightShadowIntensity (lightGeo p))
    )

printfn "%s" (finish ())
