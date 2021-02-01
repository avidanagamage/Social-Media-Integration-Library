using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public interface IProvider
    {
        string Name { get; set; }

        Task Post(MessageOptions messageOptions, Dictionary<string, string> providerParams);
    }
}