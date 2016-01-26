using Nut.Core.Domain.Users;
using Nut.Core.Events;

namespace Nut.Services.Security {
    public interface IPermissionEventHandler : IEventHandler {
        /// <summary>
        /// Authorize
        /// </summary>
        /// <param name="permissionContext"></param>
        void Authorize(PermissionContext permissionContext);
    }

    public class PermissionContext {
        public User User { get; set; }

        public string PermissionRecordSystemName { get; set; }
    }
}
