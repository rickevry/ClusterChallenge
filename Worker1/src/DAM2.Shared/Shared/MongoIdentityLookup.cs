using MongoDB.Driver;
using Proto.Cluster.Identity;

using DAM2.Shared.ProtoActorIdentity.Mongo;

namespace DAM2.Core.Shared
{
    public class MongoIdentityLookup
    {
        public static IIdentityLookup GetIdentityLookup(string clusterName, string connectionString, string pidCollection, string pidDatabaseName)
        {
            var db = GetMongo(connectionString, pidDatabaseName);
            var identity = new IdentityStorageLookup(
                new MongoIdentityStorage(clusterName, db.GetCollection<PidLookupEntity>(pidCollection), 200)
            );
            return identity;
        }

        
        private static IMongoDatabase GetMongo(string connectionString, string databaseName)
        {
            var url = MongoUrl.Create(connectionString);
            var settings = MongoClientSettings.FromUrl(url);
            //settings.WaitQueueTimeout = TimeSpan.FromSeconds(10);
            //settings.WaitQueueSize = 10000;
            
            var client = new MongoClient(settings);
            var database = client.GetDatabase(databaseName);
            return database;
        }
    }
}
