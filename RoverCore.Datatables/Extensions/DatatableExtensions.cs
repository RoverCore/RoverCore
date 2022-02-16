using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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

		    records = sortColumnDirection == "asc" ? records.OrderBy(sortColumn) : records.OrderByDescending(sortColumn);

		    var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateProjection<TEntity, TEntityDTO>());

            recordsTotal = await records.CountAsync();
            var recordsDto = records.ProjectTo<TEntityDTO>(mapperConfiguration);

			// Apply any search filters to the dto version of the entity
            if (!string.IsNullOrEmpty(searchValue))
            {
                recordsDto = recordsDto.ApplySearch(searchValue);
            }

            var recordsFiltered = await recordsDto.CountAsync();
			var data = await recordsDto.Skip(request.Start).Take(request.Length).ToListAsync();

		    var jsonData = new DtResponseData
		    {
			    Draw = request.Draw,
			    RecordsFiltered = recordsFiltered,
			    RecordsTotal = recordsTotal,
			    Data = data
		    };

			Debug.WriteLine(recordsDto.ToQueryString());

		    return jsonData;
	    }

		/// <summary>
		/// Credit to Ivan Stoev - https://stackoverflow.com/questions/59754832/ef-core-expression-tree-equivalent-for-iqueryable-search
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static IQueryable<T> ApplySearch<T>(this IQueryable<T> source, string search)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(search)) return source;

            var parameter = Expression.Parameter(typeof(T), "e");
            // The following simulates closure to let EF Core create parameter rather than constant value (in case you use `Expresssion.Constant(search)`)
            var value = Expression.Property(Expression.Constant(new { search }), nameof(search));
            var body = SearchStrings(parameter, value);
            if (body == null) return source;

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
            return source.Where(predicate);
        }

        static Expression SearchStrings(Expression target, Expression search)
        {
            Expression result = null;

            var properties = target.Type
                .GetProperties()
                .Where(x => x.CanRead);

            foreach (var prop in properties)
            {
                Expression condition = null;
                var propValue = Expression.MakeMemberAccess(target, prop);
                if (prop.PropertyType == typeof(string))
                {
                    var comparand = Expression.Call(propValue, nameof(string.ToLower), Type.EmptyTypes);
                    condition = Expression.Call(comparand, nameof(string.Contains), Type.EmptyTypes, search);
                }
                else if (!prop.PropertyType.Namespace.StartsWith("System."))
                {
                    condition = SearchStrings(propValue, search);
                }
                if (condition != null)
                    result = result == null ? condition : Expression.OrElse(result, condition);
            }

            return result;
        }

	}
}
