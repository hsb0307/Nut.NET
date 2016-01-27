using Nut.Core.Infrastructure;

namespace Nut.WebAPI.Infrastructure {
    public class AutoMapperStartupTask : IStartupTask {
        public void Execute() {
            
        }

        public int Order {
            get { return 1; }
        }
    }
}