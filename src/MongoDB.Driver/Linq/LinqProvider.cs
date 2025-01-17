﻿/* Copyright 2010-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;

namespace MongoDB.Driver.Linq
{
    /// <summary>
    /// Represents the different LINQ providers.
    /// </summary>
    public enum LinqProvider
    {
        /// <summary>
        /// The LINQ provider that was first shipped with version 2.0 of the driver,
        /// </summary>
        V2 = 2,

        /// <summary>
        /// The LINQ provider that is planned to be the default in version 3.0 of the driver and can be optionally used before that.
        /// </summary>
        V3 = 3
    }

    internal static class LinqProviderExtensions
    {
        public static LinqProviderAdapter GetAdapter(this LinqProvider linqProvider) =>
            linqProvider switch
            {
                LinqProvider.V2 => LinqProviderAdapter.V2,
                LinqProvider.V3 => LinqProviderAdapter.V3,
                _ => throw new ArgumentException($"Unknown LINQ provider: {linqProvider}.", nameof(linqProvider))
            };
    }
}
