using AutoMapper;
using Trackable.Models;

namespace Trackable.Web.DTOs
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<Asset, AssetDto>()
                .ReverseMap();

            CreateMap<AssetProperties, AssetPropertiesDto>()
                .ReverseMap();

            // Default to Polygon if AreaType not specified for backwards compatibility
            CreateMap<GeoFence, GeoFenceDto>()
                .ForMember(d => d.FencePolygon, opt => opt.MapFrom(src => src.GeoFenceArea.AreaType == GeoFenceAreaType.Polygon ? ((PolygonGeoFenceArea)src.GeoFenceArea).FencePolygon : null))
                .ForMember(d => d.FenceCenter, opt => opt.MapFrom(src => src.GeoFenceArea.AreaType == GeoFenceAreaType.Circular ? ((CircularGeoFenceArea)src.GeoFenceArea).Center : null))
                .ForMember(d => d.RadiusInMeters, opt => opt.MapFrom(src => src.GeoFenceArea.AreaType == GeoFenceAreaType.Circular ? (long?)((CircularGeoFenceArea)src.GeoFenceArea).RadiusInMeters : null))
                .ReverseMap()
                .ForMember(d => d.GeoFenceArea, opt => opt.MapFrom(src =>
                        src.AreaType == GeoFenceAreaType.Circular
                        ? (IGeoFenceArea)new CircularGeoFenceArea { Center = src.FenceCenter, RadiusInMeters = src.RadiusInMeters.Value }
                        : new PolygonGeoFenceArea { FencePolygon = src.FencePolygon }));

            CreateMap<Location, LocationDto>()
                .ReverseMap();

            CreateMap<TrackingPoint, TrackingPointDto>()
                .ForMember(d => d.Time, opt => opt.MapFrom(src => src.DeviceTimestampUtc))
                .ReverseMap();

            CreateMap<TrackingDevice, TrackingDeviceDto>()
                .ReverseMap();

            CreateMap<Trip, TripDto>()
                .ReverseMap();

            CreateMap<TripLeg, TripLegDto>()
                .ReverseMap();

            CreateMap<User, UserDto>()
                .ReverseMap();
        }
    }
}
