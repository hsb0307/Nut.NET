using AutoMapper;
using Nop.Admin.Models.Plugins;
using Nut.Admin.Models.App;
using Nut.Admin.Models.Localization;
using Nut.Admin.Models.Logging;
using Nut.Admin.Models.Stores;
using Nut.Admin.Models.Users;
using Nut.Core.Domain.App;
using Nut.Core.Domain.Localization;
using Nut.Core.Domain.Logging;
using Nut.Core.Domain.Stores;
using Nut.Core.Domain.Users;
using Nut.Core.Infrastructure;
using Nut.Core.Plugins;

namespace Nut.Admin.Infrastructure {
    public class AutoMapperStartupTask : IStartupTask {

        public void Execute() {
            //customer roles
            Mapper.CreateMap<UserRole, UserRoleModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<UserRoleModel, UserRole>()
                .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore());

            //plugins
            Mapper.CreateMap<PluginDescriptor, PluginModel>()
                .ForMember(dest => dest.ConfigurationUrl, mo => mo.Ignore())
                .ForMember(dest => dest.CanChangeEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.IsEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.LogoUrl, mo => mo.Ignore())
                .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());

            //stores
            Mapper.CreateMap<Store, StoreModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<StoreModel, Store>();

            //language
            Mapper.CreateMap<Language, LanguageModel>()
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.FlagFileNames, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<LanguageModel, Language>()
                .ForMember(dest => dest.LocaleStringResources, mo => mo.Ignore());

            //ActivityLogType
            Mapper.CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(dest => dest.SystemKeyword, mo => mo.Ignore());
            Mapper.CreateMap<ActivityLogType, ActivityLogTypeModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(dest => dest.ActivityLogTypeName, mo => mo.MapFrom(src => src.ActivityLogType.Name))
                .ForMember(dest => dest.UserName, mo => mo.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());

            // Department
            Mapper.CreateMap<Department, DepartmentModel>()
                //.ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableDepartments, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.ParentName, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<DepartmentModel, Department>()
                .ForMember(dest => dest.Deleted, mo => mo.Ignore());

            // AppVersion
            Mapper.CreateMap<AppVersion, AppVersionModel>()
                //.ForMember(dest => dest., mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<AppVersionModel, AppVersion>();
            // .ForMember(dest => dest., mo => mo.Ignore());

        }

        public int Order {
            get { return 0; }
        }
    }
}