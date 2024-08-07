﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Partify.DataAccess.UnitOfWorks;
using Partify.Service.Helpers;
using Partify.Service.Services.Accounts;
using Partify.Service.Services.AdCategories;
using Partify.Service.Services.AdComments;
using Partify.Service.Services.AdImages;
using Partify.Service.Services.Ads;
using Partify.Service.Services.AdScores;
using Partify.Service.Services.AdViews;
using Partify.Service.Services.Assets;
using Partify.Service.Services.Facilities;
using Partify.Service.Services.FavoriteAds;
using Partify.Service.Services.Permissions;
using Partify.Service.Services.UserRolePermissions;
using Partify.Service.Services.UserRoles;
using Partify.Service.Services.Users;
using Partify.WebApi.ApiServices.Accounts;
using Partify.WebApi.ApiServices.AdCategories;
using Partify.WebApi.ApiServices.AdComments;
using Partify.WebApi.ApiServices.AdImages;
using Partify.WebApi.ApiServices.Ads;
using Partify.WebApi.ApiServices.AdScores;
using Partify.WebApi.ApiServices.AdViews;
using Partify.WebApi.ApiServices.Facilities;
using Partify.WebApi.ApiServices.FavoriteAds;
using Partify.WebApi.ApiServices.Permissions;
using Partify.WebApi.ApiServices.UserRolePermissions;
using Partify.WebApi.ApiServices.UserRoles;
using Partify.WebApi.ApiServices.Users;
using Partify.WebApi.Helpers;
using Partify.WebApi.Middlewares;
using Partify.WebApi.Validators.Accounts;
using System.Text;

namespace Partify.WebApi.Extensions;

public static class ServicesCollectionExtension
{
	public static void AddServices(this IServiceCollection services)
	{
		services.AddScoped<IAdService, AdService>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IAssetService, AssetService>();
		services.AddScoped<IAdViewService, AdViewService>();
		services.AddScoped<IAdImageService, AdImageService>();
		services.AddScoped<IAdScoreService, AdScoreService>();
		services.AddScoped<IAccountService, AccountService>();
		services.AddScoped<IUserRoleService, UserRoleService>();
		services.AddScoped<IFacilityService, FacilityService>();
		services.AddScoped<IUserRoleService, UserRoleService>();
		services.AddScoped<IAdCommentService, AdCommentService>();
		services.AddScoped<IFavoriteAdService, FavoriteAdService>();
		services.AddScoped<IPermissionService, PermissionService>();
		services.AddScoped<IAdCategoryService, AdCategoryService>();
		services.AddScoped<IUserRolePermissionService, UserRolePermissionService>();
	}

	public static void AddApiServices(this IServiceCollection services)
	{
		services.AddScoped<IAdApiService, AdApiService>();
		services.AddScoped<IUserApiService, UserApiService>();
		services.AddScoped<IAdViewApiService, AdViewApiService>();
		services.AddScoped<IAdImageApiService, AdImageApiService>();
		services.AddScoped<IAdScoreApiService, AdScoreApiService>();
		services.AddScoped<IAccountApiService, AccountApiService>();
		services.AddScoped<IUserRoleApiService, UserRoleApiService>();
		services.AddScoped<IFacilityApiService, FacilityApiService>();
		services.AddScoped<IUserRoleApiService, UserRoleApiService>();
		services.AddScoped<IAdCommentApiService, AdCommentApiService>();
		services.AddScoped<IFavoriteAdApiService, FavoriteAdApiService>();
		services.AddScoped<IPermissionApiService, PermissionApiService>();
		services.AddScoped<IAdCategoryApiService, AdCategoryApiService>();
		services.AddScoped<IUserRolePermissionApiService, UserRolePermissionApiService>();
	}

	public static void AddExceptions(this IServiceCollection services)
	{
		services.AddExceptionHandler<NotFoundExceptionMiddleware>();
		services.AddExceptionHandler<ForbiddenExceptionMiddleware>();
		services.AddExceptionHandler<AlreadyExistExceptionMiddleware>();
		services.AddExceptionHandler<InternalServerExceptionMiddleware>();
		services.AddExceptionHandler<ArgumentIsNotValidExceptionMiddleware>();
	}

	public static void AddInjectHelper(this WebApplication app)
	{
		var scope = app.Services.CreateScope();
		ServiceHelper.UserRoleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
		ServiceHelper.UserRolePermissionService = scope.ServiceProvider.GetRequiredService<IUserRolePermissionService>();
	}

	public static void AddPathInitializer(this WebApplication app)
	{
		HttpContextHelper.ContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
		EnvironmentHelper.JwtKey = app.Configuration.GetSection("Jwt:Key").Value;
		EnvironmentHelper.TokenLifeTimeInHour = app.Configuration.GetSection("Jwt:LifeTime").Value;
		EnvironmentHelper.SmtpHost = app.Configuration.GetSection("Email:SmtpHost").Value;
		EnvironmentHelper.SmtpPort = app.Configuration.GetSection("Email:SmtpPort").Value;
		EnvironmentHelper.EmailAddress = app.Configuration.GetSection("Email:EmailAddress").Value;
		EnvironmentHelper.EmailPassword = app.Configuration.GetSection("Email:EmailPassword").Value;
		EnvironmentHelper.SuperAdminLogin = app.Configuration.GetSection("SuperAdmin:Login").Value;
		EnvironmentHelper.SuperAdminPassword = app.Configuration.GetSection("SuperAdmin:Password").Value;
	}

	public static void AddValidators(this IServiceCollection services)
	{
		services.AddTransient<AccountLoginValidator>();
		services.AddTransient<AccountCreateValidator>();
		services.AddTransient<AccountVerifyValidator>();
		services.AddTransient<AccountSendCodeValidator>();
		services.AddTransient<AccountRegisterModelValidator>();
		services.AddTransient<AccountResetPasswordValidator>();
	}

	public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthentication(x =>
		{
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(o =>
		{
			var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
			o.SaveToken = true;
			o.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = configuration["Jwt:Issuer"],
				ValidAudience = configuration["Jwt:Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(key)
			};
		});
	}

	public static void ConfigureSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(setup =>
		{
			var jwtSecurityScheme = new OpenApiSecurityScheme
			{
				BearerFormat = "JWT",
				Name = "JWT Authentication",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = JwtBearerDefaults.AuthenticationScheme,
				Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

				Reference = new OpenApiReference
				{
					Id = JwtBearerDefaults.AuthenticationScheme,
					Type = ReferenceType.SecurityScheme
				}
			};

			setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

			setup.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{ jwtSecurityScheme, Array.Empty<string>() }
				});
		});
	}
}
