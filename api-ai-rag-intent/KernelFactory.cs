// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using api_ai_rag_intent.Plugins.SqlServer;
using api_ai_rag_intent.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;

namespace api_ai_rag_intent;

/// <summary>
/// Responsible for initializing Semantic <see cref="Kernel"/> based on the configuration.
/// </summary>
internal static class KernelFactory
{
    // Azure settings
    private const string SettingNameAzureApiKey = "ApiKey";
    private const string SettingNameAzureEndpoint = "ApiEndpoint";
    private const string SettingNameAzureModelCompletion = "ApiDeploymentName";
    private const string SettingNameAzureModelEmbedding = "EmbeddingName";

    // Open AI settings
    private const string SettingNameOpenAIApiKey = "OPENAI_API_KEY";
    private const string SettingNameOpenAIModelCompletion = "OPENAI_API_COMPLETION_MODEL";
    private const string SettingNameOpenAIModelEmbedding = "OPENAI_API_EMBEDDING_MODEL";

    /// <summary>
    /// Penalty for using any model less than GPT4 for SQL generation.
    /// </summary>
    private const string DefaultChatModel = "gpt-4o";

    private const string DefaultEmbedModel = "text-embedding-ada-002";

    /// <summary>
    /// Factory method for <see cref="IServiceCollection"/>
    /// </summary>
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public static ISemanticTextMemory CreateMemory(IServiceProvider provider)
    {
        var builder = new MemoryBuilder();

        var loggerFactory = provider.GetService<ILoggerFactory>();
        if (loggerFactory != null)
        {
            builder.WithLoggerFactory(loggerFactory);
        }

        builder.WithMemoryStore(new VolatileMemoryStore());

        var apikey = Helper.GetEnvironmentVariable(SettingNameAzureApiKey);
        if (!string.IsNullOrWhiteSpace(apikey))
        {
            var endpoint = Helper.GetEnvironmentVariable(SettingNameAzureEndpoint) ??
                           throw new InvalidDataException($"No endpoint configured in {SettingNameAzureEndpoint}.");

            var modelEmbedding =
                Helper.GetEnvironmentVariable(SettingNameAzureModelEmbedding) ??
                DefaultEmbedModel;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            builder.WithAzureOpenAITextEmbeddingGeneration(modelEmbedding, endpoint, apikey);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            return builder.Build();
        }

        apikey = Helper.GetEnvironmentVariable(SettingNameOpenAIApiKey);
        if (!string.IsNullOrWhiteSpace(apikey))
        {
            var modelEmbedding =
                Helper.GetEnvironmentVariable(SettingNameOpenAIModelEmbedding) ??
                DefaultEmbedModel;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            builder.WithOpenAITextEmbeddingGeneration(modelEmbedding, apikey);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            return builder.Build();
        }

        throw new InvalidDataException($"No api-key configured in {SettingNameAzureApiKey} or {SettingNameOpenAIApiKey}.");
    }
}
