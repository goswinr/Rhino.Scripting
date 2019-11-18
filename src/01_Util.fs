﻿namespace Rhino.Scripting

open System
open System.Globalization


module  Util = 
    
    type OPT = Runtime.InteropServices.OptionalAttribute
    type DEF = Runtime.InteropServices.DefaultParameterValueAttribute
    type EXT = Runtime.CompilerServices.ExtensionAttribute
    
    let inline notNull (value : 'T) = match value with | null -> false   | _ -> true// Fsharp core does it like this too. dont use RefrenceEquals
    
    ///Returns the value on the left unless it is null, then it returns the value on the right.
    let inline (|?) a b = if Object.ReferenceEquals(a,null) then b else a // a more fancy version: https://gist.github.com/jbtule/8477768#file-nullcoalesce-fs

    ///apply function ( like |> ) but ignore result. return original input
    let inline (|>>) a f =  f a |> ignore ; a

    let fail() = failwith "Rhino.Scripting failed (inner exception should show more helpful message)"   
 
    ///Get first element of triple (tuple of three element)
    let inline t1 (a,_,_) = a
    ///Get second element of triple (tuple of three element)
    let inline t2 (_,b,_) = b
    ///Get third element of triple (tuple of three element)
    let inline t3 (_,_,c) = c    
    
    ///so that python range expressions dont need top be translated to F#
    let internal range(l) = seq{0..(l-1)} 

    ///if first value is 0.0 return second else first
    let internal ifZero1 a b = if a = 0.0 then b else a
    
    ///if second value is 0.0 return first else second
    let internal ifZero2 a b = if b = 0.0 then a else b

module UtilMath =
    
    let enUs = CultureInfo.GetCultureInfo("en-us")
    let deAt = CultureInfo.GetCultureInfo("de-at")
    
    ///First tries to parses english float (period as decimal separator),
    ///if this fails tries to parse german floats,(comma as decimal separator)
    let parseFloatEnDe (x:string) =
        match Double.TryParse(x, NumberStyles.Float, enUs) with
        | true, f -> f
        | _ ->  match Double.TryParse(x, NumberStyles.Any, deAt) with
                | true, f -> f
                | _ -> failwithf "Could not parse '%s' into a floating point number using englisch and german culture settings" x
        
    ///Get Float from any input. This helper enables more generic code in parsing sequences
    let inline floatOfObj (o:^T) = 
        match box o with 
        | :? float   as x -> x
        | :? int     as x -> float (x)
        | :? single  as x -> float (x)
        | :? int64   as x -> float (x)
        | :? decimal as x -> float (x)
        | :? string  as x -> parseFloatEnDe (x)
        | _               -> 
            try 
                float(o)
            with _ -> 
                failwithf "Could not convert %s '%A' into a floating point number" (o.GetType().Name) o   
    
    let internal rand = System.Random () 

    ///Allows ints to be multiplied by floats
    ///<c>int(round(float(i) * f))</c> 
    let inline ( *. ) (i:int) (f:float) = int(round(float(i) * f)) // or do it like this:https://stackoverflow.com/questions/2812084/overload-operator-in-f/2812306#2812306
    
    ///Gives a float from int / int division
    ///<c>(float(i)) / (float(j))</c> 
    let inline ( /. ) (i:int) (j:int) = (float(i)) / (float(j)) // or do it like this:https://stackoverflow.com/questions/2812084/overload-operator-in-f/2812306#2812306


    //gives a int from int / float division
    //int(round( float(i) / j ))
    //let inline ( /| ) (i:int) (j:float) = int(round( float(i) / j ))
   
    
    ///Test is a floating point value is Infinity or Not a Number
    let inline isNanOrInf f = Double.IsInfinity f || Double.IsNaN f

    let private piOne80 = Math.PI / 180.   // precompute division
    let private one80Pi = 180. / Math.PI // precompute division
    ///converts Angels from Degrees to Radians
    let inline toRadians degrees = piOne80 * degrees

    ///converts Angels from Radians to Degrees
    let inline toDegrees radians = one80Pi * radians
    
    ///given mean  and standardDeviation returns a random value from this Gaussian distribution
    ///if mean is 0 and stDev is 1 then 99% of values are  are within -2.3 to +2.3 ; 70% within -1 to +1
    let randomStandardDeviation mean standardDeviation =
        let u1 = rand.NextDouble()
        let u2 = rand.NextDouble()
        let randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2) //random normal(0,1)
        //random normal(mean,stdDev^2)
        mean + standardDeviation * randStdNormal 
        

module Compare = 

    ///Point must be at middle of expression: like this: min <=. x .<= max
    let inline (<=.) left middle = (left <= middle, middle)
    ///Point must be at middle of expression: like this: min <=. x .<= max
    let inline (>=.) left middle = (left >= middle, middle)
    ///Point must be at middle of expression: like this: min <=. x .<= max
    let inline (.<=) (leftResult, middle) right = leftResult && (middle <= right)
    ///Point must be at middle of expression: like this: min <=. x .<= max
    let inline (.>=) (leftResult, middle) right = leftResult && (middle >= right)
    ///for inner expressions: like this: min <. x .<. y .< max
    let inline (.<=.) (leftResult, middle) right = leftResult && (middle <= right), right
    ///for inner expressions: like this: min <. x .<. y .< max
    let inline (.>=.) (leftResult, middle) right = leftResult && (middle >= right), right

    ///Point must be at middle of expression: like this: min <. x .< max
    let inline (<.) left middle = (left < middle, middle)
    ///Point must be at middle of expression: like this: min <. x .< max
    let inline (>.) left middle = (left > middle, middle)
    ///Point must be at middle of expression: like this: min <. x .< max
    let inline (.<) (leftResult, middle) right = leftResult && (middle < right)
    ///Point must be at middle of expression: like this: min <. x .< max
    let inline (.>) (leftResult, middle) right = leftResult && (middle > right)
    ///For inner expressions: like this: min <. x .<. y .< max
    let inline (.<.) (leftResult, middle) right = leftResult && (middle < right), right
    ///For inner expressions: like this: min <. x .<. y .< max
    let inline (.>.) (leftResult, middle) right = leftResult && (middle > right), right
    

module MinMaxSort = 
    
    ///If both are equal then the first is returned
    let inline minBy f a b =  if f a > f b then b else a
    ///If both are equal then the first is returned
    let inline maxBy f a b =  if f a < f b then b else a
    let inline min3 (a,b,c) = min a b |> min c
    let inline max3 (a,b,c) = max a b |> max c
    let inline min4 (a,b,c,d) = min a b |> min c |> min d    
    let inline max4 (a,b,c,d) = max a b |> max c |> max d
    ///If they are equal then the the order is kept
    let inline sort2 (a,b)  = if a <= b  then a,b else b,a
    ///If they are equal then the the order is kept
    let inline sort2By f (a,b) = if f a <= f b  then a,b else b,a
        
    ///If any are equal then the the order is kept
    let inline sort3 (a,b,c) = 
        if a<=b then 
            if b<=c then a,b,c
            else // c<b
                if a <= c then a,c,b
                else           c,a,b
        else // b<a
            if a<=c then b,a,c
            else //c<a
                if b<=c then b,c,a 
                else         c,b,a
        
    ///If any are equal after Function is applied then the the order is kept
    let inline sort3By f (a,b,c) = 
        if f a <= f b then 
            if f b <= f c then a,b,c
            else // c<b
                if f a <= f c then a,c,b
                else               c,a,b
        else // b<a
            if f a <= f c then b,a,c
            else //c<a
                if f b <= f c then b,c,a 
                else               c,b,a

    let inline cmp a b =
        if   a=b then  0
        elif a<b then -1
        else           1 
        
    ///Gets the positiv differnce between 2 numbers. 
    ///Avoids the integer( or byte) overflow and underflow risk of "abs(a-b)"
    let inline diff a b =
        if   a<b then b-a
        else          a-b 

      
 module DynamicOperator =
    // taken from http://www.fssnip.net/2V/title/Dynamic-operator-using-Reflection
    open System.Reflection
    open Microsoft.FSharp.Reflection

    // Various flags that specify what members can be called 
    // NOTE: Remove 'BindingFlags.NonPublic' if you want a version that can call only public methods of classes
    let private staticFlags   = BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Static 
    let private instanceFlags = BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Instance
    let private ctorFlags = instanceFlags
    let inline private asMethodBase(a:#MethodBase) = a :> MethodBase

    // The operator takes just instance and a name. Depending on how it is used
    // it either calls method (when 'R is function) or accesses a property
    let (?) (o:obj) name : 'R =
      // The return type is a function, which means that we want to invoke a method
      if FSharpType.IsFunction(typeof<'R>) then

        // Get arguments (from a tuple) and their types
        let argType, resType = FSharpType.GetFunctionElements(typeof<'R>)
        // Construct an F# function as the result (and cast it to the
        // expected function type specified by 'R)
        FSharpValue.MakeFunction(typeof<'R>, fun args ->
      
          // We treat elements of a tuple passed as argument as a list of arguments
          // When the 'o' object is 'System.Type', we call static methods
          let methods, instance, args = 
            let args = 
              // If argument is unit, we treat it as no arguments,
              // if it is not a tuple, we create singleton array,
              // otherwise we get all elements of the tuple
              if argType = typeof<unit> then [| |]
              elif not(FSharpType.IsTuple(argType)) then [| args |]
              else FSharpValue.GetTupleFields(args)

            // Static member call (on value of type System.Type)?
            if (typeof<System.Type>).IsAssignableFrom(o.GetType()) then 
              let methods = (unbox<Type> o).GetMethods(staticFlags) |> Array.map asMethodBase
              let ctors = (unbox<Type> o).GetConstructors(ctorFlags) |> Array.map asMethodBase
              Array.concat [ methods; ctors ], null, args
            else 
              o.GetType().GetMethods(instanceFlags) |> Array.map asMethodBase, o, args
        
          // A simple overload resolution based on the name and the number of parameters only
          // TODO: This doesn't correctly handle multiple overloads with same parameter count
          let methods = 
            [ for m in methods do
                if m.Name = name && m.GetParameters().Length = args.Length then yield m ]
        
          // If we find suitable method or constructor to call, do it!
          match methods with 
          | [] -> failwithf "No method '%s' with %d arguments found" name args.Length
          | _::_::_ -> failwithf "Multiple methods '%s' with %d arguments found" name args.Length
          | [:? ConstructorInfo as c] -> c.Invoke(args)
          | [ m ] -> m.Invoke(instance, args) ) |> unbox<'R>

      else
        // The result type is not an F# function, so we're getting a property
        // When the 'o' object is 'System.Type', we access static properties
        let typ, flags, instance = 
          if (typeof<System.Type>).IsAssignableFrom(o.GetType()) 
            then unbox o, staticFlags, null
            else o.GetType(), instanceFlags, o
      
        // Find a property that we can call and get the value
        let prop = typ.GetProperty(name, flags)
        if prop |> isNull && instance |> isNull then 
          // The syntax can be also used to access nested types of a type
          let nested = typ.Assembly.GetType(typ.FullName + "+" + name)
          // Return nested type if we found one
          if nested |> isNull then 
            failwithf "Property or nested type '%s' not found in '%s'." name typ.Name 
          elif not ((typeof<'R>).IsAssignableFrom(typeof<System.Type>)) then
            let rname = (typeof<'R>.Name)
            failwithf "Cannot return nested type '%s' as a type '%s'." nested.Name rname
          else nested |> box |> unbox<'R>
        else
          // Call property and return result if we found some
          let meth = prop.GetGetMethod(true)
          if prop |> isNull then failwithf "Property '%s' found, but doesn't have 'get' method." name
          try meth.Invoke(instance, [| |]) |> unbox<'R>
          with _ -> failwithf "Failed to get value of '%s' property (of type '%s')" name typ.Name