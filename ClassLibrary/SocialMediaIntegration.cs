using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class SocialMediaIntegration
    {
        private readonly IList<IProvider> _providers = new List<IProvider>();

        public async Task Post(Dictionary<string, string> providerParams, MessageOptions messageOptions)
        {
            _providers.Where(x => providerParams.ContainsKey(x.Name)).ToList()
                .ForEach(async x => { await x.Post(messageOptions, providerParams); });
        }

        public void AddProvider(IProvider provider)
        {
            _providers.Add(provider);
        }
    }
}
