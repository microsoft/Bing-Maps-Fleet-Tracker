namespace Trackable.Web.DTOs
{
    public class AssetPropertiesDto
    {
        public int Id { get; set; }

        public double? AssetHeight { get; set; }

        public double? AssetWidth { get; set; }

        public double? AssetLength { get; set; }

        public double? AssetWeight { get; set; }

        public int? AssetAxels { get; set; }

        public int? AssetTrailers { get; set; }

        public bool? AssetSemi { get; set; }

        public double? AssetMaxGradient { get; set; }

        public double? AssetMinTurnRadius { get; set; }
    }
}
