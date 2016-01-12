using System.Data.Entity;
using Nut.Core.Infrastructure;

namespace Nut.Plugin.APP.Version.Data {
    public class EfStartUpTask : IStartupTask {
        public void Execute() {
            Database.SetInitializer<APPVersionObjectContext>(null);
        }

        public int Order {
            get { return 0; }
        }
    }
}
