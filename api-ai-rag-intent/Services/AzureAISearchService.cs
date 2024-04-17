using api_ai_rag_intent.Interfaces;
using api_ai_rag_intent.Util;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text;
using System.Text.Json.Serialization;

namespace api_ai_rag_intent.Services
{
    internal class IndexSchema
    {
        [JsonPropertyName("moniker")]
        public string Moniker { get; set; } = string.Empty;

        [JsonPropertyName("db_id")]
        public string DbId { get; set; } = string.Empty;

        [JsonPropertyName("db_name")]
        public string DbName { get; set; } = string.Empty;

        [JsonPropertyName("db_name_vector")]
        public ReadOnlyMemory<float> DbNameVector { get; set; }
    }

    internal class AzureAISearchService : IAzureAISearchService
    {
        private readonly List<string> _defaultVectorFields = new() { "db_name_vector" };

        private readonly SearchIndexClient _indexClient;

        public AzureAISearchService(SearchIndexClient indexClient)
        {
            this._indexClient = indexClient;
        }

        public async Task<string> SimpleVectorSearchAsync(ReadOnlyMemory<float> embedding, string query, string index, int k = 3)
        {
            StringBuilder content = new StringBuilder();

            // Get client for search operations
            SearchClient searchClient = this._indexClient.GetSearchClient(index);

            // Perform the vector similarity search  
            var searchOptions = new SearchOptions
            {
                VectorSearch = new()
                {
                    Queries = { new VectorizedQuery(embedding.ToArray()) { Fields = { "db_name_vector" } } }
                },
                QueryType = SearchQueryType.Full,
                Size = k,
                Select = { "moniker", "db_id", "db_name"  },
            };

            SearchResults<SearchDocument> response;
            try
            {
                response = await searchClient.SearchAsync<SearchDocument>(query, searchOptions);

            }
            catch (Exception ex)
            {
                // Log exception details here
                Console.WriteLine(ex.Message);
                throw; // Re-throw the exception to propagate it further
            }

            //SearchResults<SearchDocument> response = await searchClient.SearchAsync<SearchDocument>(query, searchOptions);

            int count = 0;
            await foreach (SearchResult<SearchDocument> result in response.GetResultsAsync())
            {
                count++;
                Console.WriteLine($"Moniker: {result.Document["moniker"]}");
                Console.WriteLine($"Db_Id: {result.Document["db_id"]}");
                Console.WriteLine($"Db_Name: {result.Document["db_name"]}");
                Console.WriteLine($"Score: {result.Score}\n");
                var thefields = "Moniker: " + result.Document["moniker"] + ", Db_Id: " + result.Document["db_id"] + ", Db_Name: " + result.Document["db_name"];
                //content.AppendLine("document:");
                //var thefields = "Moniker: " + result.Moniker + "Db_Id: " + result.DbId + "Db_Name: " + result.DbName;
                //content.AppendLine(thefields);
                //content.AppendLine("DB_Name: " + result.DbName);

                content.AppendLine(thefields ?? string.Empty);
            }
            Console.WriteLine($"Total Results: {count}");

            return content.ToString();
        }

        public async Task<string> SemanticHybridSearchAsync(ReadOnlyMemory<float> embedding, string query, string index, string semanticconfigname, int k = 3)
        {
            // This is a combination of a semantic and vector search
            StringBuilder content = new StringBuilder();

            try
            {
                SearchClient searchClient = this._indexClient.GetSearchClient(index);

                var searchOptions = new SearchOptions
                {
                    VectorSearch = new()
                    {
                        Queries = { new VectorizedQuery(embedding.ToArray()) { KNearestNeighborsCount = k, Fields = { "vector" } } }
                    },
                    SemanticSearch = new()
                    {
                        SemanticConfigurationName = semanticconfigname,
                        QueryCaption = new(QueryCaptionType.Extractive),
                        QueryAnswer = new(QueryAnswerType.Extractive),
                    },
                    QueryType = SearchQueryType.Semantic,
                    Select = { "moniker", "db_id", "db_name" },

                };

                // Perform search request
                Response<SearchResults<IndexSchema>> response = await searchClient.SearchAsync<IndexSchema>(query, searchOptions);
                List<IndexSchema> results = new();
                // Collect search results
                await foreach (SearchResult<IndexSchema> result in response.Value.GetResultsAsync())
                {
                    if (result.SemanticSearch.RerankerScore > 0.5 || result.Score > 0.03)
                    {
                        results.Add(result.Document);
                        //content.AppendLine(result.Document.ToString());
                    }
                }
                var sortedResults = results
                    .OrderByDescending(result => result.DbId)
                    .Take(3)
                    .ToList();
                foreach (var result in sortedResults)
                {
                    content.AppendLine("document:");
                    var thefields = "Moniker: " + result.Moniker + "Db_Id: " + result.DbId + "Db_Name: " + result.DbName;
                    content.AppendLine(thefields);
                    content.AppendLine("DB_Name: " + result.DbName);
                }

            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Total Results: 0");
            }
            return content.ToString();
        }

        public async Task<string> HybridSearchAsync(ReadOnlyMemory<float> embedding, string query, string index, int k = 100)
        {
            StringBuilder content = new StringBuilder();

            SearchResults<SearchDocument> response;
            try
            {
                // Get client for search operations
                SearchClient searchClient = this._indexClient.GetSearchClient(index);

                // Perform the hybrid search combining text fields and vector field
                var searchOptions = new SearchOptions
                {
                    // Filter = query, // Add text query filter
                    QueryType = SearchQueryType.Full,
                    Size = 80,
                    Select = { "moniker", "db_id", "db_name" }
                };

                // Add vector search query if embedding is provided
                //if (embedding.Length > 0)
                //{
                //    searchOptions.VectorSearch = new()
                //    {
                //        Queries = { new VectorizedQuery(embedding.ToArray()) { Fields = { "db_name_vector" } } }
                //    };
                //}
                // searchOptions.OrderBy.Add("db_id asc"); none of the fields in the index are sortable
                response = await searchClient.SearchAsync<SearchDocument>(query, searchOptions); // Use "*" to search all documents
            }
            catch (Exception ex)
            {
                // Log exception details here
                Console.WriteLine(ex.Message);
                throw; // Re-throw the exception to propagate it further
            }

            int count = 0;
            await foreach (SearchResult<SearchDocument> result in response.GetResultsAsync())
            {
                count++;
                Console.WriteLine($"Moniker: {result.Document["moniker"]}");
                Console.WriteLine($"Db_Id: {result.Document["db_id"]}");
                Console.WriteLine($"Db_Name: {result.Document["db_name"]}");
                Console.WriteLine($"Score: {result.Score}\n");
                var thefields = "Moniker: " + result.Document["moniker"] + ", Db_Id: " + result.Document["db_id"] + ", Db_Name: " + result.Document["db_name"];

                content.AppendLine(thefields ?? string.Empty);
            }
            Console.WriteLine($"Total Results: {count}");

            return content.ToString();
        }

        public async Task<string> HybridSearch2Async(ReadOnlyMemory<float> embedding, string query, string index, int k = 100)
        {
            //***
            // This is a combination of a semantic and vector search
            StringBuilder content = new StringBuilder();
          
            SearchClient searchClient = this._indexClient.GetSearchClient(index);

            //SearchResults<IndexSchema> response;
            //var results = searchClient.Search(query);
           // Response<SearchResults<IndexSchema>> response = await searchClient.SearchAsync<IndexSchema>(query);

            // Perform search request
            Response<SearchResults<IndexSchema>> response = await searchClient.SearchAsync<IndexSchema>(query);
            List<IndexSchema> results = new();
            // Collect search results
            int count = 0;
            await foreach (SearchResult<IndexSchema> result in response.Value.GetResultsAsync())
            {
                if (result.SemanticSearch.RerankerScore > 0.5 || result.Score > 0.03)
                {
                    results.Add(result.Document);
                    //content.AppendLine(result.Document.ToString());
                }
                count++;
                Console.WriteLine($@"Moniker: {result.Document.Moniker}");
                Console.WriteLine($@"Db_Id: {result.Document.DbId}");
                Console.WriteLine($@"Db_Name: {result.Document.DbName}");
                Console.WriteLine($"Score: {result.Score}\n");
                var thefields = "Moniker: " + result.Document.Moniker + ", Database ID: " + result.Document.DbId + ", Db_Name: " + result.Document.DbName + "\n";
                content.AppendLine(thefields ?? string.Empty);
            }
            Console.WriteLine($"Total Results: {count}");

            return content.ToString();

        }
}
}
