using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Certes.Acme.Resource;
using Certes.Jws;
using Moq;
using Xunit;

namespace Certes.Acme;

public class AccountContextTests
{
    private Uri location = new Uri("http://acme.d/account/101");
    private Mock<IAcmeContext> contextMock = new Mock<IAcmeContext>();
    private Mock<IAcmeHttpClient> httpClientMock = new Mock<IAcmeHttpClient>();

    [Fact]
    public async Task CanDeactivateAccount()
    {
        var expectedPayload = new JwsPayload();
        var expectedAccount = new Account();

        contextMock.Reset();
        httpClientMock.Reset();

        contextMock
            .Setup(c => c.GetDirectory())
            .ReturnsAsync(Helper.MockDirectoryV2);
        contextMock
            .Setup(c => c.Sign(It.IsAny<object>(), location, It.IsAny<JsonTypeInfo>()))
            .Callback((object payload, Uri loc, JsonTypeInfo jsonTypeInfo) =>
            {
                Assert.Equal(
                    JsonSerializer.Serialize(new Account { Status = AccountStatus.Deactivated }, jsonTypeInfo),
                    JsonSerializer.Serialize(payload, jsonTypeInfo));
                Assert.Equal(location, loc);
            })
            .ReturnsAsync(expectedPayload);
        contextMock.SetupGet(c => c.HttpClient).Returns(httpClientMock.Object);
        httpClientMock
            .Setup(c => c.Post<Account>(location, expectedPayload, It.IsAny<JsonTypeInfo>(), It.IsAny<JsonTypeInfo<Account>>()))
            .ReturnsAsync(new AcmeHttpResponse<Account>(location, expectedAccount, null, null));

        var instance = new AccountContext(contextMock.Object, location);
        var account = await instance.Deactivate();

        httpClientMock.Verify(c => c.Post<Account>(location, expectedPayload, It.IsAny<JsonTypeInfo>(), It.IsAny<JsonTypeInfo<Account>>()), Times.Once);
        Assert.Equal(expectedAccount, account);
    }

    [Fact]
    public async Task CanLoadResource()
    {
        var expectedAccount = new Account();

        var expectedPayload = new JwsSigner(Helper.GetKeyV2())
            .Sign("", AcmeJsonSerializerContext.Unindented.String, null, location, "nonce");

        contextMock.Reset();
        httpClientMock.Reset();

        contextMock
            .Setup(c => c.GetDirectory())
            .ReturnsAsync(Helper.MockDirectoryV2);
        contextMock
            .SetupGet(c => c.AccountKey)
            .Returns(Helper.GetKeyV2());
        contextMock.SetupGet(c => c.HttpClient).Returns(httpClientMock.Object);
        contextMock
            .Setup(c => c.Sign(It.IsAny<object>(), location, It.IsAny<JsonTypeInfo>()))
            .Callback((object payload, Uri loc, JsonTypeInfo jsonTypeInfo) =>
            {
                Assert.Null(payload);
                Assert.Equal(location, loc);
            })
            .ReturnsAsync(expectedPayload);
        httpClientMock
            .Setup(c => c.ConsumeNonce())
            .ReturnsAsync("nonce");
        httpClientMock
            .Setup(c => c.Post<Account>(location, It.IsAny<JwsPayload>(), It.IsAny<JsonTypeInfo>(), It.IsAny<JsonTypeInfo<Account>>()))
            .Callback((Uri _, object o, JsonTypeInfo requestJsonTypeInfo, JsonTypeInfo<Account> responseJsonTypeInfo) =>
            {
                var p = (JwsPayload)o;
                Assert.Equal(expectedPayload.Payload, p.Payload);
                Assert.Equal(expectedPayload.Protected, p.Protected);
            })
            .ReturnsAsync(new AcmeHttpResponse<Account>(location, expectedAccount, null, null));

        var instance = new AccountContext(contextMock.Object, location);
        var account = await instance.Resource();

        Assert.Equal(expectedAccount, account);
    }

    [Fact]
    public async Task CanLoadOrderList()
    {
        var loc = new Uri("http://acme.d/acct/1/orders");
        var account = new Account
        {
            Orders = loc
        };
        var expectedPayload = new JwsSigner(Helper.GetKeyV2())
            .Sign(new Account(), AcmeJsonSerializerContext.Unindented.Account, null, location, "nonce");

        contextMock.Reset();
        httpClientMock.Reset();

        contextMock
            .Setup(c => c.GetDirectory())
            .ReturnsAsync(Helper.MockDirectoryV2);
        contextMock
            .SetupGet(c => c.AccountKey)
            .Returns(Helper.GetKeyV2());
        contextMock.SetupGet(c => c.HttpClient).Returns(httpClientMock.Object);
        contextMock
            .Setup(c => c.Sign(It.IsAny<object>(), location, It.IsAny<JsonTypeInfo>()))
            .ReturnsAsync(expectedPayload);
        httpClientMock
            .Setup(c => c.ConsumeNonce())
            .ReturnsAsync("nonce");
        httpClientMock
            .Setup(c => c.Post<Account>(location, It.IsAny<JwsPayload>(), It.IsAny<JsonTypeInfo>(), It.IsAny<JsonTypeInfo<Account>>()))
            .ReturnsAsync(new AcmeHttpResponse<Account>(location, account, null, null));

        var ctx = new AccountContext(contextMock.Object, location);
        var orders = await ctx.Orders();

        Assert.IsType<OrderListContext>(orders);
        Assert.Equal(loc, orders.Location);
    }
}
