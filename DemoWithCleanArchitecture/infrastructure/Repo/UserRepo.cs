﻿using Application.Contract;
using Application.DTOs;
using Domain.Entities;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace infrastructure.Repo
{
    internal class UserRepo : IUser
    {
        private readonly AppDbContext appDbContext;

        public UserRepo(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await FindUserByEmail(loginDTO.Email!); // null이 아님을 보장한다는 의미에서 ! 를 붙임

            if (getUser == null) return new LoginResponse(false, "User not found");

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
            {
                return new LoginResponse(true, "Login successfully", GenerateJWTToken(getUser));
            }
            else
            {
                return new LoginResponse(false, "Invalid credentials");
            }

        }

        private string GenerateJWTToken(ApplicationUser getUser)
        {
            
        }

        private async Task<ApplicationUser> FindUserByEmail(string email) => 
            await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            var getUser = await FindUserByEmail(registerUserDTO.Email!);
            if (getUser == null)
                return new RegistrationResponse(false, "User Already exist");

            appDbContext.Users.Add(new ApplicationUser()
            {
                Name = registerUserDTO.Name,
                Email = registerUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDTO.Password)
            });

            await appDbContext.SaveChangesAsync();
            return new RegistrationResponse(true, "Registration completed");
        }
    }
}
