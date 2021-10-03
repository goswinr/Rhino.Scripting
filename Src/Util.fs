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

    // ------- Abreviations so that declarations are not so long:

    /// OptionalAttribute for member parameters
    type internal OPT = Runtime.InteropServices.OptionalAttribute

    /// DefaultParameterValueAttribute for member parameters
    type internal DEF =   Runtime.InteropServices.DefaultParameterValueAttribute

    ///<summary>Checks if a string is a good string for use in Rhino Object Names or User Dictionary keys and values.
    /// A good string may not inculde line returns, tabs, and leading or traling whitespace.
    /// Confusing or ambiguous characters that look like ASCII but are some other unicode are also not allowed. </summary>
    ///<param name="name">(string) The string to check.</param>
    ///<param name="allowEmpty">(bool) set to true to make empty strings pass. </param>
    ///<param name="limitToAscii">(bool) set to true to only allow chars between unicode points 32 till 126 (ASCII) </param>
    ///<returns>(bool) true if the string is a valid name.</returns>
    let isGoodStringId( name:string, allowEmpty:bool, limitToAscii:bool ) : bool = 
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
                        if c >= ' ' && c <= '~' then // always OK , unicode points 32 till 126 ( regular letters numbers and symbols)
                            loop(i+1)
                        elif limitToAscii then 
                            false
                        else                            
                            let cat = Char.GetUnicodeCategory(c)
                            match cat with
                            // always OK :
                            | UnicodeCategory.UppercaseLetter //TODO improve via https://github.com/vhf/confusable_homoglyphs or https://github.com/codebox/homoglyph/blob/master/raw_data/chars.txt
                            | UnicodeCategory.LowercaseLetter
                            | UnicodeCategory.CurrencySymbol ->
                                loop(i+1)

                            // sometimes Ok :
                            | UnicodeCategory.OtherSymbol       // 166:'�'  167:'�'   169:'�'  174:'�' 176:'�' 182:'�'
                            | UnicodeCategory.MathSymbol        // 172:'�' 177:'�' 215:'�' 247:'�' | exclude char 215 that looks like x
                            | UnicodeCategory.OtherNumber ->    //178:'�' 179:'�' 185:'�' 188:'�' 189:'�' 190:'�'
                                if c <= '÷' && c <> '×' then loop(i+1) // anything below char 247 is ok , but exclude MathSymbol char 215 that looks like letter x
                                else false

                            // NOT Ok :
                            | UnicodeCategory.OpenPunctuation  // only ( [ and { is allowed
                            | UnicodeCategory.ClosePunctuation // exclude if out of unicode points 32 till 126
                            | UnicodeCategory.Control
                            | UnicodeCategory.SpaceSeparator         // only regular space  ( that is char 32)     is alowed
                            | UnicodeCategory.ConnectorPunctuation   // only simple underscore  _    is alowed
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
                            | UnicodeCategory.OtherPunctuation // exclude if out of unicode points 32 till 126
                            | UnicodeCategory.DecimalDigitNumber // exclude if out of unicode points 32 till 126
                            | UnicodeCategory.InitialQuotePunctuation
                            | UnicodeCategory.FinalQuotePunctuation
                            | UnicodeCategory.OtherLetter
                            | _ -> false
                loop 0