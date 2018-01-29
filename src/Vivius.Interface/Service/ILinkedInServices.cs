using System.Threading.Tasks;
using Vivius.Model.User;

namespace Venter.Service.Interface
{
    public interface ILinkedInServices
    {
        Task<UserItem> UpdatePersonDataAsync(string accesstoken, UserItem user);
    }
}