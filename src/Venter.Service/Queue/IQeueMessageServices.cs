using System.Threading.Tasks;
using Multiblog.Model.User;

namespace Multiblog.Service.Queue
{
    public interface IQeueMessageServices
    {
        Task SendCreateUser(RegisterUserModel messageObject);
    }
}