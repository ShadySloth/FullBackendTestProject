using Bogus;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace Tests;

public class DELETEBook
{
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
    }

    [Test]
    public async Task DeleteBookTest()
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
            response = await _httpClient.DeleteAsync("http://localhost:5000/api/book/1");
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

        using (new AssertionScope())
        {
            using (var conn = Helper.DataSource.OpenConnection())
            {
                (conn.ExecuteScalar<int>("SELECT COUNT(*) FROM library.books WHERE bookId = 1;", book) == 0)
                    .Should()
                    .BeTrue("Expected to see that there are no books with ID 1, but there was");
            }

            response.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}