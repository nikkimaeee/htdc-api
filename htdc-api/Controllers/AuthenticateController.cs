using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using htdc_api.Authentication;
using htdc_api.Enumerations;
using htdc_api.Interface;
using htdc_api.Models;
using htdc_api.Models.Authentication;
using htdc_api.Models.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;

namespace htdc_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuthenticateController : BaseController
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMD5CryptoService _md5CryptoService;
    private readonly static string reservedCharacters = "!*'();:@&=+$,/?%#[]";
    public AuthenticateController(
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IMapper mapper,
        UserManager<IdentityUser> userManager,
        IMD5CryptoService md5CryptoService,
        IEmailSender emailSender) : base(context, mapper, userManager,emailSender, configuration)
    {
        _roleManager = roleManager;
        _md5CryptoService = md5CryptoService;
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userProfile = _context.UserProfiles.FirstOrDefault(x => x.AspNetUserId == user.Id);
            if (userProfile == null || (userProfile != null && !userProfile.IsActive))
            {
                return Unauthorized();
            }

            var lastLogin = userProfile.LastLogin != null ? userProfile.LastLogin.Value : DateTime.MinValue;
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Verified", user.EmailConfirmed.ToString(), ClaimValueTypes.Boolean)
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            userProfile.LastLogin = DateTime.UtcNow;
            _context.Entry(userProfile).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                role = userRoles,
                username = model.Username,
                firstName = userProfile.FirstName,
                lastName = userProfile.LastName,
                lastLogin = lastLogin,
                id = user.Id,
                isVerified = user.EmailConfirmed
            });
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        using (var identitydbContextTransaction = _context.Database.BeginTransaction())
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var userExists = await _userManager.FindByEmailAsync(model.Email);
                    if (userExists != null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response
                                { Status = StatusEnum.Error.GetDisplayName(), Message = "Email already exists!" });
                    }

                    IdentityUser user = new()
                    {
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response
                            {
                                Status = StatusEnum.Error.GetDisplayName(),
                                Message = "SignUp failed! Please check details and try again."
                            });
                    }

                    await _userManager.AddToRoleAsync(user, UserRoles.Patient);

                    var profile = new UserProfile
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        AspNetUserId = user.Id,
                        Avatar = "",
                        IsActive = true,
                        IsPwd = model.IsPwd,
                        IsSenior = model.IsSenior,
                        IsPregnant = model.IsPregnant,
                        Phone = model.Phone
                    };

                    var patientInformation = _mapper.Map<PatientInformation>(profile);
                    patientInformation.Email = model.Email;
                    profile.PatientInformation = patientInformation;
                    await _context.UserProfiles.AddAsync(profile);

                    byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                    byte[] key = Guid.NewGuid().ToByteArray();
                    string token = Convert.ToBase64String(time.Concat(key).ToArray());

                    var url = _configuration.GetSection("baseUrl").Value;
                    var confirmationEmail = $"{url}/confirm?user={user.Id}&token={UrlEncode(token)}";
                    await _context.VerificationTokens.AddAsync(new VerificationTokens
                    {
                        AspNetUserId = user.Id,
                        Token = token,
                        IssuedDate = DateTime.UtcNow,
                        ExpiresDate = DateTime.UtcNow.AddHours(1)
                    });
                    await _context.SaveChangesAsync();

                    identitydbContextTransaction.Commit();
                    
                    var emailBody = $"Click the url to verify email address for {user.UserName}\n {confirmationEmail}";
                    SendEmail(model.Email, emailBody, "Verify Email");  
                }

                return Ok(new Response
                    { Status = StatusEnum.Success.GetDisplayName(), Message = "Sign up successfully!" });
            }
            catch (Exception ex)
            {
                identitydbContextTransaction.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response
                    {
                        Status = StatusEnum.Error.GetDisplayName(), Message = ex.Message + "/n/n/n" + ex.StackTrace
                    });
            }
        }
    }

    [HttpPost]
    [Route("Verify")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> VerifyEmail(VerifyEmailPayload payload)
    {
        try
        {
            var dateNow = DateTime.UtcNow;
            var user = await _userManager.FindByIdAsync(payload.UserId);
            var existingToken = await _context.VerificationTokens.FirstOrDefaultAsync(x => x.Token == payload.Token);
            if (existingToken == null || existingToken.AspNetUserId != user.Id)
            {
                return BadRequest("Invalid Token!");
            }

            if (dateNow > existingToken.ExpiresDate)
            {
                return BadRequest("Verification Url already expired!");
            }
            
            user.EmailConfirmed = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message + "/n/n" + ex.StackTrace);
        }
    }

    [HttpPost]
    [Route("Resend")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Resend(VerifyEmailPayload payload)
    {
        var user = await _userManager.FindByEmailAsync(payload.Email);
        byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        byte[] key = Guid.NewGuid().ToByteArray();
        string token = Convert.ToBase64String(time.Concat(key).ToArray());

        var url = _configuration.GetSection("baseUrl").Value;
        var confirmationEmail = $"{url}/confirm?user={user.Id}&token={UrlEncode(token)}";
        await _context.VerificationTokens.AddAsync(new VerificationTokens
        {
            AspNetUserId = user.Id,
            Token = token,
            IssuedDate = DateTime.UtcNow,
            ExpiresDate = DateTime.UtcNow.AddHours(1)
        });

        await _context.SaveChangesAsync();

        var emailBody = $"Click the url to verify email address for {user.UserName}\n {confirmationEmail}";
        SendEmail(payload.Email, emailBody, "Verify Email");

        return Ok();
    }


    [HttpPost]
    [Route("ForgotPassword")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPasswordLink(VerifyEmailPayload payload)
    {
        var user = await _userManager.FindByEmailAsync(payload.Email);
        if(user != null)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());

            var url = _configuration.GetSection("baseUrl").Value;
            var confirmationEmail = $"{url}/resetpassword?user={user.Id}&token={UrlEncode(token)}";
            await _context.VerificationTokens.AddAsync(new VerificationTokens
            {
                AspNetUserId = user.Id,
                Token = token,
                IssuedDate = DateTime.UtcNow,
                ExpiresDate = DateTime.UtcNow.AddHours(1)
            });

            await _context.SaveChangesAsync();

            var emailBody = $"Click the url to reset password <br/>{confirmationEmail}";
            SendEmail(payload.Email, emailBody, "Reset Password");
        } else
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = "Invalid Email!" });
        }

        return Ok(new Response
        { Status = StatusEnum.Success.GetDisplayName(), Message = "Sent!" });
    }

    [HttpPost]
    [Route("ResetPassword")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ResetPassword(VerifyEmailPayload payload)
    {
        try
        {
            var dateNow = DateTime.UtcNow;
            var user = await _userManager.FindByIdAsync(payload.UserId);
            var existingToken = await _context.VerificationTokens.FirstOrDefaultAsync(x => x.Token == payload.Token);
            if (existingToken == null || existingToken.AspNetUserId != user.Id)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = "Invalid Token!" });
            }

            if (dateNow > existingToken.ExpiresDate)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = "Reset Password Url already expired!" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, payload.Password);

            return Ok(new Response
            { Status = StatusEnum.Success.GetDisplayName(), Message = "Password reset successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = ex.Message + "/n/n" + ex.StackTrace });
        }
    }



    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
    
    private static string UrlEncode(string value)
    {
        if (String.IsNullOrEmpty(value))
            return String.Empty;

        var sb = new StringBuilder();

        foreach (char @char in value)
        {
            if (reservedCharacters.IndexOf(@char) == -1)
                sb.Append(@char);
            else
                sb.AppendFormat("%{0:X2}", (int)@char);
        }
        return sb.ToString();
    }
}