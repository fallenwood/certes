namespace Certes.Acme.Resource;

using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

public class AccountTests
{
    [Fact]
    public void CanGetSetProperties()
    {
        var account = new Account();
        account.VerifyGetterSetter(a => a.Status, AccountStatus.Valid);
        account.VerifyGetterSetter(a => a.Contact, new string[] { "mailto:hello@example.com" });
        account.VerifyGetterSetter(a => a.Orders, new Uri("http://certes.is.working"));
        account.VerifyGetterSetter(a => a.TermsOfServiceAgreed, true);

        var r = new Account.Payload();
        r.VerifyGetterSetter(a => a.OnlyReturnExisting, true);
    }

    [Fact]
    public void CanBeSerialized()
    {
        var srcJson = File.ReadAllText("./Data/account.json");
        var deserialized = JsonSerializer.Deserialize<Account>(srcJson, AcmeJsonSerializerContext.Unindented.Account);
        var json = JsonSerializer.Serialize(deserialized, AcmeJsonSerializerContext.Unindented.Account);

        Assert.Equal(AccountStatus.Valid, deserialized.Status);

        var a = Regex.Replace(srcJson, @"\s", "");
        var b = Regex.Replace(json, @"\s", "").Replace("\u002B", "+");
        Assert.Equal(Regex.Replace(srcJson, @"\s", "").Length, Regex.Replace(json, @"\s", "").Replace("\u002B", "+").Length);
    }
}
