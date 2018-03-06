using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Multiblog.Core.Model.User;
using Multiblog.Core.Repositories.User;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using XUnit.Multiblog.Constant;
using XUnit.Multiblog.DataAttributes;
using XUnit.Multiblog.DataAttributes.Users;
using XUnit.Multiblog.Utils;

namespace XUnit.Multiblog.Repository
{
    public class UserShould : IClassFixture<UserTestCollection>
    {
        private readonly ITestOutputHelper _output;
        private readonly UserTestCollection _userTestCollection;

        private readonly ILogger<UserRepository> _fakeLogger = TestLogger.Create<UserRepository>();

        private readonly UserRepository _sut;

        public UserShould(ITestOutputHelper output, UserTestCollection userTestCollection)
        {
            _output = output;
            _userTestCollection = userTestCollection;

            _sut = new UserRepository(_fakeLogger, DBSetting.Value);
        }

        #region Registration
        [Theory]
        [TestUsers]
        [Trait("User", "Registration")]
        public async Task BeCreated(UserItem item)
        {
            string id = await _sut.CreateAsync(item);

            _output.WriteLine("Check if CreateAsync returns OjectId.");
            Assert.True(ObjectId.TryParse(id, out ObjectId temp), "CreateAsync did not return a valid ObjectId");
        }

        [Theory]
        [TestUserDoublet]
        [Trait("User", "Registration")]
        public async Task NotBeDoublet(UserItem item)
        {
            string id = await _sut.CreateAsync(item);

            _output.WriteLine("Check if CreateAsync returns OjectId.");
            Assert.True(ObjectId.TryParse(id, out ObjectId temp), "CreateAsync did not return a valid ObjectId");

            id = await _sut.CreateAsync(item);

            _output.WriteLine("Check if CreateAsync returns Null then doublet.");
            Assert.True(string.IsNullOrEmpty(id), "CreateAsync did not return null for doublet.");
        }

        #endregion Registration

        #region Favorite

        [Theory]
        [TestUserSetFavorit]
        [Trait("User", "Favorite")]
        public async Task BeAbleToSetFavrite(string email, string favorite)
        {
            string userId = await _sut.GetIdByEmailAsync(email);
            bool ret = await _sut.SetFavoriteAsync(userId, favorite);

            _output.WriteLine("Check if SetFavoriteAsync returns true.");
            Assert.True(ret, "SetFavoriteAsync did not return true");
        }

        [Theory]
        [TestUserFavoritWihtWrongUser]
        [Trait("User", "Favorite")]
        public async Task BeAbleToSetFavriteWihtWrongUser(string userId, string favorite)
        {
            var ex = await Assert.ThrowsAsync<FormatException>(() => _sut.SetFavoriteAsync(userId, favorite));

            Assert.Equal("UserId did not have excepted format.", ex.Message);
        }

        [Theory]
        [TestUserFavoritWihtWrongFavorite]
        [Trait("User", "Favorite")]
        public async Task BeAbleToSetFavriteWihtWrongFavorite(string userId, string favorite)
        {
            var ex = await Assert.ThrowsAsync<FormatException>(() => _sut.SetFavoriteAsync(userId, favorite));

            Assert.Equal("RefId did not have excepted format.", ex.Message);
        }

        [Theory]
        [TestUserIdThatDontExistAndFavorite]
        [Trait("User", "Favorite")]
        public async Task UserDontExistSetFavorite(string userId, string favorite)
        {
            bool ret = await _sut.RemoveFavoriteAsync(userId, favorite);

            _output.WriteLine("Check if SetFavoriteAsync returns false when user does not exist.");
            Assert.False(ret, "SetFavoriteAsync did not return false");
        }

        [Theory]
        [TestUserRemoveFavorit]
        [Trait("User", "Favorite")]
        public async Task BeAbleToRemoveFavrite(string email, string favorite)
        {
            string userId = await _sut.GetIdByEmailAsync(email);

            bool ret = await _sut.RemoveFavoriteAsync(userId, favorite);

            _output.WriteLine("Check if SetFavoriteAsync returns true.");
            Assert.True(ret, "SetFavoriteAsync did not return true");
        }

        [Theory]
        [TestUserFavoritWihtWrongUser]
        [Trait("User", "Favorite")]
        public async Task BeAbleToRemoveFavriteWihtWrongUser(string userId, string favorite)
        {
            var ex = await Assert.ThrowsAsync<FormatException>(() => _sut.RemoveFavoriteAsync(userId, favorite));

            Assert.Equal("UserId did not have excepted format.", ex.Message);
        }

        [Theory]
        [TestUserFavoritWihtWrongFavorite]
        [Trait("User", "Favorite")]
        public async Task BeAbleToRemoveFavriteWihtWrongFavorite(string userId, string favorite)
        {
            var ex = await Assert.ThrowsAsync<FormatException>(() => _sut.RemoveFavoriteAsync(userId, favorite));

            Assert.Equal("RefId did not have excepted format.", ex.Message);
        }

        [Theory]
        [TestUserIdThatDontExistAndFavorite]
        [Trait("User", "Favorite")]
        public async Task UserDontExistRemoveFavorite(string userId, string favorite)
        {
            bool ret = await _sut.RemoveFavoriteAsync(userId, favorite);

            _output.WriteLine("Check if SetFavoriteAsync returns false when user does not exist.");
            Assert.False(ret, "SetFavoriteAsync did not return false");
        }

        #endregion Favorite

        #region Get

        [Theory]
        [TestUserWhitdEmail]
        [Trait("User", "Get")]
        public async Task BeAbleToGetEmail(string userId, string email)
        {
            email = email.ToLower();

            string emailReturn = await _sut.GetEmailAsync(userId);

            _output.WriteLine($"Check if GetEmailAsync returns emailadress {email}.");
            Assert.True(email == emailReturn, "GetEmailAsync did not returns emailadress");
        }
        
        [Theory]
        [TestUserWrongIdFormat]
        [Trait("User", "Get")]
        public async Task ShouldNotGetEmailWithWrongUserId(string userId)
        {
            var ex = await Assert.ThrowsAsync<FormatException>(() => _sut.GetEmailAsync(userId));

            Assert.Equal("UserId did not have excepted format.", ex.Message);
        }

        [Theory]
        [TestUserWhitdEmail]
        [Trait("User", "Get")]
        public async Task BeAbleToGetIdFromEmail(string userId, string email)
        {
            email = email.ToLower();

            string id = await _sut.GetIdByEmailAsync(email);

            _output.WriteLine($"Check if GetIdByEmailAsync returns id {userId}.");
            Assert.True(id == userId, "GetIdByEmailAsync did not returns id");
        }

        [Theory]
        [TestUsersEmailDontExist]
        [Trait("User", "Get")]
        public async Task ShouldNotGetIdFromEmailWithWrongUserEmail(string email)
        {
            string id = await _sut.GetIdByEmailAsync(email);

            _output.WriteLine($"Check if GetIdByEmailAsync returns null.");
            Assert.True(id == null, "GetIdByEmailAsync did not return null.");
        }

        [Fact]
        [Trait("User", "Get")]
        public async Task ShouldNotHaveNullAsCheckEmail()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetIdByEmailAsync(null));

            Assert.Equal("Email cant be null or empty.", ex.ParamName);
        }

        [Theory]
        [TestUsersProfile]
        [Trait("User", "Get")]
        public async Task BeAbleToGetProfile(string userId, UserProfileItem profile)
        {
            UserProfileItem item = await _sut.GetProfileAsync(userId);

            _output.WriteLine($"Check if GetProfileAsync returns a profile.");
            Assert.True(item.Equals(profile), "GetProfileAsync did not returns rigth profile");
        }

        [Theory]
        [TestUserIdThatDontExist]
        [Trait("User", "Get")]
        public async Task ShouldNotGetProfile(string userId)
        {
            UserProfileItem item = await _sut.GetProfileAsync(userId);

            _output.WriteLine($"Check if GetIdByEmailAsync returns null.");
            Assert.True(item == null, "GetIdByEmailAsync did not return null.");
        }

        [Fact]
        [Trait("User", "Get")]
        public async Task ShouldNotHaveNullAsProfile()
        {
            var ex = await Assert.ThrowsAsync<FormatException>(() => _sut.GetProfileAsync(null));

            Assert.Equal("UserId did not have excepted format.", ex.Message);
        }

        #endregion Get

        #region Check

        [Theory]
        [TestUsersEmail]
        [Trait("User", "Check")]
        public async Task CheckIfEmailExist(string email)
        {
            bool exist = await _sut.CheckIfExistAsync(email);

            _output.WriteLine($"Check if CheckIfExistAsync returns true for emailadress {email}.");
            Assert.True(exist, "CheckIfExistAsync did not returns true");
        }

        [Theory]
        [TestUsersEmailDontExist]
        [Trait("User", "Check")]
        public async Task CheckIfEmailDontExist(string email)
        {
            bool exist = await _sut.CheckIfExistAsync(email);

            _output.WriteLine($"Check if CheckIfExistAsync returns false for emailadress {email}.");
            Assert.False(exist, "CheckIfExistAsync did not returns false");
        }

        [Fact]
        [Trait("User", "Check")]
        public async Task ShouldNotHaveNullAsEmail()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CheckIfExistAsync(null));

            Assert.Equal("Email cant be null or empty.", ex.ParamName);
        }

        [Theory]
        [TestUsersEmail]
        [Trait("User", "Check")]
        public async Task CheckIfEmailVerified(string email)
        {
            bool verify = await _sut.CheckIfEmailVerifyAsync(email);

            _output.WriteLine($"Check if CheckIfEmailVerifyAsync returns true for emailadress {email}.");
            Assert.True(verify, "CheckIfEmailVerifyAsync did not returns true");
        }

        [Theory]
        [TestUsersEmailDontExist]
        [Trait("User", "Check")]
        public async Task CheckIfEmailIsNotVerify(string email)
        {
            bool verify = await _sut.CheckIfEmailVerifyAsync(email);

            _output.WriteLine($"Check if CheckIfEmailVerifyAsync returns false for emailadress {email}.");
            Assert.False(verify, "CheckIfEmailVerifyAsync did not returns false");
        }

        [Fact]
        [Trait("User", "Check")]
        public async Task ShouldNotHaveNullAsEmailVerify()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CheckIfEmailVerifyAsync(null));

            Assert.Equal("Email cant be null or empty.", ex.ParamName);
        }

        #endregion Check

        #region Update

        [Theory]
        [TestUserProfileAndId]
        [Trait("User", "Update")]
        public async Task BeAbleToUpdateProfile(string userId, UserProfileItem profile)
        {
            bool ret = await _sut.UpdateProfileAsync(userId, profile);

            _output.WriteLine($"Check if UpdateProfileAsync returns true.");
            Assert.True(ret, "UpdateProfileAsync did not returns true");
        }

        [Fact]
        [Trait("User", "Update")]
        public async Task ShouldNotUpdateProfileWithWrongUserId()
        {
            var ex = await Assert.ThrowsAsync<FormatException>(() => _sut.UpdateProfileAsync(null, new UserProfileItem()));

            Assert.Equal("UserId did not have excepted format.", ex.Message);
        }

        #endregion Update

        #region Update

        [Theory]
        [TestUserIdToDelete]
        [Trait("User", "Delete")]
        public async Task BeAbleToBeDeleted(string userId)
        {
            bool ret = await _sut.DeleteAccountAsync(userId);

            _output.WriteLine($"Check if DeleteAccountAsync returns true.");
            Assert.True(ret, "DeleteAccountAsync did not returns true");
        }

        [Fact]
        [Trait("User", "Delete")]
        public async Task ShouldNotDeleteWithWrongUserId()
        {
            var ex = await Assert.ThrowsAsync<FormatException>(() => _sut.DeleteAccountAsync(null));

            Assert.Equal("UserId did not have excepted format.", ex.Message);
        }

        #endregion Update
    }
}
