using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Advert.Managers;
using Bazaar.MongoDbStorage.Profiles;
using Bazaar.MongoDbStorage.Users;
using Bazaar.Models.Account;
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
        private readonly IHostingEnvironment _hostEnv;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly SignInManager<MongoDbUser> _signInManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        
        public AccountController(
            ILoggerFactory loggerFactory, 
            IHostingEnvironment hostEnv, 
            UserManager<MongoDbUser> userManager, 
            SignInManager<MongoDbUser> signInManager,
            ProfilesManager<MongoDbProfile> profilesManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _hostEnv = hostEnv;
            _userManager = userManager;
            _signInManager = signInManager;
            _profilesManager = profilesManager;
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
                
                var profile = await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(model.Email, null);

                if (profile != null)
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
                    CreatedAt = DateTime.UtcNow,
                };
                
                myProfile.ImagePath = $"/images/profiles/{myProfile._id.ToString()}/photo/profile__photo.jpg";

                var photoDir = $"{_hostEnv.WebRootPath}/images/profiles/{myProfile._id.ToString()}/photo/";

                if (!Directory.Exists(photoDir))
                {
                    Directory.CreateDirectory(photoDir);
                }

                System.IO.File.Copy($"{_hostEnv.WebRootPath}/images/profile__photo.jpg", photoDir);
  
                await _profilesManager.CreateProfileAsync(myProfile);

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