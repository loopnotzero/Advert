﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Advert.Common.Posts;
using Advert.Common.Metrics;
using Advert.Exceptions;
using Advert.Managers;
using Advert.Models.Post;
using Advert.Models.Profiles;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Profiles;
using Advert.MongoDbStorage.Users;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Advert.Controllers
{
    public class PostsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        
        private readonly PostsManager<MongoDbPost> _postsManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly PostsVotesManager<MongoDbPostVote> _postsVotesManager;
        private readonly PostCommentsManager<MongoDbPostComment> _postCommentsManager;
        private readonly PostsViewsCountManager<MongoDbPostViewsCount> _postsViewsCountManager;
        private readonly PostCommentsVotesManager<MongoDbPostCommentVote> _postCommentsVotesManager;
        
        private const string NoProfileImage = "/images/no-image.png";

        public PostsController(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            UserManager<MongoDbUser> userManager,
            ProfilesManager<MongoDbProfile> profilesManager,
            PostsManager<MongoDbPost> postsManager,
            PostsVotesManager<MongoDbPostVote> postsVotesManager,
            PostCommentsManager<MongoDbPostComment> postCommentsManager,
            PostsViewsCountManager<MongoDbPostViewsCount> postsViewsCountManager,
            PostCommentsVotesManager<MongoDbPostCommentVote> postCommentsVotesManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;         

            _postsManager = postsManager;
            _profilesManager = profilesManager;
            _postsVotesManager = postsVotesManager;
            _postCommentsManager = postCommentsManager;
            _postsViewsCountManager = postsViewsCountManager;
            _postCommentsVotesManager = postCommentsVotesManager;
        }

        [HttpGet]
//        [Authorize]
        public async Task<IActionResult> GetPosts([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "keyword")] string keyword = null)
        {
            try
            {
                var postsPerPage = _configuration.GetSection("AdvertOptions").GetValue<int>("PostsPerPage");

                var offset = (page - 1) * postsPerPage;

                List<MongoDbPost> posts;

                if (string.IsNullOrEmpty(keyword))
                {
                    posts = await _postsManager.FindPostsAsync(offset, postsPerPage);
                }
                else
                {
                    posts = await _postsManager.FindPostsWhichContainsKeywordAsync(offset, postsPerPage, keyword);
                }

                var lastPage = (long) Math.Ceiling((double) await _postsManager.EstimatedPostsCountAsync() / postsPerPage);

                var maxPages = _configuration.GetSection("AdvertOptions").GetValue<int>("MaxPages");

                var middlePosition = (long) Math.Ceiling((double) maxPages / 2);

                var beginPage = page - middlePosition;

                if (beginPage <= 0)
                {
                    beginPage = 1;
                }

                var endPage = page + middlePosition - 1;

                if (endPage > lastPage)
                {
                    endPage = lastPage;
                }
                else
                {
                    if (endPage < maxPages)
                    {
                        endPage = lastPage < maxPages ? lastPage : maxPages;
                    }
                }

                MongoDbProfile profile = null;

                if (User.Identity.IsAuthenticated)
                {
                    profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
                }
                
                List<MongoDbPostVote> postsVotes = null;

                if (User.Identity.IsAuthenticated)
                {
                    postsVotes = await _postsVotesManager.FindPostsVotesAsync(profile._id);
                }

                return View(new PostsAggregatorViewModel
                    {
                        BeginPage = beginPage,
                        EndPage = endPage,
                        CurrentPage = page,
                        LastPage = lastPage,

                        Profile = profile == null ? null : new ProfileModel
                        {
                            Id = profile._id.ToString(),
                            Name = profile.Name,
                            ImagePath = profile.ImagePath ?? NoProfileImage,
                        },

                        Posts = posts.Select(post => new PostViewModel
                        {
                            PostId = post._id.ToString(),
                            ProfileId = post.ProfileId.ToString(),
                            ProfileName = post.ProfileName,
                            ProfileImagePath = post.ProfileImagePath ?? NoProfileImage,
                            Text = post.Text.Length > 1000 ? post.Text.Substring(0, 1000) + "..." : post.Text,
                            Title = post.Title,
                            Price = post.Price,
                            Currency = post.Currency,
                            Location = post.Location,
                            Tags = post.Tags,
                            LikesCount = ((double) post.LikesCount).ToMetric(),
                            SharesCount = ((double) 0).ToMetric(),
                            ViewsCount = ((double) post.ViewsCount).ToMetric(),
                            CommentsCount = ((double) post.CommentsCount).ToMetric(),
                            CreatedAt = post.CreatedAt.Humanize(),
                            IsPostVoted = postsVotes != null && postsVotes.Count > 0 && postsVotes.Any(x => x.PostId.Equals(post._id)),
                            IsTopicOwner = profile != null && post.ProfileId.Equals(profile._id)
                        }),

                        PlacesApi = _configuration.GetSection("GoogleApiServices").GetValue<string>("PlacesApi"),

                        RecommendedPosts = posts
                            .OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount))
                            .Select(post => new RecommendedPostViewModel
                            {
                                PostId = post._id.ToString(),
                                ProfileName = post.ProfileName,
                                ProfileImagePath = post.ProfileImagePath ?? NoProfileImage,
                                Title = post.Title,
                                CreatedAt = post.CreatedAt.Humanize(),
                            }).ToList()
                    });  
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
//        [Authorize]
        [Route("/Posts")]
        public async Task<IActionResult> GetPostContent([FromQuery(Name = "postId")] string postId, [FromQuery(Name = "page")] int page = 1)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var postViewsCount = new MongoDbPostViewsCount
                {
                    PostId = ObjectId.Parse(postId),
                    ProfileId = profile._id,
                    CreatedAt = DateTime.UtcNow
                };
                
                await _postsViewsCountManager.CreatePostViewsCountAsync(postViewsCount);
               
                var post = await _postsManager.FindPostByIdAsync(postViewsCount.PostId);
                
                post.ViewsCount = await _postsViewsCountManager.CountPostViewsCountAsync(postViewsCount.PostId);
                
                await _postsManager.UpdatePostAsync(post);
       
                var postComments = await _postCommentsManager.FindPostCommentsAsync(postId, 0, null, SortDefinition.Descending);
   
                var commentsReplies = new Dictionary<ObjectId, PostCommentViewModel>();

                var postsCommentsVotes = await _postCommentsVotesManager.FindPostsCommentsVotesAsync(profile._id);

                foreach (var comments in postComments.OrderBy(x => x.ReplyTo).GroupBy(comment => comment.ReplyTo))
                {
                    if (comments.Key == ObjectId.Empty)
                    {
                        foreach (var comment in comments)
                        {
                            commentsReplies.Add(comment._id, new PostCommentViewModel
                            {
                                IsCommentOwner = comment.ProfileId.Equals(profile._id),
                                IsCommentVoted = postsCommentsVotes.Any(x => x.CommentId.Equals(comment._id) && x.ProfileId.Equals(profile._id)),
                                PostId = comment.PostId.ToString(),
                                Text = comment.Text,
                                ReplyTo = comment.ReplyTo.ToString(),
                                CreatedAt = comment.CreatedAt.Humanize(),
                                CommentId = comment._id.ToString(),
                                VotesCount = ((double) comment.VotesCount).ToMetric(),
                                ProfileId = comment.ProfileId.ToString(),
                                ProfileName = comment.ProfileName,
                                ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,
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
                                        return new PostCommentViewModel
                                        {
                                            IsCommentOwner = comment.ProfileId.Equals(profile._id),
                                            IsCommentVoted = postsCommentsVotes.Any(x => x.CommentId.Equals(comment._id) && x.ProfileId.Equals(profile._id)),
                                            PostId = comment.PostId.ToString(),
                                            Text = comment.Text,
                                            ReplyTo = comment.ReplyTo.ToString(),
                                            CreatedAt = comment.CreatedAt.Humanize(),
                                            CommentId = comment._id.ToString(),
                                            VotesCount = ((double) comment.VotesCount).ToMetric(),
                                            ProfileId = comment.ProfileId.ToString(),
                                            ProfileName = comment.ProfileName,
                                            ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,
                                        };       
                                    }).ToList();
                                }
                            }
                            else
                            {
                                postComment.Comments.AddRange(comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                {                             
                                    return new PostCommentViewModel
                                    {
                                        IsCommentOwner = comment.ProfileId.Equals(profile._id),
                                        IsCommentVoted = postsCommentsVotes.Any(x => x.CommentId.Equals(comment._id) && x.ProfileId.Equals(profile._id)),
                                        PostId = comment.PostId.ToString(),
                                        Text = comment.Text,
                                        ReplyTo = comment.ReplyTo.ToString(),
                                        CreatedAt = comment.CreatedAt.Humanize(),
                                        CommentId = comment._id.ToString(),
                                        VotesCount = ((double) comment.VotesCount).ToMetric(),
                                        ProfileId = comment.ProfileId.ToString(),
                                        ProfileName = comment.ProfileName,
                                        ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,
                                    };
                                }));
                            }
                        }
                    }
                }

                var posts = await _postsManager.FindPostsAsync(_configuration.GetSection("AdvertOptions").GetValue<int>("PostsPerPage"));

                var orderedPosts = posts.OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount));

                var postsVotes = await _postsVotesManager.FindPostsVotesAsync(profile._id);

                return View(new PostsAggregatorViewModel
                {
                    Profile = new ProfileModel
                    {
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        ImagePath = profile.ImagePath ?? NoProfileImage,
                    },                
                    
                    Posts = new List<PostViewModel>
                    {
                        new PostViewModel
                        {
                            PostId = post._id.ToString(),
                            ProfileId = post.ProfileId.ToString(),
                            ProfileName = post.ProfileName,
                            ProfileImagePath = post.ProfileImagePath ?? NoProfileImage,
                            Text = post.Text,
                            Title = post.Title,
                            Price = post.Price,
                            Currency = post.Currency,
                            Location = post.Location,
                            Tags = post.Tags,
                            LikesCount = ((double) post.LikesCount).ToMetric(),
                            SharesCount = ((double)0).ToMetric(),
                            ViewsCount = ((double) post.ViewsCount).ToMetric(),
                            CommentsCount = ((double) post.CommentsCount).ToMetric(),
                            CreatedAt = post.CreatedAt.Humanize(),
                            IsPostVoted = postsVotes.Any(x => x.PostId.Equals(post._id) && x.ProfileId.Equals(profile._id)),
                            IsTopicOwner = post.ProfileId.Equals(profile._id)
                        }
                    }, 
                    
                    PlacesApi = _configuration.GetSection("GoogleApiServices").GetValue<string>("PlacesApi"),
                    
                    RecommendedPosts = orderedPosts.Select(recommendedPost => new RecommendedPostViewModel
                    {
                        PostId = recommendedPost._id.ToString(),
                        ProfileName = recommendedPost.ProfileName,
                        ProfileImagePath = recommendedPost.ProfileImagePath ?? NoProfileImage,
                        Title = recommendedPost.Title,
                        CreatedAt = recommendedPost.CreatedAt.Humanize(),                      
                    }).ToList(),  
                    
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
        public async Task<IActionResult> CreatePostAsync([FromBody] PostViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var post = new MongoDbPost
                {
                    Text = viewModel.Text,
                    Title = viewModel.Title,
                    Price = viewModel.Price,
                    Location = viewModel.Location,
                    Currency = viewModel.Currency,
                    ReleaseType = ReleaseType.Moderating,
                    ProfileId = profile._id,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath,
                    CreatedAt = DateTime.UtcNow,
                };

                await _postsManager.CreatePostAsync(post);

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
        [Route("/Posts/UpdatePostByIdAsync")]
        public async Task<IActionResult> UpdatePostByIdAsync([FromQuery(Name = "postId")] string postId, [FromBody] PostViewModel viewModel)
        {
            try
            {
                var post = await _postsManager.FindPostByIdAsync(ObjectId.Parse(postId));

                post.Text = viewModel.Text;
                post.Title = viewModel.Title;
                post.Price = viewModel.Price;
                post.Location = viewModel.Location;
                post.Currency = viewModel.Currency;
                post.ChangedAt = DateTime.UtcNow;

                var result = await _postsManager.UpdatePostAsync(post);

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
        public async Task<IActionResult> UpdatePostCommentAsync([FromQuery(Name = "postId")] string postId, [FromQuery(Name = "commentId")] string commentId, [FromBody] PostCommentViewModel viewModel)
        {
            try
            {
                var postComment = await _postCommentsManager.FindPostComment(postId, ObjectId.Parse(commentId));

                postComment.Text = viewModel.Text;
     
                var result = await _postCommentsManager.ReplacePostCommentAsync(postId, postComment._id, postComment);
               
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
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var post = await _postsManager.FindPostByIdAsync(ObjectId.Parse(postId));
  
                return Ok(new PostViewModel
                {
                    PostId = post._id.ToString(),
                    ProfileName = post.ProfileName,
                    ProfileImagePath = post.ProfileImagePath ?? NoProfileImage,
                    Text = post.Text,
                    Title = post.Title,
                    Price = post.Price,
                    Currency = post.Currency,
                    Location = post.Location,
                    Tags = post.Tags,
                    LikesCount = ((double) post.LikesCount).ToMetric(),
                    SharesCount = ((double) 0).ToMetric(),
                    ViewsCount = ((double) post.ViewsCount).ToMetric(),
                    CommentsCount = ((double) post.CommentsCount).ToMetric(),
                    CreatedAt = post.CreatedAt.Humanize(),
                    IsTopicOwner = post.ProfileId.Equals(profile._id)
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
        public async Task<IActionResult> GetPostCommentAsync([FromQuery(Name = "postId")] string postId, [FromQuery(Name = "commentId")] string commentId)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var postComment = await _postCommentsManager.FindPostComment(postId, ObjectId.Parse(commentId));
  
                var postsCommentsVotes = await _postCommentsVotesManager.FindPostsCommentsVotesAsync(profile._id);

                return Ok(new PostCommentViewModel
                {
                    IsCommentOwner = postComment.ProfileId.Equals(profile._id),
                    IsCommentVoted = postsCommentsVotes.Any(x => x.CommentId.Equals(postComment._id) && x.ProfileId.Equals(profile._id)),
                    PostId = postComment.PostId.ToString(),
                    Text = postComment.Text,
                    ReplyTo = postComment.ReplyTo.ToString(),
                    CreatedAt = postComment.CreatedAt.Humanize(),
                    CommentId = postComment._id.ToString(),
                    VotesCount = ((double) postComment.VotesCount).ToMetric(),
                    ProfileId = postComment.ProfileId.ToString(),
                    ProfileName = postComment.ProfileName,
                    ProfileImagePath = postComment.ProfileImagePath ?? NoProfileImage,
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
                await _postsManager.DeletePostByIdAsync(ObjectId.Parse(postId));
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
        public async Task<IActionResult> DeletePostCommentAsync([FromQuery(Name = "postId")] string postId, [FromQuery(Name = "commentId")] string commentId)
        {
            try
            {
                var updateResult = await _postCommentsManager.DeletePostCommentAsync(postId, ObjectId.Parse(commentId));

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
                var post = await _postsManager.FindPostByIdAsync(ObjectId.Parse(postId));
                
                return Ok(new PostTagsViewModel
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
        public async Task<IActionResult> CreatePostTagsByPostIdAsync([FromQuery(Name = "postId")] string postId, [FromBody] PostTagsViewModel viewModel)
        {
            try
            {
                var post = await _postsManager.FindPostByIdAsync(ObjectId.Parse(postId));

                post.Tags = new List<string>();
                
                post.Tags.AddRange(viewModel.Tags);
                
                await _postsManager.UpdatePostAsync(post);                           
                
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
        public async Task<IActionResult> CreatePostVoteAsync([FromQuery(Name = "postId")] string postId, [FromBody] PostVoteViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
                  
                var vote = await _postsVotesManager.FindPostVoteAsync(ObjectId.Parse(postId), profile._id);

                if (vote == null)
                {
                    await _postsVotesManager.CreatePostVoteAsync(new MongoDbPostVote
                    {
                        PostId = ObjectId.Parse(postId),
                        ProfileId = profile._id,
                        VoteType = viewModel.VoteType,
                        CreatedAt = DateTime.UtcNow
                    });

                    var votesCount = await _postsVotesManager.CountPostVotesAsync(ObjectId.Parse(postId), VoteType.Like);

                    await _postsManager.UpdatePostLikesCountByPostId(ObjectId.Parse(postId), votesCount);

                    return Ok(new PostVoteViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) long.Parse(viewModel.VotesCount) + 1).ToMetric()
                    });
                }
                else
                {
                    if (vote.VoteType == viewModel.VoteType)
                    {
                        await _postsVotesManager.DeletePostVoteByIdAsync(vote._id);
                    }

                    var votesCount = await _postsVotesManager.CountPostVotesAsync(ObjectId.Parse(postId), VoteType.Like);

                    await _postsManager.UpdatePostLikesCountByPostId(ObjectId.Parse(postId), votesCount);
         
                    return Ok(new PostVoteViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double)  long.Parse(viewModel.VotesCount) - 1).ToMetric()
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
        public async Task<IActionResult> CreatePostCommentAsync([FromBody] PostCommentViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
                
                var postId = ObjectId.Parse(viewModel.PostId);

                var postComment = new MongoDbPostComment
                {
                    Text = viewModel.Text,
                    ReplyTo = viewModel.ReplyTo == null ? ObjectId.Empty : ObjectId.Parse(viewModel.ReplyTo),
                    CreatedAt = DateTime.UtcNow,
                    PostId = postId,
                    ProfileId = profile._id,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath ?? NoProfileImage,
                    VotesCount = 0,
                };
              
                await _postCommentsManager.CreatePostComment(viewModel.PostId, postComment);
                
                var comment = await _postCommentsManager.FindPostComment(viewModel.PostId, postComment._id);
                
                var post = await _postsManager.FindPostByIdAsync(comment.PostId);
                
                post.CommentsCount = await _postCommentsManager.CountPostCommentsAsync(viewModel.PostId);
                
                await _postsManager.UpdatePostAsync(post);

                return Ok(new PostCommentViewModel
                {
                    Text = comment.Text,
                    ReplyTo = comment.ReplyTo == ObjectId.Empty ? null : comment.ReplyTo.ToString(),
                    CreatedAt = comment.CreatedAt.Humanize(),
                    CommentId = comment._id.ToString(),
                    PostId = viewModel.PostId,
                    ProfileId = comment.ProfileId.ToString(),
                    ProfileName = comment.ProfileName,
                    ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,     
                    VotesCount = ((double)comment.VotesCount).ToMetric()
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
        public async Task<IActionResult> CreatePostCommentVoteAsync([FromQuery(Name = "postId")] string postId, [FromQuery(Name = "commentId")] string commentId, [FromBody] PostCommentVoteViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var commentVote = await _postCommentsVotesManager.FindPostCommentVoteOrDefaultAsync(ObjectId.Parse(commentId), profile._id, null);

                if (commentVote == null)
                {
                    var postCommentVote = new MongoDbPostCommentVote
                    {
                        PostId = ObjectId.Parse(postId),
                        VoteType = viewModel.VoteType,
                        CommentId = ObjectId.Parse(commentId),
                        ProfileId = profile._id,
                        CreatedAt = DateTime.UtcNow,
                    };
                        
                    await _postCommentsVotesManager.CreatePostCommentVoteAsync(postCommentVote);
                    
                    var votesCount = await _postCommentsVotesManager.CountPostCommentVotesByCommentIdAsync(postCommentVote.CommentId);
                   
                    var postComment = await _postCommentsManager.FindPostComment(postId, postCommentVote.CommentId);
                    
                    postComment.VotesCount = votesCount;
                    
                    await _postCommentsManager.ReplacePostCommentAsync(postId, postCommentVote.CommentId, postComment);
                    
                    return Ok(new PostCommentVoteViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) long.Parse(viewModel.VotesCount) + 1).ToMetric()
                    });
                }
                else
                {
                    await _postCommentsVotesManager.DeletePostCommentVoteByIdAsync(commentVote._id);

                    var votesCount = await _postCommentsVotesManager.CountPostCommentVotesByCommentIdAsync(commentVote.CommentId);
                   
                    var postComment = await _postCommentsManager.FindPostComment(postId, commentVote.CommentId);
                    
                    postComment.VotesCount = votesCount;
                    
                    await _postCommentsManager.ReplacePostCommentAsync(postId, postComment._id, postComment);
                    
                    return Ok(new PostCommentVoteViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) long.Parse(viewModel.VotesCount) - 1).ToMetric()
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
                var commentsCount = await _postCommentsManager.CountPostCommentsAsync(postId);
                return Ok(new PostCommentsCountModel
                {
                    PostId = postId,
                    CommentsCount = ((double)commentsCount).ToMetric()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
        
        [HttpPost("AddImage")]
        [Route("/Posts/AddPostPhotos")]
        public async Task<IActionResult> AddPostPhotos(string returnUrl, IFormFile file)
        {
            ViewData["ReturnUrl"] = returnUrl;
                 
            if (file.Length <= 0)
            {
                //todo: return View with ModelErrors
                return BadRequest();
            }
            
            var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

            var profilePhotosDir = $"{_hostingEnvironment.WebRootPath}/images/profiles/{profile._id.ToString()}/photos";

            if (!Directory.Exists(profilePhotosDir))
            {
                Directory.CreateDirectory(profilePhotosDir);
            }
            
            var profilePhotosDirFullPath = Path.Combine(profilePhotosDir, file.FileName);

            using (var stream = new FileStream(profilePhotosDirFullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //todo: Create profile post photo
            
//            await _profilesImagesManager.CreateProfileImageAsync(new MongoDbProfileImage
//            {
//                ProfileId = profile.Id,
//                ImagePath = $"/images/profiles/{profile.Id}/{file.FileName}",
//                CreatedAt = DateTime.UtcNow
//            });
//
            return Ok();
        }
    }
}