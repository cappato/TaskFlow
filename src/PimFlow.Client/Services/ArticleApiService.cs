using System.Net.Http.Json;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.Enums;

namespace PimFlow.Client.Services;

public class ArticleApiService : IArticleApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiEndpoint = "api/articles";

    public ArticleApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
    {
        try
        {
            var articles = await _httpClient.GetFromJsonAsync<IEnumerable<ArticleDto>>(ApiEndpoint);
            return articles ?? Enumerable.Empty<ArticleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting articles: {ex.Message}");
            return Enumerable.Empty<ArticleDto>();
        }
    }

    public async Task<ArticleDto?> GetArticleByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ArticleDto>($"{ApiEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting article {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByAttributeAsync(string attributeName, string value)
    {
        try
        {
            var articles = await _httpClient.GetFromJsonAsync<IEnumerable<ArticleDto>>(
                $"{ApiEndpoint}/attribute?attributeName={attributeName}&value={value}");
            return articles ?? Enumerable.Empty<ArticleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting articles by attribute: {ex.Message}");
            return Enumerable.Empty<ArticleDto>();
        }
    }

    public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, createArticleDto);
            response.EnsureSuccessStatusCode();
            
            var article = await response.Content.ReadFromJsonAsync<ArticleDto>();
            return article ?? throw new InvalidOperationException("Failed to create article");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating article: {ex.Message}");
            throw;
        }
    }

    public async Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{id}", updateArticleDto);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<ArticleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating article: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting article: {ex.Message}");
            return false;
        }
    }
}
