// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Trackable.Services
{
    public interface IInstrumentationService
    {
        Task<bool?> GetInstrumentationApproval();

        Task SetInstrumentationApproval(bool accepted);

        Task PostInstrumentationAsync();

        Task PostExceptionAsync(string Exception);

        Task PostWarningAsync(string Exception);
    }
}
