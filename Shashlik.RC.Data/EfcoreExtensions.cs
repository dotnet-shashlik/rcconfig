using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.RC.Data
{
    public static class EfcoreExtensions
    {

        /// <summary>
        /// 获取上下文所有的已注册的实体类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetAllEntityTypes(this DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Model.GetEntityTypes().Select(r => r.ClrType).ToList().AsReadOnly();
        }

        /// <summary>
        /// 判断给定的类型是否继承自<paramref name="genericType"/>泛型类型,
        /// <para>
        /// 例typeof(Child).IsChildTypeOfGenericType(typeof(IParent&lt;&gt;));
        /// </para>
        /// </summary>
        /// <param name="childType">子类型</param>
        /// <param name="genericType">泛型父级,例:typeof(IParent&lt;&gt;)</param>
        /// <returns></returns>
        public static bool IsChildTypeOfGenericType(this Type childType, Type genericType)
        {
            var interfaceTypes = childType.GetTypeInfo().ImplementedInterfaces;

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (childType.IsGenericType && childType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = childType.BaseType;
            if (baseType == null) return false;

            return IsChildTypeOfGenericType(baseType, genericType);
        }


        /// <summary>
        /// 是否为<paramref name="parentType"/>的子类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public static bool IsChildTypeOf(this Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否为<typeparamref name="T"/>的子类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsChildTypeOf<T>(this Type type)
        {
            return type.IsChildTypeOf(typeof(T));
        }


        /// <summary>
        /// 注册某个程序集中所有<typeparamref name="TEntityBase"/>的非抽象子类为实体
        /// </summary>
        /// <typeparam name="TEntityBase">实体基类</typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly">注册程序集</param>
        public static void RegisterEntitiesFromAssembly<TEntityBase>(this ModelBuilder modelBuilder, Assembly assembly, Action<EntityTypeBuilder, Type> registerAfter = null)
            where TEntityBase : class
        {
            modelBuilder.RegisterEntitiesFromAssembly(assembly, r => !r.IsAbstract && r.IsClass && r.IsChildTypeOf<TEntityBase>(), registerAfter);
        }

        /// <summary>
        /// 注册程序集中满足条件的所有类型
        /// </summary>
        /// <param name="modelBuilder">builder</param>
        /// <param name="assembly">程序集</param>
        /// <param name="entityTypePredicate">类型选择条件</param>
        public static void RegisterEntitiesFromAssembly(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            Func<Type, bool> entityTypePredicate,
            Action<EntityTypeBuilder, Type> registerAfter = null)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            //反射得到ModelBuilder的ApplyConfiguration<TEntity>(...)方法
            var applyConfigurationMethod = modelBuilder.GetType().GetMethods().First(r =>
            {
                var genericArguments = r.GetGenericArguments();
                return r.Name == "ApplyConfiguration" && genericArguments.Length == 1 && genericArguments[0].Name == "TEntity";
            });

            //所有fluent api配置类
            var configTypes = assembly
                               .DefinedTypes
                               .Where(t =>
                                 !t.IsAbstract && t.BaseType != null && t.IsClass
                                 && t.IsChildTypeOfGenericType(typeof(IEntityTypeConfiguration<>))).ToList();

            HashSet<Type> registedTypes = new HashSet<Type>();
            //存在fluent api配置的类,必须在Entity方法之前调用
            configTypes.ForEach(mappingType =>
            {
                var entityType = mappingType.GetTypeInfo().ImplementedInterfaces.First().GetGenericArguments().Single();

                //如果不满足条件的实体,不注册
                if (!entityTypePredicate(entityType))
                    return;

                var map = Activator.CreateInstance(mappingType);
                applyConfigurationMethod.MakeGenericMethod(entityType)
                     .Invoke(modelBuilder, new object[] { map });

                registedTypes.Add(entityType);
            });

            assembly
                .DefinedTypes
                //.Where(r => !registedTypes.Contains(r))
                .Where(entityTypePredicate)
                .ToList()
                .ForEach(r =>
                {
                    //直接调用Entity方法注册实体
                    var builder = modelBuilder.Entity(r);
                    registerAfter?.Invoke(builder, r);
                });
        }
    }
}
