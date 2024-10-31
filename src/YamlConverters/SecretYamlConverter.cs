using Duende.IdentityServer.Models;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenIdConnectServer.YamlConverters
{
    public class SecretYamlConverter : IYamlTypeConverter //<Claim>
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Secret);
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer rootSerializer)
        {
            throw new NotSupportedException();
        }

        public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            string s = parser.Consume<Scalar>().Value;
            return new Secret(s.Sha256());
        }
    }
}
