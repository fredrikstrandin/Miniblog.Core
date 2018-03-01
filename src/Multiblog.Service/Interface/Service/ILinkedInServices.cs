using System.Threading.Tasks;
using Multiblog.Core.Model.User;

namespace Multiblog.Service.Interface
{
    public interface ILinkedInServices
    {
        Task<UserItem> UpdatePersonDataAsync(string accesstoken, UserItem user);
    }
}