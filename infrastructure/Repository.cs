using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure;

public class Repository
{
    private readonly NpgsqlDataSource _dataSource;

    public Repository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<Book> GetAllBooks()
    {
        var sql = $@"select * from library.books;";
        
        using(var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Book>(sql);
        }
    }


    public Book CreateBook(string title, string publisher, string coverImgUrl)
    {
        var sql = $@"INSERT INTO 
                    library.books (title, publisher, coverimgurl)
                    VALUES (@title, @publisher, @coverImgUrl)
                    RETURNING *;";
        
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Book>(sql, new { title, publisher, coverImgUrl });
        }
    }


    public Book UpdateBook(int bookId, string title, string publisher, string coverImgUrl)
    {
        var sql = $@"UPDATE library.books SET
                        title = @title,
                        publisher = @publisher,
                        coverimgurl = @coverImgUrl
                        WHERE bookid = @bookId
                        RETURNING *;";
        
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Book>(sql, new { bookId, title, publisher, coverImgUrl });
        }
    }

    public object DeleteBook(int bookId)
    {
        var sql = $@"DELETE FROM library.books WHERE bookid = @bookId;";
        
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { bookId }) == 1;
        }
    }
}