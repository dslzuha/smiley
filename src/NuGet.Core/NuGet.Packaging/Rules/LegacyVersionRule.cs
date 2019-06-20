// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using NuGet.Common;

namespace NuGet.Packaging.Rules
{
    /// <summary>
    /// Warn if the version is not parsable by older nuget clients.
    /// </summary>
    /// <remarks>This rule should be removed once more users move to SemVer 2.0.0 capable clients.</remarks>
    internal class LegacyVersionRule : IPackageRule
    {
        public string MessageFormat { get; }

        public LegacyVersionRule(string messageFormat)
        {
            MessageFormat = messageFormat;
        }
        // NuGet 2.12 regex for version parsing.
        private const string LegacyRegex = @"^(?<Version>\d+(\s*\.\s*\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?$";

        public IEnumerable<PackagingLogMessage> Validate(PackageArchiveReader builder)
        {
            var regex = new Regex(LegacyRegex, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            var nuspecReader = builder.NuspecReader;
            if (!regex.IsMatch(nuspecReader.GetVersion().ToFullString()))
            {
                yield return PackagingLogMessage.CreateWarning(
                    string.Format(CultureInfo.CurrentCulture, MessageFormat, nuspecReader.GetVersion().ToFullString()),
                    NuGetLogCode.NU5105);
            }
        }
    }
}