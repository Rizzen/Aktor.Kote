open Akka.Actor
open System
open Akka.FSharp
open Aktor.Kote.Akka.FSharp.Kote
open Aktor.Kote.Akka.FSharp.KoteHome
    
[<EntryPoint>]
let main argv =   
    let home = spawn system "koteHome" <| koteHomeActor 10
    home <! StrokeEveryone ()
    
    Console.ReadLine() |> ignore
    0 // return an integer exit code
