using System;
using Autofac;
using DegreedAssignment.Config;
using DegreedAssignment.Services;
using Microsoft.Extensions.Options;

namespace DegreedAssignment.Modules;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(context =>
        {
            var config = context.Resolve<IOptions<ApiSettings>>().Value;
            var client = new HttpClient
            {
                BaseAddress = new Uri(config.DadJokeApiUrl)
            };
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            return client;
        }).As<HttpClient>().InstancePerLifetimeScope();

        builder.RegisterType<JokeService>().AsSelf().InstancePerLifetimeScope();
    }
}

