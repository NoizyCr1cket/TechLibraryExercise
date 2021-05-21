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
        Task<List<Book>> GetBooksAsync(int page, int pageSize);
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

        public async Task<List<Book>> GetBooksAsync(int page, int pageSize)
        {
            var queryable = _dataContext.Books.AsQueryable();

            return await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int bookid)
        {
            return await _dataContext.Books.SingleOrDefaultAsync(x => x.BookId == bookid);
        }
    }
}
