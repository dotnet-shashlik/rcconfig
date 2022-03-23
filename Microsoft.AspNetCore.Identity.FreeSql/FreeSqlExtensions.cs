using FreeSql.Internal.Model;
using Shashlik.Utils.Extensions;

namespace Microsoft.AspNetCore.Identity.FreeSql;

public static class FreeSqlExtensions
{
    /// <summary>
    /// 新增并自动设置实体主键值
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T1"></typeparam>
    /// <exception cref="InvalidCastException"></exception>
    public static async Task InsertAndIdentityIfAsync<T1>(this IFreeSql dbContext, T1 entity, CancellationToken cancellationToken = default)
        where
        T1 : class
    {
        var col = dbContext.GetPrimaryKeyIdentity(typeof(T1));
        if (col is null)
        {
            await dbContext.Insert(entity)
                .ExecuteAffrowsAsync(cancellationToken);

            return;
        }

        var id = await dbContext.Insert(entity)
            .ExecuteIdentityAsync(cancellationToken);

        var p = col.Table.Properties[col.CsName];
        if (p is null)
            throw new InvalidCastException($"not found property \"Id\" in: {typeof(T1)}");
        var newId = id.ParseTo(p.PropertyType);
        p.SetValue(entity, newId);
    }

    /// <summary>
    /// 新增并自动设置实体主键值
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="entity"></param>
    /// <typeparam name="T1"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public static void InsertAndIdentityIf<T1>(this IFreeSql dbContext, T1 entity) where T1 : class
    {
        var col = dbContext.GetPrimaryKeyIdentity(typeof(T1));
        if (col is null)
        {
            dbContext.Insert(entity).ExecuteAffrows();

            return;
        }

        var id = dbContext.Insert(entity)
            .ExecuteIdentity();

        var p = col.Table.Properties[col.CsName];
        if (p is null)
            throw new InvalidCastException($"not found property \"Id\" in: {typeof(T1)}");
        var newId = id.ParseTo(p.PropertyType);
        p.SetValue(entity, newId);
    }

    public static ColumnInfo? GetPrimaryKeyIdentity(this IFreeSql dbContext, Type entityType)
    {
        var col = dbContext.CodeFirst.GetTableByEntity(entityType).Columns
            .SingleOrDefault(r => r.Value.Attribute.IsPrimary && r.Value.Attribute.IsIdentity).Value;
        return col;
    }
}