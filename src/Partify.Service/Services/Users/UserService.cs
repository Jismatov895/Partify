﻿using Microsoft.EntityFrameworkCore;
using Partify.DataAccess.UnitOfWorks;
using Partify.Domain.Entities.Users;
using Partify.Service.Configurations;
using Partify.Service.Exceptions;
using Partify.Service.Extensions;
using Partify.Service.Helpers;

namespace Partify.Service.Services.Users;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
	public async ValueTask<User> ModifyAsync(long id, User user)
	{
		var exsitUser = await unitOfWork.UserRepository.SelectAsync(user => user.Id == id)
			?? throw new NotFoundException("This user is not found");

		var alreadyExistUser = await unitOfWork.UserRepository
			.SelectAsync(u => u.Phone == user.Phone && u.Id != id);
		if (alreadyExistUser is not null)
			throw new AlreadyExistException($"This user is already exist with this phone | Phone={user.Phone}");

		exsitUser.Email = user.Email;
		exsitUser.Phone = user.Phone;
		exsitUser.LastName = user.LastName;
		exsitUser.FirstName = user.FirstName;
		exsitUser.UpdatedById = HttpContextHelper.GetUserId;

		var updatedUser = await unitOfWork.UserRepository.UpdateAsync(exsitUser);
		await unitOfWork.SaveAsync();

		return updatedUser;
	}

	public async ValueTask<bool> DeleteAsync(long id)
	{
		var exsitUser = await unitOfWork.UserRepository.SelectAsync(user => user.Id == id)
			?? throw new NotFoundException("This user is not found");

		exsitUser.DeletedById = HttpContextHelper.GetUserId;
		await unitOfWork.UserRepository.DeleteAsync(exsitUser);
		await unitOfWork.SaveAsync();
		return true;
	}

	public async ValueTask<User> GetAsync(long id)
	{
		return await unitOfWork.UserRepository.SelectAsync(user => user.Id == id)
			?? throw new NotFoundException("This user is not found");
	}

	public async ValueTask<IEnumerable<User>> GetAllAsync(PaginationParams @params, Filter filter, string search = null)
	{
		var users = unitOfWork.UserRepository.Select(isTracking: false);

		if (!string.IsNullOrWhiteSpace(search))
			users = users.Where(u =>
				u.Phone.ToString().Contains(search) ||
				u.Email.ToLower().Contains(search.ToLower()) ||
				u.LastName.ToLower().Contains(search.ToLower()) ||
				u.FirstName.ToLower().Contains(search.ToLower()));

		return await users.ToPaginateAsQueryable(@params).OrderBy(filter).ToListAsync();
	}

	public async ValueTask<User> ChangePasswordAsync(string oldPasword, string newPassword, string confirmPassword)
	{
		var existUser = await unitOfWork.UserRepository.SelectAsync(user => user.Id == HttpContextHelper.GetUserId)
			?? throw new NotFoundException("User is not found");

		if (!PasswordHasher.Verify(oldPasword, existUser.Password))
			throw new ArgumentIsNotValidException("Old password is invalid");

		if (newPassword != confirmPassword)
			throw new ArgumentIsNotValidException("Password is not match");

		existUser.Password = PasswordHasher.Hash(newPassword);
		await unitOfWork.UserRepository.UpdateAsync(existUser);
		await unitOfWork.SaveAsync();

		return existUser;
	}
}