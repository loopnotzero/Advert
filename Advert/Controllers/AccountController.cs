using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Advert.Common.Posts;
using Advert.Managers;
using Advert.Models.Account;
using Advert.MongoDbStorage.Profiles;
using Advert.MongoDbStorage.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Advert.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly SignInManager<MongoDbUser> _signInManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        
        public AccountController(
            ILoggerFactory loggerFactory, 
            ILookupNormalizer keyNormalizer, 
            IConfiguration configuration, 
            UserManager<MongoDbUser> userManager, 
            SignInManager<MongoDbUser> signInManager,
            ProfilesManager<MongoDbProfile> profilesManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
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

                await _profilesManager.CreateProfileAsync(new MongoDbProfile
                {
                    Name = model.Name,
                    Email = model.Email,
                    CreatedAt = DateTime.UtcNow,
                });

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