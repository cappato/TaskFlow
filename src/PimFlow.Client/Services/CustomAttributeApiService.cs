using System.Net.Http.Json;
using System.Text.Json;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.Common;

namespace PimFlow.Client.Services;

public class CustomAttributeApiService : ICustomAttributeApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string ApiEndpoint = "api/customattributes";

    public CustomAttributeApiService(HttpClient httpClient, JsonSerializerOptions jsonOptions)
    {
        _httpClient = httpClient;
        _jsonOptions = jsonOptions;
    }

    public async Task<IEnumerable<CustomAttributeDto>> GetAllAttributesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<CustomAttributeDto>>>(ApiEndpoint, _jsonOptions);
            return response?.Data ?? Enumerable.Empty<CustomAttributeDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting attributes: {ex.Message}");
            return Enumerable.Empty<CustomAttributeDto>();
        }
    }

    public async Task<IEnumerable<CustomAttributeDto>> GetActiveAttributesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<CustomAttributeDto>>>($"{ApiEndpoint}/active", _jsonOptions);
            return response?.Data ?? Enumerable.Empty<CustomAttributeDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting active attributes: {ex.Message}");
            return Enumerable.Empty<CustomAttributeDto>();
        }
    }

    public async Task<CustomAttributeDto?> GetAttributeByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CustomAttributeDto>($"{ApiEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting attribute {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<CustomAttributeDto> CreateAttributeAsync(CreateCustomAttributeDto createAttributeDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, createAttributeDto);
            response.EnsureSuccessStatusCode();
            
            var attribute = await response.Content.ReadFromJsonAsync<CustomAttributeDto>();
            return attribute ?? throw new InvalidOperationException("Failed to create attribute");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating attribute: {ex.Message}");
            throw;
        }
    }

    public async Task<CustomAttributeDto?> UpdateAttributeAsync(int id, UpdateCustomAttributeDto updateAttributeDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{id}", updateAttributeDto);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<CustomAttributeDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating attribute: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAttributeAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting attribute: {ex.Message}");
            return false;
        }
    }
}
