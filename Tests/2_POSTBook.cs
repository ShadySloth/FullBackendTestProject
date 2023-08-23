using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests;

public class POSTBook
{
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
    }

    [Test]
    public async Task PostBookTest()
    {
        Helper.TriggerRebuild();
        var input = new Book()
        {
            Title = "Mock book title",
            Publisher = "Mock book publisher",
            CoverImgUrl = "https://something.com/some.jpg"
        };
        var expected = new Book()
        {
            Title = input.Title,
            Publisher = input.Publisher,
            CoverImgUrl = input.CoverImgUrl,
            BookId = 1
        };

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/book", input);

        }
        catch (HttpRequestException e)
        {
            throw new Exception($@"
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
It looks like you failed to get a response from the API.
Are you 100% sure the API is already running on localhost port 5000?
Below is the inner exception.
Best regards, Alex
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
", e);
        }

        Book responseObject;
        try
        {
            responseObject = JsonConvert.DeserializeObject<Book>(
                await response.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            throw new Exception($@"
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
Hey buddy, I've tried to take the response body from the API and turn into a class object,
but that failed. Below is what you sent me + the inner exception.

Best regards, Alex
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
RESPONSE BODY: {response.Content}

EXCEPTION:
", e);
        }


        using (new AssertionScope())
        {
            responseObject.Should().BeEquivalentTo(expected, Helper.MyBecause(responseObject, expected));
            response.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}