using Microsoft.Data.Sqlite;
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

            Seed();
        }

        private DbConnection CreateInMemoryConnection()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        private void Seed()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _dataContext.AddRange(
                new Book() { BookId = 1 },
                new Book() { BookId = 2 },
                new Book() { BookId = 3 },
                new Book() { BookId = 4 },
                new Book() { BookId = 5 });

            _dataContext.SaveChanges();
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
    }
}
