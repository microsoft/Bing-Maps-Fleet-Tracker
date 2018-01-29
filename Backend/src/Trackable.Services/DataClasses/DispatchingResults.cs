using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public class DispatchingResults
    {
        public DispatchingResults() {}

        public IEnumerable<string> ItineraryText;

        public IEnumerable<Point> ItineraryPoints;

        public IEnumerable<string> ItineraryDistance;

        public IEnumerable<Point> RoutePoints;

        public IEnumerable<Point> AlternativeCarRoutePoints;
    }
}
