using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStaging.DataAnnotations;

namespace Pgsql.Model
{
    [Table(name: "user", Schema = "public")]
    public partial class User
    {
        [PrimaryKey]
        [Column(TypeName = "varchar(36)")]
        public string id { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string loginname { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string password { get; set; }
        public string nickname { get; set; }
        public bool? sex { get; set; }
        public int age { get; set; }
        [Column(TypeName = "numeric(10,2)")]
        public decimal money { get; set; }
        public DateTime createtime { get; set; }
        [Column(TypeName = "money")]
        public decimal wealth { get; set; }
        [Column(TypeName = "public.et_role")]
        public et_role? role { get; set; }
        public string IP { get; set; }
        public string[] citys { get; set; }
        public byte? sex2 { get; set; }
        public System.Collections.BitArray sex3 { get; set; }
        [Column(TypeName = "float4")]
        public double? sex4 { get; set; }
        [Column(TypeName = "float8")]
        public double? sex5 { get; set; }
        public TimeSpan? sex6 { get; set; }
        [Column(TypeName = "time")]
        public TimeSpan? sex7 { get; set; }
        [Column(TypeName = "date")]
        public DateTime? sex8 { get; set; }
        public DateTimeOffset? sex9 { get; set; }
        [Column(TypeName = "timestamptz")]
        public DateTime? sex10 { get; set; }
        [Column(TypeName = "text")]
        public string sex11 { get; set; }
        public short? sex12 { get; set; }
        public long? sex13 { get; set; }
        [Column(TypeName = "bpchar(1)")]
        public string sex14 { get; set; }
        [Column(TypeName = "float4")]
        public double? sex15 { get; set; }
        [Column(TypeName = "float4")]
        public double[] sex16 { get; set; }
    }
}
