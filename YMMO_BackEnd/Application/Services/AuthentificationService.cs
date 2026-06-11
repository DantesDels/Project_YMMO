using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using YMMO.Backend.Application.DTOs.Authentification;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Interfaces;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.Backend.Application.Services;

public class AuthentificationService : IAuthentificationService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IContactRepository _contactRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly IUserAccessor _userAccessor;

    public AuthentificationService(
        IPasswordHasher passwordHasher,
        IContactRepository contactRepository, 
        IClientRepository clientRepository, 
        IConfiguration config,
        IMapper mapper,
        IUserAccessor userAccessor
        )
    {
        _passwordHasher = passwordHasher;
        _contactRepository = contactRepository;
        _clientRepository = clientRepository;
        _config = config;
        _mapper = mapper;
        _userAccessor = userAccessor;
    }
    
    public async Task<bool> UpdatePasswordAsync(UpdatePasswordDto dto)
    {
        var contactId = _userAccessor.GetCurrentUserId();
        var contact = await _contactRepository.GetByIdAsync(contactId);
        
        if (contact == null) 
        {
            throw new KeyNotFoundException("Le compte utilisateur est introuvable en base de données.");
        }
        
        if (!_passwordHasher.Verify(dto.OldPassword, contact.PasswordHash))
        {
            throw new UnauthorizedAccessException("L'ancien mot de passe est incorrect.");
        }

        string newHashedPassword = _passwordHasher.Hash(dto.NewPassword);
        contact.UpdatePassword(newHashedPassword);

        await _contactRepository.UpdateAsync(contact);
        return true;
    }

    private string GenerateJwtToken(Contact user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.NameIdentifier, user.ContactId.ToString()),
            new Claim(ClaimTypes.Role, user.ContactRole.ToString())
        };
        
        if (user is Agent agent)
        {
            claims.Add(new Claim("AgencyId", agent.AgencyID.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims, 
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<AuthentificationDto.AuthentificationResponse> LoginAsync(AuthentificationDto.LoginRequest request)
    {
        var contact = await _clientRepository.GetByEmailAsync(request.Email);
        if (contact == null || !_passwordHasher.Verify(request.Password, contact.PasswordHash))
        {
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");
        }

        var token = GenerateJwtToken(contact);
    
        return new AuthentificationDto.AuthentificationResponse(
            token, 
            contact.FirstName, 
            contact.ContactId
        );
    }

    public async Task<AuthentificationDto.AuthentificationResponse> RegisterAsync(AuthentificationDto.RegisterRequest request)
    {
        var existing = await _clientRepository.GetByEmailAsync(request.Email);
        if (existing != null) throw new ArgumentException("Cet email est déjà utilisé.");

        // ANCIENNE VERSION
        /*
        var newClient = new Client
        {
            CreatedAt = DateTime.UtcNow,
            FirstName = request.Username,
            LastName = "Utilisateur",
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = hashedPassword,
        };
        */

        // NOUVELLE VERSION
        var newClient = _mapper.Map<Client>(request);
        newClient.CreatedAt = DateTime.UtcNow;
        newClient.LastName = "Utilisateur";
        newClient.PasswordHash = _passwordHasher.Hash(request.Password);
        
        await _clientRepository.AddAsync(newClient);

        var token = GenerateJwtToken(newClient);
    
        return new AuthentificationDto.AuthentificationResponse(
            token, 
            newClient.FirstName, 
            newClient.ContactId
        );
    }

    public async Task LogoutAsync(string token)
    {
        await Task.CompletedTask;
    }
}