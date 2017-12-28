using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ACTransit.CusRel.Repositories
{
    public interface ITokenCacheRepository
    {
        void AddOrUpdate(TokenCacheItem item);
        TokenCacheItem Get(string key);
        void Remove(string key);
    }

    [Table("CacheItems")]
    public class TokenCacheItem
    {
        [Key]
        public string Key { get; set; }

        [Index]
        public DateTime Expires { get; set; }

        public byte[] Token { get; set; }
    }

    public class EFTokenCacheDataContext : DbContext
    {
        static EFTokenCacheDataContext()
        {
            Database.SetInitializer<EFTokenCacheDataContext>(new CreateInitializer());
        }

        public EFTokenCacheDataContext() : base("name=TokenCache") { }

        public DbSet<TokenCacheItem> Tokens { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }

        public void Seed(EFTokenCacheDataContext Context)
        {
#if DEBUG
            // Create default objects here
            //Context.CacheItems.Add(new CacheItem { });
#endif
            // Normal seeding goes here
            Context.SaveChanges();
        }

        public class DropCreateIfChangeInitializer : DropCreateDatabaseIfModelChanges<EFTokenCacheDataContext>
        {
            protected override void Seed(EFTokenCacheDataContext context)
            {
                context.Seed(context);
                base.Seed(context);
            }
        }

        public class CreateInitializer : CreateDatabaseIfNotExists<EFTokenCacheDataContext>
        {
            //public override void InitializeDatabase(EFTokenCacheDataContext context)
            //{
            //    base.InitializeDatabase(context);
            //}

            protected override void Seed(EFTokenCacheDataContext context)
            {
                context.Seed(context);
                base.Seed(context);
            }
        }
    }

    public class EFTokenCacheRepository : ITokenCacheRepository
    {
        public void AddOrUpdate(TokenCacheItem item)
        {
            using (var db = new EFTokenCacheDataContext())
            {
                var items = db.Set<TokenCacheItem>();
                var dbItem = items.Find(item.Key);
                if (dbItem == null)
                {
                    dbItem = new TokenCacheItem { Key = item.Key };
                    items.Add(dbItem);
                }
                dbItem.Token = item.Token;
                dbItem.Expires = item.Expires;
                db.SaveChanges();
            }
        }

        public TokenCacheItem Get(string key)
        {
            using (var db = new EFTokenCacheDataContext())
            {
                var items = db.Set<TokenCacheItem>();
                return items.Find(key);
            }
        }

        public void Remove(string key)
        {
            using (var db = new EFTokenCacheDataContext())
            {
                var items = db.Set<TokenCacheItem>();
                var item = items.Find(key);
                if (item == null) return;
                items.Remove(item);
                db.SaveChanges();
            }
        }

        private const int CleanupPeriodMilliseconds = (1000 * 60) * 60; // setup polling to cleanup expired tokens: 60 mins
        private const int SkewHours = 1;

        static EFTokenCacheRepository()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(CleanupPeriodMilliseconds);
                    try
                    {
                        // delete tokens older then one hour past expiration
                        var now = DateTime.UtcNow;
                        var skew = now.Subtract(TimeSpan.FromHours(SkewHours));
                        RemoveAllBefore(skew);
                    }
                    catch (Exception)
                    {
                        // log error
                    }
                }
            });
        }

        private static void RemoveAllBefore(DateTime date)
        {
            using (var db = new EFTokenCacheDataContext())
            {
                var items = db.Set<TokenCacheItem>();
                var query =
                    from item in items
                    where item.Expires <= date
                    select item;
                foreach (var item in query)
                {
                    items.Remove(item);
                }
                db.SaveChanges();
            }
        }
    }
}
