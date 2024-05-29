using ScadeSuiteWeb.Server.Database;
using ScadeSuiteWeb.Server.Models;

namespace ScadeSuiteWeb.Server.Repositories;


public interface IBookRepository
{
    public List<Book> GetAll();

    public Book? GetById(int id);

    public bool Add(Book book);

    public bool Remove(Book book);

}



public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Book> GetAll()
    {
        return _context.Books.ToList();
    }

    public Book? GetById(int id)
    {
        var b = _context.Books.FirstOrDefault(b => b.Id == id);
        if (b is not null)
        {
            return b;
        }
        return null;
    }

    public bool Add(Book book)
    {
        _context.Books.Add(book);
        _context.SaveChanges();
        return true;
    }

    public bool Remove(Book book)
    {
        _context.Books.Remove(book);
        _context.SaveChanges();
        return true;
    }
}