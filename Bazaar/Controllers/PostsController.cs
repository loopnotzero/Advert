using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Profiles;
using Bazaar.Exceptions;
using Bazaar.Models.Post;
using Bazaar.Models.Profiles;
using Bazaar.Models.Settings;
using Bazaar.MongoDbStorage.Posts;
using Bazaar.MongoDbStorage.Profiles;
using Bazaar.MongoDbStorage.Users;
using Bazaar.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Bazaar.Controllers
{
    public class PostsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly PostsService<MongoDbPost> _postsService;
        private readonly ProfilesService<MongoDbProfile> _profilesService;
        private readonly PostsVotesService<MongoDbPostVote> _postsVotesService;
        private readonly PostCommentsService<MongoDbPostComment> _postCommentsService;
        private readonly PostsViewsCountService<MongoDbPostViewsCount> _postsViewsCountService;
        private readonly PostCommentsVotesService<MongoDbPostCommentVote> _postCommentsVotesService;
        private readonly PostsPhotosService<MongoDbPostPhoto> _postsPhotosService;

        public PostsController(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment, UserManager<MongoDbUser> userManager,
            ProfilesService<MongoDbProfile> profilesService, PostsService<MongoDbPost> postsService,
            PostsVotesService<MongoDbPostVote> postsVotesService,
            PostsPhotosService<MongoDbPostPhoto> postsPhotosService,
            PostCommentsService<MongoDbPostComment> postCommentsService,
            PostsViewsCountService<MongoDbPostViewsCount> postsViewsCountService,
            PostCommentsVotesService<MongoDbPostCommentVote> postCommentsVotesService)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;

            _postsService = postsService;
            _profilesService = profilesService;
            _postsVotesService = postsVotesService;
            _postsPhotosService = postsPhotosService;
            _postCommentsService = postCommentsService;
            _postsViewsCountService = postsViewsCountService;
            _postCommentsVotesService = postCommentsVotesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery(Name = "keyword")] string keyword = null)
        {
            try
            {
                List<MongoDbPost> posts;

                if (string.IsNullOrEmpty(keyword))
                {
                    posts = await _postsService.FindPostsAsync(0, _configuration.GetSection("BazaarOptions").GetValue<int>("PostsPerPage"));
                }
                else
                {
                    posts = await _postsService.FindPostsByKeywordAsync(0, _configuration.GetSection("BazaarOptions").GetValue<int>("PostsPerPage"), keyword);
                }

                IProfile profile = null;

                if (User.Identity.IsAuthenticated)
                {
                    profile = await _profilesService.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
                }

                var postsVotes = new List<IPostVote>();

                if (profile != null)
                {
                    postsVotes.AddRange(await _postsVotesService.FindPostsVotesAsync(profile._id));
                }

                var countryCodes = _configuration.GetSection("CountryCodes").Get<List<CountryCode>>();

                return View(new PostsAggregatorModel
                {
                    Posts = posts.Select(post => new PostModel
                    {
                        Hidden = post.Hidden,
                        IsOwner = User.Identity.IsAuthenticated && profile != null && post.ProfileId.Equals(profile._id),
                        IsVoted = postsVotes.Count > 0 && postsVotes.Any(x => x.PostId.Equals(post._id)),
                        PostId = post._id.ToString(),
                        Text = post.Text.Length > 1000 ? post.Text.Substring(0, 1000) + "..." : post.Text,
                        Title = post.Title,
                        Currency = post.Currency,
                        Location = post.Location,
                        CreatedAt = post.CreatedAt.Humanize(),
                        ViewsCount = ((double) post.ViewsCount).ToMetric(),
                        LikesCount = ((double) post.LikesCount).ToMetric(),
                        SharesCount = ((double) 0).ToMetric(),
                        CommentsCount = ((double) post.CommentsCount).ToMetric(),
                        ProfileId = post.ProfileId.ToString(),
                        ProfileName = post.ProfileName,
                        ProfileImagePath = post.ProfileImagePath,
                        Price = post.Price,
                        Tags = post.Tags,
                    }),

                    Profile = profile == null
                        ? null
                        : new ProfileModel
                        {
                            Owner = profile.NormalizedEmail.Equals(HttpContext.User.Identity.Name.ToUpper()),
                            Id = profile._id.ToString(),
                            Name = profile.Name,
                            Email = profile.Email,
                            Gender = profile.Gender.Humanize(),
                            Location = profile.Location,
                            Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                            CreatedAt = profile.CreatedAt.Humanize(),
                            ImagePath = profile.ImagePath,
                            CallingCode = profile.CallingCode,
                            PhoneNumber = profile.PhoneNumber,
                            CountryCodes = countryCodes.Select(x => new CountryCode
                            {
                                CountryName = x.CountryName,
                                CallingCode = x.CallingCode
                            })
                        },

                    PlacesApi = _configuration.GetSection("GoogleApiServices").GetValue<string>("PlacesApi"),
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/Posts")]
        public async Task<IActionResult> GetPostContent([FromQuery(Name = "postId")] string postId)
        {
            try
            {
                var post = await _postsService.FindPostByIdAsync(ObjectId.Parse(postId));

                var postsVotes = new List<IPostVote>();

                var postsCommentsVotes = new List<IPostCommentVote>();

                IProfile profile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    profile = await _profilesService.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                    await _postsViewsCountService.CreatePostViewsCountAsync(new MongoDbPostViewsCount
                    {
                        PostId = ObjectId.Parse(postId),
                        ProfileId = profile._id,
                        CreatedAt = DateTime.UtcNow
                    });

                    post.ViewsCount = await _postsViewsCountService.CountPostViewsCountAsync(post._id);

                    await _postsService.UpdatePostAsync(post);

                    postsVotes.AddRange(await _postsVotesService.FindPostsVotesAsync(profile._id));

                    postsCommentsVotes.AddRange(
                        await _postCommentsVotesService.FindPostsCommentsVotesAsync(profile._id));
                }

                var postComments =
                    await _postCommentsService.FindPostCommentsAsync(postId, 0, null, SortDefinition.Descending);

                var commentsReplies = new Dictionary<ObjectId, PostCommentModel>();

                foreach (var comments in postComments.OrderBy(x => x.ReplyTo).GroupBy(comment => comment.ReplyTo))
                {
                    if (comments.Key == ObjectId.Empty)
                    {
                        foreach (var comment in comments)
                        {
                            commentsReplies.Add(comment._id, new PostCommentModel
                            {
                                IsOwner = profile != null && comment.ProfileId.Equals(profile._id),
                                IsVoted = postsCommentsVotes.Count > 0 && postsCommentsVotes.Any(x =>
                                              x.CommentId.Equals(comment._id) && x.ProfileId.Equals(profile._id)),
                                PostId = comment.PostId.ToString(),
                                Text = comment.Text,
                                ReplyTo = comment.ReplyTo.ToString(),
                                CreatedAt = comment.CreatedAt.Humanize(),
                                CommentId = comment._id.ToString(),
                                VotesCount = ((double) comment.VotesCount).ToMetric(),
                                ProfileId = comment.ProfileId.ToString(),
                                ProfileName = comment.ProfileName,
                                ProfileImagePath = comment.ProfileImagePath
                            });
                        }
                    }
                    else
                    {
                        if (commentsReplies.TryGetValue(comments.Key, out var postComment))
                        {
                            if (postComment.Comments == null)
                            {
                                if (comments.Any())
                                {
                                    postComment.Comments = comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                    {
                                        return new PostCommentModel
                                        {
                                            IsOwner = profile != null && comment.ProfileId.Equals(profile._id),
                                            IsVoted = postsCommentsVotes.Count > 0 && postsCommentsVotes.Any(x =>
                                                          x.CommentId.Equals(comment._id) &&
                                                          x.ProfileId.Equals(profile._id)),
                                            PostId = comment.PostId.ToString(),
                                            Text = comment.Text,
                                            ReplyTo = comment.ReplyTo.ToString(),
                                            CreatedAt = comment.CreatedAt.Humanize(),
                                            CommentId = comment._id.ToString(),
                                            VotesCount = ((double) comment.VotesCount).ToMetric(),
                                            ProfileId = comment.ProfileId.ToString(),
                                            ProfileName = comment.ProfileName,
                                            ProfileImagePath = comment.ProfileImagePath
                                        };
                                    }).ToList();
                                }
                            }
                            else
                            {
                                postComment.Comments.AddRange(comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                {
                                    return new PostCommentModel
                                    {
                                        IsOwner = profile != null && comment.ProfileId.Equals(profile._id),
                                        IsVoted = postsCommentsVotes.Count > 0 && postsCommentsVotes.Any(x =>
                                                      x.CommentId.Equals(comment._id) &&
                                                      x.ProfileId.Equals(profile._id)),
                                        PostId = comment.PostId.ToString(),
                                        Text = comment.Text,
                                        ReplyTo = comment.ReplyTo.ToString(),
                                        CreatedAt = comment.CreatedAt.Humanize(),
                                        CommentId = comment._id.ToString(),
                                        VotesCount = ((double) comment.VotesCount).ToMetric(),
                                        ProfileId = comment.ProfileId.ToString(),
                                        ProfileName = comment.ProfileName,
                                        ProfileImagePath = comment.ProfileImagePath
                                    };
                                }));
                            }
                        }
                    }
                }

                var countryCodes = _configuration.GetSection("CountryCodes").Get<List<CountryCode>>();

                return View(new PostsAggregatorModel
                {
                    Posts = new List<PostModel>
                    {
                        new PostModel
                        {
                            Hidden = post.Hidden,
                            IsOwner = User.Identity.IsAuthenticated && profile != null &&
                                      post.ProfileId.Equals(profile._id),
                            IsVoted = postsVotes.Count > 0 && postsVotes.Any(x => x.PostId.Equals(post._id)),
                            PostId = post._id.ToString(),
                            Text = post.Text,
                            Title = post.Title,
                            Currency = post.Currency,
                            Location = post.Location,
                            CreatedAt = post.CreatedAt.Humanize(),
                            ViewsCount = ((double) post.ViewsCount).ToMetric(),
                            LikesCount = ((double) post.LikesCount).ToMetric(),
                            SharesCount = ((double) 0).ToMetric(),
                            CommentsCount = ((double) post.CommentsCount).ToMetric(),
                            ProfileId = post.ProfileId.ToString(),
                            ProfileName = post.ProfileName,
                            ProfileImagePath = post.ProfileImagePath,
                            Price = post.Price,
                            Tags = post.Tags,
                        }
                    },

                    Profile = profile == null
                        ? null
                        : new ProfileModel
                        {
                            Owner = profile.NormalizedEmail.Equals(HttpContext.User.Identity.Name.ToUpper()),
                            Id = profile._id.ToString(),
                            Name = profile.Name,
                            Email = profile.Email,
                            Gender = profile.Gender.Humanize(),
                            Location = profile.Location,
                            Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                            CreatedAt = profile.CreatedAt.Humanize(),
                            ImagePath = profile.ImagePath,
                            CallingCode = profile.CallingCode,
                            PhoneNumber = profile.PhoneNumber,
                            CountryCodes = countryCodes.Select(x => new CountryCode
                            {
                                CountryName = x.CountryName,
                                CallingCode = x.CallingCode
                            })
                        },

                    PlacesApi = _configuration.GetSection("GoogleApiServices").GetValue<string>("PlacesApi"),

                    PostComments = commentsReplies.Values.ToList(),
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Posts/CreatePostAsync")]
        public async Task<IActionResult> CreatePostAsync([FromBody] PostModel model)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }

                var profile = await _profilesService.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var post = new MongoDbPost
                {
                    Text = model.Text,
                    Title = model.Title,
                    Location = model.Location,
                    Currency = model.Currency,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath,
                    Price = model.Price,
                    ProfileId = profile._id,
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.Moderating,
                };

                await _postsService.CreatePostAsync(post);

                return Ok(new PostModel
                {
                    Hidden = post.Hidden,
                    IsOwner = post.ProfileId.Equals(profile._id),
                    PostId = post._id.ToString(),
                    Text = post.Text,
                    Title = post.Title,
                    Currency = post.Currency,
                    Location = post.Location,
                    CreatedAt = post.CreatedAt.Humanize(),
                    ViewsCount = ((double) post.ViewsCount).ToMetric(),
                    LikesCount = ((double) post.LikesCount).ToMetric(),
                    SharesCount = ((double) 0).ToMetric(),
                    CommentsCount = ((double) post.CommentsCount).ToMetric(),
                    ProfileId = post.ProfileId.ToString(),
                    ProfileName = post.ProfileName,
                    ProfileImagePath = post.ProfileImagePath,
                    Price = post.Price,
                    Tags = post.Tags,
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Posts/UpdatePostByIdAsync")]
        public async Task<IActionResult> UpdatePostByIdAsync([FromQuery(Name = "postId")] string postId,
            [FromBody] PostModel model)
        {
            try
            {
                var post = await _postsService.FindPostByIdAsync(ObjectId.Parse(postId));

                post.Hidden = model.Hidden;
                post.Text = model.Text;
                post.Title = model.Title;
                post.Location = model.Location;
                post.Currency = model.Currency;
                post.Price = model.Price;
                post.UpdatedAt = DateTime.UtcNow;

                var result = await _postsService.UpdatePostAsync(post);

                return Ok(new UpdateResultModel
                {
                    MatchedCount = result.MatchedCount,
                    ModifiedCount = result.ModifiedCount,
                    IsAcknowledged = result.IsAcknowledged,
                    IsModifiedCountAvailable = result.IsModifiedCountAvailable
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Posts/UpdatePostCommentAsync")]
        public async Task<IActionResult> UpdatePostCommentAsync([FromQuery(Name = "postId")] string postId,
            [FromQuery(Name = "commentId")] string commentId, [FromBody] PostCommentModel model)
        {
            try
            {
                var postComment = await _postCommentsService.FindPostComment(postId, ObjectId.Parse(commentId));

                postComment.Text = model.Text;

                var result = await _postCommentsService.ReplacePostCommentAsync(postId, postComment._id, postComment);

                return Ok(new ReplaceResultModel
                {
                    MatchedCount = result.MatchedCount,
                    ModifiedCount = result.ModifiedCount,
                    IsAcknowledged = result.IsAcknowledged,
                    IsModifiedCountAvailable = result.IsModifiedCountAvailable
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/Posts/GetPostByIdAsync")]
        public async Task<IActionResult> GetPostByIdAsync([FromQuery(Name = "postId")] string postId)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }

                var profile = await _profilesService.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var post = await _postsService.FindPostByIdAsync(ObjectId.Parse(postId));

                var postsVotes = await _postsVotesService.FindPostsVotesAsync(profile._id);

                return Ok(new PostModel
                {
                    Hidden = post.Hidden,
                    IsOwner = User.Identity.IsAuthenticated && post.ProfileId.Equals(profile._id),
                    IsVoted = postsVotes.Count > 0 && postsVotes.Any(x => x.PostId.Equals(post._id)),
                    PostId = post._id.ToString(),
                    Text = post.Text,
                    Title = post.Title,
                    Currency = post.Currency,
                    Location = post.Location,
                    CreatedAt = post.CreatedAt.Humanize(),
                    ViewsCount = ((double) post.ViewsCount).ToMetric(),
                    LikesCount = ((double) post.LikesCount).ToMetric(),
                    SharesCount = ((double) 0).ToMetric(),
                    CommentsCount = ((double) post.CommentsCount).ToMetric(),
                    ProfileId = post.ProfileId.ToString(),
                    ProfileName = post.ProfileName,
                    ProfileImagePath = post.ProfileImagePath,
                    Price = post.Price,
                    Tags = post.Tags,
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/Posts/GetPostCommentAsync")]
        public async Task<IActionResult> GetPostCommentAsync([FromQuery(Name = "postId")] string postId,
            [FromQuery(Name = "commentId")] string commentId)
        {
            try
            {
                var profile = await _profilesService.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var postComment = await _postCommentsService.FindPostComment(postId, ObjectId.Parse(commentId));

                var postsCommentsVotes = await _postCommentsVotesService.FindPostsCommentsVotesAsync(profile._id);

                return Ok(new PostCommentModel
                {
                    IsOwner = postComment.ProfileId.Equals(profile._id),
                    IsVoted = postsCommentsVotes.Any(x =>
                        x.CommentId.Equals(postComment._id) && x.ProfileId.Equals(profile._id)),
                    PostId = postComment.PostId.ToString(),
                    Text = postComment.Text,
                    ReplyTo = postComment.ReplyTo.ToString(),
                    CreatedAt = postComment.CreatedAt.Humanize(),
                    CommentId = postComment._id.ToString(),
                    VotesCount = ((double) postComment.VotesCount).ToMetric(),
                    ProfileId = postComment.ProfileId.ToString(),
                    ProfileName = postComment.ProfileName,
                    ProfileImagePath = postComment.ProfileImagePath,
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/Posts/DeletePostByIdAsync")]
        public async Task<IActionResult> DeletePostByIdAsync([FromQuery(Name = "postId")] string postId)
        {
            try
            {
                await _postsService.DeletePostByIdAsync(ObjectId.Parse(postId));
                return Ok(new
                {
                    returnUrl = Url.Action("GetPosts", "Posts")
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/Posts/DeletePostCommentAsync")]
        public async Task<IActionResult> DeletePostCommentAsync([FromQuery(Name = "postId")] string postId,
            [FromQuery(Name = "commentId")] string commentId)
        {
            try
            {
                var updateResult = await _postCommentsService.DeletePostCommentAsync(postId, ObjectId.Parse(commentId));

                return Ok(new UpdateResultModel
                {
                    MatchedCount = updateResult.MatchedCount,
                    ModifiedCount = updateResult.ModifiedCount,
                    IsAcknowledged = updateResult.IsAcknowledged,
                    IsModifiedCountAvailable = updateResult.IsModifiedCountAvailable
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/Posts/GetPostTagsByPostIdAsync")]
        public async Task<IActionResult> GetPostTagsByPostIdAsync([FromQuery(Name = "postId")] string postId)
        {
            try
            {
                var post = await _postsService.FindPostByIdAsync(ObjectId.Parse(postId));

                return Ok(new PostTagsModel
                {
                    Tags = post.Tags ?? new List<string>()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Posts/CreatePostTagsByPostIdAsync")]
        public async Task<IActionResult> CreatePostTagsByPostIdAsync([FromQuery(Name = "postId")] string postId,
            [FromBody] PostTagsModel model)
        {
            try
            {
                var post = await _postsService.FindPostByIdAsync(ObjectId.Parse(postId));

                post.Tags = new List<string>();

                post.Tags.AddRange(model.Tags);

                await _postsService.UpdatePostAsync(post);

                return Ok(new
                {
                    returnUrl = Url.Action("GetPostContent", "Posts", new {postId = post._id})
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Posts/CreatePostVoteAsync")]
        public async Task<IActionResult> CreatePostVoteAsync([FromQuery(Name = "postId")] string postId,
            [FromBody] PostVoteModel model)
        {
            try
            {
                var profile = await _profilesService.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var vote = await _postsVotesService.FindPostVoteAsync(ObjectId.Parse(postId), profile._id);

                if (vote == null)
                {
                    await _postsVotesService.CreatePostVoteAsync(new MongoDbPostVote
                    {
                        PostId = ObjectId.Parse(postId),
                        ProfileId = profile._id,
                        VoteType = model.VoteType,
                        CreatedAt = DateTime.UtcNow
                    });

                    var votesCount =
                        await _postsVotesService.CountPostVotesAsync(ObjectId.Parse(postId), VoteType.Like);

                    await _postsService.UpdatePostLikesCountByPostId(ObjectId.Parse(postId), votesCount);

                    return Ok(new PostVoteModel
                    {
                        VoteType = model.VoteType,
                        VotesCount = ((double) long.Parse(model.VotesCount) + 1).ToMetric()
                    });
                }
                else
                {
                    if (vote.VoteType == model.VoteType)
                    {
                        await _postsVotesService.DeletePostVoteByIdAsync(vote._id);
                    }

                    var votesCount =
                        await _postsVotesService.CountPostVotesAsync(ObjectId.Parse(postId), VoteType.Like);

                    await _postsService.UpdatePostLikesCountByPostId(ObjectId.Parse(postId), votesCount);

                    return Ok(new PostVoteModel
                    {
                        VoteType = model.VoteType,
                        VotesCount = ((double) long.Parse(model.VotesCount) - 1).ToMetric()
                    });
                }
            }
            catch (PostVoteException e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Posts/CreatePostCommentAsync")]
        public async Task<IActionResult> CreatePostCommentAsync([FromBody] PostCommentModel model)
        {
            try
            {
                var profile = await _profilesService.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var postId = ObjectId.Parse(model.PostId);

                var postComment = new MongoDbPostComment
                {
                    Text = model.Text,
                    ReplyTo = model.ReplyTo == null ? ObjectId.Empty : ObjectId.Parse(model.ReplyTo),
                    CreatedAt = DateTime.UtcNow,
                    PostId = postId,
                    ProfileId = profile._id,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath,
                    VotesCount = 0,
                };

                await _postCommentsService.CreatePostComment(model.PostId, postComment);

                var comment = await _postCommentsService.FindPostComment(model.PostId, postComment._id);

                var post = await _postsService.FindPostByIdAsync(comment.PostId);

                post.CommentsCount = await _postCommentsService.CountPostCommentsAsync(model.PostId);

                await _postsService.UpdatePostAsync(post);

                return Ok(new PostCommentModel
                {
                    Text = comment.Text,
                    ReplyTo = comment.ReplyTo == ObjectId.Empty ? null : comment.ReplyTo.ToString(),
                    CreatedAt = comment.CreatedAt.Humanize(),
                    CommentId = comment._id.ToString(),
                    PostId = model.PostId,
                    ProfileId = comment.ProfileId.ToString(),
                    ProfileName = comment.ProfileName,
                    ProfileImagePath = comment.ProfileImagePath,
                    VotesCount = ((double) comment.VotesCount).ToMetric()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Posts/CreatePostCommentVoteAsync")]
        public async Task<IActionResult> CreatePostCommentVoteAsync([FromQuery(Name = "postId")] string postId,
            [FromQuery(Name = "commentId")] string commentId, [FromBody] PostCommentVoteModel model)
        {
            try
            {
                var profile = await _profilesService.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var commentVote =
                    await _postCommentsVotesService.FindPostCommentVoteOrDefaultAsync(ObjectId.Parse(commentId),
                        profile._id, null);

                if (commentVote == null)
                {
                    var postCommentVote = new MongoDbPostCommentVote
                    {
                        PostId = ObjectId.Parse(postId),
                        VoteType = model.VoteType,
                        CommentId = ObjectId.Parse(commentId),
                        ProfileId = profile._id,
                        CreatedAt = DateTime.UtcNow,
                    };

                    await _postCommentsVotesService.CreatePostCommentVoteAsync(postCommentVote);

                    var votesCount =
                        await _postCommentsVotesService
                            .CountPostCommentVotesByCommentIdAsync(postCommentVote.CommentId);

                    var postComment = await _postCommentsService.FindPostComment(postId, postCommentVote.CommentId);

                    postComment.VotesCount = votesCount;

                    await _postCommentsService.ReplacePostCommentAsync(postId, postCommentVote.CommentId, postComment);

                    return Ok(new PostCommentVoteModel
                    {
                        VoteType = model.VoteType,
                        VotesCount = ((double) long.Parse(model.VotesCount) + 1).ToMetric()
                    });
                }
                else
                {
                    await _postCommentsVotesService.DeletePostCommentVoteByIdAsync(commentVote._id);

                    var votesCount =
                        await _postCommentsVotesService.CountPostCommentVotesByCommentIdAsync(commentVote.CommentId);

                    var postComment = await _postCommentsService.FindPostComment(postId, commentVote.CommentId);

                    postComment.VotesCount = votesCount;

                    await _postCommentsService.ReplacePostCommentAsync(postId, postComment._id, postComment);

                    return Ok(new PostCommentVoteModel
                    {
                        VoteType = model.VoteType,
                        VotesCount = ((double) long.Parse(model.VotesCount) - 1).ToMetric()
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/Posts/CountPostCommentsByPostIdAsync")]
        public async Task<IActionResult> CountPostCommentsByPostIdAsync([FromQuery(Name = "postId")] string postId)
        {
            try
            {
                var commentsCount = await _postCommentsService.CountPostCommentsAsync(postId);
                return Ok(new PostCommentsCountModel
                {
                    PostId = postId,
                    CommentsCount = ((double) commentsCount).ToMetric()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Posts/AddPostPhotosAsync")]
        public async Task<IActionResult> AddPostPhotosAsync([FromQuery(Name = "postId")] string postId, IFormFile file)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var photoDir = $"/posts/{postId}";

            if (!Directory.Exists($"{_hostingEnvironment.WebRootPath}/{photoDir}"))
            {
                Directory.CreateDirectory($"{_hostingEnvironment.WebRootPath}/{photoDir}");
            }

            var photoPath = $"{_hostingEnvironment.WebRootPath}/{photoDir}/{Guid.NewGuid():N}.{Path.GetExtension(file.FileName)}";
 
            using (var stream = new FileStream(photoPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var postPhoto = new MongoDbPostPhoto
            {
                PhotoPath = photoPath,
                IdentityName = HttpContext.User.Identity.Name,
                CreatedAt = DateTime.UtcNow,
            };
            
            await _postsPhotosService.CreatePostPhotosAsync(postPhoto);
            
            return Ok(new PostPhotoModel
            {
                PhotoId = postPhoto._id.ToString(),
                PhotoPath = photoPath,
            });
        }
    }
}