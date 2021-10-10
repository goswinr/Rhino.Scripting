(*
This script combines  several files into Scripting.fs before compiling.
the generated file gets delted after compiling
this must be in fsproj file:

<!-- Combine all files of the rhinoscriptingsyntax into one:  https://stackoverflow.com/a/44829863/969070 -->
 <Target Name="GenerateSomeFiles" BeforeTargets="BeforeBuild">
    <Exec Command="dotnet fsi combineIntoOneFile.fsx" />
    <ItemGroup> <Compile Include="Src/Scripting.fs" /> </ItemGroup>
  </Target>

 <!-- Delete Scripting.fs when done-->
  <Target Name="DeleteMergedFile" BeforeTargets="AfterBuild"> <Delete Files="Src/Scripting.fs" TreatErrorsAsWarnings="true" /> </Target>


*) 

module CombineIntoOneFile =   
    
    open System 
        
    let splitMarker = "{@$%^&*()*&^%$@}"
    
    let head = "Src/Scripting_Header.fs"
    
    let files = [ 
        "Src/Scripting_Coerce.fs" 
        "Src/Scripting_Printing.fs" 
        "Src/Scripting_Layer.fs" 
        "Src/Scripting_Application.fs" 
        "Src/Scripting_Block.fs" 
        "Src/Scripting_Curve.fs" 
        "Src/Scripting_Dimension.fs" 
        "Src/Scripting_Documnet.fs" 
        "Src/Scripting_Geometry.fs" 
        "Src/Scripting_Grips.fs" 
        "Src/Scripting_Group.fs" 
        "Src/Scripting_Hatch.fs" 
        "Src/Scripting_Light.fs" 
        "Src/Scripting_Line.fs" 
        "Src/Scripting_Linetype.fs" 
        "Src/Scripting_Material.fs" 
        "Src/Scripting_Mesh.fs" 
        "Src/Scripting_Object.fs" 
        "Src/Scripting_Plane.fs" 
        "Src/Scripting_PointVector.fs" 
        "Src/Scripting_Selection.fs" 
        "Src/Scripting_Surface.fs" 
        "Src/Scripting_Toolbar.fs" 
        "Src/Scripting_Transformation.fs" 
        "Src/Scripting_UserData.fs" 
        "Src/Scripting_UserInterface.fs" 
        "Src/Scripting_Utility.fs" 
        "Src/Scripting_Views.fs" 
        ]
        
    
    (* 
    //not needed since file gets deleted anyway after compilation
    let addNote file (ln:string)  = 
        if not (ln.Contains "//")  && not (String.IsNullOrWhiteSpace ln)  then  
            ln + " // don't edit this file directly! Work in "+file+" instead." 
        else  
            ln
    *) 
    
    let getLines(p:string) = 
        if IO.File.Exists p then   
            IO.File.ReadAllLines p
        else 
            eprintfn "*!*!*!*!*!* File %s not found in CurrentDirectory: %s" p Environment.CurrentDirectory
            [| |]
            
    let afterMarker file (lns:string []) :seq<string> = 
        match lns |> Array.tryFindIndex( fun ln -> ln.Contains splitMarker) with 
        |Some i ->   
            //lns.[i+1 .. ] // |> Array.map (addNote file) 
            Seq.skip i lns
        |None ->  
            eprintfn "*!*!*!*!*!* Split marker %s not found in %s !" splitMarker  file 
            Seq.empty
                
    let run() =  
        let lines = ResizeArray<string>()   
        
        // Start merging files:
        lines.AddRange(getLines head) 
        
        printfn "--Pre Build Step:"
        printf  "--Combine all files of Src/Scripting_****.fs into one:"
        for i, file in files |> Seq.indexed do 
            let n = file.Replace("Src/Scripting_", " ") 
            lines.AddRange(file|> getLines |> afterMarker file) 
            if i%7 =0 then  
                printf "\r\n  -- %s" n
            else                
                printf " %s" n
        printfn "" 
        
        IO.File.WriteAllLines("Src/Scripting.fs", lines, Text.Encoding.UTF8) 
        printfn "--Done combining files into Src/Scripting.fs. compiling now .."
        
    (*
    let runIfOld() = 
        // this case actually does not happen because the file Scripting.fs is deleted after each build
        if IO.File.Exists "Src/Scripting.fs" then   
            let t = IO.File.GetLastWriteTime "Src/Scripting.fs" 
            if DateTime.Now - t < TimeSpan.FromSeconds(30.) then  
                printfn "----Combining files skiped because Src/Scripting.fs is less than 30 sec old."
            else 
                run() 
        else 
            run()         
    *)    
        
CombineIntoOneFile.run()         
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
            
        
        
            
        