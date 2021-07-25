using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firebase_Trial
{
    public class CollectionModel
    {
        [AutoIncrement,PrimaryKey] public int ID { get; set; }
        public string imgName { get; set; }
        public string image { get; set; }
        public string Timestamp { get; set; }
    }
}
