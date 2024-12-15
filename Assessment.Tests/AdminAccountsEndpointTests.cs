using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Assessment.Tests;

[TestFixture]
public class AdminAccountsEndpointTests : EndpointTestFixture
{
    private IAuthService _authService;
    private IAccountsService _accountsService;
    private IMongoCollection<Account> _accountsCollection;
    
    [SetUp]
    public void Setup()
    {
        // Drop the collection to ensure a clean state for each test
        DropCollection("Accounts");
        _accountsCollection = GetCollection<Account>("Accounts");
        
        // Extract authentication resource
        _authService = GetService<IAuthService>();
        _accountsService = GetService<IAccountsService>();
    }

    
}