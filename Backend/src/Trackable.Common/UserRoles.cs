namespace Trackable.Common
{
    public class UserRoles
    {
        public const string Blocked = "Blocked";
        public const string Pending = "Pending";
        public const string DeviceRegistration = "DeviceRegistration";
        public const string TrackingDevice = "TrackingDevice";
        public const string Viewer = "Viewer";
        public const string Administrator = "Administrator";
        public const string Owner = "Owner";

        public static string[] Roles => new string[]
        {
            Blocked,
            Pending,
            DeviceRegistration,
            TrackingDevice,
            Viewer,
            Administrator,
            Owner
        };
    }
}
