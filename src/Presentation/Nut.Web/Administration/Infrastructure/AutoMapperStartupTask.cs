using AutoMapper;
using Nut.Admin.Models.Users;
using Nut.Core.Domain.Users;
using Nut.Core.Infrastructure;

namespace Nut.Admin.Infrastructure {
    public class AutoMapperStartupTask : IStartupTask {

        public void Execute() {
            //customer roles
            Mapper.CreateMap<UserRole, UserRoleModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<UserRoleModel, UserRole>()
                .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore());
        }

        public int Order {
            get { return 0; }
        }
    }
}