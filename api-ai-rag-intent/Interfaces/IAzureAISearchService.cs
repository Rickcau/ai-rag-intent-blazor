﻿namespace api_ai_rag_intent.Interfaces
{
    public interface IAzureAISearchService
    {
        Task<string> SimpleVectorSearchAsync(ReadOnlyMemory<float> embedding, string query, string index, int k = 3);
        Task<string> SemanticHybridSearchAsync(ReadOnlyMemory<float> embedding, string query, string index, string semanticconfigname, int k = 3);
        Task<string> HybridSearchAsync(ReadOnlyMemory<float> embedding, string query, string index, int k = 100);
        Task<string> HybridSearch2Async(ReadOnlyMemory<float> embedding, string query, string index, int k = 100);
    }
}
