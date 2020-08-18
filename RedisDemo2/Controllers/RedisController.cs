using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisDemo2.Controllers
{
    public static class RedisUltis
    {
        public static HashEntry[] ToHashEntries(this object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties.Select(property => new HashEntry(property.Name, property.GetValue(obj).ToString())).ToArray();
        }
    }
    public class ZSet
    {
        public string zsetKey { get; set; }
        public string value { get; set; }
        public double score { get; set; }
    }

    
    public class HashData
    {
        public string keyHash { get; set; }
        public fields fields { get; set; }

    }
    public class fields
    {
        public string name { get; set; }
        public string age { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _database;

        public RedisController(IDatabase database)
        {
            _database = database;
        }

        // GET: api/Cache?key=key
        [HttpGet("GetString")]
        public string GetString([FromQuery] string key)
        {
            return _database.StringGet(key);
        }

        // POST: api/AddString
        // Add string theo cap key-value
        [HttpPost("AddString")]
        public KeyValuePair<string, string> AddString([FromBody] KeyValuePair<string, string> keyValue)
        {
            _database.StringSet(keyValue.Key, keyValue.Value);
            return keyValue;
        }

        // Add vao list 
        [HttpPost("AddList")]
        public RedisValue[] AddList([FromBody] KeyValuePair<string, string> keyValue)
        {
            _database.ListLeftPush(keyValue.Key,keyValue.Value);
            return _database.ListRange(keyValue.Key,0);
        }

        // GET : List

        [HttpGet("GetList")]
        public RedisValue[] GetList([FromQuery] string keyList)
        {
            return _database.ListRange(keyList, 0);
        }

        // GET : Set
        [HttpGet("GetSet")]
        public RedisValue[] GetSet([FromQuery] string setKey)
        {
            
             return _database.SetMembers(setKey);
        }

        // POST : Add Set

        [HttpPost("AddSet")]
        public RedisValue[] AddSet([FromBody] KeyValuePair<string, string> keyValue)
        {
            _database.SetAdd(keyValue.Key, keyValue.Value);
            return _database.SetMembers(keyValue.Key);
        }

        // Post : Sorted Set

        [HttpPost("AddSortedSet")]
        public SortedSetEntry[] AddSortedSet ([FromBody] ZSet zSet)
        {
            _database.SortedSetAdd(zSet.zsetKey, zSet.value, zSet.score);
            return _database.SortedSetRangeByRankWithScores(zSet.zsetKey);
        }

        // Get : SortedSet
        [HttpGet("GetSortedSet")]
        public SortedSetEntry[] GetSortedSet ([FromQuery] string zsetKey)
        {
            return _database.SortedSetRangeByRankWithScores(zsetKey);
        }


        // POST : Hash

        [HttpPost("AddHash")]
        public HashEntry[] AddHash([FromBody] HashData hashData)
        {
            _database.HashSet(hashData.keyHash, hashData.fields.ToHashEntries());
            return _database.HashGetAll(hashData.keyHash);
        }
        
        //GET : Hash
        [HttpGet("GetHash")]
        public HashEntry[] GetHash([FromQuery] string hashKey)
        {
            return _database.HashGetAll(hashKey);
        }

       

    }
}
