using HackerNewsPage.Controllers;
using System;
using Xunit;

using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Test
{
    public class HackerNewsControllerTests 
    {
        [Fact]
        public async Task GetHackerNews_GetPage_First_Page()
        {
            // Arrange
            var memCache = A.Fake<IMemoryCache>();
            var controller = new HackerNewsController(memCache);

            // Act
            var actionResult = await controller.GetPage(0);

            // Assert
            var result = actionResult as OkObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetHackerNews_GetPage_Fifth_Page()
        {
            // Arrange
            var memCache = A.Fake<IMemoryCache>();
            var controller = new HackerNewsController(memCache);

            // Act
            var actionResult = await controller.GetPage(5);

            // Assert
            var result = actionResult as OkObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Fact]
        // Description: attempt to get a page outside the range of values that exist, returns last page
        public async Task GetHackerNews_GetPage_Last_Page_Items()
        {
            // Arrange
            var memCache = A.Fake<IMemoryCache>();
            var controller = new HackerNewsController(memCache);

            // Act
            var actionResult = await controller.GetPage(500);

            // Assert
            var result = actionResult as OkObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
        }


        [Fact]
        // Description: Search for a value that should be there, get an Ok status 
        public async Task GetHackerNews_GetSearch_Find_Value()
        {
            // Arrange
            var memCache = A.Fake<IMemoryCache>();
            var controller = new HackerNewsController(memCache);

            // Act
            var actionResult = await controller.GetSearch("H");

            // Assert
            var result = actionResult as OkObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Fact]
        // Description: Search for a value that shouldnt exist, get a NotFound status
        public async Task GetHackerNews_GetSearch_Dont_Find_Value()
        {
            // Arrange
            var memCache = A.Fake<IMemoryCache>();
            var controller = new HackerNewsController(memCache);

            // Act
            var actionResult = await controller.GetSearch("asdfksdjf;ljk");

            // Assert
            var result = actionResult as NoContentResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.NoContent);
        }


        [Fact]
        // Description: Search for a value that shouldnt exist, get a NotFound status
        public async Task GetHackerNews_GetCount()
        {
            // Arrange
            var memCache = A.Fake<IMemoryCache>();
            var controller = new HackerNewsController(memCache);

            // Act
            var actionResult = await controller.GetCount();

            // Assert
            var result = actionResult as OkObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
        }

    }
}
