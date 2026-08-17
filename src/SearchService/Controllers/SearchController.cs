using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams SearchParams)
    {
        var query = DB.PagedSearch<Item, Item>();

        query.Sort(x => x.Ascending(a => a.Make));

        if (!string.IsNullOrEmpty(SearchParams.SearchTerm))
        {
            query.Match(Search.Full, SearchParams.SearchTerm).SortByTextScore();
        }

        query = SearchParams.OrderBy switch
        {
            "make" => query.Sort(x => x.Ascending(a => a.Make)),
            "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
        };

        query = SearchParams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        if (!string.IsNullOrEmpty(SearchParams.Seller))
        {
            query.Match(x => x.Seller == SearchParams.Seller);
        }

        if (!string.IsNullOrEmpty(SearchParams.Winner))
        {
            query.Match(x => x.Winner == SearchParams.Winner);
        }
        query.PageNumber(SearchParams.PageNumber);
        query.PageSize(SearchParams.PageSize);

        var result = await query.ExecuteAsync();

        return Ok(new
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}
