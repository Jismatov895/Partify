﻿using Partify.Domain.Commons;
using System.Linq.Expressions;

namespace Partify.DataAccess.Repositories;

public interface IRepository<TEntity> where TEntity : Auditable
{
	ValueTask<TEntity> InsertAsync(TEntity entity);
	ValueTask<TEntity> UpdateAsync(TEntity entity);
	ValueTask DeleteAsync(TEntity entity);
	ValueTask<TEntity> SelectAsync(Expression<Func<TEntity, bool>> expression, string[] includes = null);
	IQueryable<TEntity> Select(
		Expression<Func<TEntity, bool>> expression = null,
		string[] includes = null,
		bool isTracking = true);
}