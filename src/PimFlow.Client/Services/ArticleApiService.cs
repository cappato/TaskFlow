using System.Net.Http.Json;
using System.Text.Json;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.DTOs.Pagination;
using PimFlow.Shared.Enums;
using PimFlow.Shared.Common;

namespace PimFlow.Client.Services;

public class ArticleApiService : IArticleApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string ApiEndpoint = "api/articles";

    public ArticleApiService(HttpClient httpClient, JsonSerializerOptions jsonOptions)
    {
        _httpClient = httpClient;
        _jsonOptions = jsonOptions;
    }

    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<ArticleDto>>>(ApiEndpoint, _jsonOptions);
            return response?.Data ?? Enumerable.Empty<ArticleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting articles: {ex.Message}");
            return Enumerable.Empty<ArticleDto>();
        }
    }

    public async Task<PagedResponse<ArticleDto>> GetArticlesPagedAsync(PagedRequest request)
    {
        try
        {
            var queryParams = new List<string>();

            queryParams.Add($"pageNumber={request.PageNumber}");
            queryParams.Add($"pageSize={request.PageSize}");

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(request.SearchTerm)}");

            if (!string.IsNullOrWhiteSpace(request.SortBy))
                queryParams.Add($"sortBy={Uri.EscapeDataString(request.SortBy)}");

            if (!string.IsNullOrWhiteSpace(request.SortDirection))
                queryParams.Add($"sortDirection={Uri.EscapeDataString(request.SortDirection)}");

            var queryString = string.Join("&", queryParams);
            var url = $"{ApiEndpoint}/paged?{queryString}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResponse<ArticleDto>>>(url, _jsonOptions);
            return response?.Data ?? PagedResponse<ArticleDto>.Empty(request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting paged articles: {ex.Message}");
            return PagedResponse<ArticleDto>.Empty(request.PageNumber, request.PageSize);
        }
    }

    public async Task<ArticleDto?> GetArticleByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<ArticleDto>>($"{ApiEndpoint}/{id}");
            return response?.Data;
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
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<ArticleDto>>>(
                $"{ApiEndpoint}/attribute?attributeName={attributeName}&value={value}");
            return response?.Data ?? Enumerable.Empty<ArticleDto>();
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

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ArticleDto>>();
            return apiResponse?.Data ?? throw new InvalidOperationException("Failed to create article");
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

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ArticleDto>>();
            return apiResponse?.Data;
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
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return apiResponse?.IsSuccess ?? false;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting article: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByBrandAsync(string brand)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<ArticleDto>>>(
                $"{ApiEndpoint}/brand/{Uri.EscapeDataString(brand)}");
            return response?.Data ?? Enumerable.Empty<ArticleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting articles by brand: {ex.Message}");
            return Enumerable.Empty<ArticleDto>();
        }
    }

    public async Task<IEnumerable<ArticleDto>> SearchArticlesAsync(string searchTerm)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<ArticleDto>>>(
                $"{ApiEndpoint}/search?term={Uri.EscapeDataString(searchTerm)}");
            return response?.Data ?? Enumerable.Empty<ArticleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching articles: {ex.Message}");
            return Enumerable.Empty<ArticleDto>();
        }
    }
}
