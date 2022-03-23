using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity.FreeSql;

public static class IdentityFreeSqlExtensions
{
    public static IdentityBuilder AddFreeSqlStoresWithIntKey(
        this IdentityBuilder builder, IFreeSql freeSql)
    {
        return builder.AddFreeSqlStores<IdentityUser<int>, IdentityRole<int>, int, IdentityUserClaim<int>, IdentityUserRole<int>,
            IdentityUserLogin<int>, IdentityUserToken<int>, IdentityRoleClaim<int>>(freeSql, true);
    }

    public static IdentityBuilder AddFreeSqlStoresWithStringKey(
        this IdentityBuilder builder, IFreeSql freeSql)
    {
        return builder.AddFreeSqlStores<IdentityUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>,
            IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>(freeSql, false);
    }

    /// <summary>
    /// 使用FreeSql存储,方法自动配置实体
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="freeSql"></param>
    /// <param name="isIdentityKey"></param>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TUserClaim"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    /// <typeparam name="TUserLogin"></typeparam>
    /// <typeparam name="TUserToken"></typeparam>
    /// <typeparam name="TRoleClaim"></typeparam>
    /// <returns></returns>
    public static IdentityBuilder AddFreeSqlStores<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(
        this IdentityBuilder builder, IFreeSql freeSql, bool isIdentityKey)
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        freeSql.CodeFirst
            .Entity<TUser>(r =>
            {
                r.ToTable("IdentityUsers");
                if (isIdentityKey)
                    r.Property(f => f.Id).Help().IsIdentity(true);
                r.HasIndex(f => f.NormalizedUserName).IsUnique();
                r.HasIndex(f => f.NormalizedEmail);
            })
            .Entity<TRole>(r =>
            {
                r.ToTable("IdentityRoles");
                if (isIdentityKey)
                    r.Property(f => f.Id).Help().IsIdentity(true);
                r.HasIndex(f => f.NormalizedName).IsUnique();
            })
            .Entity<TUserClaim>(r =>
            {
                r.ToTable("IdentityUserClaims");
                if (isIdentityKey)
                    r.Property(f => f.Id).Help().IsIdentity(true);
                r.HasIndex(f => f.UserId);
            })
            .Entity<TUserRole>(r =>
            {
                r.ToTable("IdentityUserRoles");
                r.HasKey(f => new { f.UserId, f.RoleId });
                r.HasIndex(f => f.RoleId);
            })
            .Entity<TUserLogin>(r =>
            {
                r.ToTable("IdentityUserLogins");
                r.HasKey(f => new { f.LoginProvider, f.ProviderKey });
                r.HasIndex(f => f.UserId);
            })
            .Entity<TUserToken>(r =>
            {
                r.ToTable("IdentityUserTokens");
                r.HasKey(f => new { f.UserId, f.LoginProvider, f.Name });
            })
            .Entity<TRoleClaim>(r =>
            {
                r.ToTable("IdentityRoleClaims");
                if (isIdentityKey)
                    r.Property(f => f.Id).Help().IsIdentity(true);
                r.HasIndex(f => f.RoleId);
            })
            ;

        builder.Services
            .AddScoped<IUserStore<TUser>, UserStore<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>>();
        builder.Services
            .AddScoped<IRoleStore<TRole>, RoleStore<TRole, TKey, TUserRole, TRoleClaim>>();
        return builder;
    }

    /// <summary>
    /// 使用FreeSql存储,外部自行配置实体
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TUserClaim"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    /// <typeparam name="TUserLogin"></typeparam>
    /// <typeparam name="TUserToken"></typeparam>
    /// <typeparam name="TRoleClaim"></typeparam>
    /// <returns></returns>
    public static IdentityBuilder AddFreeSqlStores<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(
        this IdentityBuilder builder)
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        builder.Services
            .AddScoped<IUserStore<TUser>, UserStore<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>>();
        builder.Services
            .AddScoped<IRoleStore<TRole>, RoleStore<TRole, TKey, TUserRole, TRoleClaim>>();
        return builder;
    }
}