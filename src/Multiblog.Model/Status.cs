namespace Multiblog.Core.Models
{
    public enum Status
    {
        Publish, //Viewable by everyone. (publish)
        Future, //Scheduled to be published in a future date. (future)
        Draft, //Incomplete post viewable by anyone with proper user role. (draft)
        Pending, //Awaiting a user with the publish_posts capability to publish. (pending)
        Private, //Viewable only to WordPress users at Administrator level. (private)
        Trash //Posts in the Trash are assigned the trash status. (trash)
    }
}