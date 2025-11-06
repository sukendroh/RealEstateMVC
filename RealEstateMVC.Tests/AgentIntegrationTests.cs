using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateMVC.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RealEstateMVC.Tests
{
    public class AgentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AgentIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // Set environment to "Testing" so Program.cs uses InMemory provider
                builder.UseSetting("environment", "Testing");

                builder.ConfigureServices(services =>
                {
                    // Remove any existing RealEstateContext registrations
                    var optionsDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<RealEstateContext>));
                    if (optionsDescriptor != null)
                        services.Remove(optionsDescriptor);

                    var contextDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(RealEstateContext));
                    if (contextDescriptor != null)
                        services.Remove(contextDescriptor);

                    // Register with InMemory provider only
                    services.AddDbContext<RealEstateContext>(options =>
                        options.UseInMemoryDatabase("IntegrationDb"));
                });
            });
        }

        [Fact]
        public async Task CreateAgent_Then_IndexShowsAgent()
        {
            var client = _factory.CreateClient();

            var form = new Dictionary<string, string>
            {
                { "FirstName", "Ivy" },
                { "LastName", "Lane" },
                { "Email", "ivy@x.com" },
                { "Company", "Acme" }
            };

            var content = new FormUrlEncodedContent(form);
            content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await client.PostAsync("/Agent/Create", content);

            var body = await response.Content.ReadAsStringAsync();
            System.Console.WriteLine("CreateAgent response status: " + response.StatusCode);
            System.Console.WriteLine("CreateAgent response body:");
            System.Console.WriteLine(body);

            // Accept either Redirect or OK depending on controller behavior
            Assert.True(
                response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.OK,
                $"Expected Redirect or OK, but got {response.StatusCode}"
            );

            // Fetch Index page and log if error occurs
            var indexResponse = await client.GetAsync("/Agent/Index");
            var indexHtml = await indexResponse.Content.ReadAsStringAsync();
            System.Console.WriteLine("Index response status: " + indexResponse.StatusCode);
            System.Console.WriteLine("Index response body:");
            System.Console.WriteLine(indexHtml);

            // Don’t immediately throw — log first
            if (indexResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException(
                    $"Index returned {indexResponse.StatusCode}. Body:\n{indexHtml}");
            }

            Assert.Contains("Ivy", indexHtml);
        }

        [Fact]
        public async Task CreateAgent_InvalidForm_ReturnsViewWithErrors()
        {
            var client = _factory.CreateClient();

            var form = new Dictionary<string, string>
            {
                { "FirstName", "" },
                { "LastName", "" },
                { "Email", "bad" },
                { "Company", "" }
            };

            var content = new FormUrlEncodedContent(form);
            content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await client.PostAsync("/Agent/Create", content);
            var body = await response.Content.ReadAsStringAsync();

            System.Console.WriteLine("InvalidForm response status: " + response.StatusCode);
            System.Console.WriteLine("InvalidForm response body:");
            System.Console.WriteLine(body);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("First name is required", body);
            Assert.Contains("Last name is required", body);
            Assert.Contains("Invalid email format", body);
            Assert.Contains("The Company field is required", body);
        }
    }
}
