using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Multiblog.Core.Repository.Qeue;
using Multiblog.Model.User;

namespace Multiblog.Service.Queue
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
