namespace SimpleBlog.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}
