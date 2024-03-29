﻿using BookStore.Application.useCases.Authors.Commands;
using BookStore.Application.useCases.Authors.Queries;
using BookStore.Application.useCases.Books.Queries;
using BookStore.Application.useCases.Extensions.PaginationExtensions;
using BookStore.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    [Authorize]
    public class AuthorsController : Controller
    {
        private readonly IMediator _mediator;
        private const int pageSize = 10;

        public AuthorsController(IMediator mediator)
            => _mediator = mediator;

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            var query = new GetAllAuthorsQuery();
            var allAuthors = await _mediator.Send(query);

            var paginate = new PaginateObjects<Author>(allAuthors, page, pageSize);
            var paginatedAuthors = paginate.paginatedObjects;

            var viewModel = new PaginationViewModel<Author>(allAuthors, paginatedAuthors, page, pageSize);

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAuthorCommand createAuthorCommand)
        {
            var author = await _mediator.Send(createAuthorCommand);

            return View("Details", author);
        }

        public async Task<IActionResult> UpdateAsync(int id)
        {
            var author = await _mediator.Send(new GetAuthorByIdQuery { Id = id });

            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(Author author)
        {
            var updateAuthorCommand = new UpdateAuthorCommand()
            {
                Name = author.Name,
                Description = author.Description,
                Id = author.Id
            };

            var updatedAuthor = await _mediator.Send(updateAuthorCommand);

            return View("Details", updatedAuthor);
        }

        [AllowAnonymous]
        public async Task<IActionResult> SearchByName(string name, int page = 1)
        {
            var getAuthorByNameCommand = new SearchAuthorQuery()
            {
                Text = name
            };
            var res = await _mediator.Send(getAuthorByNameCommand);

            var paginate = new PaginateObjects<Author>(res, page, pageSize);
            var paginatedAuthors = paginate.paginatedObjects;

            var viewModel = new PaginationViewModel<Author>(res, paginatedAuthors, page, pageSize, getAuthorByNameCommand.Text);

            return View("Index", viewModel);
        }

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var query = new DeleteAuthorCommand()
            {
                Id = id
            };
            var author = await _mediator.Send(query);
            return RedirectToAction(actionName: nameof(Index));
        }
    }
}
