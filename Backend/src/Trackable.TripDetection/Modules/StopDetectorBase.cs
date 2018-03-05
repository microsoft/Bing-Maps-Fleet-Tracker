// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Configurations;
using Trackable.Repositories;
using Trackable.Models;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    abstract class StopDetectorBase : ModuleBase<TripDetectionContext, TripDetectionContext>
    {
        protected abstract Task<TripDetectionContext> ProcessInternal(TripDetectionContext input);

        public async override Task<TripDetectionContext> Process(TripDetectionContext input, ILogger logger)
        {
            logger.LogDebugSerialize("Recieved input with segments {0}", input.TripSegments);

            input.TripSegments = CombineSegments(input);

            logger.LogDebugSerialize("Combined input segments into {0}", input.TripSegments);

            var result = await ProcessInternal(input);

            logger.LogDebugSerialize("Recieved output trip segments {0}", result.TripSegments);

            result.TripSegments = CombineSegments(result);

            logger.LogDebugSerialize("Combined output trip segments into {0}", result.TripSegments);

            return result;
        }

        private IList<TripSegmentBase> CombineSegments(TripDetectionContext context)
        {
            if (context.TripSegments == null || !context.TripSegments.Any())
            {
                if (context.FilteredOrderedPoints == null || !context.FilteredOrderedPoints.Any())
                {
                    return new List<TripSegmentBase>();
                }
                else
                {
                    return new List<TripSegmentBase> { new MovingSegment(context.FilteredOrderedPoints) };
                }
            }

            var originalSegments = context.TripSegments;
            var combinedSegments = new List<TripSegmentBase>();

            var points = new List<TrackingPoint>();
            for (int i = 0; i < originalSegments.Count - 1; i++)
            {
                points.AddRange(originalSegments[i].Points);

                if (originalSegments[i].IsMovingSegment != originalSegments[i + 1].IsMovingSegment)
                {
                    if (originalSegments[i].IsMovingSegment)
                    {
                        combinedSegments.Add(new MovingSegment(points));
                    }
                    else
                    {
                        combinedSegments.Add(new StoppedSegment(points));
                    }

                    points = new List<TrackingPoint>();
                }
            }

            points.AddRange(originalSegments.Last().Points);

            if (originalSegments.Last().IsMovingSegment)
            {
                combinedSegments.Add(new MovingSegment(points));
            }
            else
            {
                combinedSegments.Add(new StoppedSegment(points));
            }

            return combinedSegments;
        }
    }
}
