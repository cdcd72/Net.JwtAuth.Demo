using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.DTOs.Account;
using NUnit.Framework;

namespace API.Tests.Controllers;

public class DemoControllerTests
{
    private readonly HttpClient _client;

    public DemoControllerTests()
    {
        _client = new Api().CreateClient();
    }
    
    [Test]
    public async Task GetToken()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });

        Assert.NotNull(accessDto?.AccessToken);
    }

    #region GetNotLimitedTest
    
    [Test]
    public async Task GetNotLimited_Success()
    {
        var response = await _client.GetAsync("v1/api/Demo/situation/1");
        
        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.AreEqual("Not Authorize", responseString);
    }
    
    #endregion
    
    #region GetLimitedTest

    [Test]
    public async Task GetLimited_Fail()
    {
        var response = await _client.GetAsync("/v1/api/Demo/situation/2");
    
        // 401
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Test]
    public async Task GetLimited_Success()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });
    
        var request = new HttpRequestMessage(HttpMethod.Get, "v1/api/Demo/situation/2");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);
        
        var response = await _client.SendAsync(request);
    
        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.AreEqual("Authorized", responseString);
    }

    #endregion
    
    #region GetLimitedWithAdminRoleTest

    [Test]
    public async Task GetLimitedWithAdminRole_Fail()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });
    
        var request = new HttpRequestMessage(HttpMethod.Get, "/v1/api/Demo/situation/3");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);
        
        var response = await _client.SendAsync(request);
    
        // 403
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Test]
    public async Task GetLimitedWithAdminRole_Success()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "admin@example.com",
            Password = "securePa55"
        });
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/v1/api/Demo/situation/3");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);
        
        var response = await _client.SendAsync(request);
    
        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    
        var responseString = await response.Content.ReadAsStringAsync();
    
        Assert.AreEqual("Authorized With Role = Admin", responseString);
    }

    #endregion

    #region GetLimitedWithAdminOrUserRoleTest
    
    [Test]
    public async Task GetLimitedWithAdminOrUserRole_WithAdminRole_Success()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "admin@example.com",
            Password = "securePa55"
        });
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/v1/api/Demo/situation/4");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);
        
        var response = await _client.SendAsync(request);
    
        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    
        var responseString = await response.Content.ReadAsStringAsync();
    
        Assert.AreEqual("Authorized With Role = Admin or User", responseString);
    }
    
    [Test]
    public async Task GetLimitedWithAdminOrUserRole_WithUserRole_Success()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/v1/api/Demo/situation/4");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);
        
        var response = await _client.SendAsync(request);
    
        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    
        var responseString = await response.Content.ReadAsStringAsync();
    
        Assert.AreEqual("Authorized With Role = Admin or User", responseString);
    }
    
    #endregion

    #region Private Method

    private async Task<AccessDto?> Login(LoginDto loginDto)
    {
        var response = 
            await _client.PostAsync("/v1/api/Account/login", 
                new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));
        
        return JsonSerializer.Deserialize<AccessDto>(await response.Content.ReadAsStringAsync());
    }

    #endregion
}