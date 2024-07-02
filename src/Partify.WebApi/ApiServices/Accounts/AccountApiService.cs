﻿using AutoMapper;
using Partify.Domain.Entities.Users;
using Partify.Service.Services.Accounts;
using Partify.WebApi.Models.Users;

namespace Partify.WebApi.ApiServices.Accounts;

public class AccountApiService(IAccountService accountService, IMapper mapper) : IAccountApiService
{
	public async ValueTask RegisterAsync(UserRegisterModel registerModel)
	{
		await accountService.RegisterAsync(mapper.Map<User>(registerModel));
	}

	public async ValueTask RegisterVerifyAsync(long phone, string code)
	{
		await accountService.RegisterVerifyAsync(phone, code);
	}

	public async ValueTask<UserViewModel> CreateAsync(long phone)
	{
		var result = await accountService.CreateAsync(phone);
		return mapper.Map<UserViewModel>(result);
	}

	public async ValueTask<LoginViewModel> LoginAsync(long phone, string password)
	{
		var result = await accountService.LoginAsync(phone, password);
		var mappedResult = mapper.Map<LoginViewModel>(result.user);
		mappedResult.Token = result.token;
		return mappedResult;
	}

	public ValueTask<bool> SendCodeAsync(string email)
	{
		return accountService.SendCodeAsync(email);
	}

	public async ValueTask<bool> VerifyAsync(string email, string code)
	{
		return await accountService.VerifyAsync(email, code);
	}

	public async ValueTask<UserViewModel> ResetPasswordAsync(long phone, string newPassword)
	{
		var result = await accountService.ResetPasswordAsync(phone, newPassword);
		return mapper.Map<UserViewModel>(result);
	}
}