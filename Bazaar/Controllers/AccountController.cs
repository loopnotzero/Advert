using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bazaar.MongoDbStorage.Profiles;
using Bazaar.MongoDbStorage.Users;
using Bazaar.Models.Account;
using Bazaar.Normalizers;
using Bazaar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bazaar.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly SignInManager<MongoDbUser> _signInManager;
        private readonly ProfilesService<MongoDbProfile> _profilesService;

        public AccountController(
            ILoggerFactory loggerFactory, 
            ILookupNormalizer keyNormalizer,
            IHostingEnvironment hostingEnvironment, 
            UserManager<MongoDbUser> userManager, 
            SignInManager<MongoDbUser> signInManager,
            ProfilesService<MongoDbProfile> profilesService)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _keyNormalizer = keyNormalizer;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _profilesService = profilesService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/Account/SignOut")]
        public async Task<IActionResult> SignOut([FromQuery(Name = "returnUrl")] string returnUrl = null)
        {
            try
            {
                await _signInManager.SignOutAsync();
     
                return Ok(new ErrorModel
                {
                    ReturnUrl = returnUrl
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    Errors = new List<IdentityError>
                    {
                        new IdentityError
                        {
                            Description = e.Message
                        }
                    },
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("/Account/LogInByEmail")]
        public async Task<IActionResult> LogInByEmail([FromBody] LogInModel model, [FromQuery(Name = "returnUrl")] string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ErrorModel
                    {
                        Errors = new List<IdentityError>
                        {
                            new IdentityError
                            {
                                Description = "Model is not valid"
                            }
                        },
                    });
                }
                
                var emailAddressAttr = new EmailAddressAttribute();

                if (!emailAddressAttr.IsValid(model.Email))
                {
                    return Ok(new ErrorModel
                    {
                        Errors = new List<IdentityError>
                        {
                            new IdentityError
                            {
                                Description = string.Format(emailAddressAttr.ErrorMessage, model.Email)
                            }
                        },
                    });
                }

                var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (signInResult.Succeeded)
                    return Ok(new ErrorModel
                    {
                        ReturnUrl = returnUrl
                    });
                
                if (signInResult.IsLockedOut)
                {
                    return Ok(new ErrorModel
                    {
                        Errors = new List<IdentityError>
                        {
                            new IdentityError
                            {
                                //todo: Error code
                                Description = "Is locked out"
                            }
                        }
                    });
                }

                if (signInResult.IsNotAllowed)
                {
                    return Ok(new ErrorModel
                    {
                        Errors = new List<IdentityError>
                        {
                            new IdentityError
                            {
                                //todo: Error code
                                Description = "Is not allowed"
                            }
                        }
                    });
                }

                if (signInResult.RequiresTwoFactor)
                {
                    return Ok(new ErrorModel
                    {
                        Errors = new List<IdentityError>
                        {
                            new IdentityError
                            {
                                //todo: Error code
                                Description = "Requires two factor"
                            }
                        }
                    });
                }

                return Ok(new ErrorModel
                {
                    Errors = new List<IdentityError>
                    {
                        new IdentityError
                        {
                            Description = "Wrong email or password. Try again or click Forgot password to reset it."
                        }
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    Errors = new List<IdentityError>
                    {
                        new IdentityError
                        {
                            Description = e.Message
                        }
                    },
                });
            }           
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("/Account/CreateNewAccount")]
        public async Task<IActionResult> CreateNewAccount([FromBody] SignUpModel model, [FromQuery(Name = "returnUrl")] string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ErrorModel
                    {
                        Errors = new List<IdentityError>
                        {
                            new IdentityError
                            {
                                Description = "Model is not valid"
                            }
                        },
                    });
                }
                
                var emailAddressAttr = new EmailAddressAttribute();

                if (!emailAddressAttr.IsValid(model.Email))
                {
                    return Ok(new ErrorModel
                    {
                        Errors = new List<IdentityError>
                        {
                            new IdentityError
                            {
                                Description = string.Format(emailAddressAttr.ErrorMessage, model.Email)
                            }
                        },
                    });
                }

                var byEmailAsync = await _userManager.FindByEmailAsync(model.Name);

                if (byEmailAsync != null)
                {
                    return Ok(new ErrorModel
                    {
                        Errors = new List<IdentityError>
                        {
                            new IdentityError
                            {
                                Description = "That email is taken. Try another."
                            }
                        },
                    });
                }  
                
                var user = new MongoDbUser
                {              
                    Email = model.Email,
                    UserName = model.Email,
                };

                var identityResult = await _userManager.CreateAsync(user, model.Password);

                if (!identityResult.Succeeded)
                    return Ok(new ErrorModel
                    {
                        Errors = identityResult.Errors.ToList(),
                    });
                
                await _signInManager.SignInAsync(user, false);

                var myProfile = new MongoDbProfile
                {
                    Name = model.Name,
                    Email = model.Email,
                    IdentityName = _keyNormalizer.NormalizeKey(model.Email),
                    NormalizedName = _keyNormalizer.NormalizeKey(model.Name),
                    CreatedAt = DateTime.UtcNow,
                };
                
                var photoDir = $"/profiles/{myProfile._id.ToString()}";
                
                myProfile.Photo = $"{photoDir}/profile__photo.jpg";

                var photoSystemDir = $"{_hostingEnvironment.WebRootPath}/profiles/{myProfile._id.ToString()}";

                if (!Directory.Exists(photoSystemDir))
                {
                    Directory.CreateDirectory(photoSystemDir);
                }
                
                System.IO.File.Copy($"{_hostingEnvironment.WebRootPath}/assets/profile__photo.jpg", photoSystemDir);
  
                await _profilesService.CreateProfileAsync(myProfile);

                return Ok(new ErrorModel
                {
                    ReturnUrl = returnUrl
                });

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    Errors = new List<IdentityError>
                    {
                        new IdentityError
                        {
                            Description = e.Message
                        }
                    },
                });
            }
        }
    }
}