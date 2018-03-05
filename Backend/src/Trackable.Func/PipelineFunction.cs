// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Trackable.Func.Shared;
using Microsoft.Extensions.Logging;
using System.Linq;
using Trackable.Common;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Trackable.TripDetection;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Trackable.Func
{
    public static class PipelineFunction
    {
        [FunctionName("PipelineInitiator")]
        public async static Task Initiate(
            [TimerTrigger("0 0/30 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Queue("%TripDetectionQueue%")]ICollector<string> outgoingMessage,
            Binder binder,
            TraceWriter log)
        {
            using (var trackableContext = new TrackableContext(log))
            {
                await trackableContext.ExecuteAsync(async () =>
                {
                    var logger = trackableContext.LoggerFactory.CreateLogger("PipelineExecuter");
                    logger.LogInformation("Running pipleine initiator at {0}", myTimer.ScheduleStatus.Last);

                    // Load asset ids and configured trip detector
                    var tripDetector = await trackableContext.TripDetectorFactory.Create();
                    var assetIds = (await trackableContext.AssetRepository.GetAllAsync()).Select(a => a.Id);
                    logger.LogDebugSerialize("Loaded AssetIds {0}", assetIds);

                    // Create a new message for each assetId
                    foreach (var assetId in assetIds)
                    {
                        var blobPath = $"{ myTimer.ScheduleStatus.Last.ToString() }::{ assetId }";
                        var blobAttributes = new Attribute[]
                        {
                            new BlobAttribute( $"{Utils.GetAppSetting("TripDetectionContainerName")}/{blobPath}", FileAccess.Write)
                        };

                        using (var outStream = await binder.BindAsync<Stream>(blobAttributes))
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(outStream, new AzurePipelineState
                            {
                                TripDetectorType = (int)tripDetector.TripDetectorType,
                                ModuleIndex = 0,
                                Payload = assetId
                            });
                            outStream.Close();
                        }

                        logger.LogInformation("wrote blob for asset {0}", assetId);

                        outgoingMessage.Add(blobPath);

                        logger.LogInformation("wrote queue message for asset {0}", assetId);

                    }
                });
            }
        }

        [FunctionName("PipelineExecutor")]
        public async static Task Executor(
            [QueueTrigger("%TripDetectionQueue%")]string incomingMessage,
            [Blob("%TripDetectionContainerName%/{queueTrigger}")]CloudBlockBlob payloadBlob,
            [Queue("%TripDetectionQueue%")]ICollector<string> outgoingMessage,
            TraceWriter log)
        {
            using (var trackableContext = new TrackableContext(log))
            {
                await trackableContext.ExecuteAsync(async () =>
                {
                    var logger = trackableContext.LoggerFactory.CreateLogger("PipelineExecuter");
                    logger.LogInformation("Running module executor for incoming message {0}", incomingMessage);

                    // Retreive input blob contents
                    AzurePipelineState pipelineState;
                    var formatter = new BinaryFormatter();
                    formatter.Binder = new AdvancedSerializationBinder();
                    using (var inputMemoryStream = new MemoryStream())
                    {
                        await payloadBlob.DownloadToStreamAsync(inputMemoryStream);

                        inputMemoryStream.Seek(0, SeekOrigin.Begin);

                        pipelineState = (AzurePipelineState)formatter.Deserialize(inputMemoryStream);
                    }

                    logger.LogDebugSerialize("State rehydrated {0}", pipelineState);

                    // Get trip detector
                    var tripDetector = await trackableContext.TripDetectorFactory.Create((TripDetectorType)pipelineState.TripDetectorType);
                    var pipeline = new Pipeline(trackableContext.LoggerFactory);

                    // Execute pipeline
                    var output = await pipeline.ExecuteModule(tripDetector.GetModuleLoaders().ElementAt(pipelineState.ModuleIndex), pipelineState.Payload);

                    logger.LogDebugSerialize("Executed module with index {0} and recieved output {1}", pipelineState.ModuleIndex, output);

                    pipelineState.ModuleIndex++;
                    pipelineState.Payload = output;

                    if (pipelineState.ModuleIndex < tripDetector.GetModuleLoaders().Count())
                    {
                        using (var outStream = new MemoryStream())
                        {
                            formatter.Serialize(outStream, pipelineState);
                            outStream.Seek(0, SeekOrigin.Begin);
                            await payloadBlob.UploadFromStreamAsync(outStream);
                        }

                        logger.LogInformation("Wrote blob output for message {0}", incomingMessage);

                        outgoingMessage.Add(incomingMessage);

                        logger.LogInformation("Wrote queue output for message {0}", incomingMessage);
                    }
                    else
                    {
                        logger.LogInformation("Finished processing {0}", incomingMessage);
                    }
                });
            }
        }
    }
}
