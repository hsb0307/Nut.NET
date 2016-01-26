using Nut.Core.Events;
using Nut.Core.Domain.Users;

namespace Nut.Services.Users {

    public interface IUserEventHandler : IEventHandler {

        /// <summary>
        /// Creatings the specified user context.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        void Creating(UserContext userContext);

        /// <summary>
        /// Createds the specified user context.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        void Created(UserContext userContext);


        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        void ChangePassword(UserContext userContext);

    }

    public class UserContext {

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UserContext"/> is cancel.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
