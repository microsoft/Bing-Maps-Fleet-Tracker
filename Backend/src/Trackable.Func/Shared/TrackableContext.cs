// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Trackable.Repositories;
using Trackable.TripDetection;
using Trackable.EntityFramework;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace Trackable.Func.Shared
{
    public class TrackableContext : IDisposable
    {
        public TrackableDbContext DbContext { get; }

        public TripDetectorFactory TripDetectorFactory { get; }

        public ILoggerFactory LoggerFactory { get; }

        public IAssetRepository AssetRepository { get; }

        public ILocationRepository LocationRepository { get; }

        public ITrackingPointRepository TrackingPointRepository { get; }

        public IConfigurationRepository ConfigurationRepository { get; }

        public ITripRepository TripRepository { get; }

        private readonly TraceWriter traceWriter;

        public TrackableContext(TraceWriter writer)
        {
            try
            {
                if (Utils.GetAppSetting("Environment") == "Local")
                {
                    TrackableDbContext.LoadSqlServerTypes(Environment.CurrentDirectory);
                }
                else
                {
                    TrackableDbContext.LoadSqlServerTypes(@"D:\home\site\wwwroot");
                }

                this.DbContext = new TrackableDbContext(Utils.GetAppSetting("DatabaseConnection"));
                this.LocationRepository = RepositoryFactory.CreateLocationRepository(this.DbContext);
                this.TrackingPointRepository = RepositoryFactory.CreateTrackingPointRepository(this.DbContext);
                this.ConfigurationRepository = RepositoryFactory.CreateConfigurationRepository(this.DbContext);
                this.TripRepository = RepositoryFactory.CreateTripRepository(this.DbContext);
                this.AssetRepository = RepositoryFactory.CreateAssetRepository(this.DbContext);
                this.TripDetectorFactory = new TripDetectorFactory(
                    this.ConfigurationRepository,
                    this.TripRepository,
                    this.TrackingPointRepository,
                    this.LocationRepository,
                    Utils.GetAppSetting("BingMapsKey"));

                this.LoggerFactory = new LoggerFactory();
                this.LoggerFactory.AddProvider(new TraceWriterProvider(writer, null));
                this.traceWriter = writer;
            }
            catch (Exception e)
            {
                writer.Error("Error initializing trackable context", e);
                throw;
            }
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public async Task ExecuteAsync(Func<Task> t)
        {
            try
            {
                await t();
            }
            catch (Exception e)
            {
                this.traceWriter.Error("Failed to execute pipeline", e);
            }
        }
    }
}
