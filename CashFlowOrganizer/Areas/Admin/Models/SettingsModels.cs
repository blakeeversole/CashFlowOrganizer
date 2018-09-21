using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Areas.Admin.Models
{
    public class ManageUsersViewModel
    { 
        public List<UserFields> UserList { get; set; }
    }

    public class UserFields
    {
        public string AspNetUsersID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }

    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Descr { get; set; }
        [Required]
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
    }

    public class Admin
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserEdit
    {
        public string AspNetUsersID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}