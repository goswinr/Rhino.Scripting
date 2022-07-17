namespace Rhino

open System
open Rhino
open System.Globalization

[<AutoOpen>]
module internal Util = 

    /// If first value is 0.0 return second else first
    let inline ifZero1 a b = if a = 0.0 then b else a

    /// If second value is 0.0 return first else second
    let inline ifZero2 a b =  if b = 0.0 then a else b

    // ------- Abbreviations so that declarations are not so long:

    /// OptionalAttribute for member parameters
    type internal OPT = Runtime.InteropServices.OptionalAttribute

    /// DefaultParameterValueAttribute for member parameters
    type internal DEF =  Runtime.InteropServices.DefaultParameterValueAttribute


    ///<summary>Checks if a string is a ASCII string for use in Rhino Object Names or User Dictionary keys and values.
    /// Only allows chars between Unicode points 32 till 126 (ASCII) </summary>
    /// May not include line returns, tabs, and leading or trailing whitespace.
    ///<param name="name">(string) The string to check.</param>
    ///<param name="allowEmpty">(bool) set to true to make empty strings pass. </param>    
    ///<returns>(bool) true if the string is a valid name.</returns>
    let isASCIIStringId( name:string, allowEmpty:bool) : bool = 
        if isNull name then false
        elif allowEmpty && name = "" then true
        else
            let tr = name.Trim()
            if tr.Length <> name.Length then false
            else
                let rec loop i = 
                    if i = name.Length then true
                    else
                        let c = name.[i]
                        if c >= ' ' && c <= '~' then // always OK , Unicode points 32 till 126 ( regular letters numbers and symbols)
                            loop(i+1)                        
                        else                            
                            false
                loop 0
    
    ///<summary>Checks if a string is a good string for use in Rhino Object Names or User Dictionary keys and values.
    /// A good string may not include line returns, tabs, and leading or trailing whitespace.
    /// Confusing or ambiguous Unicode characters that look like ASCII but are some other Unicode are also not allowed. </summary>
    ///<param name="name">(string) The string to check.</param>
    ///<param name="allowEmpty">(bool) set to true to make empty strings pass. </param>   
    ///<returns>(bool) true if the string is a valid name.</returns>
    let isGoodStringId( name:string, allowEmpty:bool) : bool = 
        if isNull name then false
        elif allowEmpty && name = "" then true
        else
            let tr = name.Trim()
            if tr.Length <> name.Length then false
            else
                let rec loop i = 
                    if i = name.Length then true
                    else
                        let c = name.[i]
                        if c >= ' ' && c <= '~' then // always OK , Unicode points 32 till 126 ( regular letters numbers and symbols)
                            loop(i+1)                        
                        else                            
                            let cat = Char.GetUnicodeCategory(c)
                            match cat with
                            // always OK :
                            | UnicodeCategory.UppercaseLetter //TODO improve via https://github.com/vhf/confusable_homoglyphs or https://github.com/codebox/homoglyph/blob/master/raw_data/chars.txt
                            | UnicodeCategory.LowercaseLetter
                            | UnicodeCategory.CurrencySymbol ->
                                loop(i+1)

                            // sometimes OK :
                            | UnicodeCategory.OtherSymbol       
                            | UnicodeCategory.MathSymbol         
                            | UnicodeCategory.OtherNumber ->    
                                if (c <= '÷' && c <> '×')  || c = '⌀' || c='∅'  then // anything below char 247 (÷) is OK , but exclude MathSymbol char 215 that looks like letter x, ⌀ 8960 DIAMETER SIGN, and ∅ 8709 EMPTY SET (somtimes used as diameter too)"
                                    loop(i+1) 
                                else 
                                    false

                            // NOT OK :
                            | UnicodeCategory.OpenPunctuation  // only ( [ and { is allowed
                            | UnicodeCategory.ClosePunctuation // exclude if out of Unicode points 32 till 126
                            | UnicodeCategory.Control
                            | UnicodeCategory.SpaceSeparator         // only regular space  ( that is char 32)     is allowed
                            | UnicodeCategory.ConnectorPunctuation   // only simple underscore  _    is allowed
                            | UnicodeCategory.DashPunctuation        // only minus - is allowed
                            | UnicodeCategory.TitlecaseLetter
                            | UnicodeCategory.ModifierLetter
                            | UnicodeCategory.NonSpacingMark
                            | UnicodeCategory.SpacingCombiningMark
                            | UnicodeCategory.EnclosingMark
                            | UnicodeCategory.LetterNumber
                            | UnicodeCategory.LineSeparator
                            | UnicodeCategory.ParagraphSeparator
                            | UnicodeCategory.Format
                            | UnicodeCategory.OtherNotAssigned
                            | UnicodeCategory.ModifierSymbol
                            | UnicodeCategory.OtherPunctuation // exclude if out of Unicode points 32 till 126
                            | UnicodeCategory.DecimalDigitNumber // exclude if out of Unicode points 32 till 126
                            | UnicodeCategory.InitialQuotePunctuation
                            | UnicodeCategory.FinalQuotePunctuation
                            | UnicodeCategory.OtherLetter
                            | _ -> false
                loop 0


    ///<summary>Checks if a string is a acceptable string for use in Rhino Object Names or User Dictionary keys and values.
    /// A acceptable string may not include line returns, tabs, and leading or trailing whitespace.
    /// Confusing or ambiguous Unicode characters that look like ASCII are allowed. </summary>
    ///<param name="name">(string) The string to check.</param>
    ///<param name="allowEmpty">(bool) set to true to make empty strings pass. </param>    
    ///<returns>(bool) true if the string is a valid name.</returns>
    let isAcceptableStringId(name:string, allowEmpty:bool ) : bool = 
        if isNull name then false
        elif allowEmpty && name = "" then true
        else
            let tr = name.Trim()
            if tr.Length <> name.Length then false
            else
                let rec loop i = 
                    if i = name.Length then true
                    else
                        let c = name.[i]
                        if   c < ' ' then false // is a control character
                        elif c <= '~' then // always OK , Unicode points 32 till 126 ( regular letters numbers and symbols)
                            loop(i+1)
                        else                            
                            let cat = Char.GetUnicodeCategory(c)
                            match cat with
                            // always OK :
                            | UnicodeCategory.UppercaseLetter 
                            | UnicodeCategory.LowercaseLetter
                            | UnicodeCategory.CurrencySymbol 
                            | UnicodeCategory.OtherSymbol       
                            | UnicodeCategory.MathSymbol        
                            | UnicodeCategory.OtherNumber    
                            | UnicodeCategory.SpaceSeparator         
                            | UnicodeCategory.ConnectorPunctuation   
                            | UnicodeCategory.DashPunctuation        
                            | UnicodeCategory.TitlecaseLetter
                            | UnicodeCategory.ModifierLetter
                            | UnicodeCategory.NonSpacingMark
                            | UnicodeCategory.SpacingCombiningMark
                            | UnicodeCategory.EnclosingMark
                            | UnicodeCategory.LetterNumber
                            | UnicodeCategory.LineSeparator
                            | UnicodeCategory.Format
                            | UnicodeCategory.OtherNotAssigned
                            | UnicodeCategory.ModifierSymbol
                            | UnicodeCategory.OtherPunctuation 
                            | UnicodeCategory.DecimalDigitNumber 
                            | UnicodeCategory.InitialQuotePunctuation
                            | UnicodeCategory.FinalQuotePunctuation
                            | UnicodeCategory.OtherLetter    
                            | UnicodeCategory.OpenPunctuation  
                            | UnicodeCategory.ClosePunctuation 
                                -> loop(i+1)                                

                            // NOT OK :
                            | UnicodeCategory.ParagraphSeparator
                            | UnicodeCategory.Control                            
                            | UnicodeCategory.Surrogate                          
                            | UnicodeCategory.PrivateUse                          
                            | _ -> false
                loop 0



    