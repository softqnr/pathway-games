using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Xamarin.Forms;

namespace PathwayGames.Infrastructure.Json
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static ShouldSerializeContractResolver Instance { get; } = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (typeof(Point).IsAssignableFrom(member.DeclaringType) && member.Name == nameof(Point.IsEmpty))
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}
