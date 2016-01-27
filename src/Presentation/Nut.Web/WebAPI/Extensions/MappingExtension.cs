using AutoMapper;

namespace Nut.WebAPI.Extensions {
    public static class MappingExtension {

        public static TDestination MapTo<TSource, TDestination>(this TSource source) {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) {
            return Mapper.Map(source, destination);
        }

    }
}