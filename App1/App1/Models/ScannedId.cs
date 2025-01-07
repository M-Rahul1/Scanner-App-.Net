using SQLite;
using SQLite.Net.Attributes;

namespace App1.Models
{
    public class ScannedItem
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Key { get; set; }
        public string Id { get; set; }
       public string ScannedValue { get; set; }

    }

    public class ScannedId
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Key { get; set; }
        public string Id { get; set; }
        public string ScannedValue { get; set; }
    }
}
