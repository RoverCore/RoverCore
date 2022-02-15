using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RoverCore.Datatables.DTOs;
using RoverCore.Datatables.Extensions;

namespace RoverCore.Datatables.Extensions
{
    public static class DatatableExtensions
    {
	    /// <summary>
	    /// Uses the datatables request parameters to query entity and retrieve the appropriate records
	    /// </summary>
	    /// <typeparam name="TEntity"></typeparam>
	    /// <typeparam name="TEntityDTO"></typeparam>
	    /// <param name="entity"></param>
	    /// <param name="request"></param>
	    /// <returns></returns>
	    public static async Task<DtResponseData> GetDatatableResponse<TEntity, TEntityDTO>(this IQueryable<TEntity> entity, DtRequest request)
		    where TEntityDTO : DtBaseResponse
		    where TEntity : class
	    {
		    var sortColumn = request.Columns[request.Order[0].Column].Name;
		    var sortColumnDirection = request.Order[0].Dir;
		    var searchValue = request.Search.Value;

		    int recordsTotal = 0;
		    var records = entity;

		    sortColumn = string.IsNullOrEmpty(sortColumn) ? "Id" : sortColumn.Replace(" ", "");
		    sortColumnDirection = string.IsNullOrEmpty(sortColumnDirection) ? "asc" : sortColumnDirection;

		    if (!string.IsNullOrEmpty(searchValue))
		    {
			    /*
		        records = records.Where(m => m.Name.Contains(searchValue)
		                                     || m.NormalizedName.Contains(searchValue)
		                                     || m.ConcurrencyStamp.Contains(searchValue));
			    */
		    }

		    records = sortColumnDirection == "asc" ? records.OrderBy(sortColumn) : records.OrderByDescending(sortColumn);

		    var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateProjection<TEntity, TEntityDTO>());

		    recordsTotal = await records.CountAsync();
		    var data = await records.ProjectTo<TEntityDTO>(mapperConfiguration).Skip(request.Start).Take(request.Length).ToListAsync();

		    var jsonData = new DtResponseData
		    {
			    Draw = request.Draw,
			    RecordsFiltered = recordsTotal,
			    RecordsTotal = recordsTotal,
			    Data = data
		    };

		    return jsonData;
	    }
	}
}
