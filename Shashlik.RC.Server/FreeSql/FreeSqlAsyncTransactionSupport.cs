using System;
using System.Collections.Generic;
using System.Data;
using FreeSql;
using FreeSql.Internal;

namespace Shashlik.RC.Server.FreeSql;

public class FreeSqlAsyncTransactionSupport : IFreeSql
{
    public FreeSqlAsyncTransactionSupport(IFreeSql context)
    {
        Context = context;
    }

    private IFreeSql Context { get; }

    public void Dispose()
    {
        Context.Dispose();
    }

    public IInsert<T1> Insert<T1>() where T1 : class
    {
        return Context.Insert<T1>().WithTransactionIfRequire();
    }

    public IInsert<T1> Insert<T1>(T1 source) where T1 : class
    {
        return Context.Insert(source).WithTransactionIfRequire();
    }

    public IInsert<T1> Insert<T1>(T1[] source) where T1 : class
    {
        return Context.Insert(source).WithTransactionIfRequire();
    }

    public IInsert<T1> Insert<T1>(List<T1> source) where T1 : class
    {
        return Context.Insert(source).WithTransactionIfRequire();
    }

    public IInsert<T1> Insert<T1>(IEnumerable<T1> source) where T1 : class
    {
        return Context.Insert(source).WithTransactionIfRequire();
    }

    public IInsertOrUpdate<T1> InsertOrUpdate<T1>() where T1 : class
    {
        return Context.InsertOrUpdate<T1>().WithTransactionIfRequire();
    }

    public IUpdate<T1> Update<T1>() where T1 : class
    {
        return Context.Update<T1>().WithTransactionIfRequire();
    }

    public IUpdate<T1> Update<T1>(object dywhere) where T1 : class
    {
        return Context.Update<T1>(dywhere).WithTransactionIfRequire();
    }

    public ISelect<T1> Select<T1>() where T1 : class
    {
        return Context.Select<T1>();
    }

    public ISelect<T1> Select<T1>(object dywhere) where T1 : class
    {
        return Context.Select<T1>(dywhere);
    }

    public IDelete<T1> Delete<T1>() where T1 : class
    {
        return Context.Delete<T1>().WithTransactionIfRequire();
    }

    public IDelete<T1> Delete<T1>(object dywhere) where T1 : class
    {
        return Context.Delete<T1>(dywhere).WithTransactionIfRequire();
    }

    public void Transaction(Action handler)
    {
        Context.Transaction(handler);
    }

    public void Transaction(IsolationLevel isolationLevel, Action handler)
    {
        Context.Transaction(isolationLevel, handler);
    }

    public IAdo Ado => Context.Ado;
    public IAop Aop => Context.Aop;
    public ICodeFirst CodeFirst => Context.CodeFirst;
    public IDbFirst DbFirst => Context.DbFirst;
    public GlobalFilter GlobalFilter => Context.GlobalFilter;
}