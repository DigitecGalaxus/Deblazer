using System;
using System.Collections.Generic;
using System.Linq;
using Dg.Deblazer.Internal;

namespace Dg.Deblazer.SqlGeneration
{
    /// <summary>
    /// The following Methods are implemented as extension methods because they should only be available for DbEntities and not for JoinElements
    /// (TCurrent has to be DbEntity not just IQueryReturnType)
    /// </summary>
    public static class QueryExtensions
    {
        public static QueryJoinElements<TQuery, TOriginal, Id<TCurrent>> Join<TBack, TOriginal, TCurrent, TWrapper, TQuery>(
            this Query<TBack, TOriginal, TCurrent, TWrapper, TQuery> query,
            IEnumerable<int> elements)
            where TBack : QueryBase
            where TOriginal : DbEntity, IId
            where TCurrent : DbEntity, IId
            where TWrapper : QueryWrapper
            where TQuery : Query<TBack, TOriginal, TCurrent, TWrapper, TQuery>
        {
            var queryInternal = (IQueryInternal<TBack, TOriginal, TCurrent, TWrapper, TQuery>)query;
            return queryInternal.JoinIds(JoinType.Inner, QueryWrapper.IdColumnName, elements.Select(e => new Id<TCurrent>(e)));
        }

        public static QueryJoinElements<TQuery, TOriginal, Id<TCurrent>> Join<TBack, TOriginal, TCurrent, TWrapper, TQuery>(
            this Query<TBack, TOriginal, TCurrent, TWrapper, TQuery> query,
            IEnumerable<int?> elements)
            where TBack : QueryBase
            where TOriginal : DbEntity, IId
            where TCurrent : DbEntity, IId
            where TWrapper : QueryWrapper
            where TQuery : Query<TBack, TOriginal, TCurrent, TWrapper, TQuery>
        {
            var queryInternal = (IQueryInternal<TBack, TOriginal, TCurrent, TWrapper, TQuery>)query;
            return queryInternal.JoinIds(JoinType.Inner, QueryWrapper.IdColumnName, elements.Where(e => e.HasValue).Select(e => new Id<TCurrent>(e.Value)));
        }

        public static QueryJoinElements<TQuery, TOriginal, Id<TCurrent>> Join<TBack, TOriginal, TCurrent, TWrapper, TQuery>(
            this Query<TBack, TOriginal, TCurrent, TWrapper, TQuery> query,
            IEnumerable<Id<TCurrent>> elements)
            where TBack : QueryBase
            where TOriginal : DbEntity, IId
            where TCurrent : DbEntity, IId
            where TWrapper : QueryWrapper
            where TQuery : Query<TBack, TOriginal, TCurrent, TWrapper, TQuery>
        {
            var queryInternal = (IQueryInternal<TBack, TOriginal, TCurrent, TWrapper, TQuery>)query;
            return queryInternal.JoinIds(JoinType.Inner, QueryWrapper.IdColumnName, elements);
        }

        public static QueryJoinElements<TQuery, TOriginal, Id<TOther>> Join<TBack, TOriginal, TCurrent, TWrapper, TQuery, TOther>(
            this Query<TBack, TOriginal, TCurrent, TWrapper, TQuery> query,
            Func<TWrapper, QueryElMemberId<TOther>> func,
            IEnumerable<int> elements,
            JoinType joinType = JoinType.Inner)
            where TBack : QueryBase
            where TOriginal : DbEntity, IId
            where TCurrent : DbEntity, IId
            where TWrapper : QueryWrapper
            where TQuery : Query<TBack, TOriginal, TCurrent, TWrapper, TQuery>
            where TOther : DbEntity, IId
        {
            Func<TWrapper, QueryElMember<int>> f = x => new QueryElMember<int>(func(x).MemberName);

            var queryInternal = (IQueryInternal<TBack, TOriginal, TCurrent, TWrapper, TQuery>)query;
            return queryInternal.JoinIds(joinType, f(queryInternal.GetWrapper()).MemberName, elements.Select(e => new Id<TOther>(e)));
        }

        public static QueryJoinElements<TQuery, TOriginal, Id<TOther>> Join<TBack, TOriginal, TCurrent, TWrapper, TQuery, TOther>(
            this Query<TBack, TOriginal, TCurrent, TWrapper, TQuery> query,
            Func<TWrapper, QueryElMemberNullableId<TOther>> func,
            IEnumerable<int> elements,
            JoinType joinType = JoinType.Inner)
            where TBack : QueryBase
            where TOriginal : DbEntity, IId
            where TCurrent : DbEntity, IId
            where TWrapper : QueryWrapper
            where TQuery : Query<TBack, TOriginal, TCurrent, TWrapper, TQuery>
            where TOther : DbEntity, IId
        {
            var queryInternal = (IQueryInternal<TBack, TOriginal, TCurrent, TWrapper, TQuery>)query;
            return queryInternal.JoinIds(joinType, func(queryInternal.GetWrapper()).MemberName, elements.Select(e => new Id<TOther>(e)));
        }

        public static QueryJoinElements<TQuery, TOriginal, Id<TOther>> Join<TBack, TOriginal, TCurrent, TWrapper, TQuery, TOther>(
    this Query<TBack, TOriginal, TCurrent, TWrapper, TQuery> query,
    Func<TWrapper, QueryElMemberNullableId<TOther>> func,
    IEnumerable<Id<TOther>> elements,
    JoinType joinType = JoinType.Inner)
    where TBack : QueryBase
    where TOriginal : DbEntity, IId
    where TCurrent : DbEntity, IId
    where TWrapper : QueryWrapper
    where TQuery : Query<TBack, TOriginal, TCurrent, TWrapper, TQuery>
    where TOther : DbEntity, IId
        {
            var queryInternal = (IQueryInternal<TBack, TOriginal, TCurrent, TWrapper, TQuery>)query;
            return queryInternal.JoinIds(joinType, func(queryInternal.GetWrapper()).MemberName, elements);
        }

        public static QueryJoinElements<TQuery, TOriginal, Id<TOther>> Join<TBack, TOriginal, TCurrent, TWrapper, TQuery, TOther>(
            this Query<TBack, TOriginal, TCurrent, TWrapper, TQuery> query,
            Func<TWrapper, QueryElMemberNullableId<TOther>> func,
            IEnumerable<int?> elements,
            JoinType joinType = JoinType.Inner)
            where TBack : QueryBase
            where TOriginal : DbEntity, IId
            where TCurrent : DbEntity, IId
            where TWrapper : QueryWrapper
            where TQuery : Query<TBack, TOriginal, TCurrent, TWrapper, TQuery>
            where TOther : DbEntity, IId
        {
            var queryInternal = (IQueryInternal<TBack, TOriginal, TCurrent, TWrapper, TQuery>)query;
            return queryInternal.JoinIds(joinType, func(queryInternal.GetWrapper()).MemberName, elements.Where(e => e.HasValue).Select(e => new Id<TOther>(e.Value)));
        }

        public static QueryJoinElements<TQuery, TOriginal, TValue> Join<TBack, TOriginal, TCurrent, TWrapper, TQuery, TValue>(
            this Query<TBack, TOriginal, TCurrent, TWrapper, TQuery> query,
            Func<TWrapper, QueryElMember<TValue>> func,
            IEnumerable<TValue> elements,
            JoinType joinType = JoinType.Inner)
            where TBack : QueryBase
            where TOriginal : DbEntity, IId
            where TCurrent : DbEntity, IId
            where TWrapper : QueryWrapper
            where TQuery : Query<TBack, TOriginal, TCurrent, TWrapper, TQuery>
        {
            var queryInternal = (IQueryInternal<TBack, TOriginal, TCurrent, TWrapper, TQuery>)query;
            var queryElMember = func(queryInternal.GetWrapper());
            var joinedQuery = new QueryJoinElements<TQuery, TOriginal, TValue>(queryInternal.Db, elements);
            return queryInternal.Join(joinType, queryElMember.MemberName, joinedQuery);
        }
    }
}
