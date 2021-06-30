// Copyright Fabulous contributors. See LICENSE.md for license.
namespace HelloEverydayFabulous

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Forecast.Models
open Xamarin.Forms

module ForecastView = 
    type Model = 
      { Location: Locality }

    //type Msg = 
    //    | GoBack 

    let initModel = { Location = Locality() }

    let init () = initModel, Cmd.none

    let view (model: Model) dispatch =
        View.ContentPage(
          content = View.StackLayout(padding = Thickness 20.0, 
            children = [
                View.Label(text = $"City: {model.Location.City}")
                View.Label(text = $"Canton: {model.Location.Canton}")
                View.Label(text = $"Abbreviation: {model.Location.Abbreviation}")
                View.Label(text = $"Country: {model.Location.Country}")
                View.Button(text = "Go Back", command = (fun () -> dispatch GoBack), horizontalOptions = LayoutOptions.Center)
            ]))

