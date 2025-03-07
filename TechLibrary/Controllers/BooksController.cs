﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TechLibrary.Domain;
using TechLibrary.Models;
using TechLibrary.Services;
using System;

namespace TechLibrary.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(ILogger<BooksController> logger, IBookService bookService, IMapper mapper)
        {
            _logger = logger;
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] int? page = null, [FromQuery] int? pageSize = null, [FromQuery] string query = null)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                _logger.LogInformation($"Get books on page {page} with page size {pageSize}.");

                var books = _bookService.GetBooksPaginatedAsync(page.Value, pageSize.Value, query);

                var bookResponse = _mapper.Map<PaginatedListResponse<BookResponse>>(books);

                return Ok(bookResponse);
            }
            else
            {
                if (page.HasValue)
                {
                    _logger.LogInformation("Get books missing value for page size.");

                    return UnprocessableEntity("Parameter pageSize is missing a value.");
                }
                else if (pageSize.HasValue)
                {
                    _logger.LogInformation("Get books missing value for page.");

                    return UnprocessableEntity("Parameter page is missing a value.");
                }
                else
                {
                    _logger.LogInformation("Get all books.");

                    var books = await _bookService.GetBooksAsync();

                    var bookResponse = _mapper.Map<List<BookResponse>>(books);

                    return Ok(bookResponse);
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Get book by id {id}");

            var book = await _bookService.GetBookByIdAsync(id);

            var bookResponse = _mapper.Map<BookResponse>(book);

            return Ok(bookResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookRequest request)
        {
            _logger.LogInformation($"Update book with id {id}");

            var currentBook = await _bookService.GetBookByIdAsync(id);

            var updatedBook = _mapper.Map<BookRequest, Book>(request, opt => opt.AfterMap((src, dest) =>
            { 
                dest.BookId = id;
                dest.LongDescr = currentBook.LongDescr;
            }));

            await _bookService.UpdateBookAsync(updatedBook);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(BookRequest request)
        {
            _logger.LogInformation("Create book");

            var newBook = _mapper.Map<Book>(request);

            var id = await _bookService.CreateBookAsync(newBook);

            return Ok(id);
        }
    }
}
