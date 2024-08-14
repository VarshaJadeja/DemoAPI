using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

namespace Test;

public class ProductControllerTest : IClassFixture<InMemoryWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly InMemoryWebApplicationFactory<Program> _factory;
    private readonly IMongoDatabase _database;

    public ProductControllerTest(InMemoryWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _database = factory.Services.GetRequiredService<IMongoDatabase>();
    }

    [Fact]
    public async Task Get_ReturnsListOfProducts()
    {
        DatabaseSetup.Seed(_database);
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var actualProducts = JsonConvert.DeserializeObject<List<ProductDetails>>(responseString);

        actualProducts.Should().HaveCountGreaterThan(0);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task Login_Successful_Test()
    {
        DatabaseSetup.SeedUser(_database);
        // Arrange
        var loginRequest = new LoginRequest
        {
            UserName = "123",
            Password = "123"
        };
        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("api/User/login", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseString);

        Assert.NotNull(loginResponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
