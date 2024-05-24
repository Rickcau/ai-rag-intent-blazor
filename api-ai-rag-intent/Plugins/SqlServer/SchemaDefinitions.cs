// Copyright (c) Microsoft. All rights reserved.

using api_ai_rag_intent.Functions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace api_ai_rag_intent.Plugins.SqlServer;

/// <summary>
/// Defines the schemas initialized by the console.
/// </summary>
internal static class SchemaDefinitions
{
    /// <summary>
    /// Enumerates the names of the schemas to be registered with the console.
    /// </summary>
    /// <remarks>
    /// After testing with the sample data-sources, try one of your own!
    /// </remarks>
    public static IEnumerable<string> GetNames()
    {
        var schemas = ChatProvider.Configuration.GetSection("Schemas").Get<string[]>() ?? Array.Empty<string>();
        return schemas;
    }
}
