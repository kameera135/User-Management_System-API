﻿using Data;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using Azure.Core;
using System;

namespace UMS.Api.Repositories
{
    public class PasswordPolicyRepository : IPasswordPolicyRepository
    {
        private readonly IRepository m_dbContext;
        //private readonly ICommonConfigRepository m_commonConfigRepository;

        public PasswordPolicyRepository(IRepository dbContext) //ApplicationDbContext applicationDbContext
        {
            m_dbContext = dbContext;
            //m_commonConfigRepository = commonConfigRepository;
        }
    }
}