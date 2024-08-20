namespace Certes.Acme;

using System;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;


/// <summary>
/// Represents the context of ACME entity.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
internal class EntityContext<T>
{
    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>
    /// The context.
    /// </value>
    protected IAcmeContext Context { get; }

    /// <summary>
    /// Gets the entity location.
    /// </summary>
    /// <value>
    /// The entity location.
    /// </value>
    public Uri Location { get; }

    /// <summary>
    /// The timespan after which to retry the request
    /// </summary>
    public int RetryAfter { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public JsonTypeInfo<T> JsonTypeInfo { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="EntityContext{T}"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="location">The location.</param>
    /// <param name="jsonTypeInfo"></param>
    public EntityContext(
        IAcmeContext context,
        Uri location,
        JsonTypeInfo<T> jsonTypeInfo)
    {
        Context = context;
        Location = location;
        this.JsonTypeInfo = jsonTypeInfo;
    }

    /// <summary>
    /// Gets the resource entity data.
    /// </summary>
    /// <returns>The resource entity data.</returns>
    public virtual async Task<T> Resource()
    {
        var resp = await Context.HttpClient.Post<T>(Context, Location, null, true, AcmeJsonSerializerContext.Unindented.Uri, this.JsonTypeInfo);
        return resp.Resource;
    }
}
