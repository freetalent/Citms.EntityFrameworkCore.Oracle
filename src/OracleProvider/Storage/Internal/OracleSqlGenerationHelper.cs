// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Oracle.Storage.Internal
{
    public class OracleSqlGenerationHelper : RelationalSqlGenerationHelper
    {
        public OracleSqlGenerationHelper([NotNull] RelationalSqlGenerationHelperDependencies dependencies)
            : base(dependencies)
        {
        }

        public override string GenerateParameterName(string name)
        {
            while (name.StartsWith("_", StringComparison.Ordinal)
                   || Regex.IsMatch(name, @"^\d"))
            {
                name = name.Substring(1);
            }
            
            var parameterName= ":" + name;
            if (parameterName.Length > 30)
            {
                return parameterName.Substring(0, 30);
            }
            else
            {
                return parameterName;
            }
        }

        public override void GenerateParameterName(StringBuilder builder, string name)
        {
            builder.Append(GenerateParameterName(name));
        }

        public override string BatchTerminator => "GO" + Environment.NewLine + Environment.NewLine;

        public override string DelimitIdentifier(string identifier)
            => $"\"{EscapeIdentifier(Check.NotEmpty(identifier, nameof(identifier)).ToUpper())}\""; // Interpolation okay; strings

        public override void DelimitIdentifier(StringBuilder builder, string identifier)
        {
            Check.NotEmpty(identifier, nameof(identifier));

            builder.Append('"');
            EscapeIdentifier(builder, identifier.ToUpper());
            builder.Append('"');
        }
    }
}
