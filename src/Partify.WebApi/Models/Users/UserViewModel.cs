﻿using Partify.WebApi.Models.UserRoles;

namespace Partify.WebApi.Models.Users;

public record UserViewModel(
	long Id,
	string FirstName,
	string LastName,
	long Phone,
	string Email,
	UserRoleViewModel Role);