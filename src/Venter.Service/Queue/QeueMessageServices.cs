using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miniblog.Core.Repository.Qeue;
using Vivus.Model.User;

namespace Venter.Service.Queue
{
    public class QeueMessageServices : IQeueMessageServices
    {
        private readonly IQeueMessageRepository _qeueMessageRepository;
        public QeueMessageServices(IQeueMessageRepository qeueMessageRepository)
        {
            _qeueMessageRepository = qeueMessageRepository;
        }

        public async Task SendCreateUser(RegisterUserModel messageObject)
        {
            await _qeueMessageRepository.SendCreateUser(messageObject);
        }
    }
}
