namespace TestMongoDB.Entity
{
    public class Use
    {
        public string _id { get; set; }
        public string Name { get; set; }

        public string Sex { get; set; }


        public int Num { get; set; }
        public long IdNum { get; set; }

        public Guid SerialNo { get; set; }

        public Use AboutUse { get; set; }
    }


    public class EntityBase
    {
        public string _id { get; set; }
    }


    public class People
    {
        public string? _id { get; set; }
        public string Name { get; set; }

        public string Sex { get; set; }


        public int Num { get; set; }
        public long IdNum { get; set; }

        public Guid SerialNo { get; set; }

        public List<string> Phone { get; set; }
    }


}
