using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
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
        /// <summary>
        /// Creates a viewmodel that can be used to render an index page that uses datatables
        /// </summary>
        /// <typeparam name="TEntityDTO"></typeparam>
        /// <returns></returns>
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
        public static async Task<DtResponseData> GetDatatableResponseAsync<TEntity, TEntityDTO>(this IQueryable<TEntity> entity, DtRequest request, MapperConfiguration? config = null)
		    where TEntityDTO : class
		    where TEntity : class
	    {
            var mapperConfiguration = config ?? new MapperConfiguration(cfg => cfg.CreateProjection<TEntity, TEntityDTO>());
            int recordsTotal = 0;

            var records = ApplySorting<TEntity>(entity, request);

            recordsTotal = await records.CountAsync();
            var recordsDto = records.ProjectTo<TEntityDTO>(mapperConfiguration);

			// Apply any search filters to the dto version of the entity
            if (!string.IsNullOrEmpty(request.Search.Value))
            {
                recordsDto = recordsDto.ApplySearch(request.Search.Value);
            }

            var recordsFiltered = await recordsDto.CountAsync();
			var data = await recordsDto
                .Skip(request.Start)
                .Take(request.Length)
                .ToListAsync();
            
            var dataTransformed = TransformRecords<TEntityDTO>(data);

		    var jsonData = new DtResponseData
		    {
			    Draw = request.Draw,
			    RecordsFiltered = recordsFiltered,
			    RecordsTotal = recordsTotal,
			    Data = dataTransformed
		    };

		    return jsonData;
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
        public static DtResponseData GetDatatableResponse<TEntity, TEntityDTO>(this IQueryable<TEntity> entity, DtRequest request, MapperConfiguration? config = null)
            where TEntityDTO : class
            where TEntity : class
        {
            var mapperConfiguration = config ?? new MapperConfiguration(cfg => cfg.CreateProjection<TEntity, TEntityDTO>());
            int recordsTotal = 0;

            var records = ApplySorting<TEntity>(entity, request);

            recordsTotal = records.Count();
            var recordsDto = records.ProjectTo<TEntityDTO>(mapperConfiguration);

            // Apply any search filters to the dto version of the entity
            if (!string.IsNullOrEmpty(request.Search.Value))
            {
                recordsDto = recordsDto.ApplySearch(request.Search.Value);
            }

            var recordsFiltered = recordsDto.Count();
            var data = recordsDto.Skip(request.Start)
                .Take(request.Length)
                .ToList();

            var dataTransformed = TransformRecords<TEntityDTO>(data);

            var jsonData = new DtResponseData
            {
                Draw = request.Draw,
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal,
                Data = dataTransformed
            };

            return jsonData;
        }

        /// <summary>
        /// Adds sorting method to an existing IQueryable based on the datatables request parameters
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static IQueryable<TEntity> ApplySorting<TEntity>(IQueryable<TEntity> entity, DtRequest request)
        {
            var sortColumn = request.Columns[request.Order[0].Column].Name;
            var sortColumnDirection = request.Order[0].Dir;

            sortColumnDirection = string.IsNullOrEmpty(sortColumnDirection) ? "asc" : sortColumnDirection;

            if (!String.IsNullOrEmpty(sortColumn))
            {
                sortColumn = sortColumn.Replace(" ", "");

                entity = sortColumnDirection == "asc"
                    ? entity.OrderBy(sortColumn)
                    : entity.OrderByDescending(sortColumn);
            }

            return entity;
        }

        /// <summary>
        /// Transforms fields into a format that can be read by Datatables
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static ICollection TransformRecords<T>(List<T> data)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var dataTransformed = data.Select(x =>
            {
                var record = new Dictionary<string, Object>();

                foreach (PropertyInfo property in properties)
                {
                    var propname = Char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
                    var value = property.GetValue(x);

                    // Convert dates to a format digestable by datatables
                    if (property.PropertyType == typeof(DateTime) && value != null)
                    {
                        value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm");
                    }

                    record.Add(propname, value);
                }

                return record;
            }).ToList();

            return dataTransformed;
        }
	}
}
