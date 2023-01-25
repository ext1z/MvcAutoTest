namespace WebAutoTest.Services;


public class CookieService
{
    public string? GetUserPhoneFromCookie(HttpContext context)
    {
        if (context.Request.Cookies.ContainsKey("UserPhone"))
        {
            return context.Request.Cookies["UserPhone"];
        }

        return null;
    }

    public void SendUserPhoneToCookie(string userPhone, HttpContext context)
    {
        context.Response.Cookies.Append("UserPhone", userPhone);
    }
}

