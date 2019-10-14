namespace Rhino.Scripting

open System.Collections.Generic



///<summary>A System.Collections.Generic.Dictionary with default Values that get created upon accessing a key.
/// If accessing a non exiting key , the default function is called to create and set it. 
/// Like defaultdict in Python</summary>    
///<param name="defaultFun">(unit->'V): The function to create a default Value</param>
type DefaultDict< 'K,'V when 'K:equality > (defaultFun: unit->'V) =
    
    let DD = Dictionary<'K,'V>() // the internal Dictionary

    let dGet(k) =
        let ok,v = DD.TryGetValue(k)
        if ok then 
            v
        else 
            let v = defaultFun()
            DD.[k] <- v
            v

    member _.Item 
        with get k   = dGet k        
        and  set k v = DD.[k] <- v
    
    /// Get a value and remove it from Dictionary, like *.pop() in Python 
    member dd.Pop(k) =
        let ok,v = DD.TryGetValue(k)
        if ok then
            DD.Remove k |>ignore
            v
        else 
            failwithf "DefaultDict: Cannot pop key '%A' from %O " k dd

    /// Returns a seq of key and value tuples
    member _.Items =
        seq { for KeyValue(k,v) in DD -> k,v}
        
    override dd.ToString() = 
        stringBuffer {
            yield "DefaultDict with "
            yield DD.Count.ToString()
            yield! "entries"
            for k,v in dd.Items  |> Seq.truncate 3 do // add sorting ? print 3 lines??
                yield  k.ToString()
                yield " : "
                yield! v.ToString()
            yield "..."
            }
            

    // TODO add XML doc str

    // properties

    member _.Comparer with get() = DD.Comparer

    member _.Count with get() = DD.Count

    member _.Keys with get() = DD.Keys

    member _.Values with get() = DD.Values

    // methods

    member _.Add(k,v) = DD.Add(k,v)

    member _.Clear() = DD.Clear()

    member _.ContainsKey(k) = DD.ContainsKey(k)

    member _.ContainsValue(v) = DD.ContainsValue(v)    

    member _.Remove(k) = DD.Remove(k)

    member _.TryGetValue(k) = DD.TryGetValue(k)

    member _.GetEnumerator() = DD.GetEnumerator()

    //interfaces

    interface IEnumerable<KeyValuePair<'K ,'V>> with
        member _.GetEnumerator() = (DD:>IDictionary<'K,'V>).GetEnumerator()

    interface System.Collections.IEnumerable with // Non generic needed too ? 
        member __.GetEnumerator() = DD.GetEnumerator():> System.Collections.IEnumerator
    
    //interface System.Collections.ICollection with // Non generic needed too ? 
        
    
    interface ICollection<KeyValuePair<'K,'V>> with 
        member _.Add(x) = (DD:>ICollection<KeyValuePair<'K,'V>>).Add(x)

        member _.Clear() = DD.Clear()

        member _.Remove x = (DD:>ICollection<KeyValuePair<'K,'V>>).Remove x

        member _.Contains x = (DD:>ICollection<KeyValuePair<'K,'V>>).Contains x

        member _.CopyTo(arr, i) = (DD:>ICollection<KeyValuePair<'K,'V>>).CopyTo(arr, i)

        member _.IsReadOnly = false

        member _.Count = DD.Count

    interface IDictionary<'K,'V> with 
        member _.Item 
            with get k = dGet k
            and  set k v = DD.[k] <- v 
       
        member _.Keys = (DD:>IDictionary<'K,'V>).Keys 

        member _.Values = (DD:>IDictionary<'K,'V>).Values

        member _.Add(k, v) = DD.Add(k,v)

        member _.ContainsKey k = DD.ContainsKey k

        member _.TryGetValue(k,r ) = DD.TryGetValue(k,ref r) 

        member _.Remove(k) = DD.Remove(k)

    interface IReadOnlyCollection<KeyValuePair<'K,'V>> with 
        member _.Count = DD.Count

    interface IReadOnlyDictionary<'K,'V> with 
        member _.Item 
            with get k = dGet k
       
        member _.Keys = (DD:>IReadOnlyDictionary<'K,'V>).Keys 

        member _.Values = (DD:>IReadOnlyDictionary<'K,'V>).Values

        member _.ContainsKey k = DD.ContainsKey k

        member _.TryGetValue(k,r ) = DD.TryGetValue(k,ref r) 




    //member _.GetObjectData() = DD.GetObjectData()

    //member _.OnDeserialization() = DD.OnDeserialization()

    //member _.Equals() = DD.Equals()

    //member _.GetHashCode() = DD.GetHashCode()

    //member _.GetType() = DD.GetType()

    //


    (*
    interface _.IDictionary`2() = DD.IDictionary`2()

    interface _.ICollection`1() = DD.ICollection`1()

    interface _.IEnumerable`1() = DD.IEnumerable`1()

    interface _.IEnumerable() = DD.IEnumerable()

    interface _.IDictionary() = DD.IDictionary()

    interface _.ICollection() = DD.ICollection()

    interface _.IReadOnlyDictionary`2() = DD.IReadOnlyDictionary`2()

    interface _.IReadOnlyCollection`1() = DD.IReadOnlyCollection`1()

    interface _.ISerializable() = DD.ISerializable()

    interface _.IDeserializationCallback() = DD.IDeserializationCallback()
    *)