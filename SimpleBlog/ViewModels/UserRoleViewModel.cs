﻿namespace SimpleBlog.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public List<string> Roles { get; set; }
    }
}
