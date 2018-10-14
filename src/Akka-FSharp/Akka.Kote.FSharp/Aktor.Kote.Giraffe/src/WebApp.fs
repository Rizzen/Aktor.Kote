module Aktor.Kote.Giraffe.WebApp

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Aktor.Kote.Giraffe.Actors
open Akka.FSharp
open Akka.Actor
open Giraffe.HttpStatusCodeHandlers
open SwaggerForFsharp.Giraffe.Dsl
open SwaggerForFsharp.Giraffe

open SwaggerForFsharp.Giraffe.Common
open SwaggerForFsharp.Giraffe.Generator
open SwaggerForFsharp.Giraffe.Dsl
// ---------------------------------
// Models
// ---------------------------------

type Message =
    {
        Text : string
    }

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open GiraffeViewEngine

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "Aktor.Kote.Giraffe" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let partial () =
        h1 [] [ encodedText "Aktor.Kote.Giraffe" ]

    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
        ] |> layout

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model     = { Text = greetings }
    let view      = Views.index model
    htmlView view

let katsHandler = 
    let json = json "there will be kats!"
    Successful.ok json    
    
let docAddendums =
    fun (route:Analyzer.RouteInfos) (path:string, verb:HttpVerb, pathDef:PathDefinition) ->
    
        // routef params are automatically added to swagger, but you can customize their names like this 
        let changeParamName oldName newName (parameters:ParamDefinition list) =
            parameters |> Seq.find (fun p -> p.Name = oldName) |> fun p -> { p with Name = newName }
    
        match path, verb, pathDef with
        | _,_, def when def.OperationId = "say_hello_in_french" ->
            let firstname = def.Parameters |> changeParamName "arg0" "Firstname"
            let lastname = def.Parameters |> changeParamName "arg1" "Lastname"
            "/hello/{Firstname}/{Lastname}", verb, { def with Parameters = [firstname; lastname] }
        | _ -> path, verb, pathDef

let port = 5000

let docsConfig c = 

    let describeWith desc = 
        { desc
            with
                Title="Sample 1"
                Description="Create a swagger with Giraffe"
                TermsOfService="Coucou"
        } 
    
    { c with 
        Description = describeWith
        Host = sprintf "localhost:%d" port
       // DocumentationAddendums = docAddendums
    }
    
let webApp (system : ActorSystem) =
    swaggerOf (choose [
        GET >=>
            choose [
                route "/" >=> indexHandler "world"
                routef "/hello/%s" indexHandler
                route "/kats" >=> katsHandler 
            ]
        setStatusCode 404 >=> text "Not Found" ])
    |> withConfig docsConfig

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (system : ActorSystem) (app : IApplicationBuilder)  =
    let actorWebApp = webApp system
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles() 
        .UseGiraffe(actorWebApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Error
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore