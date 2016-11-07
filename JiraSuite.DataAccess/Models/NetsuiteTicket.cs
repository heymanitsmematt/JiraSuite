﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace JiraSuite.DataAccess.Models
{
    public class Company
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }

    public class Stage
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name
        {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }

    public class Status
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name
        {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }

    public class Profile
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name
        {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }

    public class Category
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name
        {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }

    public class Assigned
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name
        {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }

    public class Priority
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name
        {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }

    public class Contact
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name
        {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }

    public class Columns
    {
        private List<JiraIssue> _jiraIusIssues = new List<JiraIssue>();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string casenumber { get; set; }
        public string title { get; set; }
        public virtual Company company { get; set; }
        public virtual Stage stage { get; set; }
        public virtual Status status { get; set; }
        public virtual Profile profile { get; set; }
        public string startdate { get; set; }
        public string createddate { get; set; }
        public virtual Category category { get; set; }
        public virtual Assigned assigned { get; set; }
        public virtual Priority priority { get; set; }
        public bool helpdesk { get; set; }
        public bool custevent10 { get; set; }
        public bool custevent27 { get; set; }
        public virtual List<JiraIssue> JiraIssues { get { return _jiraIusIssues; } set { _jiraIusIssues = value; } }
        public virtual Contact contact { get; set; }
        public string escalatedto { get; set; }
    }

    public class NetsuiteApiResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string id { get; set; }
        public string recordtype { get; set; }
        public virtual Columns columns { get; set; }
        
    }
}
