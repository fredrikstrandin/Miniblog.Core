using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Venter.Utilities.Converter;
using Miniblog.Core.Model.User;
using Miniblog.Core.Model.Utils;

namespace Miniblog.Core.Repository.Model
{
    [BsonIgnoreExtraElements]
    internal class UserEntity : BaseEntity
    {
        [BsonIgnoreIfDefault]
        public string FirstName { get; set; }
        [BsonIgnoreIfDefault]
        public string LastName { get; set; }

        [BsonIgnore]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        private string _email { get; set; }
        public string Email { get { return _email; } set { _email = value?.ToLower().Trim() ?? null; } }
        [BsonIgnoreIfDefault]
        public bool EmailVerified { get; set; }

        [BsonIgnoreIfDefault]
        public string City { get; set; }

        [BsonIgnoreIfDefault]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> GPSPosition { get; set; }

        private DateTime _dateOfBirth { get; set; }
        [BsonIgnoreIfDefault]
        public DateTime DateOfBirth { get { return _dateOfBirth; } set { _dateOfBirth = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, DateTimeKind.Utc); } }
        [BsonIgnoreIfDefault]
        public string ProfileImageUrl { get; set; }
        [BsonIgnoreIfDefault]
        public int PreferredLanguage { get; set; }

        [BsonIgnoreIfNull]
        public List<ObjectId> Favorite { get; set; }

        /// <summary>
        /// Collection over External providers
        /// </summary>
        [BsonIgnoreIfNull]
        public List<ProviderSubEntity> Providers { get; set; }
        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        [BsonIgnoreIfNull]
        public Gender? Gender { get; set; }

        [BsonIgnoreIfDefault]
        public string Headline { get; set; }
        [BsonIgnoreIfDefault]
        public string Summary { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string NickName { get; set; }
        public string About { get; set; }
        public List<ObjectId> BlogOwnerList { get; set; }

        public static implicit operator UserEntity(UserItem item)
        {
            if (item == null)
                return null;

            var user = new UserEntity();

            if (ObjectId.TryParse(item.Id, out ObjectId id))
            {
                user.Id = id;
            }

            user.FirstName = item.FirstName;
            user.LastName = item.LastName;
            user.Email = item.Email;
            user.ProfileImageUrl = item.ProfileImageUrl;
            user.Favorite = item.Favorite.Select(x => ObjectId.Parse(x)).ToList();
            user.EmailVerified = item.EmailVerified;
            user.DateOfBirth = item.DateOfBirth;

            if (item.GPSPosition != null)
            {
                user.GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(item.GPSPosition.Longitude, item.GPSPosition.Latitude));
            }

            if (item.Providers != null && item.Providers.Count > 0)
            {
                item.Providers.ForEach(x => user.Providers.Add(x));
            }

            user.Summary = item.Summary;
            user.Headline = item.Headline;

            return user;
        }

        public static implicit operator UserItem(UserEntity item)
        {
            if (item == null)
                return null;

            var user = new UserItem();

            if (item.Id != null)
            {
                user.Id = item.Id.ToString();
            }

            user.FirstName = item.FirstName;
            user.LastName = item.LastName;
            user.Email = item.Email;
            user.ProfileImageUrl = item.ProfileImageUrl;
            user.Favorite = item.Favorite.Select(x => x.ToString()).ToList();
            user.EmailVerified = item.EmailVerified;
            user.DateOfBirth = item.DateOfBirth;
            user.Gender = item.Gender;
            user.GPSPosition = new GPSPosition() { Latitude = item.GPSPosition.Coordinates.Latitude, Longitude = item.GPSPosition.Coordinates.Latitude };
            item.Providers.ForEach(x => user.Providers.Add(x));
            user.Summary = item.Summary;
            user.Headline = item.Headline;

            return user;
        }
    }
}
