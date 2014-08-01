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

    public static MembershipCreateStatus Register(string username, string password, string email, bool isApproved, string firstName, string lastName)
    {
        MembershipCreateStatus createStatus;
        Membership.CreateUser(username, password, email, null, null, isApproved, Guid.NewGuid(), out createStatus);

        if (createStatus == MembershipCreateStatus.Success)
        {
            using (DataContext context = new DataContext())
            {
                DabTrial.Domain.Tables.User user = context.Users.FirstOrDefault(Usr => Usr.UserName == username);
                user.FirstName = firstName;
                user.LastName = lastName;
                context.SaveChanges();
            }

            if (isApproved)
            {
                FormsAuthentication.SetAuthCookie(username, false);
            }
        }

        return createStatus;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="persistCookie"></param>
    /// <returns></returns>
    public static bool Login(string username, string password, out int failedLoginAttempts ,bool persistCookie = false)
    {
        using (DataContext context = new DataContext())
        {
            var usr = CodeFirstMembershipProvider.ValidateUser(context, username, password);
            if (usr != null) 
            { 
                (new UserService(DBcontext: context)).SetIpStudyCentre(usr.StudyCentreId);
                FormsAuthentication.SetAuthCookie(username, persistCookie);
                failedLoginAttempts = 0;
                return true;
            }
            usr = context.Users.Where(u => u.UserName == username).FirstOrDefault();
            failedLoginAttempts = usr == null ? -1 : usr.PasswordFailuresSinceLastSuccess;
            return false;
        }
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
        CodeFirstMembershipProvider codeFirstMembership = (CodeFirstMembershipProvider)Membership.Provider;
        return codeFirstMembership.CreateAccount(userName, password, requireConfirmationToken);
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
        IDictionary<string, object> values = (propertyValues == null)
            ?null
            :values = new RouteValueDictionary(propertyValues);
        CodeFirstMembershipProvider codeFirstMembership = (CodeFirstMembershipProvider)Membership.Provider;
        return codeFirstMembership.CreateUserAndAccount(userName, password, requireConfirmationToken, values);
    }

    public static List<MembershipUser> FindUsersByEmail(string email, int pageIndex, int pageSize)
    {
        int totalRecords;
        return Membership.FindUsersByEmail(email, pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList();
    }

    public static List<MembershipUser> FindUsersByName(string username, int pageIndex, int pageSize)
    {
        int totalRecords;
        return Membership.FindUsersByName(username, pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList();
    }

    public static List<MembershipUser> GetAllUsers(int pageIndex, int pageSize)
    {
        int totalRecords;
        return Membership.GetAllUsers(pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList();
    }

    public static void InitializeDatabaseConnection(string connectionStringName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {

    }

    public static void InitializeDatabaseConnection(string connectionString, string providerName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {

    }
}