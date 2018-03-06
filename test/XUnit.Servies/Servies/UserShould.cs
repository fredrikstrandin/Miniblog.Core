using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using Multiblog.Core.Interface.Repositories;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Model.User;
using Multiblog.Core.Services.Mail;
using Multiblog.Model.Mail;
using Multiblog.Service.UserService;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using XUnit.Multiblog.DataAttributes;
using XUnit.Multiblog.Utils;

namespace XUnit.Multiblog.Servies
{
    public class UserShould : IClassFixture<MongoDbDatabaseSetting>
    {
        private readonly ITestOutputHelper _output;
        private readonly MongoDbDatabaseSetting _dbSettning;

        private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
        private readonly ILogger<UserService> _fakeLogger = TestLogger.Create<UserService>();
        private readonly Mock<IMailService> _mockMailService = new Mock<IMailService>();

        private readonly UserService _sut;

        public UserShould(ITestOutputHelper output,
            MongoDbDatabaseSetting dbSetting)
        {
            _output = output;
            _dbSettning = dbSetting;

            _sut = new UserService(_mockUserRepository.Object, _fakeLogger, _mockMailService.Object);

            _output.WriteLine("UserItem should include email.");
            _mockUserRepository.Setup(x => x.CreateAsync(It.Is<UserItem>(q => !string.IsNullOrEmpty(q.Email))))
                .ReturnsAsync(ObjectId.GenerateNewId().ToString());

            _output.WriteLine("SendVerifyMail should have a parameter of VerifyItem.");
            _mockMailService.Setup(x => x.SendVerifyMail(It.IsAny<VerifyItem>()))
                .ReturnsAsync(true);

        }

        [Theory]
        [TestUsers]
        [Trait("User", "Registration")]
        public async Task BeCreated(UserItem item)
        {
            _output.WriteLine("Run CreateAsync with email.");
            string id = await _sut.CreateAsync(item);

            _output.WriteLine("Check if CreateAsync returns OjectId.");
            Assert.True(ObjectId.TryParse(id, out ObjectId temp), "CreateAsync did not return a valid ObjectId");
        }

        //[Theory]
        //[TestUsers]
        //[Trait("User", "Registration")]
        //public async Task BeCreatedWithOutSendEmail(UserItem item)
        //{
        //    _output.WriteLine("Run CreateAsync with email.");
        //    string id = await _sut.CreateAsync(item, false);

        //    _output.WriteLine("Check if SendVerifyMail is never called.");
        //    _mockMailService.Verify(x => x.SendVerifyMail(It.IsAny<VerifyItem>()), Times.Never, "SendVerifyMail should not be called.");

        //    _output.WriteLine("Check if CreateAsync returns OjectId.");
        //    Assert.True(ObjectId.TryParse(id, out ObjectId temp), "CreateAsync did not return a valid ObjectId");
        //}

        [Fact]
        [Trait("User", "Registration")]
        public async Task IsNull()
        {
            _output.WriteLine("Start create user");

            await Assert.ThrowsAsync<ArgumentNullException>("User argument cant be null", () => _sut.CreateAsync(null));
        }
    }
}
