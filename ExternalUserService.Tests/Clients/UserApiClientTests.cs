using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExternalUserService.Clients;
using ExternalUserService.Models;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace ExternalUserService.Tests.Clients
{
    public class UserApiClientTests
    {
        private UserApiClient CreateClient(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };

            return new UserApiClient(httpClient);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenApiIsSuccessful()
        {
            // Arrange
            var json = @"{ ""data"": { ""id"": 1, ""email"": ""george.bluth@reqres.in"", ""first_name"": ""George"", ""last_name"": ""Bluth"" } }";
            var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });

            // Act
            var response = await client.GetUserByIdAsync(1);

            // Assert
            response.Should().NotBeNull();
            response.Data.Should().NotBeNull();
            response.Data.Id.Should().Be(1);
            response.Data.FirstName.Should().Be("George");
            response.Data.LastName.Should().Be("Bluth");
            response.Data.Email.Should().Be("george.bluth@reqres.in");
        }

        [Fact]
        public async Task GetUserByIdAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var client = CreateClient(new HttpResponseMessage(HttpStatusCode.NotFound));

            // Act
            Func<Task> act = async () => await client.GetUserByIdAsync(999);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
        }
    }
}
