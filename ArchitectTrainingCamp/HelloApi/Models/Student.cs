using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelloApi.Models
{
    public class Student
    {
        [Required]
        [Range(1,10,ErrorMessage = "id 为 1-10 之间的数字")]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
