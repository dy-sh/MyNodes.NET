namespace MyNodes.Users
{
    public class UserPermissions
    {
        public bool DashboardObserver { get; set; }
        public bool DashboardEditor { get; set; }
        public bool EditorObserver { get; set; }
        public bool EditorEditor { get; set; }
        public bool EditorProtectedAccess { get; set; }
        public bool HardwareObserver { get; set; }
        public bool LogsObserver { get; set; }
        public bool LogsEditor { get; set; }
        public bool ConfigObserver { get; set; }
        public bool ConfigEditor { get; set; }
        public bool UsersObserver { get; set; }
        public bool UsersEditor { get; set; }
    }
}
