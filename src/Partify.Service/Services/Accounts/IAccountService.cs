﻿using Partify.Domain.Entities.Users;

namespace Partify.Service.Services.Accounts;

public interface IAccountService
{
	ValueTask RegisterAsync(User user);
	ValueTask RegisterVerifyAsync(long phone, string code);
	ValueTask<User> CreateAsync(long phone);
	ValueTask<(User user, string token)> LoginAsync(long phone, string password);
	ValueTask<bool> SendCodeAsync(string email);
	ValueTask<bool> VerifyAsync(string email, string code);
	ValueTask<User> ResetPasswordAsync(long phone, string newPassword);
}