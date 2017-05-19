using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsForms.Models
{
    public class Ventigrater
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 20)]
        public string Title { get; set; }
        [Range(0, 20)]
        public int Experience { get; set; }
    }
}
