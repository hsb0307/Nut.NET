using Nut.Core.Events;
using Nut.Core.Domain.Users;

namespace Nut.Services.Authentication {

    public interface IAuthenticationEventHandler : IEventHandler {
        /// <summary>
        /// Called after a user has logged in
        /// </summary>
        /// <param name="userContext">user Context</param>
        void SignIn(UserContext userContext);
        /// <summary>
        /// Called when a user explicitly logs out
        /// </summary>
        void SignOut();
    }

    public class UserContext {
        public User User { get; set; }
    }
}
