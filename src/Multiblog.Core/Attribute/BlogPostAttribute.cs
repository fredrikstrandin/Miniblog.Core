//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Options;
//using Multiblog.Core.Model.Setting;
//using Multiblog.Core.Models;
//using Multiblog.Model;
//using Multiblog.Service.Interface;

//namespace Multiblog.Core.Attribute
//{
//    public class BlogPostAttribute : ActionFilterAttribute
//    {
//        private readonly IBlogPostService _blogPostService;
//        private readonly BlogSettings _blogSettings;

//        public BlogPostAttribute(IBlogPostService blogPostService,
//            IOptions<BlogSettings> blogSettings)
//        {
//            _blogPostService = blogPostService;
//            _blogSettings = blogSettings.Value;
//        }

//        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
//        {
//            if (actionExecutingContext.RouteData.Values["tenant"] is BlogItem)
//            {
//                var blogItem = actionExecutingContext.RouteData.Values["tenant"] as BlogItem;
//                string slug = actionExecutingContext.ActionArguments["slug"] as string;
//                Post post = _blogPostService.GetPostBySlug(blogItem.Id, slug).GetAwaiter().GetResult();

//                //actionExecutingContext.RouteData.Values["controller"] = "Gallery";
//                var controller = (Controller)actionExecutingContext.Controller;
//                actionExecutingContext.Result = controller.RedirectToAction(actionExecutingContext.RouteData.Values["action"] as string, "Gallery", actionExecutingContext.RouteData.Values);

//            }
            
//            base.OnActionExecuting(actionExecutingContext);
//        }

//        public override void OnActionExecuted(ActionExecutedContext context)
//        {
//            var controller = (Controller)context.Controller;
//            context.Result = controller.RedirectToAction(context.RouteData.Values["action"] as string, "blog", context.RouteData.Values);

//        }
//    }
//}
