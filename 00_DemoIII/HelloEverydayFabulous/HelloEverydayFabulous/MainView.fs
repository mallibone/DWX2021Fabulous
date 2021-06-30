namespace HelloEverydayFabulous

open Fabulous
open Fabulous.XamarinForms
open Forecast
open Forecast.Models
open Xamarin.Forms

module MainView =
    type Model = 
      { Query:string; Locations:list<Locality>; IsBusy:bool;  }
      
    let localityService = LocalityService()

    let initModel = { Query = ""; Locations = []; IsBusy = false }
    
    let executeSearch query =
        async {
            let! locations = localityService.SearchLocalities(query) |> Async.AwaitTask
            return NavigationMessage.MainViewMessage (SearchResult (locations |> Seq.toList))
        }
        |> Cmd.ofAsyncMsg
    
    let update msg model =
        match msg with
        | QueryChanged query -> { model with Query = query }, Cmd.ofMsg (NavigationMessage.MainViewMessage Search)
        | Search -> {model with IsBusy = true}, (executeSearch model.Query)
        | SearchResult locations -> {model with IsBusy = false; Locations = locations}, Cmd.none
        
    let renderLocality model =
        model.Locations |> List.map (fun location -> View.TextCell (location.ToString()))

    let view (model: Model) dispatch =
        View.ContentPage(content =
            View.Grid(
                        margin = Thickness(12.0),
                        rowdefs = [Dimension.Auto; Dimension.Star],
                        coldefs = [Dimension.Star; Dimension.Auto],
                        backgroundColor = Color.Blue,
                        children = [
                            View.SearchBar(
                                            text = model.Query,
                                            textChanged = (fun text -> dispatch (NavigationMessage.MainViewMessage (QueryChanged text.NewTextValue))),
                                            searchCommand = (fun text -> dispatch (NavigationMessage.MainViewMessage (Search))),
                                            searchCommandCanExecute = (model.IsBusy <> true)
                                ).Row(0).Column(0)
                            View.ActivityIndicator(
                                isRunning = model.IsBusy,
                                isEnabled = model.IsBusy,
                                isVisible = model.IsBusy).Row(0).Column(1)
                            View.ListView(
                                items = renderLocality model,
                                itemSelected = (fun indx -> indx |> Option.iter (fun i -> dispatch (NavigationMessage.NavigateToForecastView model.Locations.[i])))
                                ).Row(1).ColumnSpan(2)
                        ])
                )
