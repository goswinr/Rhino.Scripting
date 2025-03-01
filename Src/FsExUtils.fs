namespace Rhino.Scripting

open Rhino
open System

[<RequireQualifiedAccess>]
module internal NiceFormat =

    // TODO
    let float x = sprintf "%f" x


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