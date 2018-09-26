module Aktor.Kote.Akka.FSharp.Kote
open Akka.Actor
open Akka.FSharp

let system = System.create "system" (Configuration.defaultConfig())

type KoteMessage = 
    | Stroke of unit
    | Status of unit
    | Hunger of int
    
type KoteState = { Name:string; Hunger:int }

let handleStatus (status:KoteState) = printfn "Kote %s hunger on %A" status.Name status.Hunger

let koteActor name (mailbox: Actor<KoteMessage>) = 
    let rec idle (state:KoteState) = actor {
        let! message = mailbox.Receive()
        
        match message with 
        | Stroke s -> printf "Kote %s say Purr Purr Purr \n" name
        | Hunger h -> let newHunger = state.Hunger + h
                      if (newHunger >= 50) 
                      then printfn "Kote is now Hungry!"
                           return! hungry {Name = name; Hunger = newHunger}
                      printfn "Kote received Hunger %A" h
                      return! idle { Name = name; Hunger = newHunger}
        | Status s -> handleStatus state
        
        return! idle state
        }
        and hungry state = actor {
            let! message = mailbox.Receive() 
            
            match message with 
            | Stroke s -> printfn "Meow Meow Meow"
            | Hunger h -> let newHunger = state.Hunger + h
                          if (newHunger >= 50) then return! death state
                          printfn "Kat Need Food! Hunger is %A" (newHunger)
                          return! hungry { Name=name; Hunger=newHunger }
            | Status s -> handleStatus state
            
            return! hungry state
        }
        and sleeping state = actor {
            let! message = mailbox.Receive()
            return! sleeping state
        }
        and death state = actor {
            let! message = mailbox.Receive()
            printf "Kote %s death with hunger %A" state.Name state.Hunger
            mailbox.Context.Self.GracefulStop |> ignore
        }
    idle { Name = name; Hunger = 0 }

