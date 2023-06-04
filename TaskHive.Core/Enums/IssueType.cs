using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskHive.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IssueType
    {
        Clarification = 1,
        DefectRemoval,
        Design,
        Deployment,
        DesignReview,
        Documentation,
        Estimation,
        ExpertTask,
        Integration,
        Monitoring,
        Planning,
        Reproduction,
        Review,
        Testing,
        TestDesign,
        Verification,
        WorkLog,
        Invoice,
        Approval,
        Coordination,
        SolutionDesign,
        SolutionDesignReview,
        Implementation
    }
}
