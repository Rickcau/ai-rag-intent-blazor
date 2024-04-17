
using api_ai_rag_intent.Interfaces;
using api_ai_rag_intent.Plugins;
using api_ai_rag_intent.Services;
using api_ai_rag_intent.Util;
using Azure.Search.Documents.Indexes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

//
string _apiDeploymentName = Helper.GetEnvironmentVariable("ApiDeploymentName");
string _apiEndpoint = Helper.GetEnvironmentVariable("ApiEndpoint");
string _apiKey = Helper.GetEnvironmentVariable("ApiKey");
string _apiAISearchEndpoint = Helper.GetEnvironmentVariable("AISearchURL");
string _apiAISearchKey = Helper.GetEnvironmentVariable("AISearchKey");
string _textEmbeddingName = Helper.GetEnvironmentVariable("EmbeddingName");

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddTransient<Kernel>(s =>
        {
            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                _apiDeploymentName,
                _apiEndpoint,
                _apiKey
                );
            builder.Services.AddSingleton<SearchIndexClient>(s =>
            {
                string endpoint = _apiAISearchEndpoint;
                string apiKey = _apiAISearchKey;
                return new SearchIndexClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
            });

            // Add Singleton for AzureAISearch 
            builder.Services.AddSingleton<IAzureAISearchService, AzureAISearchService>();

#pragma warning disable SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            builder.AddAzureOpenAITextEmbeddingGeneration(_textEmbeddingName, _apiEndpoint, _apiKey);
#pragma warning restore SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            // builder.Plugins.AddFromType<DBQueryPlugin>();  
           //  builder.Plugins.AddFromType<UniswapV3SubgraphPlugin>();
            builder.Plugins.AddFromType<AzureAISearchPlugin>();

            return builder.Build();
        });

        services.AddSingleton<IChatCompletionService>(sp =>
                     sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());
        const string systemmsg = @$"You are a friendly AI assistant that can summarize data for users when that ask about moniker, db_id and db_name.  You are to try to summarize to the best of your ability";
        services.AddSingleton<ChatHistory>(s =>
        {
            var chathistory = new ChatHistory();
            chathistory.AddSystemMessage(systemmsg);
            return chathistory;
        });

    })
    .Build();

host.Run();

