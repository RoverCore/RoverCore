using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RoverCore.Datatables.DTOs;
using RoverCore.Datatables.Extensions;
using RoverCore.Datatables.Models;

namespace RoverCore.Datatables.Extensions
{
    public static class DatatableExtensions
    {
        public static DtMetadata GetDtMetadata<TEntityDTO>()
        {
            Type dtoType = typeof(TEntityDTO);
            DtMetadata metadata = new DtMetadata();

            PropertyInfo[] properties = dtoType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                DtMetaColumn column = new DtMetaColumn();

                var keyAttribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute)) as KeyAttribute;
                var nameAttribute = Attribute.GetCustomAttribute(property, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                
                column.IsDate = property.PropertyType == typeof(DateTime);
                column.IsPrimaryKey = keyAttribute != null;
                column.Visible = !column.IsPrimaryKey;
                column.Searchable = property.PropertyType == typeof(string);
                column.Orderable = true;
                column.Data = Char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
                column.Name = property.Name;
                column.DisplayName = nameAttribute?.DisplayName ?? property.Name;

                metadata.Columns.Add(column);

                if (column.IsPrimaryKey)
                {
                    metadata.KeyIndex = metadata.Columns.Count - 1;
                }
            }

            return metadata;
        }

        /// <summary>
        /// Uses the datatables request parameters to query entity and retrieve the appropriate records.  The TEntityDTO should be completely flattened and not include other objects as fields.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TEntityDTO"></typeparam>
        /// <param name="entity"></param>
        /// <param name="request"></param>
        /// <param name="config">Optional Automapper configuration that maps TEntity to TEntityDTO</param>
        /// <returns></returns>
        public static async Task<DtResponseData> GetDatatableResponse<TEntity, TEntityDTO>(this IQueryable<TEntity> entity, DtRequest request, MapperConfiguration? config = null)
		    where TEntityDTO : class
		    where TEntity : class
	    {
		    var sortColumn = request.Columns[request.Order[0].Column].Name;
		    var sortColumnDirection = request.Order[0].Dir;
		    var searchValue = request.Search.Value;

		    int recordsTotal = 0;
		    var records = entity;

		    
		    sortColumnDirection = string.IsNullOrEmpty(sortColumnDirection) ? "asc" : sortColumnDirection;

		    if (!String.IsNullOrEmpty(sortColumn))
		    {
			    sortColumn = sortColumn.Replace(" ", "");

                records = sortColumnDirection == "asc"
				    ? records.OrderBy(sortColumn)
				    : records.OrderByDescending(sortColumn);
		    }

		    var mapperConfiguration = config ?? new MapperConfiguration(cfg => cfg.CreateProjection<TEntity, TEntityDTO>());

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

        static Expression? SearchStrings(Expression target, Expression search)
        {
            Expression? result = null;

            var properties = target.Type
                .GetProperties()
                .Where(x => x.CanRead);

            foreach (var prop in properties)
            {
                Expression? condition = null;
                var propValue = Expression.MakeMemberAccess(target, prop);
                if (prop.PropertyType == typeof(string))
                {
                    var comparand = Expression.Call(propValue, nameof(string.ToLower), Type.EmptyTypes);
                    condition = Expression.Call(comparand, nameof(string.Contains), Type.EmptyTypes, search);
                }
                else if (!(prop!.PropertyType!.Namespace!.StartsWith("System.")))
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
