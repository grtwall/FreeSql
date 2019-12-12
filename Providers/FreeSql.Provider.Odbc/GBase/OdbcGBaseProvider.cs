﻿using FreeSql.Internal;
using FreeSql.Internal.CommonProvider;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FreeSql.Odbc.GBase
{

    public class OdbcGBaseProvider<TMark> : IFreeSql<TMark>
    {

        static OdbcGBaseProvider()
        {
        }

        public ISelect<T1> Select<T1>() where T1 : class => new OdbcGBaseSelect<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public ISelect<T1> Select<T1>(object dywhere) where T1 : class => new OdbcGBaseSelect<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);
        public IInsert<T1> Insert<T1>() where T1 : class => new OdbcGBaseInsert<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression);
        public IInsert<T1> Insert<T1>(T1 source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(T1[] source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(List<T1> source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(IEnumerable<T1> source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IUpdate<T1> Update<T1>() where T1 : class => new OdbcGBaseUpdate<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public IUpdate<T1> Update<T1>(object dywhere) where T1 : class => new OdbcGBaseUpdate<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);
        public IDelete<T1> Delete<T1>() where T1 : class => new OdbcGBaseDelete<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public IDelete<T1> Delete<T1>(object dywhere) where T1 : class => new OdbcGBaseDelete<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);

        public IAdo Ado { get; }
        public IAop Aop { get; }
        public ICodeFirst CodeFirst { get; }
        public IDbFirst DbFirst { get; }
        public OdbcGBaseProvider(string masterConnectionString, string[] slaveConnectionString)
        {
            this.InternalCommonUtils = new OdbcGBaseUtils(this);
            this.InternalCommonExpression = new OdbcGBaseExpression(this.InternalCommonUtils);

            this.Ado = new OdbcGBaseAdo(this.InternalCommonUtils, masterConnectionString, slaveConnectionString);
            this.Aop = new AopProvider();

            this.DbFirst = new OdbcGBaseDbFirst(this, this.InternalCommonUtils, this.InternalCommonExpression);
            this.CodeFirst = new OdbcGBaseCodeFirst(this, this.InternalCommonUtils, this.InternalCommonExpression);
        }

        internal CommonUtils InternalCommonUtils { get; }
        internal CommonExpression InternalCommonExpression { get; }

        public void Transaction(Action handler) => Ado.Transaction(handler);
        public void Transaction(Action handler, TimeSpan timeout) => Ado.Transaction(handler, timeout);

        public GlobalFilter GlobalFilter { get; } = new GlobalFilter();

        ~OdbcGBaseProvider() => this.Dispose();
        int _disposeCounter;
        public void Dispose()
        {
            if (Interlocked.Increment(ref _disposeCounter) != 1) return;
            (this.Ado as AdoProvider)?.Dispose();
        }
    }
}