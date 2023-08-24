using Bogus;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests;

public class GETBooks
{
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
    }
    
    [Test]
    public async Task GetAllBooksTest()
    {
        Helper.TriggerRebuild();
        var expected = new List<object>();
        for (var i = 1; i < 10; i++)
        {
            var book = new Book()
            {
                BookId = i,
                Title = new Faker().Random.Word(),
                Publisher = new Faker().Random.Word(),
                CoverImgUrl = new Faker().Random.Word(),
            };
            expected.Add(book);
            var sql = $@" 
            insert into library.books (title, publisher, coverimgurl) VALUES (@title, @publisher, @coverImgUrl);
            ";
            using (var conn = Helper.DataSource.OpenConnection())
            {
                conn.Execute(sql, book);
            }
        }

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync("http://localhost:5000/api/books");
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

        IEnumerable<Book> responseObject;
        try
        {
            responseObject = JsonConvert.DeserializeObject<IEnumerable<Book>>(
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
RESPONSE BODY: {await response.Content.ReadAsStringAsync()}

EXCEPTION:", e);
        }

        using (new AssertionScope())
        {
            responseObject.Should().BeEquivalentTo(expected, Helper.MyBecause(responseObject, expected));
            response.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}