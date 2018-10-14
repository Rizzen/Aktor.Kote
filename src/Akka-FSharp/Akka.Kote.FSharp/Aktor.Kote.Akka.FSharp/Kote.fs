module Aktor.Kote.Akka.FSharp.Kote

open System
open Akka.Actor
open Akka.FSharp


type KoteMessage = 
    | Stroke of unit
    | Status of unit
    | Hunger of int
    
type KoteState = { Name:string; Hunger:int }

let system = System.create "system" (Configuration.defaultConfig())

let handleStatus (status:KoteState) = printfn "Kote %s hunger on %A" status.Name status.Hunger

let commonHungerHandler state value hungerState notHungerState = 
    let newHunger = state.Hunger + value
    if (newHunger >= 50) 
        then hungerState { Name = state.Name; Hunger = newHunger }
    else notHungerState { Name = state.Name; Hunger = newHunger }

//---------------- 
// FSM Actor
//----------------
let koteActor name (mailbox: Actor<KoteMessage>) = 
    let rec idle (state:KoteState) = actor {
        let! message = mailbox.Receive()
        
        match message with 
        | Stroke s -> printf "Kote %s say Purr Purr Purr \n" name
        | Hunger h -> return! commonHungerHandler state h hungry idle
        | Status s -> handleStatus state
                      mailbox.Sender() <! (sprintf "name: %s, hunger: %A" state.Name state.Hunger)
        
        return! idle state
        }
        and hungry state = actor {
            let! message = mailbox.Receive()
            
            match message with 
            | Stroke s -> printfn "Meow Meow Meow"
            | Hunger h -> let newHunger = state.Hunger + h
                          if (newHunger >= 50) 
                            then return! death state
                          printfn "Kat Need Food! Hunger is %A" (newHunger)
                          return! hungry { Name = name; Hunger = newHunger }
            | Status s -> handleStatus state
            
            return! hungry state
        }
        and sleeping state = actor {
            let! message = mailbox.Receive()
            
            match message with 
            | Stroke s -> printfn "Murr"
                          return! idle state
            | Hunger h -> return! commonHungerHandler state h hungry idle
            | Status s -> handleStatus state
            
            return! sleeping state
        }
        and death state = actor {
            let! message = mailbox.Receive()
            printf "Kote %s death with hunger %A" state.Name state.Hunger
            mailbox.Context.Self.GracefulStop |> ignore
        }
    idle { Name = name; Hunger = 0 }

