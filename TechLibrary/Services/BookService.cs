using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechLibrary.Data;
using TechLibrary.Domain;
using TechLibrary.Models;

namespace TechLibrary.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync();
        /// <summary>
        /// Get books by page.
        /// </summary>
        /// <param name="page">The page number starting at 1.</param>
        /// <param name="pageSize">The size for each page.</param>
        /// <param name="query">Optional query text to use to search for books.</param>
        PaginatedList<Book> GetBooksPaginatedAsync(int page, int pageSize, string query = null);
        Task<Book> GetBookByIdAsync(int bookid);
    }

    public class BookService : IBookService
    {
        private readonly DataContext _dataContext;

        public BookService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            var queryable = _dataContext.Books.AsQueryable();

            return await queryable.ToListAsync();
        }

        public PaginatedList<Book> GetBooksPaginatedAsync(int page, int pageSize, string query = null)
        {
            IQueryable<Book> queryable;

            if (string.IsNullOrWhiteSpace(query))
            {
                queryable = _dataContext.Books.AsQueryable();
            }
            else
            {
                var uppercaseQuery = query.ToUpper();
                queryable = _dataContext.Books.Where(book => book.Title.ToUpper().Contains(uppercaseQuery) || book.ShortDescr.ToUpper().Contains(uppercaseQuery));
            }

            return new PaginatedList<Book>(queryable, page, pageSize);
        }

        public async Task<Book> GetBookByIdAsync(int bookid)
        {
            return await _dataContext.Books.SingleOrDefaultAsync(x => x.BookId == bookid);
        }
    }
}
