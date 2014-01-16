using DabTrial.CustomMembership;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
public sealed class WebSecurity
{
    public static HttpContextBase Context
    {
        get { return new HttpContextWrapper(HttpContext.Current); }
    }

    public static HttpRequestBase Request
    {
        get { return Context.Request; }
    }

    public static HttpResponseBase Response
    {
        get { return Context.Response; }
    }

    public static System.Security.Principal.IPrincipal User
    {
        get { return Context.User; }
    }

    public static bool IsAuthenticated
    {
        get { return User.Identity.IsAuthenticated; }
    }

    public static MembershipCreateStatus Register(string Username, string Password, string Email, bool IsApproved, string FirstName, string LastName)
    {
        MembershipCreateStatus CreateStatus;
        Membership.CreateUser(Username, Password, Email, null, null, IsApproved, Guid.NewGuid(), out CreateStatus);

        if (CreateStatus == MembershipCreateStatus.Success)
        {
            using (DataContext Context = new DataContext())
            {
                DabTrial.Domain.Tables.User user = Context.Users.FirstOrDefault(Usr => Usr.UserName == Username);
                user.FirstName = FirstName;
                user.LastName = LastName;
                Context.SaveChanges();
            }

            if (IsApproved)
            {
                FormsAuthentication.SetAuthCookie(Username, false);
            }
        }

        return CreateStatus;
    }

    public static Boolean Login(string Username, string Password, bool persistCookie = false)
    {
        using (DataContext context = new DataContext())
        {
            var usr = CodeFirstMembershipProvider.ValidateUser(context, Username, Password);
            if (usr != null) 
            { 
                (new UserService(DBcontext: context)).SetIpStudyCentre(usr.StudyCentreId);
                FormsAuthentication.SetAuthCookie(Username, persistCookie);
                return true;
            }
        }
        return false;

    }

    public static void Logout()
    {
        FormsAuthentication.SignOut();
    }

    public static MembershipUser GetUser(string Username)
    {
        return Membership.GetUser(Username);
    }

    public static bool ChangePassword(string userName, string currentPassword, string newPassword)
    {
        bool success = false;
        try
        {
            MembershipUser currentUser = Membership.GetUser(userName, true);
            success = currentUser.ChangePassword(currentPassword, newPassword);
        }
        catch (ArgumentException)
        {

        }

        return success;
    }

    public static bool DeleteUser(string Username)
    {
        return Membership.DeleteUser(Username);
    }

    public static int GetUserId(string userName)
    {
        MembershipUser user = Membership.GetUser(userName);
        return (int)user.ProviderUserKey;
    }

    public static string CreateAccount(string userName, string password)
    {
        return CreateAccount(userName, password, requireConfirmationToken: false);
    }

    public static string CreateAccount(string userName, string password, bool requireConfirmationToken = false)
    {
        CodeFirstMembershipProvider CodeFirstMembership = Membership.Provider as CodeFirstMembershipProvider;
        return CodeFirstMembership.CreateAccount(userName, password, requireConfirmationToken);
    }

    public static string CreateUserAndAccount(string userName, string password)
    {
        return CreateUserAndAccount(userName, password, propertyValues: null, requireConfirmationToken: false);
    }

    public static string CreateUserAndAccount(string userName, string password, bool requireConfirmation)
    {
        return CreateUserAndAccount(userName, password, propertyValues: null, requireConfirmationToken: requireConfirmation);
    }

    public static string CreateUserAndAccount(string userName, string password, IDictionary<string, object> values)
    {
        return CreateUserAndAccount(userName, password, propertyValues: values, requireConfirmationToken: false);
    }

    public static string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false)
    {
        CodeFirstMembershipProvider CodeFirstMembership = Membership.Provider as CodeFirstMembershipProvider;

        IDictionary<string, object> values = null;
        if (propertyValues != null)
        {
            values = new RouteValueDictionary(propertyValues);
        }

        return CodeFirstMembership.CreateUserAndAccount(userName, password, requireConfirmationToken, values);
    }

    public static List<MembershipUser> FindUsersByEmail(string Email, int PageIndex, int PageSize)
    {
        int totalRecords;
        return Membership.FindUsersByEmail(Email, PageIndex, PageSize, out totalRecords).Cast<MembershipUser>().ToList();
    }

    public static List<MembershipUser> FindUsersByName(string Username, int PageIndex, int PageSize)
    {
        int totalRecords;
        return Membership.FindUsersByName(Username, PageIndex, PageSize, out totalRecords).Cast<MembershipUser>().ToList();
    }

    public static List<MembershipUser> GetAllUsers(int PageIndex, int PageSize)
    {
        int totalRecords;
        return Membership.GetAllUsers(PageIndex, PageSize, out totalRecords).Cast<MembershipUser>().ToList();
    }

    public static void InitializeDatabaseConnection(string connectionStringName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {

    }

    public static void InitializeDatabaseConnection(string connectionString, string providerName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {

    }
}