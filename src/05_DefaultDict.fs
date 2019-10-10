namespace Rhino.Scripting

open System.Collections.Generic



///<summary>A System.Collections.Generic.Dictionary with default Values that get created upon accessing a key.
/// If accessing a non exiting key , the default function is called to create and set it. 
/// Like defaultdict in Python</summary>    
///<param name="defaultFun">(unit->'V): The function to create a default Value</param>
type DefaultDict< 'K,'V when 'K:equality > (defaultFun: unit->'V) =
    
    let D = Dictionary<'K,'V>() // the internal Dictionary

    let dGet(k) =
        let ok,v = D.TryGetValue(k)
        if ok then 
            v
        else 
            let v = defaultFun()
            D.[k] <- v
            v

    member _.Item 
        with get k   = dGet k        
        and  set k v = D.[k] <- v
    
    /// Get a value and remove it from Dictionary, like *.pop() in Python 
    member dd.Pop(k) =
        let ok,v = D.TryGetValue(k)
        if ok then
            D.Remove k |>ignore
            v
        else 
            failwithf "DefaultDict: Cannot pop key '%A' from %O " k dd

    /// Returns a seq of key and value tuples
    member _.Items =
        seq { for KeyValue(k,v) in D -> k,v}
        
    override dd.ToString() = 
        stringBuffer {
            yield "DefaultDict with "
            yield D.Count.ToString()
            yield! "entries"
            for k,v in dd.Items  |> Seq.truncate 3 do // add sorting ? print 3 lines??
                yield  k.ToString()
                yield " : "
                yield! v.ToString()
            yield "..."
            }
            

    // TODO add XML doc str

    // properties

    member _.Comparer with get() = D.Comparer

    member _.Count with get() = D.Count

    member _.Keys with get() = D.Keys

    member _.Values with get() = D.Values

    // methods

    member _.Add(k,v) = D.Add(k,v)

    member _.Clear() = D.Clear()

    member _.ContainsKey(k) = D.ContainsKey(k)

    member _.ContainsValue(v) = D.ContainsValue(v)    

    member _.Remove(k) = D.Remove(k)

    member _.TryGetValue(k) = D.TryGetValue(k)

    member _.GetEnumerator() = D.GetEnumerator()

    //interfaces

    interface IEnumerable<KeyValuePair<'K ,'V>> with
        member _.GetEnumerator() = (D:>IDictionary<'K,'V>).GetEnumerator()

    interface System.Collections.IEnumerable with // Non generic needed too ? 
        member __.GetEnumerator() = D.GetEnumerator():> System.Collections.IEnumerator
    
    //interface System.Collections.ICollection with // Non generic needed too ? 
        
    
    interface ICollection<KeyValuePair<'K,'V>> with 
        member _.Add(x) = (D:>ICollection<KeyValuePair<'K,'V>>).Add(x)

        member _.Clear() = D.Clear()

        member _.Remove x = (D:>ICollection<KeyValuePair<'K,'V>>).Remove x

        member _.Contains x = (D:>ICollection<KeyValuePair<'K,'V>>).Contains x

        member _.CopyTo(arr, i) = (D:>ICollection<KeyValuePair<'K,'V>>).CopyTo(arr, i)

        member _.IsReadOnly = false

        member _.Count = D.Count

    interface IDictionary<'K,'V> with 
        member _.Item 
            with get k = dGet k
            and  set k v = D.[k] <- v 
       
        member _.Keys = (D:>IDictionary<'K,'V>).Keys 

        member _.Values = (D:>IDictionary<'K,'V>).Values

        member _.Add(k, v) = D.Add(k,v)

        member _.ContainsKey k = D.ContainsKey k

        member _.TryGetValue(k,r ) = D.TryGetValue(k,ref r) 

        member _.Remove(k) = D.Remove(k)

    interface IReadOnlyCollection<KeyValuePair<'K,'V>> with 
        member _.Count = D.Count

    interface IReadOnlyDictionary<'K,'V> with 
        member _.Item 
            with get k = dGet k
       
        member _.Keys = (D:>IReadOnlyDictionary<'K,'V>).Keys 

        member _.Values = (D:>IReadOnlyDictionary<'K,'V>).Values

        member _.ContainsKey k = D.ContainsKey k

        member _.TryGetValue(k,r ) = D.TryGetValue(k,ref r) 




    //member _.GetObjectData() = D.GetObjectData()

    //member _.OnDeserialization() = D.OnDeserialization()

    //member _.Equals() = D.Equals()

    //member _.GetHashCode() = D.GetHashCode()

    //member _.GetType() = D.GetType()

    //


    (*
    interface _.IDictionary`2() = D.IDictionary`2()

    interface _.ICollection`1() = D.ICollection`1()

    interface _.IEnumerable`1() = D.IEnumerable`1()

    interface _.IEnumerable() = D.IEnumerable()

    interface _.IDictionary() = D.IDictionary()

    interface _.ICollection() = D.ICollection()

    interface _.IReadOnlyDictionary`2() = D.IReadOnlyDictionary`2()

    interface _.IReadOnlyCollection`1() = D.IReadOnlyCollection`1()

    interface _.ISerializable() = D.ISerializable()

    interface _.IDeserializationCallback() = D.IDeserializationCallback()
    *)