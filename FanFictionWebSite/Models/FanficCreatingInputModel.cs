using FanFictionWebSite.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Models
{
    public class FanficCreatingInputModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int FanficId { get; set; }
        [Required]
        public bool IsUpdating { get; set; }
        [Required]
        public string Category { get; set; }
    }
}
