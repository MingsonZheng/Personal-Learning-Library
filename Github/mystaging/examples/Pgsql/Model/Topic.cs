using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStaging.DataAnnotations;

namespace Pgsql.Model
{
    [Table(name: "topic", Schema = "public")]
    public partial class Topic
    {
        [PrimaryKey]
        public Guid id { get; set; }
        public string title { get; set; }
        public DateTime? create_time { get; set; }
        public DateTime? update_time { get; set; }
        public DateTime? last_time { get; set; }
        public Guid? user_id { get; set; }
        public string name { get; set; }
        public int? age { get; set; }
        public bool? sex { get; set; }
        [Column(TypeName = "date")]
        public DateTime? createtime { get; set; }
        [Column(TypeName = "time")]
        public TimeSpan? updatetime { get; set; }
    }
}
