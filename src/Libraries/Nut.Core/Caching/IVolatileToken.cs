
namespace Nut.Core.Caching {
    public interface IVolatileToken {
        bool IsCurrent { get; }
    }
}
