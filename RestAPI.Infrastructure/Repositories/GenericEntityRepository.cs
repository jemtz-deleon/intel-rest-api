using System.ComponentModel;
using System.Reflection;
using System.Text;
using Dapper;
using RestAPI.Core.Interfaces;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.Types;

namespace RestAPI.Infrastructure.Repositories;

public abstract class GenericEntityRepository<T> : IRepository<T> where T : IEntity, new()
{
    protected ILogger<GenericEntityRepository<T>> _logger;
    private readonly IConfiguration _configuration;
    protected readonly string _entityName;

    protected GenericEntityRepository(ILogger<GenericEntityRepository<T>> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _entityName = typeof(T).Name;
    }

    private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

    private MySqlConnection SqlConnection()
    {
        return new MySqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
    }

    protected MySqlConnection CreateConnection()
    {
        var connection = SqlConnection();
        connection.Open();
        return connection;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>($"SELECT * FROM {_entityName}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{GetType().Name}.GetAllAsync - Error: {ex.Message}");
            throw ex;
        }
    }

    // TODO: In the context of this "Purchasing" App, we shouldn't allow hard deletes.
    // Typically we would ALWAYS store these for historical/accounting purposes.
    // This should be a soft delete instead.
    public async Task<int> DeleteAsync(object id)
    {
        try
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync($"DELETE FROM {_entityName} WHERE Id = @Id", new { Id = id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{GetType().Name}.DeleteAsync - Error: {ex.Message}");
            throw ex;
        }
    }

    public async Task<T> GetByIdAsync(object id)
    {
        try
        {
            using (var connection = CreateConnection())
            {
                // if (result is null) return new T();
                return await connection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_entityName} WHERE Id = @Id", new { Id = id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{GetType().Name}.GetByIdAsync - Error: {ex.Message}");
            throw ex;
        }
    }

    public Task<int> AddRangeAsync(IEnumerable<T> entityList)
    {
        throw new NotImplementedException();
    }

    public async Task<int> AddAsync(T entity)
    {
        try
        {
            var insertQuery = GenerateInsertQuery();

            using (var connection = CreateConnection())
            {
                // TODO: Change to ExecuteScalarAsync so we can get the actual Id inserted
                // instead of the # of records affected.
                // Could be a good idea to pass the Id up when doing an Insert.
                return await connection.ExecuteAsync(insertQuery, entity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{GetType().Name}.AddAsync - Error: {ex.Message}");
            throw ex;
        }
    }

    public async Task<int> UpdateAsync(T entity)
    {
        try
        {
            var updateQuery = GenerateUpdateQuery();
            entity.DateUpdated = new MySqlDateTime(DateTime.Now).GetDateTime();
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(updateQuery, entity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{GetType().Name}.UpdateAsync - Error: {ex.Message}");
            return -1;
        }
    }

    private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
    {
        return (from prop in listOfProperties
                let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                select prop.Name).ToList();
    }

    private string GenerateInsertQuery()
    {
        var insertQuery = new StringBuilder($"INSERT INTO {_entityName}");
        insertQuery.Append("(");
        List<string> properties = GenerateListOfProperties(GetProperties);

        properties.ForEach(prop => { insertQuery.Append($"`{prop}`,"); });
        insertQuery.Remove(insertQuery.Length - 1, 1).Append(") VALUES (");
        properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
        insertQuery.Remove(insertQuery.Length - 1, 1).Append(")");

        return insertQuery.ToString();
    }

    private string GenerateUpdateQuery()
    {
        var updateQuery = new StringBuilder($"UPDATE {_entityName} SET ");
        List<string> properties = GenerateListOfProperties(GetProperties);

        properties.ForEach(prop =>
        {
            if (!prop.Equals("Id")) updateQuery.Append($"{prop} = @{prop},");
        });

        updateQuery.Remove(updateQuery.Length - 1, 1).Append(" WHERE Id = @Id");
        return updateQuery.ToString();
    }
}