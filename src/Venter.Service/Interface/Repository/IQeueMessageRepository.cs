using System.Threading.Tasks;
using Multiblog.Model.User;

namespace Multiblog.Core.Repository.Qeue
{
    public interface IQeueMessageRepository
    {
        Task SendCreateUser(RegisterUserModel messageObject);
    }
}