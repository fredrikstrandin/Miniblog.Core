using System.Threading.Tasks;
using Vivus.Model.User;

namespace Vivius.Repository.Qeue
{
    public interface IQeueMessageRepository
    {
        Task SendCreateUser(RegisterUserModel messageObject);
    }
}