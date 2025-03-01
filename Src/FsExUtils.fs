namespace Rhino.Scripting

open Rhino
open System


/// Exposes the settings used in toNiceString pretty printing function
[<RequireQualifiedAccess>]
module NiceStringSettings =

    /// Set this to change the printing of floats larger than 10'000
    let mutable thousandSeparator = '\'' // = just one quote '

    /// If the absolute value of a float is below this, display ±0.0
    /// Default = 1e-24.
    /// Double.Epsilon = no rounding down
    /// This value can be set for example by hosting apps that have a build in absolute tolerance like Rhino3d
    let mutable userZeroTolerance = 1e-24 // Double.Epsilon

[<RequireQualifiedAccess>]
module internal NiceFormat =

    module private Literals =

        /// string for RhinoMath.UnsetDouble -1.23432101234321e+308
        [<Literal>]
        let RhinoMathUnsetDouble = "RhinoMath.UnsetDouble" // for https://developer.rhino3d.com/api/RhinoCommon/html/F_Rhino_RhinoMath_UnsetValue.htm

        /// string for RhinoMath.UnsetSingle -1.234321e+38f
        [<Literal>]
        let RhinoMathUnsetSingle = "RhinoMath.UnsetSingle" // for https://developer.rhino3d.com/api/RhinoCommon/html/F_Rhino_RhinoMath_UnsetSingle.htm

        [<Literal>]
        let PositiveInfinity = "∞"

        [<Literal>]
        let NegativeInfinity = "-∞"

        [<Literal>]
        let NaN = "NaN"

        [<Literal>]
        let CloseToZeroPositive = "≈+0.0"

        [<Literal>]
        let CloseToZeroNegative = "≈-0.0"

        [<Literal>]
        let BelowUserZeroTolerance = "~0.0"

    //let internal deDE = Globalization.CultureInfo("de-DE")
    let internal invC = Globalization.CultureInfo.InvariantCulture

    /// Assumes a string that represent a float or int with '.' as decimal separator and no other input formatting
    let internal addThousandSeparators (s:string) =
        let b = Text.StringBuilder(s.Length + s.Length / 3 + 1)
        let inline add (c:char) = b.Append(c) |> ignore

        let inline doBeforeComma st en =
            for i=st to en-1 do // don't go to last one because it shall never get a separator
                let rest = en-i
                add s.[i]
                if rest % 3 = 0 then add NiceStringSettings.thousandSeparator
            add s.[en] //add last (never with sep)

        let inline doAfterComma st en =
            add s.[st] //add fist (never with sep)
            for i=st+1 to en do // don't go to last one because it shall never get a separator
                let pos = i-st
                if pos % 3 = 0 then add NiceStringSettings.thousandSeparator
                add s.[i]

        let start =
            if s.[0] = '-' then  add '-'; 1 // add minus if present and move start location
            else                          0

        match s.IndexOf('.') with
        | -1 ->
            match s.IndexOf("e",StringComparison.OrdinalIgnoreCase) with
            | -1 -> doBeforeComma start (s.Length-1)
            | e -> // if float is in scientific notation don't insert comas into it too:
                doBeforeComma start (s.Length-1)
                for ei = e to s.Length-1 do add s.[ei]
        | i ->
            if i>start then
                doBeforeComma start (i-1)
            add '.'
            if i < s.Length then
                match s.IndexOf("e",StringComparison.OrdinalIgnoreCase) with
                | -1 -> doAfterComma (i+1) (s.Length-1)
                | e -> // if float is in scientific notation don't insert comas into it too:
                    doAfterComma (i+1) (e-1)
                    for ei = e to s.Length-1 do add s.[ei]

        b.ToString()


    /// Formatting with automatic precision
    /// e.g.: 0 digits behind comma if above 1000
    /// if there are more than 15 zeros behind the comma just '~0.0' will be displayed
    /// if the value is smaller than NiceStringSettings.roundToZeroBelow '0.0' will be shown.
    /// this is Double.Epsilon by default
    let float  (x:float) =
        if   Double.IsNaN x then Literals.NaN
        elif x = Double.NegativeInfinity then Literals.NegativeInfinity
        elif x = Double.PositiveInfinity then Literals.PositiveInfinity
        elif x = -1.23432101234321e+308  then Literals.RhinoMathUnsetDouble // for https://developer.rhino3d.com/api/RhinoCommon/html/F_Rhino_RhinoMath_UnsetValue.htm
        elif x = 0.0 then "0.0" // not "0" as in sprintf "%g"
        else
            let  a = abs x
            if   a <  NiceStringSettings.userZeroTolerance then Literals.BelowUserZeroTolerance // do this check up here, value might be very high
            elif a >= 10000.          then x.ToString("#")|> addThousandSeparators
            elif a >= 1000.           then x.ToString("#")
            elif a >= 100.            then x.ToString("#.#" , invC)
            elif a >= 10.             then x.ToString("0.0#" , invC)
            elif a >= 1.              then x.ToString("0.0##" , invC)
            elif a >= 0.1             then x.ToString("0.####" , invC)
            elif a >= 0.01            then x.ToString("0.#####" , invC)
            elif a >= 0.001           then x.ToString("0.######" , invC)|> addThousandSeparators
            elif a >= 0.000_1         then x.ToString("0.#######" , invC)|> addThousandSeparators
            elif a >= 0.000_01        then x.ToString("0.########" , invC)|> addThousandSeparators
            elif a >= 0.000_001       then x.ToString("0.#########" , invC)|> addThousandSeparators
            elif a >= 0.000_0001      then x.ToString("0.##########" , invC)|> addThousandSeparators
            elif a >= 0.000_000_001   then x.ToString("0.#############" , invC)|> addThousandSeparators
            elif x >= 0.0             then Literals.CloseToZeroPositive
            else                           Literals.CloseToZeroNegative

    /// Formatting with automatic precision
    /// e.g.: 0 digits behind comma if above 1000
    let single (x:float32) =
        if   Single.IsNaN x then Literals.NaN
        elif x = Single.NegativeInfinity then Literals.PositiveInfinity
        elif x = Single.PositiveInfinity then Literals.NegativeInfinity
        elif x = -1.234321e+38f then Literals.RhinoMathUnsetSingle // for https://developer.rhino3d.com/api/RhinoCommon/html/F_Rhino_RhinoMath_UnsetSingle.htm
        elif x = 0.0f then "0.0" // not "0" as in sprintf "%g"
        else
            let  a = abs x
            if   a <  float32 NiceStringSettings.userZeroTolerance then Literals.BelowUserZeroTolerance // do this check up here, value might be very high
            elif a >= 10000.f      then x.ToString("#")|> addThousandSeparators
            elif a >= 1000.f       then x.ToString("#")
            elif a >= 100.f        then x.ToString("#.#" , invC)
            elif a >= 10.f         then x.ToString("0.0#" , invC)
            elif a >= 1.f          then x.ToString("0.0##" , invC)
            elif a >= 0.1f         then x.ToString("0.####" , invC)
            elif a >= 0.01f        then x.ToString("0.#####" , invC)
            elif a >= 0.001f       then x.ToString("0.######" , invC) |> addThousandSeparators
            elif a >= 0.000_1f     then x.ToString("0.#######" , invC) |> addThousandSeparators
            elif a >= 0.000_01f    then x.ToString("0.########" , invC)|> addThousandSeparators
            elif x >= 0.0f         then Literals.CloseToZeroPositive
            else                        Literals.CloseToZeroNegative


module internal  Seq =

    /// Returns true if the given Rarr has equal or less than count items.
    let hasMaximumItems count (xs : seq<'T>) : bool =
        match xs with
        | :? ('T[]) as a ->     a.Length <= count
        | :? ('T ResizeArray) as a  -> a.Count  <= count
        | :? ('T Collections.Generic.IList) as a -> a.Count  <= count
        | _ ->
            let mutable k = 0
            use e = xs.GetEnumerator()
            while e.MoveNext() && k <= count do  k <- k+1
            k <= count

    /// Counts for how many items of the Seq the predicate returns true.
    /// same as Seq.filter and then Seq.length
    let countIf (predicate : 'T -> bool) (xs : seq<'T>) : int = //countBy is something else !!
        let mutable k = 0
        for x in xs do
            if predicate x then
                k <- k + 1
        k
module internal ResizeArray =

    let inline map (mapping: 'T -> 'U) (resizeArray: ResizeArray<'T>) : ResizeArray<'U> =
        resizeArray.ConvertAll(System.Converter mapping)


    let inline mapSeq (mapping: 'T -> 'U) (xs: seq<'T>) : ResizeArray<'U> =
        let res = ResizeArray()
        for x in xs do
            res.Add(mapping x)
        res

    let inline mapArr (mapping: 'T -> 'U) (arr: 'T[]) : ResizeArray<'U> =
        let res = ResizeArray(arr.Length)
        for x in arr do
            res.Add(mapping x)
        res



[<AutoOpenAttribute>]
module FsExUtils =

    type Dict<'K,'V> = Collections.Generic.Dictionary<'K,'V>


    type ``[]``<'T>  with //Generic Array

        /// Get (or set) the last item in the Array.
        /// equal to this.[this.Length - 1]
        /// (this is an Extension Member from FsEx.ExtensionsArray)
        member inline this.Last
            with get() =
                if this.Length = 0 then RhinoScriptingException.Raise "Rhino.Scripting.FsExUtils arr.Last: Failed to get last item of empty Array"
                this.[this.Length - 1]
            and set (v:'T) =
                if this.Length = 0 then RhinoScriptingException.Raise "Rhino.Scripting.FsExUtils arr.Last: Failed to set last item of empty Array"
                this.[this.Length - 1] <- v

    type Collections.Generic.HashSet<'T> with
        member inline this.DoesNotContain (item:'T) =
            not (this.Contains item)


    type Collections.Generic.List<'T> with

        /// Gets an item in the ResizeArray by index.
        /// Allows for negative index too ( -1 is last item,  like Python)
        member inline xs.GetNeg index =
            let len = xs.Count
            let ii = if index < 0 then len + index else index
            xs.[ii]

    /// Returns false if the value is null.
    /// The opposite of isNull
    let inline notNull (value :'T when 'T: null) = // FSharp core does it like this too. don't use Object.ReferenceEquals (because of Generics)
        match value with
        | null -> false
        | _ -> true


    /// Apply function, like |> , but ignore result.
    /// Return original input.
    /// let inline (|>!) x f =  f x |> ignore ; x
    /// Be aware of correct indenting see:
    /// https://stackoverflow.com/questions/64784154/indentation-change-after-if-else-expression-not-taken-into-account
    let inline ( |>! ) x f =
        f x |> ignore //https://twitter.com/GoswinR/status/1316988132932407296
        x

        /// Null coalescing:
    /// Returns the value on the left unless it is null, then it returns the value on the right.
    let inline ( |? ) (a:'T) (b:'T)  =
        // a more fancy version: https://gist.github.com/jbtule/8477768#file-nullcoalesce-fs
        match a with
        | null -> b
        | _    -> a // if Object.ReferenceEquals(a, null) then b else a


    /// Test is a floating point number (with Measure)  is NaN (Not a Number) or Infinity.
    let inline isNanOrInf (f:float<'T>) =
        Double.IsInfinity (float f) || Double.IsNaN (float f)


    /// Converts Angels from Degrees to Radians.
    let inline toRadians (degrees:float<'T>):float =  // no measure on returned float !
        0.0174532925199433 * float degrees // 0.0174532925199433 = Math.PI / 180.

    /// Converts Angels from Radians to Degrees.
    let inline toDegrees (radians:float<'T>) :float = // no measure on returned float !
        57.2957795130823 * float radians // 57.2957795130823 = 180. / Math.PI


    /// American English culture (used for float parsing).
    let enUs = Globalization.CultureInfo.GetCultureInfo "en-US"

    /// German culture (used for float parsing).
    let deAt = Globalization.CultureInfo.GetCultureInfo "de-DE"

        /// First tries to parses float with En-Us CultureInfo (period as decimal separator),
    /// if this fails tries to parse parses float with De-At CultureInfo (comma as decimal separator).
    let tryParseFloatEnDe (x:string) : option<float> =
        match Double.TryParse(x, Globalization.NumberStyles.Float, enUs) with
        | true, f -> Some f
        | _ ->  match Double.TryParse(x, Globalization.NumberStyles.Any, deAt) with
                | true, f -> Some f
                | _ -> None


    /// First tries to parses float with En-Us CultureInfo (period as decimal separator).
    /// If this fails tries to parse parses float with De-At CultureInfo (comma as decimal separator).
    let parseFloatEnDe (x:string) : float =
        match tryParseFloatEnDe x  with
        | Some f -> f
        | None ->  RhinoScriptingException.Raise "Rhino.Scripting.FsExUtils.parseFloatEnDe Could not parse '%s' into a floating point number using English or German culture settings" x


    /// Get first element of Triple (Tuple of three elements)
    let inline t1 (a,_,_) = a

    /// Get second element of Triple (Tuple of three elements)
    let inline t2 (_,b,_) = b

    /// Get third element of Triple (Tuple of three elements)
    let inline t3 (_,_,c) = c