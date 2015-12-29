using System.Data.Entity.ModelConfiguration;

namespace Nut.Data.Mapping
{
    public abstract class NutEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class
    {
        protected NutEntityTypeConfiguration()
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