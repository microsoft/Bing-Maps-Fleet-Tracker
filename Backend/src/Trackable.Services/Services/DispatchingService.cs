// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common.Exceptions;
using Trackable.Models;

namespace Trackable.Services
{
    class DispatchingService : IDispatchingService
    {
        private readonly string routingUrl;
        private readonly string bingMapsKey;

        private readonly string[] distanceUnitStrings = { "mi", "km" };
        private readonly string[] dimensionUnitStrings = { "m", "ft" };
        private readonly string[] weightUnitStrings = { "kg", "lb" };

        private readonly string[] avoidStrings = { "highways", "tolls", "minimizeHighways", "minimizeTolls" };
        private readonly string[] optimizeStrings = { "time", "timeWithTraffic" };
        private readonly string[] routeStrings = { "routePath" };
        private readonly string[] timeTypeStrings = { "Arrival", "Departure" };
        private readonly string[] hazardeousMaterialsString = { "E", "G", "F", "C", "FS", "O", "P", "R", "Cr",
            "PI", "WH", "Other", "None", "AllAppropriateForLoad"};

        private static HttpClient httpClient = new HttpClient();


        public DispatchingService(IConfiguration configuration)
        {
            this.routingUrl = "https://dev.virtualearth.net/REST/v1/Routes/Truck?";
            this.bingMapsKey = configuration["SubscriptionKeys:BingMaps"];
        }

        public async Task<IEnumerable<DispatchingResults>> CallRoutingAPI(DispatchingParameters dispatchingParameters, AssetProperties assetProperties)
        {
            var url = $"{this.routingUrl}{GenerateURL(dispatchingParameters, assetProperties)}&key={this.bingMapsKey}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new BadArgumentException($"Dispatching API returned: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResult = JObject.Parse(content);

            var result = ExtractDispatchingResults(apiResult);

            if (dispatchingParameters.GetAlternativeCarRoute)
            {
                var alternativeUrl = $"{this.routingUrl}{GenerateURL(dispatchingParameters, assetProperties, true)}&key={this.bingMapsKey}";

                response = await httpClient.GetAsync(alternativeUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new BadArgumentException($"Alternative dispatching API returned: {response.StatusCode}");
                }

                content = await response.Content.ReadAsStringAsync();
                apiResult = JObject.Parse(content);

                var alternativePath = ExtractDispatchingResults(apiResult);

                foreach (var dispatchingResult in result)
                {
                    dispatchingResult.AlternativeCarRoutePoints = alternativePath;
                }
            }

            return result;
        }

        public string GenerateURL(DispatchingParameters dispatchingParameters, AssetProperties assetProperties, bool isCarRoute = false)
        {
            var strBuilder = new StringBuilder("routeAttributes=routePath");

            if (dispatchingParameters.WayPoints != null)
            {
                strBuilder.Append(GenerateWayPointsUri(dispatchingParameters.WayPoints));
            }
            else
            {
                throw new BadArgumentException("WayPoints must be defined");
            }

            if (dispatchingParameters.MaxSolutions != null)
            {
                strBuilder.Append(GenerateMaxSolutionsUri(dispatchingParameters.MaxSolutions));
            }

            if (dispatchingParameters.DistanceUnit != null)
            {
                strBuilder.Append(GenerateDistanceUnitUri(dispatchingParameters.DistanceUnit));
            }

            if (dispatchingParameters.WeightUnit != null)
            {
                strBuilder.Append(GenerateWeightUnitUri(dispatchingParameters.WeightUnit));
            }

            if (dispatchingParameters.DimensionUnit != null)
            {
                strBuilder.Append(GenerateDimensionUnitUri(dispatchingParameters.DimensionUnit));
            }

            if (isCarRoute)
            {
                return strBuilder.ToString();
            }

            if (dispatchingParameters.Optimize != null)
            {
                strBuilder.Append(GenerateOptimizeUri(dispatchingParameters.Optimize));
            }

            if (dispatchingParameters.Avoid != null && dispatchingParameters.Avoid.Count() > 0)
            {
                strBuilder.Append(GenerateAvoidUri(dispatchingParameters.Avoid));
            }

            if (dispatchingParameters.DistanceBeforeFirstTurn != null)
            {
                strBuilder.Append($"&distanceBeforeFirstTurn={dispatchingParameters.DistanceBeforeFirstTurn}");
            }

            if (dispatchingParameters.Heading != null)
            {
                strBuilder.Append(GenerateHeadingUri(dispatchingParameters.Heading));
            }

            if (dispatchingParameters.Tolerances != null)
            {
                strBuilder.Append(GenerateTolerancesUri(dispatchingParameters.Tolerances));
            }

            if (dispatchingParameters.AvoidCrossWind == true)
            {
                strBuilder.Append("&vehicleAvoidCrossWind=true");
            }

            if (dispatchingParameters.DateTime != null)
            {
                strBuilder.Append("&dateTime=" + dispatchingParameters.DateTime.ToString());

                if (dispatchingParameters.TimeType != null)
                {
                    strBuilder.Append(GenerateTimeTypeUri(dispatchingParameters.TimeType));
                }
            }

            strBuilder.Append(GenerateAssetHeightUri(dispatchingParameters.LoadedHeight, assetProperties));
            strBuilder.Append(GenerateAssetLengthUri(dispatchingParameters.LoadedLength, assetProperties));
            strBuilder.Append(GenerateAssetWidthUri(dispatchingParameters.LoadedWidth, assetProperties));
            strBuilder.Append(GenerateAssetWeightUri(dispatchingParameters.LoadedWeight, assetProperties));


            if (dispatchingParameters.AvoidGroundingRisk == true)
            {
                strBuilder.Append("&vehicleAvoidGroundingRisk=true");
            }

            if (dispatchingParameters.HazardousMaterials != null && dispatchingParameters.HazardousMaterials.Count() > 0)
            {
                strBuilder.Append(GenerateHazardousMaterialsUri(dispatchingParameters.HazardousMaterials));
            }

            if (dispatchingParameters.HazardousPermits != null && dispatchingParameters.HazardousPermits.Count() > 0)
            {
                strBuilder.Append(GenerateHazardousPermitsUri(dispatchingParameters.HazardousPermits));
            }

            if (assetProperties == null)
            {
                return strBuilder.ToString();
            }

            if (assetProperties.AssetAxels != null)
            {
                strBuilder.Append($"&vehicleAxles={assetProperties.AssetAxels}");
            }

            if (assetProperties.AssetTrailers != null)
            {
                strBuilder.Append($"&vehicleTrailers={assetProperties.AssetTrailers}");
            }

            if (assetProperties.AssetSemi == true)
            {
                strBuilder.Append("&vehicleSemi=true");
            }

            if (assetProperties.AssetMaxGradient != null)
            {
                strBuilder.Append($"&vehicleMaxGradient={assetProperties.AssetMaxGradient}");
            }

            if (assetProperties.AssetMinTurnRadius != null)
            {
                strBuilder.Append($"&vehicleMinTurnRadius={assetProperties.AssetMinTurnRadius}");
            }

            return strBuilder.ToString();
        }

        private IEnumerable<DispatchingResults> ExtractDispatchingResults(JObject apiResult)
        {
            var results = new List<DispatchingResults>();

            foreach (var route in apiResult["resourceSets"][0]["resources"])
            {
                var points = new List<Point>();
                var directions = new List<string>();
                var directionsDistance = new List<string>();
                var directionPoints = new List<Point>();
                var distanceUnit = route["distanceUnit"];

                foreach (var pathCoord in route["routePath"]["line"]["coordinates"])
                {
                    points.Add(new Point()
                    {
                        Latitude = pathCoord[0].Value<double>(),
                        Longitude = pathCoord[1].Value<double>()
                    });
                }

                foreach (var routeLeg in route["routeLegs"])
                {
                    foreach (var itineraryItem in routeLeg["itineraryItems"])
                    {
                        directionPoints.Add(new Point()
                        {
                            Latitude = itineraryItem["maneuverPoint"]["coordinates"][0].Value<double>(),
                            Longitude = itineraryItem["maneuverPoint"]["coordinates"][1].Value<double>()
                        });

                        directions.Add(itineraryItem["instruction"]["text"].Value<string>());

                        var distance = itineraryItem["travelDistance"].Value<double>();

                        directionsDistance.Add(distance.ToString("0.###") + " " + distanceUnit);
                    }
                }

                results.Add(new DispatchingResults()
                {
                    ItineraryText = directions,
                    ItineraryPoints = directionPoints,
                    ItineraryDistance = directionsDistance,
                    RoutePoints = points
                });
            }

            return results;
        }

        private IEnumerable<Point> ExtractAlternativeDispatchingResult(JObject apiResult)
        {
            var points = new List<Point>();

            foreach (var route in apiResult["resourceSets"][0]["resources"])
            {
                foreach (var pathCoord in route["routePath"]["line"]["coordinates"])
                {
                    points.Add(new Point()
                    {
                        Latitude = pathCoord[0].Value<double>(),
                        Longitude = pathCoord[1].Value<double>()
                    });
                }
            }

            return points;
        }

        private string GenerateWayPointsUri(IEnumerable<Point> wayPoints)
        {
            if (wayPoints.Count() < 2 || wayPoints.Count() > 20)
            {
                throw new BadArgumentException("Waypoints must be between 2 and 20 points");
            }

            var strBuilder = new StringBuilder();
            var index = 0;

            foreach (var wayPoint in wayPoints)
            {
                strBuilder.Append($"&waypoint.{index++}={wayPoint.Latitude},{wayPoint.Longitude}");
            }

            return strBuilder.ToString();
        }

        private string GenerateMaxSolutionsUri(int? maxSolutions)
        {
            if (maxSolutions > 0 && maxSolutions <= 3)
            {
                return $"&maxSolutions={maxSolutions}";
            }

            throw new BadArgumentException("MaxSolution argument out of range");
        }

        private string GenerateAvoidUri(IEnumerable<AvoidTypes> avoidTypes)
        {
            switch (avoidTypes.Count())
            {
                case 1:
                    {
                        return $"&avoid={this.avoidStrings[(int)avoidTypes.First()]}";
                    }
                case 2:
                    {
                        var orderedList = avoidTypes.OrderBy(a => a).ToList();

                        if (orderedList[0] == AvoidTypes.Tolls && orderedList[1] == AvoidTypes.MinimizeTolls
                            || orderedList[0] == AvoidTypes.Highways && orderedList[1] == AvoidTypes.MinimizeHighways)
                        {
                            throw new BadArgumentException("Avoid parameters are conflicting");
                        }

                        else
                        {
                            return $"&avoid={this.avoidStrings[(int)orderedList[0]]},{this.avoidStrings[(int)orderedList[1]]}";
                        }
                    }
                default:
                    {
                        throw new BadArgumentException("Avoid paramters are conflicting");
                    }
            }
        }

        private string GenerateHeadingUri(int? heading)
        {
            if (heading >= 0 && heading < 360)
            {
                return $"&heading={heading}";
            }

            throw new BadArgumentException("Heading argument out of range");
        }

        private string GenerateOptimizeUri(OptimizeValue? optimize)
        {
            switch (optimize)
            {
                case OptimizeValue.Time:
                case OptimizeValue.TimeWithTraffic:
                    {
                        return $"&optimize={this.optimizeStrings[(int)optimize]}";
                    }
                default:
                    {
                        throw new BadArgumentException("Invalid Optimize option");
                    }
            }
        }

        private string GenerateTolerancesUri(IEnumerable<double> tolerances)
        {
            var answer = new List<string>();

            foreach (var tolerance in tolerances)
            {
                answer.Add($"{tolerance}");
            }

            return "&tolerances=" + string.Join(",", answer);
        }

        private string GenerateDistanceUnitUri(DistanceUnit? distanceUnit)
        {
            switch (distanceUnit)
            {
                case DistanceUnit.Mile:
                case DistanceUnit.Kilometer:
                    {
                        return $"&distanceUnit={this.distanceUnitStrings[(int)distanceUnit]}";
                    }
                default:
                    {
                        throw new BadArgumentException("Invalid Distance Uri Option");
                    }
            }
        }

        private string GenerateDimensionUnitUri(DimensionUnit? dimensionUnit)
        {
            switch (dimensionUnit)
            {
                case DimensionUnit.Meter:
                case DimensionUnit.Foot:
                    {
                        return $"&dimensionUnit={this.dimensionUnitStrings[(int)dimensionUnit]}";
                    }
                default:
                    {
                        throw new BadArgumentException("Invalid Dimension Uri Option");
                    }
            }
        }

        private string GenerateWeightUnitUri(WeightUnit? weightUnit)
        {
            switch (weightUnit)
            {
                case WeightUnit.Kilogram:
                case WeightUnit.Pound:
                    {
                        return $"&weightUnit={this.weightUnitStrings[(int)weightUnit]}";
                    }
                default:
                    {
                        throw new BadArgumentException("Invalid Distance Uri Option");
                    }
            }
        }
        private string GenerateTimeTypeUri(TimeType? timeType)
        {
            switch (timeType)
            {
                case TimeType.Arrival:
                case TimeType.Departure:
                    {
                        return $"&timeType={this.timeTypeStrings[(int)timeType]}";
                    }
                default:
                    {
                        throw new BadArgumentException("Invalid Time Type Uri Option");
                    }
            }
        }

        private string GenerateAssetHeightUri(double? loadedHeight, AssetProperties assetProperties)
        {
            if (loadedHeight != null && loadedHeight > 0)
            {
                return $"&vehicleHeight={loadedHeight}";
            }

            if (assetProperties != null && assetProperties.AssetHeight != null)
            {
                return $"&vehicleHeight={assetProperties.AssetHeight}";
            }

            return "";
        }

        private string GenerateAssetLengthUri(double? loadedLength, AssetProperties assetProperties)
        {
            if (loadedLength != null && loadedLength > 0)
            {
                return $"&vehicleLength={loadedLength}";
            }

            if (assetProperties != null && assetProperties.AssetLength != null)
            {
                return $"&vehicleLength={assetProperties.AssetLength}";
            }

            return "";
        }

        private string GenerateAssetWidthUri(double? loadedWidth, AssetProperties assetProperties)
        {
            if (loadedWidth != null && loadedWidth > 0)
            {
                return $"&vehicleWidth={loadedWidth}";
            }

            if (assetProperties != null && assetProperties.AssetWidth != null)
            {
                return $"&vehicleWidth={assetProperties.AssetWidth}";
            }

            return "";
        }

        private string GenerateAssetWeightUri(double? loadedWeight, AssetProperties assetProperties)
        {
            if (loadedWeight != null && loadedWeight > 0)
            {
                return $"&vehicleWeight={loadedWeight}";
            }

            if (assetProperties != null && assetProperties.AssetWeight != null)
            {
                return $"&vehicleWeight={assetProperties.AssetWeight}";
            }

            return "";
        }
        private string GenerateHazardousMaterialsUri(IEnumerable<HazardousMaterial> HazardousMaterials)
        {
            var answer = new List<string>();

            foreach (var hazardousMaterial in HazardousMaterials)
            {
                switch (hazardousMaterial)
                {
                    case HazardousMaterial.Explosive:
                    case HazardousMaterial.Gas:
                    case HazardousMaterial.Flammable:
                    case HazardousMaterial.Combustable:
                    case HazardousMaterial.FlammableSolid:
                    case HazardousMaterial.Organic:
                    case HazardousMaterial.Poison:
                    case HazardousMaterial.RadioActive:
                    case HazardousMaterial.Corrosive:
                    case HazardousMaterial.PoisonousInhalation:
                    case HazardousMaterial.GoodsHarmfulToWater:
                    case HazardousMaterial.Other:
                    case HazardousMaterial.None:
                        {
                            answer.Add(this.hazardeousMaterialsString[(int)hazardousMaterial]);
                            break;
                        }
                    default:
                        {
                            throw new BadArgumentException("Invalid Hazardous Materials option");
                        }
                }
            }

            return "&vehicleHazardousMaterials=" + string.Join(",", answer);
        }

        private string GenerateHazardousPermitsUri(IEnumerable<HazardousMaterial> vehicleHazardousPermits)
        {
            var answer = new List<string>();

            foreach (var hazardousMaterial in vehicleHazardousPermits)
            {
                switch (hazardousMaterial)
                {
                    case HazardousMaterial.Explosive:
                    case HazardousMaterial.Gas:
                    case HazardousMaterial.Flammable:
                    case HazardousMaterial.Combustable:
                    case HazardousMaterial.FlammableSolid:
                    case HazardousMaterial.Organic:
                    case HazardousMaterial.Poison:
                    case HazardousMaterial.RadioActive:
                    case HazardousMaterial.Corrosive:
                    case HazardousMaterial.PoisonousInhalation:
                    case HazardousMaterial.AllApproppriateForLoad:
                    case HazardousMaterial.None:
                        {
                            answer.Add(this.hazardeousMaterialsString[(int)hazardousMaterial]);
                            break;
                        }
                    default:
                        {
                            throw new BadArgumentException("Invalid Hazardous Permits option");
                        }
                }
            }
            return "&vehicleHazardousPermits=" + string.Join(",", answer);
        }
    }
}
