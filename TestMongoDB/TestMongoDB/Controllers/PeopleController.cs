using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestMongoDB.Entity;
using TestMongoDB.Help;

namespace TestMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        public MongoDBHelp _helpMongDb;
        public PeopleController()
        {
            //初始化连接
            _helpMongDb = new MongoDBHelp();
        }

        [HttpPost]
        public async Task<People> Add(People people)
        {
            people._id = Guid.NewGuid().ToString();
            people.SerialNo = Guid.NewGuid();
            await _helpMongDb.InsertAsync<People>(people);
            return people;
        }

        [HttpPost("Add10000")]
        public async Task<List<People>> Add10000(string name)
        {
            var peopleList=new List<People>();

            for(var i = 0; i < 10000; i++)
            {
                People people = new()
                {
                    _id = Guid.NewGuid().ToString(),
                    SerialNo = Guid.NewGuid(),
                    IdNum = i,
                    Num = i,
                    Name = name,
                    Phone = new List<string>() { "12345679", "12346789" },
                    Sex = "男"
                };
                peopleList.Add(people);
            }

            
            await _helpMongDb.BatchInsertAsync<People>(peopleList);


            return peopleList;
        }

        [HttpPost("Get")]
        public async Task<List<People>> Get()
        {
      
            var peopleList = _helpMongDb.Get<People>(x => x.Num > 0,0,999999,null);
            return peopleList;
        }
    }
}
