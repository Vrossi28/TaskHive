using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;
using Newtonsoft.Json;

namespace TaskHive.Core.Entities
{
    public class Company
    {
        [Key]
        public Guid CompanyId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public State CompanyState { get; set; }

        [JsonIgnore]
        public virtual ICollection<Account> Accounts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Workspace> Workspaces { get; set; }
    }
}
