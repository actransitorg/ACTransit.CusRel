using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using EFCache;

namespace ACTransit.CusRel.Repositories.Caching
{
    public class CachingConfiguration: DbConfiguration
    {
        public CachingConfiguration()
        {
            var transactionHandler = new CacheTransactionHandler(new InMemoryCache());

            AddInterceptor(transactionHandler);

            var cachingPolicy = new CusRelCachingPolicy();

            Loaded += (sender, args) => args.ReplaceService<DbProviderServices>( (s, _) => new CachingProviderServices(s, transactionHandler, cachingPolicy));
        }
    }

    public class CusRelCachingPolicy : CachingPolicy
    {
        protected override bool CanBeCached(ReadOnlyCollection<EntitySetBase> affectedEntitySets, string sql, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return false; // TODO: Disable 'all queries' read caching (for now), doesn't affect explicit InMemoryCache caching (GetItem/PutItem)
        }

        protected override void GetCacheableRows(ReadOnlyCollection<EntitySetBase> affectedEntitySets, out int minCacheableRows, out int maxCacheableRows)
        {
            minCacheableRows = 0;
            maxCacheableRows = int.MaxValue;
        }

        protected override void GetExpirationTimeout(ReadOnlyCollection<EntitySetBase> affectedEntitySets, out TimeSpan slidingExpiration, out DateTimeOffset absoluteExpiration)
        {
            slidingExpiration = TimeSpan.FromMinutes(1);
            absoluteExpiration = DateTimeOffset.Now.AddMinutes(1);
        }
    }
}
