using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nut.Services.Tasks;

namespace Nut.Services.Users {
    public class DeleteGuestsTask : ITask {

        private readonly IUserService _userService;

        public DeleteGuestsTask(IUserService userService) {
            this._userService = userService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute() {

            //60*24 = 1 day
            var olderThanMinutes = 1440; //TODO move to settings
            //Do not delete more than 1000 records per time. This way the system is not slowed down
            _userService.DeleteGuestUsers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
        }
    }
}
