// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using api_ai_rag_intent.Functions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace api_ai_rag_intent.Plugins.SqlServer;

/// <summary>
/// Responsible for producing a connection string for the requested schema.
/// </summary>
internal sealed class SqlConnectionProvider
{

    /// <summary>
    /// Factory method for <see cref="IServiceCollection"/>
    /// </summary>
    public static Func<IServiceProvider, SqlConnectionProvider> Create()
    {
        return CreateProvider;

        SqlConnectionProvider CreateProvider(IServiceProvider provider)
        {
            return new SqlConnectionProvider();
        }
    }

    public SqlConnectionProvider()
    {
    }

    /// <summary>
    /// Factory method for producing a live SQL connection instance.
    /// </summary>
    /// <param name="schemaName">The schema name (which should match a corresponding connectionstring setting).</param>
    /// <returns>A <see cref="SqlConnection"/> instance in the "Open" state.</returns>
    /// <remarks>
    /// Connection pooling enabled by default makes re-establishing connections
    /// relatively efficient.
    /// </remarks>
    public async Task<SqlConnection> ConnectAsync(string schemaName)
    {
        var connectionString =
           ChatProvider.Configuration.GetConnectionString(schemaName) ??
            throw new InvalidDataException($"Missing configuration for connection-string: {schemaName}");

        var connection = new SqlConnection(connectionString);

        try
        {
            await connection.OpenAsync().ConfigureAwait(false);
        }
        catch
        {
            connection.Dispose();
            throw;
        }

        return connection;
    }
}
