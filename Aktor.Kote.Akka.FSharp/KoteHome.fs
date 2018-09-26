module Aktor.Kote.Akka.FSharp.KoteHome
open Aktor.Kote.Akka.FSharp.Kote
open Akka.FSharp
open System

type KoteHomeMessages = 
     | StrokeEveryone of unit

let koteHomeActor catCount (mailbox : Actor<KoteHomeMessages>) = 
     for i in 0 .. catCount
       do i.ToString() 
          |> koteActor
          |> spawn mailbox.Context (i.ToString())
          |> ignore
     
     let rec koteHome () =  actor {
          let! message = mailbox.Receive()
          let children = mailbox.Context.GetChildren()
          match message with 
          | StrokeEveryone s -> for ref in mailbox.Context.GetChildren()
                                 do ref <! Stroke ()
          
          return! koteHome ()
     }
     koteHome ()