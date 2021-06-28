﻿namespace NServiceBus.Core.Tests.ServicePlatform.Retries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NServiceBus.Pipeline;
    using NServiceBus.Routing;
    using NUnit.Framework;
    using Testing;

    [TestFixture]
    public class ManualRetryNotificationBehaviorTests
    {
        [Test]
        public async Task Should_confirm_successful_retries_to_error_queue()
        {
            const string errorQueue = "configuredErrorQueue";
            var routingPipeline = new RoutingPipeline();
            var behavior = new ManualRetryNotificationBehavior(errorQueue);

            var context = SetupTestableContext(routingPipeline);
            // Set necessary SC headers
            context.Message.Headers["ServiceControl.Retry.UniqueMessageId"] = Guid.NewGuid().ToString("N");
            context.Message.Headers["ServiceControl.Version"] = "42";

            await behavior.Invoke(context, _ => Task.CompletedTask);

            var outgoingMessage = routingPipeline.ForkInvocations.Single();
            Assert.AreEqual(
                context.Message.Headers["ServiceControl.Retry.UniqueMessageId"],
                outgoingMessage.Message.Headers["ServiceControl.Retry.UniqueMessageId"]);

            Assert.IsTrue(outgoingMessage.Message.Headers.ContainsKey("ServiceControl.Retry.Successful"));

            Assert.AreEqual(0, outgoingMessage.Message.Body.Length);

            Assert.AreEqual(bool.TrueString, outgoingMessage.Message.Headers[Headers.ControlMessageHeader]);

            var addressTag = outgoingMessage.RoutingStrategies.Single().Apply(new Dictionary<string, string>()) as UnicastAddressTag;
            Assert.AreEqual(addressTag.Destination, errorQueue);
        }

        [Test]
        public void Should_not_confirm_when_processing_fails()
        {
            const string errorQueue = "configuredErrorQueue";
            var routingPipeline = new RoutingPipeline();
            var behavior = new ManualRetryNotificationBehavior(errorQueue);

            var context = SetupTestableContext(routingPipeline);
            // Set necessary SC headers
            context.Message.Headers["ServiceControl.Retry.UniqueMessageId"] = Guid.NewGuid().ToString("N");
            context.Message.Headers["ServiceControl.Version"] = "42";

            var exception = new Exception("some pipeline failure");
            var thrownException = Assert.ThrowsAsync<Exception>(async () => await behavior.Invoke(context, _ => Task.FromException(exception)));

            Assert.AreSame(thrownException, exception);
            Assert.AreEqual(0, routingPipeline.ForkInvocations.Count);
        }

        [Test]
        // A missing SC version header indicates an older version of SC that cannot handle the confirmation message yet
        public async Task Should_not_confirm_when_message_does_not_contain_SC_version_header()
        {
            const string errorQueue = "configuredErrorQueue";
            var routingPipeline = new RoutingPipeline();
            var behavior = new ManualRetryNotificationBehavior(errorQueue);

            var context = SetupTestableContext(routingPipeline);
            context.Message.Headers["ServiceControl.Retry.UniqueMessageId"] = Guid.NewGuid().ToString("N");

            await behavior.Invoke(context, _ => Task.CompletedTask);

            Assert.AreEqual(0, routingPipeline.ForkInvocations.Count);
        }

        [Test]
        public async Task Should_not_confirm_when_message_does_not_retry_header()
        {
            const string errorQueue = "configuredErrorQueue";
            var routingPipeline = new RoutingPipeline();
            var behavior = new ManualRetryNotificationBehavior(errorQueue);

            var context = SetupTestableContext(routingPipeline);
            context.Message.Headers["ServiceControl.Version"] = "42";

            await behavior.Invoke(context, _ => Task.CompletedTask);

            Assert.AreEqual(0, routingPipeline.ForkInvocations.Count);
        }

        static TestableTransportReceiveContext SetupTestableContext(RoutingPipeline routingPipeline)
        {
            var context = new TestableTransportReceiveContext();

            //setup fork pipeline
            var serviceCollection = new ServiceCollection();
            var pipelineModifications = new PipelineModifications();
            pipelineModifications.Additions.Add(
                RegisterStep.Create("routingFork", typeof(RoutingPipeline), "for testing", _ => routingPipeline));
            var pipelineCache = new PipelineCache(serviceCollection.BuildServiceProvider(), pipelineModifications);
            context.Extensions.Set<IPipelineCache>(pipelineCache);

            return context;
        }

        class RoutingPipeline : Behavior<IRoutingContext>
        {
            public List<IRoutingContext> ForkInvocations { get; } = new List<IRoutingContext>();

            public override Task Invoke(IRoutingContext context, Func<Task> next)
            {
                ForkInvocations.Add(context);
                return Task.CompletedTask;
            }
        }

        //TODO also for control messages
    }
}