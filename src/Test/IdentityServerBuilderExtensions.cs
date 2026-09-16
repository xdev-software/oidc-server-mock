using Duende.IdentityServer.Test;

namespace OpenIdConnectServerMock.Test;

// Based Duende.IdentityServer.Test.IdentityServerBuilderExtensions
public static class IdentityServerBuilderExtensions
{
    public static IIdentityServerBuilder AddTestUsersImproved(this IIdentityServerBuilder builder, List<TestUser> users)
    {
        // Modify service to use mutable TestUserStore
        var userStore = new MutableTestUserStore(users);
        builder.Services.AddSingleton(typeof(TestUserStore), userStore);
        builder.Services.AddSingleton(userStore);


        builder.AddProfileService<TestUserProfileService>();
        builder.AddResourceOwnerValidator<TestUserResourceOwnerPasswordValidator>();

        builder.AddBackchannelAuthenticationUserValidator<TestBackchannelLoginUserValidator>();

        return builder;
    }
}
