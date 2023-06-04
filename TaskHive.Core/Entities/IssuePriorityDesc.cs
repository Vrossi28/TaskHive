﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;

namespace TaskHive.Core.Entities
{
    public class IssuePriorityDesc
    {
        [Key]
        public PriorityType IssuePriorityId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        public ICollection<Issue> Issues { get; set; }
    }
}
