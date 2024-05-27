# SIT CAMS Client Integration

This project is intended for use to integrate any backend with the CAMS Core for authentication and authorization services.

To use this project in an ASPNet Core web API (referred to as "the project"), follow the below steps.

1. Add the sit-cams-client-integration class library as a git submodule to the project by running the following command from the project's root directory:
    ```
    git submodule add git@gitlab.com:ziphio/projects/azbil/sit-solutions/cams/sit-cams-client-integration.git
    ```

2. Add the class lib to the solution and the project by running the following command from the project's root directory (where the *.csproj file is located):
    ```
    dotnet sln add <path-to-sit-cams-client-integration-.csproj file>
    dotnet add <path-to-the-project-.csproj file> reference <path-to-sit-cams-client-integration-.csproj file>
    ```

3. To register all required services, contexts, and controllers that are used by the class lib, in `Startup.cs`, include the following call in the `ConfigureServices` method:
    ```
    services.AddCAMSServices(Configuration);
    ```

    and ensure that following calls are being made in the `Configure` method:
    ```
    app.UseAuthentication();
    app.UseAuthorization();
    ```

4. Add the required `CentralAuth` configurations for the CAMS Core and `ConnectionStrings` for the `CAMSDBContext` to the project's `appsettings.json` file. See the included `sampleAppSettings.json` for an example.

5. Finally, configure the existing controllers in the project by decorating them with the authentication and authorization attributes. The highest (super-admin) rank is rank 1. An example is given below:
    ```
    // This uses the "Token" authentication scheme to authenticate calls to all methods in the controller
    [ApiController]
    [Authorize(AuthenticationSchemes = "Token")]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        // Required controller setup
        // ...
        // ...

        // This uses the VerifyRank Authorization Attribute to check the rank of the user for this particular method only. 
        // This is what is needed for active directory rank-based authorization for SIT. The endpoint -> rank mapping 
        // must be saved in the CAMSDBContext, otherwise the default rank will be set for this method.
        [VerifyRank]
        [HttpGet("configure")]
        public string GetMethodWithVerifyRank()
        {
            return "Successfully accessed controller method with verified rank access using system DB";
        }

        // This uses the RequiredRank Authorization Attribute to check the rank of the user for this particular method only. 
        // This is what is needed for active directory rank-based authorization for SIT
        [RequiredRank(1)]
        [HttpGet("configure")]
        public string GetMethodWithLevel1Rank()
        {
            return "Successfully accessed controller method with level1 rank access";
        }
        
        // This uses the RequiredPermission Authorization Attribute to check the permissions of the user for this particular method only. 
        [RequiredPermission(PermissionConfig.Configure)]
        [HttpGet("configure")]
        public string GetMethodWithConfigurePermission()
        {
            return "Successfully accessed controller method with configure priviledges";
        }

        // This uses the RequiredRole Authorization Attribute to check the role of the user for this particular method only. 
        // This authorizes the super_admin user. It will only authorize a user of a particular role matching the role string.
        // Other than super_admin, there is no heirarchy to the roles. String matching is case insensitive.
        [RequiredRole("level1")]
        [HttpGet("configure")]
        public string GetMethodWithLevel1Role()
        {
            return "Successfully accessed controller method with level1 role access";
        }

    }
    ```

    If required, custom string representations of ranks and permissions may be added to your own configuration files that inherit from `RoleConfig.cs` and `PermissionConfig.cs`. The default rank when using VerifyRank attribute can be set in `RankConfig.cs`.

    Finally, when using `VerifyRank` attribute, `EndPointRank` entities must be saved in the DB to configure the ranks for each endpoint. There is support for partial endpoints as well (i.e. A saved rank mapping for the endpoint `api/test` can apply to any method with paths that extend from `api/test`, such as `api/test/1`).

# How add migration and create database for sit-cams-client-integration class library

1. navigate into `class library` folder
```
cd sit-cams-integration
```

3. use following command to remove last migration (if needed)
```
dotnet ef migrations remove --startup-project ../MyAPIBackend/ --context CAMSDBContext
```


3. use following command to add new migration
```
dotnet ef migrations add InitialMigration --startup-project ../MyAPIBackend/ --context CAMSDBContext
```

4. use following command to create or update database according to the migration
```
dotnet ef database update --startup-project ../MyAPIBackend/ --context CAMSDBContext
```

# CAMS loggin feature

This feature will enable to record user activity log in CAMS application
To use this feature please follow below steps

1. Make sure you have following Key values in your app setting file (Make sure your appsettings.json file is up-to-date with the sampleAppSettings.json file)
    ```

    "UseUserName": false,
    "CAMSTokenValue": "r6ugi1z6xn3fv2pag4bfa",
    "CreateActivityLogPath": "api/microservices/platforms/activity-log/create"

    ```

2. Add ISystemLogger into the constructor method in your class.
    ```
    // Add to the constructor
    private readonly ISystemLogger _systemLogger;

    public MyController(ISystemLogger systemLogger)
    {
        _systemLogger = systemLogger;
    }
    ```
    
3. then please call given "CreateLog()" method in "ISystemLogger" from anywhere of your project
    ```
    // Create new log
    [RequiredPermission(PermissionConfig.AssetCreate)]
    [HttpGet("my-controller")]
    public ActionResult<ResponseMessage> Get()
    {
        try
        {
            var msg = ResponseMessage.Get(true, "Test message");
            var tokenUserId = int.Parse(HttpContext.User.FindFirst("UserId").Value);

            _systemLogger.CreateLog("TestTypeFromADAuth", "Desc Log Here", msg, tokenUserId);
            return Ok(msg);
        }
        catch (Exception e)
        {
            return BadRequest(ResponseMessage.Get(false, e.Message));
        }
    }
    ```

# CAMS get user details

This feature will enable to get comprehensive details of the authenticated user 
(This may contains sensitive information about the specific user. so in order to prevent from getting other user's infromation, this method required to pass user token for fetch those information)
To use this feature please follow below steps

1. Make sure you have following Key values in your app setting file (Make sure your appsettings.json file is up-to-date with the sampleAppSettings.json file)
    ```

    "UserProfilePath": "api/microservices/employees/profile"

    ```

2. Add IUserInfo into the constructor method in your class.
    ```
    // Add to the constructor
    private readonly IUserInfo _userInfo;

    public MyController(IUserInfo userInfo)
    {
        _userInfo = userInfo;
    }
    ```
    
3. then please call given "GetUserDetails(userId,UserToken)" method in "IUserInfo" from anywhere of your project
    ```
    [RequiredPermission(PermissionConfig.AssetCreate)]
    [HttpGet("Get-User-Info")]
    public async Task<ActionResult<ResponseMessage>> GetUserInfo()
    {
        try
        {
            EmployeeDTO dto = new EmployeeDTO();

            var authorizationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).ToString();
            if (authorizationHeaderValue.Contains("Token"))
            {
                var userToken = authorizationHeaderValue.Replace("Token ", "").Trim();
                var tokenUserId = int.Parse(HttpContext.User.FindFirst("UserId").Value);

                dto = await _userInfo.GetUserDetails(tokenUserId, userToken);
            }
                
            return Ok(dto);
        }
        catch (Exception e)
        {
            return BadRequest(ResponseMessage.Get(false, e.Message));
        }
    }
    ```

# CAMS get all users assigned to this platform

This feature will get all users assigned to this platform with their role
To use this feature please follow below steps

1. Make sure you have following Key values in your app setting file (Make sure your appsettings.json file is up-to-date with the sampleAppSettings.json file)
    ```

    "GetAllUsersPath": "api/microservices/platforms/employees"

    ```

2. Add IUserInfo into the constructor method in your class.
    ```
    // Add to the constructor
    private readonly IUserInfo _userInfo;

    public MyController(IUserInfo userInfo)
    {
        _userInfo = userInfo;
    }
    ```
    
3. then please call given "GetAllUsersByPlatform(pageNumer,itemsPerPageLimit)" method in "IUserInfo" from anywhere of your project
    ```
    [RequiredPermission(PermissionConfig.AssetCreate)]
    [HttpGet("Get-Users")]
    public async Task<ActionResult<ResponseMessage>> GetUsers()
    {
        try
        {
            PaginationDTO<EmployeeWithRoleDTO> dto = new PaginationDTO<EmployeeWithRoleDTO>();

            var authorizationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).ToString();
            if (authorizationHeaderValue.Contains("Token"))
            {
                var userToken = authorizationHeaderValue.Replace("Token ", "").Trim();
                var tokenUserId = int.Parse(HttpContext.User.FindFirst("UserId").Value);

                dto = await _userInfo.GetAllUsersByPlatform();
            }

            return Ok(dto);
        }
        catch (Exception e)
        {
            return BadRequest(ResponseMessage.Get(false, e.Message));
        }
    }
    ```