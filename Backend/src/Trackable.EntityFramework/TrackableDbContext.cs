using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.IO;
using System.Threading.Tasks;

namespace Trackable.EntityFramework
{
    public class TrackableDbContext : DbContext
    {
        public TrackableDbContext(string connString) : base(connString)
        {
            // the terrible hack from
            // http://robsneuron.blogspot.nl/2013/11/entity-framework-upgrade-to-6.html
            var ensureDLLIsCopied =
                    System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public DbSet<AssetData> Assets { get; set; }

        public DbSet<TrackingDeviceData> TrackingDevices { get; set; }

        public DbSet<TrackingPointData> TrackingPoints { get; set; }

        public DbSet<TagData> Tags { get; set; }

        public DbSet<TripData> Trips { get; set; }

        public DbSet<TripLegData> TripLegs { get; set; }

        public DbSet<LocationData> Locations { get; set; }

        public DbSet<ConfigurationData> Configurations { get; set; }

        public DbSet<UserData> Users { get; set; }

        public DbSet<RoleData> Roles { get; set; }

        public DbSet<GeoFenceData> Fences { get; set; }

        public DbSet<GeoFenceUpdateData> GeoFenceUpdates { get; set; }

        public DbSet<AssetPropertiesData> AssetProperties { get; set; }

        public DbSet<DeploymentIdData> DeploymentId { get; set; }

        public DbSet<TokenData> Tokens { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                this.LogValidationErrors(e);
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                return await base.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                this.LogValidationErrors(e);
                throw;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetData>()
                .HasOptional(a => a.TrackingDevice)
                .WithOptionalPrincipal(d => d.Asset)
                .Map(p => p.MapKey("AssetId"));

            modelBuilder.Entity<AssetData>()
                .HasOptional(a => a.AssetProperties)
                .WithOptionalPrincipal(d => d.Asset)
                .Map(p => p.MapKey("AssetId"));

            modelBuilder.Entity<TripData>()
                .HasRequired(t => t.EndLocation)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TripData>()
               .HasRequired(t => t.StartLocation)
               .WithMany()
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<TripLegData>()
               .HasRequired(t => t.StartLocation)
               .WithMany()
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<TripLegData>()
               .HasRequired(t => t.EndLocation)
               .WithMany()
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrackingPointData>()
                .HasRequired(d => d.TrackingDevice)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrackingPointData>()
                .HasRequired(d => d.Asset)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrackingDeviceData>()
                .HasOptional(d => d.LatestPosition)
                .WithOptionalDependent()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssetData>()
                .HasOptional(d => d.LatestPosition)
                .WithOptionalDependent()
                .WillCascadeOnDelete(false);

            Database.SetInitializer<TrackableDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        private void LogValidationErrors(DbEntityValidationException execption)
        {
            foreach (var eve in execption.EntityValidationErrors)
            {
                System.Diagnostics.Trace.TraceError(
                    $"Entity of type {eve.Entry.Entity.GetType().Name} in state {eve.Entry.State} has the following validation errors:");

                foreach (var ve in eve.ValidationErrors)
                {
                    System.Diagnostics.Trace.TraceError($"- Property: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                }
            }
        }

        public static void LoadSqlServerTypes(string rootPath)
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(rootPath);

            SqlProviderServices.SqlServerTypesAssemblyName =
                "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        }
    }
}