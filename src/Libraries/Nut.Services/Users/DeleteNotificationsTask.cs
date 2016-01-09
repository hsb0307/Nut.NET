using Nut.Core.Caching;
using Nut.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.Users {
    public class DeleteNotificationsTask : ITask {
        private readonly INotificationService _notificationService;

        public DeleteNotificationsTask(INotificationService notificationService) {
            this._notificationService = notificationService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute() {
            //60*24 = 1 day
            var olderThanMinutes = 1440; //TODO move to settings
            //Do not delete more than 1000 records per time. This way the system is not slowed down
            _notificationService.Delete(DateTime.UtcNow.AddSeconds(-olderThanMinutes));
        }
    }
}
