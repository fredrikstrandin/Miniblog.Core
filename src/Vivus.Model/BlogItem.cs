using System;
using System.Collections.Generic;
using System.Text;

namespace Vivus.Model
{
    public class BlogItem
    {
        public string Id { get; set; }
        public string SubDomain { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PostsPerPage { get; set; }
    }
}
