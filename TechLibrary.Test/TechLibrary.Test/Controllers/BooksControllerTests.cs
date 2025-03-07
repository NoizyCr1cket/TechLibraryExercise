﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechLibrary.Services;

namespace TechLibrary.Controllers.Tests
{
    [TestFixture()]
    [Category("ControllerTests")]
    public class BooksControllerTests
    {

        private Mock<ILogger<BooksController>> _mockLogger;
        private Mock<IBookService> _mockBookService;
        private Mock<IMapper> _mockMapper;
        private NullReferenceException _expectedException;

        [OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Failed...");
            _mockLogger = new Mock<ILogger<BooksController>>();
            _mockBookService = new Mock<IBookService>();
            _mockMapper = new Mock<IMapper>();
        }

        [TearDown]
        public void TestTearDownAfterEach()
        {
            _mockLogger.Reset();
            _mockBookService.Reset();
            _mockMapper.Reset();
        }

        [Test()]
        public async Task GetBooks_NoParams_CallsGetBooksAsyncWithNoParams()
        {
            //  Arrange
            _mockBookService.Setup(b => b.GetBooksAsync()).Returns(Task.FromResult(It.IsAny<List<Domain.Book>>()));
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object);

            //  Act
            var result = await sut.GetBooks();

            //  Assert
            _mockBookService.Verify(s => s.GetBooksAsync(), Times.Once, "Expected GetBooksAsync to have been called once");
        }

        [Test()]
        public async Task GetBooksPaginated_WithPageAndPageSizeAndQuery_CallsGetBooksAsyncWithParams()
        {
            //  Arrange
            var page = 1;
            var pageSize = 3;
            var query = "something";
            _mockBookService.Setup(b => b.GetBooksPaginatedAsync(page, pageSize, query)).Returns(It.IsAny<Domain.PaginatedList<Domain.Book>>());
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object);

            //  Act
            var result = await sut.GetBooks(page, pageSize, query);

            //  Assert
            _mockBookService.Verify(s => s.GetBooksPaginatedAsync(page, pageSize, query), Times.Once, $"Expected GetBooksPaginatedAsync to have been called once with provided page {page}, pageSize {pageSize}, and query {query}");
        }

        [Test()]
        public async Task GetBooksPaginated_WithPageOnly_DoesNotCall()
        {
            //  Arrange
            _mockBookService.Setup(b => b.GetBooksPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), null)).Returns(It.IsAny<Domain.PaginatedList<Domain.Book>>());
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object);

            //  Act
            var result = await sut.GetBooks(1, null);

            //  Assert
            _mockBookService.Verify(s => s.GetBooksPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), null), Times.Never, $"Expected GetBooksPaginatedAsync to never have been called");
        }

        [Test()]
        public async Task GetBooksPaginated_WithPageSizeOnly_DoesNotCall()
        {
            //  Arrange
            _mockBookService.Setup(b => b.GetBooksPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), null)).Returns(It.IsAny<Domain.PaginatedList<Domain.Book>>());
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object);

            //  Act
            var result = await sut.GetBooks(null, 3);

            //  Assert
            _mockBookService.Verify(s => s.GetBooksPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), null), Times.Never, $"Expected GetBooksPaginatedAsync to never have been called");
        }

        [Test()]
        public async Task UpdateBook_CallsUpdateBookAsyncWithBookParam()
        {
            //  Arrange
            var bookRequest = new Models.BookRequest()
            {
                ISBN = "1234567890123",
                PublishedDate = "New published date",
                Descr = "New description.",
                ThumbnailUrl = "New thumbnail URL",
                Title = "New Title",
            };

            var originalBook = new Domain.Book()
            {
                BookId = 1,
                ISBN = "0987654321098",
                LongDescr = "Long description.",
                PublishedDate = "Published date.",
                ShortDescr = "Short descirption.",
                ThumbnailUrl = "Thumbnail URL",
                Title = "Title",
            };

            var updatedBook = new Domain.Book()
            {
                BookId = originalBook.BookId,
                ISBN = bookRequest.ISBN,
                LongDescr = "Long description.",
                PublishedDate = bookRequest.PublishedDate,
                ShortDescr = bookRequest.Descr,
                ThumbnailUrl = bookRequest.ThumbnailUrl,
                Title = bookRequest.Title,
            };

            _mockBookService.Setup(b => b.GetBookByIdAsync(updatedBook.BookId)).ReturnsAsync(originalBook);
            _mockMapper.Setup(m => m.Map(bookRequest, It.IsAny<Action<IMappingOperationOptions<Models.BookRequest, Domain.Book>>>()))
                .Returns(updatedBook);
            _mockBookService.Setup(b => b.UpdateBookAsync(updatedBook));
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object);

            //  Act
            await sut.UpdateBook(updatedBook.BookId, bookRequest);

            //  Assert
            _mockBookService.Verify(s => s.UpdateBookAsync(updatedBook), Times.Once, $"Expected UpdateBookAsync have been called once with provided book");
        }

        [Test()]
        public async Task CreateBook_CallsCreateBookAsyncWithBookParam()
        {
            //  Arrange
            var bookRequest = new Models.BookRequest()
            {
                ISBN = "1234567890123",
                PublishedDate = "New published date",
                Descr = "New description.",
                ThumbnailUrl = "New thumbnail URL",
                Title = "New Title",
            };

            var newBook = new Domain.Book()
            {
                ISBN = bookRequest.ISBN,
                LongDescr = "Long description.",
                PublishedDate = bookRequest.PublishedDate,
                ShortDescr = bookRequest.Descr,
                ThumbnailUrl = bookRequest.ThumbnailUrl,
                Title = bookRequest.Title,
            };

            _mockMapper.Setup(m => m.Map(bookRequest, It.IsAny<Action<IMappingOperationOptions<Models.BookRequest, Domain.Book>>>()))
                .Returns(newBook);
            _mockBookService.Setup(b => b.UpdateBookAsync(newBook));
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object);

            //  Act
            await sut.UpdateBook(newBook.BookId, bookRequest);

            //  Assert
            _mockBookService.Verify(s => s.UpdateBookAsync(newBook), Times.Once, $"Expected UpdateBookAsync have been called once with provided book");
        }
    }
}