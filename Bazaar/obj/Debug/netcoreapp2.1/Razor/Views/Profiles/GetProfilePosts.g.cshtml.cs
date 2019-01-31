#pragma checksum "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ecbdc4679b95b2fc53402a9849a6f93afadd3288"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Profiles_GetProfilePosts), @"mvc.1.0.view", @"/Views/Profiles/GetProfilePosts.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Profiles/GetProfilePosts.cshtml", typeof(AspNetCore.Views_Profiles_GetProfilePosts))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
using Bazaar.Models.Post;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ecbdc4679b95b2fc53402a9849a6f93afadd3288", @"/Views/Profiles/GetProfilePosts.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c0909514ccbe15c9b46987fb6fc827edf50cf04a", @"/Views/_ViewImports.cshtml")]
    public class Views_Profiles_GetProfilePosts : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<PostsAggregatorViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(58, 1, true);
            WriteLiteral("\n");
            EndContext();
            BeginContext(60, 49, false);
#line 4 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
Write(await Html.PartialAsync("_Header", Model.Profile));

#line default
#line hidden
            EndContext();
            BeginContext(109, 50, true);
            WriteLiteral("\n\n<section class=\"content\">\n    <div class=\"row\">\n");
            EndContext();
#line 8 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
         if (Model.Profile == null)
        {

#line default
#line hidden
            BeginContext(205, 55, true);
            WriteLiteral("            <div class=\"col-md-3 p-1\">\n                ");
            EndContext();
            BeginContext(261, 62, false);
#line 11 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
           Write(await Html.PartialAsync("SignUpOrLoginPartial", Model.Profile));

#line default
#line hidden
            EndContext();
            BeginContext(323, 20, true);
            WriteLiteral("\n            </div>\n");
            EndContext();
#line 13 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
        }
        else
        {

#line default
#line hidden
            BeginContext(376, 105, true);
            WriteLiteral("            <div class=\"col-md-3 p-1\">\n                <div class=\"card team__item\">\n                    ");
            EndContext();
            BeginContext(482, 56, false);
#line 18 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
               Write(await Html.PartialAsync("ProfilePartial", Model.Profile));

#line default
#line hidden
            EndContext();
            BeginContext(538, 82, true);
            WriteLiteral("\n                </div>\n            </div>\n            <div class=\"col-md-6 p-1\">\n");
            EndContext();
            BeginContext(639, 197, true);
            WriteLiteral("                    <div class=\"card\">\n                        <ul class=\"nav nav-tabs\">\n                            <li class=\"nav-item\">\n                                <a class=\"nav-link active\"");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 836, "\"", 869, 3);
            WriteAttributeValue("", 843, "/", 843, 1, true);
#line 26 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
WriteAttributeValue("", 844, Model.Profile.Name, 844, 19, false);

#line default
#line hidden
            WriteAttributeValue("", 863, "/Posts", 863, 6, true);
            EndWriteAttribute();
            BeginContext(870, 45, true);
            WriteLiteral(">Posts</a>\n                            </li>\n");
            EndContext();
#line 28 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
                             if (Model.Profile != null && Model.Profile.Owner)
                            {

#line default
#line hidden
            BeginContext(1024, 109, true);
            WriteLiteral("                                <li class=\"nav-item\">\n                                    <a class=\"nav-link\"");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 1133, "\"", 1167, 3);
            WriteAttributeValue("", 1140, "/", 1140, 1, true);
#line 31 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
WriteAttributeValue("", 1141, Model.Profile.Name, 1141, 19, false);

#line default
#line hidden
            WriteAttributeValue("", 1160, "/Hidden", 1160, 7, true);
            EndWriteAttribute();
            BeginContext(1168, 159, true);
            WriteLiteral(">Hidden</a>\n                                </li>\n                                <li class=\"nav-item\">\n                                    <a class=\"nav-link\"");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 1327, "\"", 1364, 3);
            WriteAttributeValue("", 1334, "/", 1334, 1, true);
#line 34 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
WriteAttributeValue("", 1335, Model.Profile.Name, 1335, 19, false);

#line default
#line hidden
            WriteAttributeValue("", 1354, "/Favorites", 1354, 10, true);
            EndWriteAttribute();
            BeginContext(1365, 53, true);
            WriteLiteral(">Favorites</a>\n                                </li>\n");
            EndContext();
#line 36 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
                            }

#line default
#line hidden
            BeginContext(1448, 54, true);
            WriteLiteral("                        </ul>\n                        ");
            EndContext();
            BeginContext(1503, 52, false);
#line 38 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
                   Write(await Html.PartialAsync("PostsPartial", Model.Posts));

#line default
#line hidden
            EndContext();
            BeginContext(1555, 28, true);
            WriteLiteral("\n                    </div>\n");
            EndContext();
            BeginContext(1601, 77, true);
            WriteLiteral("            </div>\n            <div class=\"col-md-3 p-1\">\n            </div>\n");
            EndContext();
#line 44 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
        }

#line default
#line hidden
            BeginContext(1688, 15, true);
            WriteLiteral("    </div>\n    ");
            EndContext();
            BeginContext(1704, 39, false);
#line 46 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
Write(await Html.PartialAsync("AddPostModal"));

#line default
#line hidden
            EndContext();
            BeginContext(1743, 5, true);
            WriteLiteral("\n    ");
            EndContext();
            BeginContext(1749, 40, false);
#line 47 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
Write(await Html.PartialAsync("EditPostModal"));

#line default
#line hidden
            EndContext();
            BeginContext(1789, 5, true);
            WriteLiteral("\n    ");
            EndContext();
            BeginContext(1795, 42, false);
#line 48 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
Write(await Html.PartialAsync("DeletePostModal"));

#line default
#line hidden
            EndContext();
            BeginContext(1837, 1, true);
            WriteLiteral("\n");
            EndContext();
#line 49 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
     if (Model.Profile != null)
    {
        

#line default
#line hidden
            BeginContext(1885, 58, false);
#line 51 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
   Write(await Html.PartialAsync("EditProfileModal", Model.Profile));

#line default
#line hidden
            EndContext();
#line 51 "/home/briankernighan/IdeaProjects/Advert/Bazaar/Views/Profiles/GetProfilePosts.cshtml"
                                                                   
    }

#line default
#line hidden
            BeginContext(1950, 10, true);
            WriteLiteral("</section>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<PostsAggregatorViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
