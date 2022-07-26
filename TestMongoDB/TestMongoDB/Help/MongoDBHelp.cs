﻿using MongoDB.Driver;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TestMongoDB.Help
{
    public class MongoDBHelp
    {
        private static MongoClient client;
        private static IMongoDatabase database;
        //本地配置
        private const string MongoDBConnectionStr = "mongodb://localhost:27017";
        //数据库名称
        private static string DefaultDataBaseName = "Use";
        public MongoDBHelp()
        {
            GetConnection(DefaultDataBaseName);
        }

        /// <summary>
        /// 构造函数 指定数据库
        /// </summary>
        /// <param name="dataBaseName"></param>
        public MongoDBHelp(string dataBaseName)
        {
            GetConnection(dataBaseName);
        }

        private static void GetConnection(string dataBaseName)
        {
            client = new MongoClient(MongoDBConnectionStr);
            database = client.GetDatabase(dataBaseName);
        }



        /// <summary>
        /// 异步插入一条数据，手动输入collection name
        /// </summary>
        public Task InsertAsync<T>(string collectionName, T obj)
        {
            if (database == null)
            {
                throw new Exception("没有指定数据库");
            }
            var collection = database.GetCollection<T>(collectionName);
            return collection.InsertOneAsync(obj);
        }

        /// <summary>
        /// 插入行数据，表和库不存在则自动  创建对应的  数据库 和 表
        /// </summary>
        public Task InsertAsync<T>(T obj)
        {
            return InsertAsync(typeof(T).FullName, obj);
        }



        /// <summary>
        /// 异步插入多条数据，手动输入collection name
        /// </summary>
        public Task BatchInsertAsync<T>(string collectionName, IEnumerable<T> objs)
        {
            if (database == null)
            {
                throw new Exception("没有指定数据库");
            }
            if (objs == null)
            {
                throw new ArgumentException();
            }
            var collection = database.GetCollection<T>(collectionName);

            return collection.InsertManyAsync(objs);
        }

        /// <summary>
        /// 异步插入多条数据，采用类型T的完全限定名作为collection name
        /// </summary>
        public Task BatchInsertAsync<T>(IEnumerable<T> objs)
        {
            return BatchInsertAsync(typeof(T).FullName, objs);
        }

        /// <summary>
        /// 插入一条数据
        /// </summary>
        public void Insert<T>(T obj)
        {
            InsertAsync(obj).Wait();
        }

        /// <summary>
        /// 插入多条数据
        /// </summary>
        public void Insert<T>(IEnumerable<T> objs)
        {
            BatchInsertAsync(objs).Wait();
        }

        /// <summary>
        /// MongoDB C# Driver的Find方法，返回IFindFluent。手动输入collection name
        /// </summary>
        public IFindFluent<T, T> Find<T>(string collectionName, FilterDefinition<T> filter, FindOptions options = null)
        {
            if (database == null)
            {
                throw new Exception("没有指定数据库");
            }
            var collection = database.GetCollection<T>(collectionName);
            return collection.Find(filter, options);
        }

        /// <summary>
        /// MongoDB C# Driver的Find方法，返回IFindFluent。采用类型T的完全限定名作为collection name
        /// </summary>
        public IFindFluent<T, T> Find<T>(FilterDefinition<T> filter, FindOptions options = null)
        {
            return Find(typeof(T).FullName, filter, options);
        }

        /// <summary>
        /// 取符合条件的数据 sort中多个排序条件逗号分隔，默认asc
        /// </summary>
        public List<T> Get<T>(Expression<Func<T, bool>> condition, int skip, int limit, string sort)
        {
            return Get(new List<Expression<Func<T, bool>>> { condition }, skip, limit, sort);
        }

        public List<T> Get<T>(Expression<Func<T, bool>> condition)
        {
            return Get(condition, 0, 0, null);
        }

        /// <summary>
        /// 取符合条件的数据 sort中多个排序条件逗号分隔，默认asc
        /// </summary>
        public List<T> Get<T>(List<Expression<Func<T, bool>>> conditions, int skip, int limit, string sort)
        {
            if (conditions == null || conditions.Count == 0)
            {
                conditions = new List<Expression<Func<T, bool>>> { x => true };
            }
            var builder = Builders<T>.Filter;
            var filter = builder.And(conditions.Select(x => builder.Where(x)));

            var ret = new List<T>();
            try
            {
                List<SortDefinition<T>> sortDefList = new List<SortDefinition<T>>();
                if (sort != null)
                {
                    var sortList = sort.Split(',');
                    for (var i = 0; i < sortList.Length; i++)
                    {
                        var sl = Regex.Replace(sortList[i].Trim(), @"\s+", " ").Split(' ');
                        if (sl.Length == 1 || (sl.Length >= 2 && sl[1].ToLower() == "asc"))
                        {
                            sortDefList.Add(Builders<T>.Sort.Ascending(sl[0]));
                        }
                        else if (sl.Length >= 2 && sl[1].ToLower() == "desc")
                        {
                            sortDefList.Add(Builders<T>.Sort.Descending(sl[0]));
                        }
                    }
                }
                var sortDef = Builders<T>.Sort.Combine(sortDefList);
                ret = Find(filter).Sort(sortDef).Skip(skip).Limit(limit).ToListAsync().Result;

                var sortDef2 = Builders<T>.Sort.Combine(sortDefList);

          
            }
            catch (Exception e)
            {
                //异常处理
            }
            return ret;
        }


        public List<T> Get2<T>(List<Expression<Func<T, bool>>> conditions, int skip, int limit, string sort)
        {
            if (conditions == null || conditions.Count == 0)
            {
                conditions = new List<Expression<Func<T, bool>>> { x => true };
            }
            var builder = Builders<T>.Filter;
            var filter = builder.And(conditions.Select(x => builder.Where(x)));

            var ret = new List<T>();
            try
            {
                List<SortDefinition<T>> sortDefList = new List<SortDefinition<T>>();
                if (sort != null)
                {
                    var sortList = sort.Split(',');
                    for (var i = 0; i < sortList.Length; i++)
                    {
                        var sl = Regex.Replace(sortList[i].Trim(), @"\s+", " ").Split(' ');
                        if (sl.Length == 1 || (sl.Length >= 2 && sl[1].ToLower() == "asc"))
                        {
                            sortDefList.Add(Builders<T>.Sort.Ascending(sl[0]));
                        }
                        else if (sl.Length >= 2 && sl[1].ToLower() == "desc")
                        {
                            sortDefList.Add(Builders<T>.Sort.Descending(sl[0]));
                        }
                    }
                }
                var sortDef = Builders<T>.Sort.Combine(sortDefList);
                ret = Find(filter).Sort(sortDef).Skip(skip).Limit(limit).ToListAsync().Result;

                var sortDef2 = Builders<T>.Sort.Combine(sortDefList);


            }
            catch (Exception e)
            {
                //异常处理
            }
            return ret;
        }


        public List<T> Get<T>(List<Expression<Func<T, bool>>> conditions)
        {
            return Get(conditions, 0, 0, null);
        }

        public DeleteResult Delete<T>(Expression<Func<T, bool>> condition)
        {
            var conditions = new List<Expression<Func<T, bool>>> { condition };
 
            var builder = Builders<T>.Filter;
            var filter = builder.And(conditions.Select(x => builder.Where(x)));


            if (database == null)
            {
                throw new Exception("没有指定数据库");
            }
            var collection = database.GetCollection<T>(typeof(T).FullName);

            return collection.DeleteOne(filter); 
        }


        /// <summary>
        /// 获取更新信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public UpdateResult Update<T>(Expression<Func<T, bool>> condition,T updateData)
        {

            var collection = database.GetCollection<T>(typeof(T).FullName);

            return collection.UpdateOne<T>(condition, GetUpdateDefinition<T>(updateData));



        }

        /// <summary>
        /// 获取更新信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public UpdateDefinition<T> GetUpdateDefinition<T>( T entity)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var updateDefinitionList = GetUpdateDefinitionList<T>(properties, entity);

            var updateDefinitionBuilder = new UpdateDefinitionBuilder<T>().Combine(updateDefinitionList);

            return updateDefinitionBuilder;


        }

        /// <summary>
        /// 获取更新信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfos"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<UpdateDefinition<T>> GetUpdateDefinitionList<T>(PropertyInfo[] propertyInfos, object entity)
        {
            var updateDefinitionList = new List<UpdateDefinition<T>>();

            propertyInfos = propertyInfos.Where(a => a.Name != "_id").ToArray();

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType.IsArray || typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var value = propertyInfo.GetValue(entity) as IList;

                    var filedName = propertyInfo.Name;

                    updateDefinitionList.Add(Builders<T>.Update.Set(filedName, value));
                }
                else
                {
                    var value = propertyInfo.GetValue(entity);
                    if (propertyInfo.PropertyType == typeof(decimal))
                        value = value.ToString();

                    var filedName = propertyInfo.Name;

                    updateDefinitionList.Add(Builders<T>.Update.Set(filedName, value));
                }
            }

            return updateDefinitionList;
        }
    }
}
