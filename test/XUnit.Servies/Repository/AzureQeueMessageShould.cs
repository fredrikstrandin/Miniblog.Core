using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Multiblog.Core.Repository.Qeue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using XUnit.Multiblog.Utils;

namespace XUnit.Multiblog.Repository
{
    public class AzureQeueMessageShould
    {
        private readonly ITestOutputHelper _output;

        private readonly ILogger<AzureQeueMessageShould> _fakeLogger = TestLogger.Create<AzureQeueMessageShould>();

        private readonly AzureQeueMessageRepository _sut;

        public AzureQeueMessageShould(ITestOutputHelper output)
        {
            _output = output;

            //_sut = new AzureQeueMessageRepository();
        }
        
        [Fact]
        [Trait("Queue", "Send")]
        public async Task BeSend()
        {
            //await _sut.SendCreateUser();            
        }

    }
}
