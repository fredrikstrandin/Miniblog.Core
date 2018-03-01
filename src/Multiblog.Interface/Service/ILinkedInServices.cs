using System.Threading.Tasks;
using Miniblog.Core.Model.User;

namespace Venter.Service.Interface
{
    public interface ILinkedInServices
    {
        Task<UserItem> UpdatePersonDataAsync(string accesstoken, UserItem user);
    }
}