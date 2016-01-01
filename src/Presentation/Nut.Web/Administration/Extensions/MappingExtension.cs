using System;
using AutoMapper;
using Nut.Admin.Models.Users;
using Nut.Core.Domain.Users;
using Nut.Core.Plugins;
using Nop.Admin.Models.Plugins;
using Nut.Core.Domain.Stores;
using Nut.Admin.Models.Stores;
using Nut.Core.Domain.Localization;
using Nut.Admin.Models.Localization;
using Nut.Admin.Models.Logging;
using Nut.Core.Domain.Logging;

namespace Nut.Admin.Extensions {
    public static class MappingExtension {
        public static TDestination MapTo<TSource, TDestination>(this TSource source) {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) {
            return Mapper.Map(source, destination);
        }



        #region user roles
        //customer roles
        public static UserRoleModel ToModel(this UserRole entity) {
            return Mapper.Map<UserRole, UserRoleModel>(entity);
        }

        public static UserRole ToEntity(this UserRoleModel model) {
            return Mapper.Map<UserRoleModel, UserRole>(model);
        }

        public static UserRole ToEntity(this UserRoleModel model, UserRole destination) {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Plugins

        public static PluginModel ToModel(this PluginDescriptor entity) {
            return Mapper.Map<PluginDescriptor, PluginModel>(entity);
        }

        #endregion

        #region Stores

        public static StoreModel ToModel(this Store entity) {
            return Mapper.Map<Store, StoreModel>(entity);
        }

        public static Store ToEntity(this StoreModel model) {
            return Mapper.Map<StoreModel, Store>(model);
        }

        public static Store ToEntity(this StoreModel model, Store destination) {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language entity) {
            return Mapper.Map<Language, LanguageModel>(entity);
        }

        public static Language ToEntity(this LanguageModel model) {
            return Mapper.Map<LanguageModel, Language>(model);
        }

        public static Language ToEntity(this LanguageModel model, Language destination) {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region ActivityLog
        public static ActivityLogTypeModel ToModel(this ActivityLogType entity) {
            return entity.MapTo<ActivityLogType, ActivityLogTypeModel>();
        }

        public static ActivityLogModel ToModel(this ActivityLog entity) {
            return entity.MapTo<ActivityLog, ActivityLogModel>();
        }
        #endregion
    }
}