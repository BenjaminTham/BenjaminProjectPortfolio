using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Constants;
using Ecommerce.Domain.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Stripe;
using Castle.Core.Smtp;
using System.Security.Policy;
using System.Web;

namespace Ecommerce.Application.Services;

public class AccountService : IAccountService
{
    private readonly IApplicationUserManager _userManager;
    private readonly IApplicationSignInManager _signInManager;
    private readonly IApplicationRoleManager _roleManager;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    private readonly IDataContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TokenService1 _tokenService;


    public AccountService(IApplicationUserManager userManager,
                            IApplicationSignInManager signInManager,
                            IApplicationRoleManager roleManager,
                            IMapper mapper,
                            IKeyAccessor keyAccessor, IDataContext db, ICurrentUser currentUser, IHttpContextAccessor httpContextAccessor, TokenService1 tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
        _db = db;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;

    }



    public async Task<Response<UserIdentityDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var user = _mapper.Map<ApplicationUser>(registerUserDto);
        var rs = await _userManager.RegisterUserAsync(user);
        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(new UserIdentityDto { Id = user.Id }, rs.ToString())
            : Response<UserIdentityDto>.Fail(rs.ToString());
    }


    public async Task<Response<UserIdentityDto>> SignInAsync(LoginUserDto loginUserDto)
    {
        var user = await _userManager.GetUserByNameAsync(loginUserDto.UserName);
        if (user == null) return Response<UserIdentityDto>.Fail(new UserIdentityDto { RequiresTwoFactor = false }, "Username does not exists");
        if (user.IsActive == false)
        {
            //return Response<UserIdentityDto>.Fail("Sorry you can't signin.");
            return Response<UserIdentityDto>.Fail(new UserIdentityDto { RequiresTwoFactor = false }, "Sorry you can't signin");
        }

        var conAdv = _keyAccessor?["AdvancedConfiguration"] != null ? JsonSerializer.Deserialize<AdvancedConfigurationDto>(_keyAccessor["AdvancedConfiguration"]) : new AdvancedConfigurationDto();
        var conSec = _keyAccessor?["SecurityConfiguration"] != null ? JsonSerializer.Deserialize<SecurityConfigurationDto>(_keyAccessor["SecurityConfiguration"]) : new SecurityConfigurationDto();
        var isTwoFactorEnabled = conAdv?.EnableTwoFactorAuthentication;
        var userRoles = _userManager.GetRolesAsync(user);
        var isSuperAdmin = userRoles.Result.Contains(DefaultApplicationRoles.SuperAdmin.ToString());

        if (isTwoFactorEnabled == false || isSuperAdmin == true)
        {
            user.TwoFactorEnabled = false;
        }

        bool isEmailConfirmed = false;
        if (conAdv.EnableEmailConfirmation)
        {
            isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!isEmailConfirmed) return Response<UserIdentityDto>.Fail(new UserIdentityDto { Id = user.Id, IsEmailConfirmed = false }, "Email are not Confirmed!");
        }

        var rs = await _signInManager.PasswordSignInAsync(user, loginUserDto.Password, loginUserDto.RememberMe, conSec.IsUserLockoutEnabled);

        if (rs.Succeeded)
        {
            var profilePic = await _db.Galleries.Where(c => c.Id == user.ProfilePicture).FirstOrDefaultAsync();

            var fullName = new Claim("FullName", user.FullName.ToString());
            if (profilePic?.Name != null)
            {
                await _userManager.AddClaimAsync(user, new Claim("ProfilePic", profilePic.Name.ToString()));
            }
            await _userManager.AddClaimAsync(user, fullName);
            return Response<UserIdentityDto>.Success(new UserIdentityDto { Id = user.Id }, rs.ToString());
        }
        return Response<UserIdentityDto>.Fail(new UserIdentityDto { Id = user.Id, RequiresTwoFactor = rs.Data.RequiresTwoFactor, IsLockedOut = rs.Data.IsLockedOut, IsEmailConfirmed = isEmailConfirmed }, rs.ToString());
    }

    public async Task<Response<UserIdentityDto>> TwoFactorSignInAsync(TwoStepDto twoStepDto)
    {
        var conSec = _keyAccessor?["SecurityConfiguration"] != null ? JsonSerializer.Deserialize<SecurityConfigurationDto>(_keyAccessor["SecurityConfiguration"]) : new SecurityConfigurationDto();
        var user = await _userManager.GetUserByNameAsync(twoStepDto.UserName);
        var rs = await _signInManager.TwoFactorSignInAsync(user, "Email", twoStepDto.TwoFactorCode, twoStepDto.RememberMe, conSec.IsUserLockoutEnabled);
        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(rs.ToString())
            : Response<UserIdentityDto>.Fail(rs.ToString());
    }

    public async Task<Response<UserIdentityDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null) return Response<UserIdentityDto>.Fail(new UserIdentityDto { RequiresTwoFactor = false }, "User does not exists");

        var rs = await _signInManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(rs.ToString())
            : Response<UserIdentityDto>.Fail(rs.ToString());
    }
    public async Task<Response<UserIdentityDto>> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Response<UserIdentityDto>.Fail(new UserIdentityDto { RequiresTwoFactor = false }, "User does not exists");
        var rs = await _userManager.ConfirmEmailAsync(user, token);
        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(rs.ToString())
            : Response<UserIdentityDto>.Fail(rs.ToString());
    }


    public async Task SignOutAsync()
    {
        // Remove all claims from the user
        var user = await _userManager.GetUserByIdAsync(_currentUser.UserId);
        var claims = await _userManager.GetClaimsAsync(user);
        await _userManager.RemoveClaimsAsync(user, claims.ToList());

        await _signInManager.SignOutAsync();
    }


    /*
   public async Task<Response<UserLoginDto>> SignInAsync(LoginUserDto loginUserDto)
   {
       // Retrieve user by normalized username
       var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == Normalize(loginUserDto.UserName));
       if (user == null)
       {
           return Response<UserLoginDto>.Fail("Invalid Login Attempt"); //prevent account enumeration attack
       }

       // Verify password using the stored salt
       var passwordHasher = new PasswordHasher();
       if (!passwordHasher.VerifyPassword(loginUserDto.UserName, loginUserDto.Password, user.PasswordHash, user.SecurityStamp))
       {
           return Response<UserLoginDto>.Fail("Invalid Login Attempt");
       }

       // Check if email is confirmed
       if (!user.EmailConfirmed)
       {
           //send confirmation email again
           await ResendConfirmationEmail(user);
           return Response<UserLoginDto>.Fail("Email are Not Confirmed!");
       }

       // Check if user is locked out
       if (!user.LockoutEnabled)
       {
           return Response<UserLoginDto>.Fail("Account locked");
       }

       // Create authentication ticket with user claims
       var claims = new List<Claim>
   {
       new Claim(ClaimTypes.Name, user.UserName),
       new Claim("FullName", $"{user.FirstName} {user.LastName}"),
       new Claim(ClaimTypes.Email, user.Email),
   };

       var claimsIdentity = new ClaimsIdentity(claims, "CustomScheme");
       var authProperties = new AuthenticationProperties
       {
           IsPersistent = loginUserDto.RememberMe,
           ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15) // Session timeout
       };

       var token = _tokenService.GenerateToken(user.Email);

       // Set authentication cookie
       _httpContextAccessor.HttpContext.Response.Cookies.Append("db7RzeAKUBHT7oCltNkFZLLbp51Fg9EfB7hdlhKioZ8",
           token, new CookieOptions
       {
           HttpOnly = true,
           Secure = true,
           SameSite = SameSiteMode.Strict,
           Expires = authProperties.ExpiresUtc
       });

       return Response<UserLoginDto>.Success(new UserLoginDto { Id = user.Id }, "Login successful");
   }

   private async Task<bool> ResendConfirmationEmail(Ecommerce.Domain.Entities.User user)
   {
       try
       {
           var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
           var callbackUrl = Url.Action("ConfirmEmail", "UserAccount", new { userId = user.Id, code = HttpUtility.UrlEncode(code) }, protocol: HttpContext.Request.Scheme);

           await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by clicking <a href='{callbackUrl}'>here</a>.");
           return true;
       }
       catch (Exception ex)
       {
           // Log the error (not shown)
           return false;
       }
   }
   private string Normalize(string value)
   {
       return value.ToUpperInvariant();
   }

   public class PasswordHasher
   {
       private const int SaltSize = 16; // 128 bit
       private const int KeySize = 32; // 256 bit
       private const int Iterations = 100000; // Number of iterations


       // Method to hash the password using PBKDF2
       public string HashPassword(string password, out string salt)
       {
           // Generate a cryptographically secure salt
           var saltBytes = new byte[SaltSize];
           RandomNumberGenerator.Fill(saltBytes);
           salt = Convert.ToBase64String(saltBytes);

           // Use PBKDF2 to hash the password with the salt
           var hash = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA512);
           var hashBytes = hash.GetBytes(KeySize);

           // Return the hashed password as a Base64 string
           return Convert.ToBase64String(hashBytes);
       }


       // Method to verify the password
       public bool VerifyPassword(string username, string password, string storedHash, string salt)
       {
           // Convert the stored salt back to byte array
           var saltBytes = Convert.FromBase64String(salt);

           // Use PBKDF2 to hash the provided password with the stored salt
           var hash = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA512);
           var hashBytes = hash.GetBytes(KeySize);

           // Compare the newly hashed password with the stored hash
           return Convert.ToBase64String(hashBytes) == storedHash;
       }

   }*/
}
