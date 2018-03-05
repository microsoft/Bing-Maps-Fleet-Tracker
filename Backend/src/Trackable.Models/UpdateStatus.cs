// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.Models
{
    public class UpdateStatus
    {
        public bool UpdateRequired
        {
            get
            {
                return CurrentVersionHash != LatestVersionHash || CurrentVersionName!= LatestVersionName;
            }
        }

        public string CurrentVersionName { get; set; }

        public string CurrentVersionHash { get; set; }

        public string LatestVersionName { get; set; }

        public string LatestVersionHash { get; set; }

        public Uri UpdateUrl { get; set; }
    }
}
