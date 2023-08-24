using System.Net.Http.Json;
using Bogus;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests;

public class PUTBook
{
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
    }

    [Test]
    public async Task TestPutBook()
    {
 
        Helper.TriggerRebuild();
        var book = new Book()
        {
            Title = new Faker().Random.Word(),
            Publisher = new Faker().Random.Word(),
            CoverImgUrl = new Faker().Random.Word(),
            BookId = 1
        };
        var sql =
            "insert into library.books (title, publisher, coverimgurl) VALUES (@title, @publisher, @coverImgUrl);";
        using (var conn = Helper.DataSource.OpenConnection())
        {
            conn.Execute(sql, book);
        }
        
        
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PutAsJsonAsync("http://localhost:5000/api/book/1", book);
 
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
            responseObject.Should().BeEquivalentTo(book, Helper.MyBecause(responseObject, book));
            response.IsSuccessStatusCode.Should().BeTrue();
        }

    }
}