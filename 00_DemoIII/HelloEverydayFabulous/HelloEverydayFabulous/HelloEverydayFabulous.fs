namespace HelloEverydayFabulous

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module App = 
    type Views =
        | MainView
        | ForecastView

    type Model = 
      { NavigationStack: list<Views>
        MainViewState: MainView.Model
        ForecastViewState: ForecastView.Model}

    let initModel = { NavigationStack = [ Views.MainView ]; MainViewState = MainView.initModel; ForecastViewState = ForecastView.initModel }

    let init () = initModel, Cmd.none

    let updateMainViews (model:Model) message =
        let mainModel, cmd = MainView.update message model.MainViewState
        { model with MainViewState = mainModel }, cmd
    
    let update msg model =
        match msg with
        | NavigateToForecastView location -> { model with ForecastViewState = {model.ForecastViewState with Location = location}; NavigationStack = Views.ForecastView::model.NavigationStack;}, Cmd.none
        | MainViewMessage mainMessage -> (updateMainViews model mainMessage)
        | GoBack -> {model with NavigationStack = model.NavigationStack |> List.tail}, Cmd.none

    let view (model: Model) dispatch =
        View.NavigationPage(
            popped = (fun _ -> dispatch GoBack),
            pages = [
                for page in (model.NavigationStack |> List.rev) do
                    match page with
                    | MainView ->
                        yield MainView.view model.MainViewState dispatch
                    | ForecastView ->
                        yield ForecastView.view model.ForecastViewState dispatch
                ])

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
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //
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


