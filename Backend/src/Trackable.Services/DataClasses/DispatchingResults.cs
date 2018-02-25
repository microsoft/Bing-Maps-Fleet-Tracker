using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.Services
{
    public class DispatchingResults
    {
        public IEnumerable<string> ItineraryText { get; set; }

        public IEnumerable<Point> ItineraryPoints { get; set; }

        public IEnumerable<string> ItineraryDistance { get; set; }

        public IEnumerable<Point> RoutePoints { get; set; }

        public IEnumerable<Point> AlternativeCarRoutePoints { get; set; }
    }
}
