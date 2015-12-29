﻿using AutoMapper;
using Nop.Admin.Models.Plugins;
using Nut.Admin.Models.Stores;
using Nut.Admin.Models.Users;
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
        }

        public int Order {
            get { return 0; }
        }
    }
}