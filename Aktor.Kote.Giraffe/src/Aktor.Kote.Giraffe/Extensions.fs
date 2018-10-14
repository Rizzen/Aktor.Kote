module Aktor.Kote.Giraffe.Extensions

open Aktor.Kote.Akka.FSharp.Kote
open Microsoft.AspNetCore.Builder
open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection
open Akka.Actor
open Akka.FSharp

type IApplicationBuilder with
    member this.UseAkkaKote () = 
        this
        
type IServiceCollection with 
    member this.RegisterAkka () =
        System.create "system" (Configuration.defaultConfig())
        |> this.AddSingleton<ActorSystem>