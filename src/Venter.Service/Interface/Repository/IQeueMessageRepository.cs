using System.Threading.Tasks;
using Vivus.Model.User;

namespace Miniblog.Core.Repository.Qeue
{
    public interface IQeueMessageRepository
    {
        Task SendCreateUser(RegisterUserModel messageObject);
    }
}