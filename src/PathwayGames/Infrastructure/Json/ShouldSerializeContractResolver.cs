using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PathwayGames.Models;
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
            //Ignore UserGameSettings.Id
            if (typeof(UserGameSettings).IsSubclassOf(member.DeclaringType) && member.Name == nameof(UserGameSettings.Id))
            {
                property.Ignored = true;
            }
            // Ignore Point.IsEmpty
            if (typeof(Point).IsAssignableFrom(member.DeclaringType) && member.Name == nameof(Point.IsEmpty))
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}
