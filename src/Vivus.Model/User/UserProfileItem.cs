using System;
using System.Collections.Generic;
using System.Text;

namespace Multiblog.Core.Model.User
{
    public class UserProfileItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string City { get; set; }
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// This is moste for test reson
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == typeof(UserProfileItem))
            {
                UserProfileItem item = (UserProfileItem)obj;

                return FirstName == item.FirstName &&
                    LastName == item.LastName &&
                    DateOfBirth == item.DateOfBirth &&
                    City == item.City &&
                    ProfileImageUrl == item.ProfileImageUrl;
            }

            if (obj.GetType() == typeof(UserItem))
            {
                UserItem item = (UserItem)obj;

                return FirstName == item.FirstName &&
                    LastName == item.LastName &&
                    DateOfBirth == item.DateOfBirth &&
                    City == item.City &&
                    ProfileImageUrl == item.ProfileImageUrl;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -1112471877;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FirstName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LastName);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(DateOfBirth);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(City);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProfileImageUrl);
            return hashCode;
        }
    }
}
