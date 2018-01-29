using System.Threading.Tasks;
using Vivus.Model.User;

namespace Venter.Service.Queue
{
    public interface IQeueMessageServices
    {
        Task SendCreateUser(RegisterUserModel messageObject);
    }
}