using System.Collections.Generic;

namespace PathwayGames.Infrastructure.Navigation
{
    public class NavigationParameters 
    {
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public NavigationParameters()
        {
        }

        public void Add(string key, object value)
        {
            _parameters.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _parameters.ContainsKey(key);
        }

        public object GetValue(string key)
        {
            if (_parameters.TryGetValue(key, out object param))
            {
                return param;
            }
            else
            {
                return null;
            }
        }
    }
}
