using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FreeSql;

namespace Shashlik.RC.Server.FreeSql;

public static class FreeSqlTransactionExtensions
{
    private static AsyncLocal<DbTransaction?> Transactions { get; } = new();

    public static IFreeSql BuildAsyncTransactionSupport(this FreeSqlBuilder builder)
    {
        var freeSql = builder.Build();
        return new FreeSqlAsyncTransactionSupport(freeSql);
    }

    public static async Task BeginTransactionAsync(this IFreeSql freeSql, Func<Task> handler)
    {
        if (!freeSql.Ado.MasterPool.IsAvailable)
            throw new Exception("主库不可用");
        var dbConnection = freeSql.Ado.MasterPool.Get().Value;
        if (dbConnection is null)
            throw new Exception("主库连接为空");

        var tran = Transactions.Value;
        if (tran is null)
        {
            tran = await dbConnection.BeginTransactionAsync();
            Transactions.Value = tran;
            try
            {
                await handler();
                await tran.CommitAsync();
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
            finally
            {
                Transactions.Value = null;
                await tran.DisposeAsync();
            }
        }
        else await handler();
    }

    public static IInsert<T1> WithTransactionIfRequire<T1>(this IInsert<T1> source)
        where T1 : class
    {
        if (Transactions.Value is not null)
        {
            return source.WithTransaction(Transactions.Value);
        }

        return source;
    }

    public static IUpdate<T1> WithTransactionIfRequire<T1>(this IUpdate<T1> source)
        where T1 : class
    {
        if (Transactions.Value is not null)
        {
            return source.WithTransaction(Transactions.Value);
        }

        return source;
    }

    public static IInsertOrUpdate<T1> WithTransactionIfRequire<T1>(this IInsertOrUpdate<T1> source)
        where T1 : class
    {
        if (Transactions.Value is not null)
        {
            return source.WithTransaction(Transactions.Value);
        }

        return source;
    }

    public static IDelete<T1> WithTransactionIfRequire<T1>(this IDelete<T1> source)
        where T1 : class
    {
        if (Transactions.Value is not null)
        {
            return source.WithTransaction(Transactions.Value);
        }

        return source;
    }
}