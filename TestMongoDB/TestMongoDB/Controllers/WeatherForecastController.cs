using Microsoft.AspNetCore.Mvc;
using TestMongoDB.Entity;
using TestMongoDB.Help;

namespace TestMongoDB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;


        public MongoDBHelp _helpMongDb;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            //初始化连接
            _helpMongDb = new MongoDBHelp();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {

           
            //插入数据
            var useData = new Use() { _id = Guid.NewGuid().ToString(), Name = "123456" };
            await _helpMongDb.InsertAsync<Use>(useData);
            //查询
           var getData= _helpMongDb.Get<Use>(x=>x._id==useData._id);

            //删除
            var getData2 = _helpMongDb.Delete<Use>(x=>x._id== useData._id);



            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("InsertAsync")]
        public async Task<Use> InsertAsync(string useName)
        {
            var  use = new Use();
            use._id = Guid.NewGuid().ToString();
            use.Name = useName;
            use.SerialNo = Guid.NewGuid();
            //use.AboutUse = new Use();
            await _helpMongDb.InsertAsync<Use>(use);

            return use;
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public async Task<Use> Update(Use use)
        {

            var getData = _helpMongDb.Update<Use>(x => x._id == use._id, use);
            if (getData.ModifiedCount == 0)
            {
                throw new Exception("更新失败");
            }

            return use;
        }


        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("GetData")]
        public async Task<List<Use>> GetData(string useId)
        {
            //查询
            var getData = _helpMongDb.Get<Use>(x => x._id == useId);

            return getData;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("GetDataBase")]
        public async Task<List<EntityBase>> GetDataBase(string useId)
        {
            //查询
            var getData = _helpMongDb.Get<EntityBase>(x => x._id == useId);

            return getData;
        }

        /// <summary>
        /// 删除数据库
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public async Task<long> Delete(string useId)
        {
            //查询
            var getData = _helpMongDb.Delete<Use>(x => x._id == useId);
            if (getData.DeletedCount == 0)
            {
                throw new Exception("删除失败");
            }
            return getData.DeletedCount;
        }

    }
}