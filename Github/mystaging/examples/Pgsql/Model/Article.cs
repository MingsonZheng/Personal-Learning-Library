using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyStaging.DataAnnotations;

namespace Pgsql.Model
{
    [Table(name: "article", Schema = "public")]
    public partial class Article
    {
        [PrimaryKey]
        public string id { get; set; }
        [PrimaryKey]
        public string userid { get; set; }
        public string title { get; set; }
        public JsonElement content { get; set; }
        public DateTime createtime { get; set; }
    }
}
