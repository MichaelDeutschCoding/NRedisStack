using Xunit;
using NRedisStack.Core;

namespace NRedisStack.Tests.Core;

public class CoreTests : AbstractNRedisStackTest, IDisposable
{
    private readonly string key = "CORE_TESTS";
    public CoreTests(RedisFixture redisFixture) : base(redisFixture) { }

    public void Dispose()
    {
        redisFixture.Redis.GetDatabase().KeyDelete(key);
    }

    [SkipIfRedisVersion(Comparison.LessThan, "7.1.242")]
    public void TestSetInfo()
    {
        var redis = redisFixture.Redis;

        var db = redis.GetDatabase();
        db.Execute("FLUSHALL");
        var info = db.Execute("CLIENT", "INFO").ToString();
        Assert.EndsWith("lib-name=SE.Redis lib-ver=2.6.122.38350\n", info);

        Assert.True(db.ClientSetInfo(SetInfoAttr.LibraryName, "nredisstack"));
        Assert.True(db.ClientSetInfo(SetInfoAttr.LibraryVersion, "0.8.1"));
        info = db.Execute("CLIENT", "INFO").ToString();
        Assert.EndsWith("lib-name=nredisstack lib-ver=0.8.1\n", info);
    }

    [SkipIfRedisVersion(Comparison.LessThan, "7.1.242")]
    public async Task TestSetInfoç()
    {
        var redis = redisFixture.Redis;

        var db = redis.GetDatabase();
        db.Execute("FLUSHALL");
        var info = (await db.ExecuteAsync("CLIENT", "INFO")).ToString();
        Assert.EndsWith("lib-name=SE.Redis lib-ver=2.6.122.38350\n", info);

        Assert.True( await db.ClientSetInfoAsync(SetInfoAttr.LibraryName, "nredisstack"));
        Assert.True( await db.ClientSetInfoAsync(SetInfoAttr.LibraryVersion, "0.8.1"));
        info = (await db.ExecuteAsync("CLIENT", "INFO")).ToString();
        Assert.EndsWith("lib-name=nredisstack lib-ver=0.8.1\n", info);
    }
}