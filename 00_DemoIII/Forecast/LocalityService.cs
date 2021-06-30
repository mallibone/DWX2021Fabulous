using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Forecast.Models;
using Newtonsoft.Json;

namespace Forecast
{
    public class LocalityService
    {
        private List<Locality> _localities = new();

        public Task<IEnumerable<Locality>> SearchLocalities(string searchQuery) =>
            Task.Run(() => Filter(searchQuery, CancellationToken.None));

        private async Task<IEnumerable<Locality>> Filter(string searchQuery, CancellationToken tcl)
        {
            if (_localities.Any() == false) _localities = await LoadPostalcodes();

            return string.IsNullOrEmpty(searchQuery)
                ? new List<Locality>()
                : _localities.Where(l =>
                    l.City.StartsWith(searchQuery, StringComparison.InvariantCultureIgnoreCase)
                    || l.Postalcode.ToString().StartsWith(searchQuery));
        }
        
        private async Task<List<Locality>> LoadPostalcodes()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Locality)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Forecast.Assets.SwissPostalcodes.json");
            using StreamReader reader = new System.IO.StreamReader(stream);
            string postalCodeJson = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<List<Locality>>(postalCodeJson);
        }
    }
}