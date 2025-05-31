using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;
using Stripe;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
/*

namespace Ecommerce.Application.Handlers.CustomerAccount.Commands
{
    public class OriginalRegisterCustomerCommand : IRequest<Response<UserDto1>>
    {
        public CustomerRegisterDto1 CustomerRegister { get; set; }
    }

    public class OriginalRegisterCustomerCommandHandler : IRequestHandler<OriginalRegisterCustomerCommand, Response<UserDto1>>
    {
        private readonly IDataContext _db;
        private readonly IMapper _mapper;
        private readonly IKeyAccessor _keyAccessor;
        private readonly ICurrentUser _currentUser;
        private readonly EmailService _emailService;
        private readonly TokenService1 _tokenService;

        public OriginalRegisterCustomerCommandHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor, ICurrentUser currentUser, EmailService emailService, TokenService1 tokenService)
        {
            _db = db;
            _mapper = mapper;
            _keyAccessor = keyAccessor;
            _currentUser = currentUser;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task<Response<UserDto1>> Handle(OriginalRegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            var timeNow = DateTime.UtcNow;
            await using var ts = _db.BeginTransaction();

            try
            {
                // Hash the password
                var passwordHasher = new PasswordHasher();
                string salt;

                //return salt as OUT parameter to store in security stamp field
                var passwordHash = passwordHasher.HashPassword(request.CustomerRegister.Password, out salt);

                var user = new User
                {
                    UserName = request.CustomerRegister.UserName,
                    NormalizedUserName = Normalize(request.CustomerRegister.UserName),
                    Email = request.CustomerRegister.Email,
                    NormalizedEmail = Normalize(request.CustomerRegister.Email),
                    PasswordHash = passwordHash,
                    SecurityStamp = salt, //save salt in securitystamp field
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    FirstName = request.CustomerRegister.FirstName,
                    LastName = request.CustomerRegister.LastName,
                    CreatedBy = _currentUser.UserName,
                    CreatedDate = timeNow,
                    LastModifiedBy = _currentUser.UserName,
                    LastModifiedDate = timeNow,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync(cancellationToken); // save changes to the database

                //generate token and send to their email for confirmation.
                var token = _tokenService.GenerateToken(user.Email);
                var url = $"{request.CustomerRegister.Url}?token={HttpUtility.UrlEncode(token)}&email={user.Email}";
                var message = $"<span style='margin-bottom:15.0px'>Confirm your account by clicking here: <br/><a style='font-size:18.0px' href='{url}'><strong>Confirm Email</strong></a></span>";
                await _emailService.SendEmailAsync(user.Email, "Email Confirmation", message);

                await ts.CommitAsync(cancellationToken);
                return Response<UserDto1>.Success(new UserDto1 { Id = user.Id }, "User registered successfully.");
            }
            catch (Exception ex)
            {
                await ts.RollbackAsync(cancellationToken);
                return Response<UserDto1>.Fail("An error occurred!");
            }
        }

        private string Normalize(string value)
        {
            return value.ToUpperInvariant();
        }
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

        // Method to verify the password using PBKDF2
        public bool VerifyPassword(string password, string storedHash, string salt)
        {
            // Convert the stored salt back to byte array
            var saltBytes = Convert.FromBase64String(salt);

            // Use PBKDF2 to hash the provided password with the stored salt
            var hash = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA512);
            var hashBytes = hash.GetBytes(KeySize);

            // Compare the newly hashed password with the stored hash
            return Convert.ToBase64String(hashBytes) == storedHash;
        }
    }


}
*/