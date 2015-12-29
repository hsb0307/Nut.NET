using FluentValidation;

namespace Nut.Web.Framework.Validators
{
    public abstract class BaseNutValidator<T> : AbstractValidator<T> where T : class
    {
        protected BaseNutValidator()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
    }
}