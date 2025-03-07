﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechLibrary.Data;
using TechLibrary.Domain;
using TechLibrary.Services;

namespace TechLibrary.Test.Services
{
    [TestFixture]
    public class BookServiceTests
    {
        private DataContext _dataContext;
        private DbConnection _connection;
        private BookService _bookService;

        [OneTimeSetUp]
        public void TestSetup()
        {
            _connection = CreateInMemoryConnection();
            _dataContext = new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlite(_connection).Options);
            _bookService = new BookService(_dataContext);

            Seed(_connection);
        }

        private DbConnection CreateInMemoryConnection()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        private void Seed(DbConnection connection)
        {
            // Use new DataContext so these inserts are not tracked in the same context as tests.
            using (var dataContext = new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlite(connection).Options))
            {
                dataContext.Database.EnsureDeleted();
                dataContext.Database.EnsureCreated();

                dataContext.AddRange(
                   new Book() { BookId = 1, Title = "A", ShortDescr = "B" },
                   new Book() { BookId = 2, Title = "B", ShortDescr = "C" },
                   new Book() { BookId = 3, Title = "C", ShortDescr = "C" },
                   new Book() { BookId = 4, Title = "C", ShortDescr = "C" },
                   new Book() { BookId = 5, Title = "C", ShortDescr = "C" });

                dataContext.SaveChanges();
            }
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
            _dataContext.Dispose();
            _connection.Dispose();
        }

        [Test]
        public async Task GetBooksAsync_ReturnsAllBooks()
        {
            var books = await _bookService.GetBooksAsync();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, books[0].BookId);
                Assert.AreEqual(2, books[1].BookId);
                Assert.AreEqual(3, books[2].BookId);
                Assert.AreEqual(4, books[3].BookId);
                Assert.AreEqual(5, books[4].BookId);
            });
        }

        [Test]
        public void GetBooksPaginatedAsync_PageSize0_ReturnsEmpty()
        {
            var books1 = _bookService.GetBooksPaginatedAsync(0, 0);
            var books2 = _bookService.GetBooksPaginatedAsync(1, 0);


            Assert.AreEqual(0, books1.Count);
            Assert.AreEqual(0, books2.Count);
        }

        [Test]
        public void GetBooksPaginatedAsync_Page0_ReturnsFirstPage()
        {
            var books1 = _bookService.GetBooksPaginatedAsync(0, 1);
            var books2 = _bookService.GetBooksPaginatedAsync(0, 2);

            Assert.AreEqual(1, books1.Count);
            Assert.AreEqual(1, books1[0].BookId);

            Assert.AreEqual(2, books2.Count);
            Assert.AreEqual(1, books2[0].BookId);
            Assert.AreEqual(2, books2[1].BookId);
        }

        [Test]
        public void GetBooksPaginatedAsync_Page1AndPageSize2_ReturnsCorrectTotalCountAndTotalPages()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 2);

            Assert.AreEqual(5, books.TotalCount);
            Assert.AreEqual(3, books.TotalPages);
        }

        [Test]
        public void GetBooksPaginatedAsync_Page1AndPageSize1_ReturnsFirstBook()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 1);

            Assert.AreEqual(1, books.Count);
            Assert.AreEqual(1, books[0].BookId);
        }

        [Test]
        public void GetBooksPaginatedAsync_Page2AndPageSize1_ReturnsSecondBook()
        {
            var books = _bookService.GetBooksPaginatedAsync(2, 1);

            Assert.AreEqual(1, books.Count);
            Assert.AreEqual(2, books[0].BookId);
        }

        [Test]
        public void GetBooksPaginatedAsync_Page1AndPageSize2_ReturnsFirstTwoBooks()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 2);

            Assert.AreEqual(2, books.Count);
            Assert.AreEqual(1, books[0].BookId);
            Assert.AreEqual(2, books[1].BookId);
        }

        [Test]
        public void GetBooksPaginatedAsync_Page2AndPageSize2_ReturnsSecondTwoBooks()
        {
            var books = _bookService.GetBooksPaginatedAsync(2, 2);

            Assert.AreEqual(2, books.Count);
            Assert.AreEqual(3, books[0].BookId);
            Assert.AreEqual(4, books[1].BookId);
        }

        [Test]
        public void GetBooksPaginatedAsync_Page3AndPageSize2_ReturnsLastBook()
        {
            var books = _bookService.GetBooksPaginatedAsync(3, 2);

            Assert.AreEqual(1, books.Count);
            Assert.AreEqual(5, books[0].BookId);
        }

        [Test]
        public void GetBooksPaginatedAsync_Page4AndPageSize2_ReturnsEmpty()
        {
            var books = _bookService.GetBooksPaginatedAsync(4, 2);

            Assert.AreEqual(0, books.Count);
        }

        [Test]
        public void GetBooksPaginatedAsync_QueryNull_ReturnsAllBooks()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 5, null);

            Assert.AreEqual(5, books.Count);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, books[0].BookId);
                Assert.AreEqual(2, books[1].BookId);
                Assert.AreEqual(3, books[2].BookId);
                Assert.AreEqual(4, books[3].BookId);
                Assert.AreEqual(5, books[4].BookId);
            });
        }

        [Test]
        public void GetBooksPaginatedAsync_QueryEmptyString_ReturnsAllBooks()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 5, string.Empty);

            Assert.AreEqual(5, books.Count);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, books[0].BookId);
                Assert.AreEqual(2, books[1].BookId);
                Assert.AreEqual(3, books[2].BookId);
                Assert.AreEqual(4, books[3].BookId);
                Assert.AreEqual(5, books[4].BookId);
            });
        }

        [Test]
        public void GetBooksPaginatedAsync_QueryWhitespaceString_ReturnsAllBooks()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 5, " ");

            Assert.AreEqual(5, books.Count);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, books[0].BookId);
                Assert.AreEqual(2, books[1].BookId);
                Assert.AreEqual(3, books[2].BookId);
                Assert.AreEqual(4, books[3].BookId);
                Assert.AreEqual(5, books[4].BookId);
            });
        }

        [Test]
        public void GetBooksPaginatedAsync_QueryA_ReturnsFirstBook()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 5, "A");

            Assert.AreEqual(1, books.Count);
            Assert.AreEqual(1, books[0].BookId);
        }

        [Test]
        public void GetBooksPaginatedAsync_QueryLowercaseA_ReturnsFirstBook()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 5, "a");

            Assert.AreEqual(1, books.Count);
            Assert.AreEqual(1, books[0].BookId);
        }

        [Test]
        public void GetBooksPaginatedAsync_QueryB_ReturnsFirstTwoBooks()
        {
            var books = _bookService.GetBooksPaginatedAsync(1, 5, "B");

            Assert.AreEqual(2, books.Count);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, books[0].BookId);
                Assert.AreEqual(2, books[1].BookId);
            });
        }

        [Test]
        public void GetBooksPaginatedAsync_Page2AndPageSize2AndQueryC_ReturnsLastTwoBooks()
        {
            var books = _bookService.GetBooksPaginatedAsync(2, 2, "C");

            Assert.AreEqual(2, books.Count);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(4, books[0].BookId);
                Assert.AreEqual(5, books[1].BookId);
            });
        }

        [Test]
        public async Task UpdateBookAsync_UpdateFirstBook_PersistsUpdates()
        {
            // Need to use a new connection specifically for this test so data updates don't break other tests.
            var connection = CreateInMemoryConnection();
            var dataContext = new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlite(connection).Options);
            var bookService = new BookService(dataContext);

            Seed(connection);

            var updatedBook = new Book()
            {
                BookId = 1,
                ISBN = "1234567890123",
                LongDescr = "New long description.",
                PublishedDate = "New published date",
                ShortDescr = "New short description.",
                ThumbnailUrl = "https://via.placeholder.com/150x200",
                Title = "New Title",
            };

            await bookService.UpdateBookAsync(updatedBook);

            var result = await bookService.GetBookByIdAsync(updatedBook.BookId);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(updatedBook.BookId, result.BookId);
                Assert.AreEqual(updatedBook.ISBN, result.ISBN);
                Assert.AreEqual(updatedBook.LongDescr, result.LongDescr);
                Assert.AreEqual(updatedBook.PublishedDate, result.PublishedDate);
                Assert.AreEqual(updatedBook.ShortDescr, result.ShortDescr);
                Assert.AreEqual(updatedBook.ThumbnailUrl, result.ThumbnailUrl);
                Assert.AreEqual(updatedBook.Title, result.Title);
            });
        }

        [Test]
        public async Task CreateBookAsync_PersistsBook()
        {
            // Need to use a new connection specifically for this test so data updates don't break other tests.
            var connection = CreateInMemoryConnection();
            var dataContext = new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlite(connection).Options);
            var bookService = new BookService(dataContext);

            Seed(connection);

            var newBook = new Book()
            {
                ISBN = "1234567890123",
                LongDescr = "New long description.",
                PublishedDate = "New published date",
                ShortDescr = "New short description.",
                ThumbnailUrl = "https://via.placeholder.com/150x200",
                Title = "New Title",
            };

            int id = await bookService.CreateBookAsync(newBook);

            var result = await bookService.GetBookByIdAsync(id);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(newBook.ISBN, result.ISBN);
                Assert.AreEqual(newBook.LongDescr, result.LongDescr);
                Assert.AreEqual(newBook.PublishedDate, result.PublishedDate);
                Assert.AreEqual(newBook.ShortDescr, result.ShortDescr);
                Assert.AreEqual(newBook.ThumbnailUrl, result.ThumbnailUrl);
                Assert.AreEqual(newBook.Title, result.Title);
            });
        }
    }
}
