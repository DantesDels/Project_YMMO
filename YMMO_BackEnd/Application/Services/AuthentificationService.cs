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

    public AuthentificationService(IPasswordHasher passwordHasher,IContactRepository contactRepository, IClientRepository clientRepository, IConfiguration config)
    {
        _passwordHasher = passwordHasher;
        _contactRepository = contactRepository;
        _clientRepository = clientRepository;
        _config = config;
    }
    
    public async Task<bool> UpdatePasswordAsync(Guid contactId, UpdatePasswordDto dto)
    {
        var contact = await _contactRepository.GetByIdAsync(contactId);
        if (contact == null) throw new KeyNotFoundException("Contact introuvable.");

        if (!_passwordHasher.Verify(dto.OldPassword, contact.PasswordHash))
        {
            throw new UnauthorizedAccessException("L'ancien mot de passe est incorrect.");
        }

        string newHashedPassword = _passwordHasher.Hash(dto.NewPassword);
        contact.UpdatePassword(newHashedPassword);

        await _contactRepository.UpdateAsync(contact);
        return true;
    }

    private string GenerateJwtToken(string username, Guid userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("username", username),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
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

        var token = GenerateJwtToken(contact.FirstName, contact.ContactId);
    
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

        string hashedPassword = _passwordHasher.Hash(request.Password);
    
        var newClient = new Client
        {
            CreatedAt = DateTime.UtcNow,
            FirstName = request.Username,
            LastName = "Utilisateur", 
            Email = request.Email,
            PhoneNumber = "0000000000",
            PasswordHash = hashedPassword,
        };
    
        await _clientRepository.AddAsync(newClient);

        var token = GenerateJwtToken(newClient.FirstName, newClient.ContactId);
    
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