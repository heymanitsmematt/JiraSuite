﻿using System.Data.Entity.Core.Metadata.Edm;

namespace JiraSuite.DataAccess.Models
{
    public class JiraTypeHelper
    {
        public enum SoftwareType
        {
            MCLUB,
            MOSO,
            MOSO2,
            MOSO3,
            REP
        }

        public enum JiraIssueType
        {
            Epic = 11,
            Defect = 20,
            Task = 21,
            Story = 12,
            Bug = 1
        }
    }
}