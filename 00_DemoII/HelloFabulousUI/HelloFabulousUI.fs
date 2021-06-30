// Copyright Fabulous contributors. See LICENSE.md for license.
namespace HelloFabulousUI

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open System

module App = 
    type Model = 
      { Username : string
        Password : string
        IsLoggedIn : bool
        ErrorText : string }

    type Msg = 
        | Login
        | CreateAccount 
        | UsernameChanged of string
        | PasswordChanged of string

    let initModel = { Username = ""; Password = ""; IsLoggedIn = false; ErrorText = ""; }

    let init () = initModel, Cmd.none

    let createAccount model = 
       {model with ErrorText = "Ups, something went wrong..."}, Cmd.none

    let authenticate model = 
        if model.Username = "Foo" && model.Password = "bar" then
            {model with ErrorText = ""; IsLoggedIn = true }, Cmd.none
        else
            {model with ErrorText = "Invalid Login"}, Cmd.none

    let update msg model =
        match msg with
        | UsernameChanged s -> { model with Username = s }, Cmd.none
        | PasswordChanged s -> { model with Password = s }, Cmd.none
        | Login -> authenticate model
        | CreateAccount -> createAccount model
     
    let makeEntry placeholderText text textChangedAction isPassword =
        View.Entry(placeholder = placeholderText, 
            text = text, 
            textChanged=(debounce 250 (textChangedAction)), 
            isPassword = isPassword,
            textColor = Color.Black,
            horizontalOptions = LayoutOptions.Center,
            width = 300.0)

    let mainPage model dispatch =
        View.Label(text = (sprintf "Welcome dear %s!" model.Username),
            horizontalOptions = LayoutOptions.Center,
            verticalOptions = LayoutOptions.Center,
            textColor = Color.Fuchsia,
            fontSize = FontSize.fromNamedSize(NamedSize.Large))
        
    let makePrimaryButton text commandAction =
        View.Button(text = text, 
            backgroundColor = Color.DarkCyan,
            textColor = Color.White,
            command = commandAction,
            horizontalOptions = LayoutOptions.Center, 
            width = 160.)
        
    let makeSecondaryButton text commandAction =
        View.Button(text = text, 
            command = commandAction,
            textColor = Color.DarkCyan,
            horizontalOptions = LayoutOptions.Center, 
            visual = VisualMarker.Default, 
            width = 160.)
    
    let makeErrorText errorText =
        View.Label(text = errorText, 
            textColor = Color.Red, 
            isVisible = not (String.IsNullOrEmpty(errorText)), 
            horizontalOptions = LayoutOptions.Center, 
            horizontalTextAlignment = TextAlignment.Center)
    
    let view (model: Model) dispatch =
            if model.IsLoggedIn then
                View.ContentPage(
                    content = mainPage model dispatch
                    )
            else
                View.ContentPage(
                  visual = VisualMarker.Material,
                  backgroundImage = Image.fromPath "background",
                  content = 
                    View.StackLayout(
                        padding = Thickness(20.0), 
                        backgroundColor = Color.FromRgba(1.,1.,1.,0.9),
                        verticalOptions = LayoutOptions.Center,
                        children = [ 
                            makeEntry "Username" model.Username (fun e -> dispatch (UsernameChanged e.NewTextValue)) false
                            makeEntry "Password" model.Password (fun e -> dispatch (PasswordChanged e.NewTextValue)) true
                            makeErrorText model.ErrorText
                            makePrimaryButton "Login" (fun () -> dispatch Login)
                            makeSecondaryButton "Create Account" (fun () -> dispatch CreateAccount)
                        ]))

    // Note, this declaration is needed if you enable LiveUpdate
    let program =
        XamarinFormsProgram.mkProgram init update view
#if DEBUG
        |> Program.withConsoleTrace
#endif

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
        |> XamarinFormsProgram.run app

#if DEBUG
    do runner.EnableLiveUpdate()
#endif    

// Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
// See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE
	let modelId = "model"
	override __.OnSleep() = 

	    let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
	    Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

	    app.Properties.[modelId] <- json

	override __.OnResume() = 
	    Console.WriteLine "OnResume: checking for model in app.Properties"
	    try 
		match app.Properties.TryGetValue modelId with
		| true, (:? string as json) -> 

		    Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
		    let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

		    Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
		    runner.SetCurrentModel (model, Cmd.none)

		| _ -> ()
	    with ex -> 
		App.program.onError("Error while restoring model found in app.Properties", ex)

	override this.OnStart() = 
	    Console.WriteLine "OnStart: using same logic as OnResume()"
	    this.OnResume()
#endif