using System.IO.Pipelines;
using Microsoft.Extensions.DependencyInjection;


namespace Application.Common.Messaging;

public class Dispatcher : ISender
{
    private readonly IServiceProvider _provider;

    public Dispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));

        dynamic handler = _provider.GetRequiredService(handlerType);
        var behaviors = _provider.GetServices(behaviorType).Cast<dynamic>().Reverse().ToList();

        RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle((dynamic)request, cancellationToken);

        foreach(var behavior in behaviors)
        {
            var next = pipeline;
            pipeline = () => behavior.Handle((dynamic)request, next, cancellationToken);
        }

        return await pipeline();
    }
}