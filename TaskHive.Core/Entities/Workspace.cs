using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskHive.Core.Entities
{
    public class Workspace
    {
        [Key]
        public Guid WorkspaceId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public bool HasHourlyRate { get; set; }
        [Required]
        [MaxLength(4)]
        public string Abbreviation { get; set; }
        [MaxLength(3)]
        public string? Currency { get; set; }

        [JsonIgnore]
        public virtual ICollection<WorkspaceValuePerHour> WorkspaceHourlyRates { get; }
        [JsonIgnore]
        public virtual ICollection<AccountWorkspace> AccountWorkspaces { get; }
        [JsonIgnore]
        public virtual Company Company { get; set; }
    }
}
