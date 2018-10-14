module Aktor.Kote.Giraffe.Actors

open Akka.Actor
open Akka.FSharp
open Aktor.Kote.Akka.FSharp.KoteHome

//let actorSystemHandler (system : ActorSystem) =
    //let koteHome = system.ActorSelection

let initActors () = 
    let system = System.create "system" (Configuration.defaultConfig())
    
    let home = spawn system "koteHome" <| koteHomeActor 10
    system