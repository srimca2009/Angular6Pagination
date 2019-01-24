using Angular_DataTable.Helpers;
using Angular_DataTable.Models;
using Angular_DataTable.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Angular_DataTable.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet(Name = nameof(GetAll))]
        public IActionResult GetAll([FromQuery] QueryParameters queryParameters)
        {
            List<Customer> allCustomers = _customerRepository.GetAll(queryParameters).ToList();

            var allItemCount = _customerRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount);

            var toReturn = allCustomers.Select(x => ExpandSingleItem(x));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingle))]
        public IActionResult GetSingle(int id)
        {
            Customer customer = _customerRepository.GetSingle(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleItem(customer));
        }

        [HttpPost(Name = nameof(Add))]
        public IActionResult Add([FromBody] Customer customerCreateDto)
        {
            if (customerCreateDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            customerCreateDto.Created = DateTime.Now;
            _customerRepository.Add(customerCreateDto);

            if (!_customerRepository.Save())
            {
                throw new Exception("Creating an item failed on save.");
            }

            Customer newItem = _customerRepository.GetSingle(customerCreateDto.Id);

            return CreatedAtRoute(nameof(GetSingle), new { id = newItem.Id },
                customerCreateDto);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(Delete))]
        public IActionResult Delete(int id)
        {
            Customer customer = _customerRepository.GetSingle(id);

            if (customer == null)
            {
                return NotFound();
            }

            _customerRepository.Delete(id);

            if (!_customerRepository.Save())
            {
                throw new Exception("Deleting an item failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(Update))]
        public IActionResult Update(int id, [FromBody]Customer updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            var existingCustomer = _customerRepository.GetSingle(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _customerRepository.Update(id, existingCustomer);

            if (!_customerRepository.Save())
            {
                throw new Exception("Updating an item failed on save.");
            }

            return Ok(ExpandSingleItem(existingCustomer));
        }

        private List<Link> CreateLinksForCollection(QueryParameters queryParameters, int totalCount)
        {
            var links = new List<Link>();
            try
            {
                var dd = Url.Link(nameof(Add), null);
                links.Add(
                    new Link(Url.Link(nameof(Add), null), "create", "POST"));

                // self 
                links.Add(
                 new Link(Url.Link(nameof(GetAll), new
                 {
                     pagecount = queryParameters.PageCount,
                     page = queryParameters.Page,
                     orderby = queryParameters.OrderBy
                 }), "self", "GET"));

                links.Add(new Link(Url.Link(nameof(GetAll), new
                {
                    pagecount = queryParameters.PageCount,
                    page = 1,
                    orderby = queryParameters.OrderBy
                }), "first", "GET"));

                links.Add(new Link(Url.Link(nameof(GetAll), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.GetTotalPages(totalCount),
                    orderby = queryParameters.OrderBy
                }), "last", "GET"));

                if (queryParameters.HasNext(totalCount))
                {
                    links.Add(new Link(Url.Link(nameof(GetAll), new
                    {
                        pagecount = queryParameters.PageCount,
                        page = queryParameters.Page + 1,
                        orderby = queryParameters.OrderBy
                    }), "next", "GET"));
                }

                if (queryParameters.HasPrevious())
                {
                    links.Add(new Link(Url.Link(nameof(GetAll), new
                    {
                        pagecount = queryParameters.PageCount,
                        page = queryParameters.Page - 1,
                        orderby = queryParameters.OrderBy
                    }), "previous", "GET"));
                }
            }
            catch (Exception ex)
            {

                throw;
            }



            return links;
        }

        private dynamic ExpandSingleItem(Customer customer)
        {
            var links = GetLinks(customer.Id);

            var resourceToReturn = customer.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<Link> GetLinks(int id)
        {
            var links = new List<Link>();

            links.Add(
              new Link(Url.Link(nameof(GetSingle), new { id = id }),
              "self",
              "GET"));

            links.Add(
              new Link(Url.Link(nameof(Delete), new { id = id }),
              "delete",
              "DELETE"));

            links.Add(
              new Link(Url.Link(nameof(Add), null),
              "create",
              "POST"));

            links.Add(
               new Link(Url.Link(nameof(Update), new { id = id }),
               "update",
               "PUT"));

            return links;
        }

    }
}