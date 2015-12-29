using System;
using AutoMapper;
using Nut.Admin.Models.Users;
using Nut.Core.Domain.Users;

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


    }
}