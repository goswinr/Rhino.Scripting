namespace Rhino.Scripting

open System

open Rhino.Geometry

open FsEx
open FsEx.UtilMath


module Topology =
        
        // the same fuction exist on FsEX.Rarr module too but with extra error checking
        // swap the values of two given indices in Rarr
        let inline private swap i j (xs:Rarr<'T>) :unit = 
            if i<>j then  
                let ti = xs.[i]
                xs.[i] <- xs.[j]
                xs.[j] <- ti
        
        // the same fuction exist on FsEX.Rarr module too but with extra error checking
        // like Rarr.minIndexBy but starting to search only from a given index
        let inline private minIndexByFrom  (compareBy: 'T -> 'U)  fromIdx (xs:Rarr<'T>) : int = 
            let mutable idx = fromIdx
            let mutable mi = compareBy xs.[fromIdx]
            for j = fromIdx + 1 to xs.LastIndex do 
                let this = compareBy xs.[j]
                if this < mi then  
                    idx <- j
                    mi <- this
            idx

        /// Sorts elements in place to be in a circular structure.
        /// for each line end point it finds the next closest line start point 
        /// ( does not check other line end points tha might be closer)  
        /// Line is used as an abstraction to hold start and end of arbitrary curve
        let sortToLoop(getLine: 'T -> Line) (xs:Rarr<'T>)  = 
            for i = 0 to xs.Count - 2 do // only run till second last
                let thisLine = getLine xs.[i] 
                // could be optimised using a R-Tree for very large lists instead of minBy function
                let nextIdx = xs |> minIndexByFrom (fun c -> RhinoScriptSyntax.DistanceSquare ((getLine c).From ,  thisLine.To) ) (i+1)
                xs |> swap (i+1) nextIdx
               
        /// Sorts elements in place  to be in a circular structure.
        /// for each line end it finds the next closest start point or end point
        /// Line is used as an abstraction to hold start and end of arbitrary curve
        /// Reverse input in place,  where required
        let sortToLoopWithReversing (getLine: 'T -> Line) (reverseInPlace: int -> 'T -> unit) (xs:Rarr<'T>) : unit = 
            for i = 0 to xs.Count - 2 do // only run till second last
                let thisLine = getLine xs.[i] 
                // could be optimised using a R-Tree for very large lists instead of minBy function
                let nextIdxSt = xs |> minIndexByFrom (fun c -> RhinoScriptSyntax.DistanceSquare ((getLine c).From ,  thisLine.To) ) (i+1)
                let nextIdxEn = xs |> minIndexByFrom (fun c -> RhinoScriptSyntax.DistanceSquare ((getLine c).To   ,  thisLine.To) ) (i+1)
                // check if closest endpoint is closer than closest startpoint 
                if  RhinoScriptSyntax.DistanceSquare ((getLine xs.[nextIdxSt]).From ,  thisLine.To) <=  
                    RhinoScriptSyntax.DistanceSquare ((getLine xs.[nextIdxEn]).To   ,  thisLine.To) then 
                        xs |> swap (i+1) nextIdxSt
                else 
                        reverseInPlace nextIdxEn xs.[nextIdxEn]
                        xs |> swap (i+1) nextIdxEn
               
