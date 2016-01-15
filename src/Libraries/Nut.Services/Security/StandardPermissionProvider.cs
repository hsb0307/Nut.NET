using System.Collections.Generic;
using Nut.Core.Domain.Users;
using Nut.Core.Domain.Security;

namespace Nut.Services.Security {
    /// <summary>
    /// Standard permission provider
    /// </summary>
    public partial class StandardPermissionProvider : IPermissionProvider {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord AllowCustomerImpersonation = new PermissionRecord { Name = "Admin area. Allow Customer Impersonation", SystemName = "AllowCustomerImpersonation", Category = "Customers" };
        public static readonly PermissionRecord ManageUsers = new PermissionRecord { Name = "Admin area. Manage User", SystemName = "ManageUsers", Category = "Users" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Admin area. Manage Languages", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Admin area. Manage Settings", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "Admin area. Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "Admin area. Manage ACL", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageStores = new PermissionRecord { Name = "Admin area. Manage Stores", SystemName = "ManageStores", Category = "Configuration" };
        public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "Admin area. Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance = new PermissionRecord { Name = "Admin area. Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };
        public static readonly PermissionRecord HtmlEditorManagePictures = new PermissionRecord { Name = "Admin area. HTML Editor. Manage pictures", SystemName = "HtmlEditor.ManagePictures", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleTasks = new PermissionRecord { Name = "Admin area. Manage Schedule Tasks", SystemName = "ManageScheduleTasks", Category = "Configuration" };
        //APP
        public static readonly PermissionRecord ManageAppVersions = new PermissionRecord { Name = "Admin area. Manage AppVersions", SystemName = "ManageAppVersions", Category = "App" };
        public static readonly PermissionRecord ManageNewCates = new PermissionRecord { Name = "Admin area. Manage NewCates", SystemName = "ManageNewCates", Category = "NewCates" };


        public static readonly PermissionRecord PublicStoreAllow = new PermissionRecord { Name = "Public store. Allow", SystemName = "PublicStoreAllow", Category = "PublicStore" };

        public virtual IEnumerable<PermissionRecord> GetPermissions() {
            return new[]
            {
                AccessAdminPanel,
                AllowCustomerImpersonation,
                ManageUsers,
                ManageLanguages,
                ManageSettings,
                ManageActivityLog,
                ManageAcl,
                ManageStores,
                ManagePlugins,
                HtmlEditorManagePictures,
                ManageScheduleTasks,
                ManageMaintenance
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions() {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemUserRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        AccessAdminPanel,
                        AllowCustomerImpersonation,
                        ManageUsers,
                        ManageLanguages,
                        ManageSettings,
                        ManageActivityLog,
                        ManageAcl,
                        ManageStores,
                        ManagePlugins,
                        HtmlEditorManagePictures,
                        ManageScheduleTasks,
                        ManageMaintenance
                    }
                },
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemUserRoleNames.Guests,
                    PermissionRecords = new[]
                    {
                        PublicStoreAllow
                    }
                },
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemUserRoleNames.Registered,
                    PermissionRecords = new[]
                    {
                        PublicStoreAllow
                    }
                }
            };
        }
    }
}