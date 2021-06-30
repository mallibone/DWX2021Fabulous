namespace HelloEverydayFabulous

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Forecast.Models
open Xamarin.Forms

type MainViewMessage =
    | QueryChanged of string
    | Search
    | SearchResult of list<Locality>
    
type NavigationMessage = 
    | NavigateToColorView of Color
    | NavigateToForecastView of Locality
    | MainViewMessage of MainViewMessage
    | GoBack 
