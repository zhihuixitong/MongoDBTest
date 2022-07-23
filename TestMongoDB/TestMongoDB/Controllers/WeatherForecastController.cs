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
            //��ʼ������
            _helpMongDb = new MongoDBHelp();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {

           
            //��������
            var useData = new Use() { _id = Guid.NewGuid().ToString(), Name = "123456" };
            await _helpMongDb.InsertAsync<Use>(useData);
            //��ѯ
           var getData= _helpMongDb.Get<Use>(x=>x._id==useData._id);

            //ɾ��
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
        /// �������ݿ�
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
        /// �������ݿ�
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public async Task<Use> Update(Use use)
        {

            var getData = _helpMongDb.Update<Use>(x => x._id == use._id, use);
            if (getData.ModifiedCount == 0)
            {
                throw new Exception("����ʧ��");
            }

            return use;
        }


        /// <summary>
        /// ��ȡ���ݿ�
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("GetData")]
        public async Task<List<Use>> GetData(string useId)
        {
            //��ѯ
            var getData = _helpMongDb.Get<Use>(x => x._id == useId);

            return getData;
        }

        /// <summary>
        /// ��ȡ���ݿ�
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("GetDataBase")]
        public async Task<List<EntityBase>> GetDataBase(string useId)
        {
            //��ѯ
            var getData = _helpMongDb.Get<EntityBase>(x => x._id == useId);

            return getData;
        }

        /// <summary>
        /// ɾ�����ݿ�
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public async Task<long> Delete(string useId)
        {
            //��ѯ
            var getData = _helpMongDb.Delete<Use>(x => x._id == useId);
            if (getData.DeletedCount == 0)
            {
                throw new Exception("ɾ��ʧ��");
            }
            return getData.DeletedCount;
        }

    }
}