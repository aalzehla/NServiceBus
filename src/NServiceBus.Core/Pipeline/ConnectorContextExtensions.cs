namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using Persistence;
    using Pipeline;
    using Routing;
    using Transport;

    /// <summary>
    /// Contains extensions methods to map behavior contexts.
    /// </summary>
    public static partial class ConnectorContextExtensions
    {
        /// <summary>
        /// Creates a <see cref="IRoutingContext" /> based on the current context.
        /// </summary>
        public static IRoutingContext CreateRoutingContext(this ForkConnector<ITransportReceiveContext, IRoutingContext> forkConnector, OutgoingMessage outgoingMessage, string localAddress, ITransportReceiveContext sourceContext)
        {
            Guard.AgainstNull(nameof(outgoingMessage), outgoingMessage);
            Guard.AgainstNullAndEmpty(nameof(localAddress), localAddress);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new RoutingContext(outgoingMessage, new UnicastRoutingStrategy(localAddress), sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IRoutingContext" /> based on the current context.
        /// </summary>
        public static IRoutingContext CreateRoutingContext(this StageConnector<IForwardingContext, IRoutingContext> stageConnector, OutgoingMessage outgoingMessage, RoutingStrategy routingStrategy, IForwardingContext sourceContext)
        {
            Guard.AgainstNull(nameof(outgoingMessage), outgoingMessage);
            Guard.AgainstNull(nameof(routingStrategy), routingStrategy);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new RoutingContext(outgoingMessage, routingStrategy, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IRoutingContext" /> based on the current context.
        /// </summary>
        public static IRoutingContext CreateRoutingContext(this StageConnector<IAuditContext, IRoutingContext> stageConnector, OutgoingMessage outgoingMessage, RoutingStrategy routingStrategy, IAuditContext sourceContext)
        {
            Guard.AgainstNull(nameof(outgoingMessage), outgoingMessage);
            Guard.AgainstNull(nameof(routingStrategy), routingStrategy);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new RoutingContext(outgoingMessage, routingStrategy, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IRoutingContext" /> based on the current context.
        /// </summary>
        public static IRoutingContext CreateRoutingContext(this StageConnector<IOutgoingPhysicalMessageContext, IRoutingContext> stageConnector, OutgoingMessage outgoingMessage, IReadOnlyCollection<RoutingStrategy> routingStrategies, IOutgoingPhysicalMessageContext sourceContext)
        {
            Guard.AgainstNull(nameof(outgoingMessage), outgoingMessage);
            Guard.AgainstNull(nameof(routingStrategies), routingStrategies);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new RoutingContext(outgoingMessage, routingStrategies, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IIncomingPhysicalMessageContext" /> based on the current context.
        /// </summary>
        public static IIncomingPhysicalMessageContext CreateIncomingPhysicalMessageContext(this StageForkConnector<ITransportReceiveContext, IIncomingPhysicalMessageContext, IBatchDispatchContext> stageForkConnector, IncomingMessage incomingMessage, ITransportReceiveContext sourceContext)
        {
            Guard.AgainstNull(nameof(incomingMessage), incomingMessage);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new IncomingPhysicalMessageContext(incomingMessage, sourceContext);
        }

        internal static IIncomingPhysicalMessageContext CreateIncomingPhysicalMessageContext(this IStageForkConnector<ITransportReceiveContext, IIncomingPhysicalMessageContext, IBatchDispatchContext> stageForkConnector, IncomingMessage incomingMessage, ITransportReceiveContext sourceContext)
        {
            _ = stageForkConnector;

            return new IncomingPhysicalMessageContext(incomingMessage, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IIncomingPhysicalMessageContext" /> based on the current context.
        /// </summary>
        public static IIncomingPhysicalMessageContext CreateIncomingPhysicalMessageContext(this StageConnector<ITransportReceiveContext, IIncomingPhysicalMessageContext> stageConnector, IncomingMessage incomingMessage, ITransportReceiveContext sourceContext)
        {
            Guard.AgainstNull(nameof(incomingMessage), incomingMessage);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new IncomingPhysicalMessageContext(incomingMessage, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IIncomingLogicalMessageContext" /> based on the current context.
        /// </summary>
        public static IIncomingLogicalMessageContext CreateIncomingLogicalMessageContext(this StageConnector<IIncomingPhysicalMessageContext, IIncomingLogicalMessageContext> stageConnector, LogicalMessage logicalMessage, IIncomingPhysicalMessageContext sourceContext)
        {
            Guard.AgainstNull(nameof(logicalMessage), logicalMessage);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new IncomingLogicalMessageContext(logicalMessage, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IInvokeHandlerContext" /> based on the current context.
        /// </summary>
        public static IInvokeHandlerContext CreateInvokeHandlerContext(this StageConnector<IIncomingLogicalMessageContext, IInvokeHandlerContext> stageConnector, MessageHandler messageHandler, ICompletableSynchronizedStorageSession storageSession, IIncomingLogicalMessageContext sourceContext)
        {
            Guard.AgainstNull(nameof(messageHandler), messageHandler);
            Guard.AgainstNull(nameof(storageSession), storageSession);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new InvokeHandlerContext(messageHandler, storageSession, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IBatchDispatchContext" /> based on the current context.
        /// </summary>
        public static IBatchDispatchContext CreateBatchDispatchContext(this StageForkConnector<ITransportReceiveContext, IIncomingPhysicalMessageContext, IBatchDispatchContext> stageForkConnector, IReadOnlyCollection<TransportOperation> transportOperations, IIncomingPhysicalMessageContext sourceContext)
        {
            Guard.AgainstNull(nameof(transportOperations), transportOperations);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new BatchDispatchContext(transportOperations, sourceContext);
        }

        internal static IBatchDispatchContext CreateBatchDispatchContext(this IStageForkConnector<ITransportReceiveContext, IIncomingPhysicalMessageContext, IBatchDispatchContext> stageForkConnector, IReadOnlyCollection<TransportOperation> transportOperations, IIncomingPhysicalMessageContext sourceContext)
        {
            _ = stageForkConnector;

            return new BatchDispatchContext(transportOperations, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IDispatchContext" /> based on the current context.
        /// </summary>
        public static IDispatchContext CreateDispatchContext(this StageConnector<IBatchDispatchContext, IDispatchContext> stageConnector, IReadOnlyCollection<TransportOperation> transportOperations, IBatchDispatchContext sourceContext)
        {
            Guard.AgainstNull(nameof(transportOperations), transportOperations);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new DispatchContext(transportOperations, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IDispatchContext" /> based on the current context.
        /// </summary>
        public static IDispatchContext CreateDispatchContext(this StageConnector<IRoutingContext, IDispatchContext> stageConnector, IReadOnlyCollection<TransportOperation> transportOperations, IRoutingContext sourceContext)
        {
            Guard.AgainstNull(nameof(transportOperations), transportOperations);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new DispatchContext(transportOperations, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IOutgoingLogicalMessageContext" /> based on the current context.
        /// </summary>
        public static IOutgoingLogicalMessageContext CreateOutgoingLogicalMessageContext(this StageConnector<IOutgoingPublishContext, IOutgoingLogicalMessageContext> stageConnector, OutgoingLogicalMessage outgoingMessage, IReadOnlyCollection<RoutingStrategy> routingStrategies, IOutgoingPublishContext sourceContext)
        {
            Guard.AgainstNull(nameof(outgoingMessage), outgoingMessage);
            Guard.AgainstNull(nameof(routingStrategies), routingStrategies);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new OutgoingLogicalMessageContext(
                sourceContext.MessageId,
                sourceContext.Headers,
                outgoingMessage,
                routingStrategies,
                sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IOutgoingLogicalMessageContext" /> based on the current context.
        /// </summary>
        public static IOutgoingLogicalMessageContext CreateOutgoingLogicalMessageContext(this StageConnector<IOutgoingReplyContext, IOutgoingLogicalMessageContext> stageConnector, OutgoingLogicalMessage outgoingMessage, IReadOnlyCollection<RoutingStrategy> routingStrategies, IOutgoingReplyContext sourceContext)
        {
            Guard.AgainstNull(nameof(outgoingMessage), outgoingMessage);
            Guard.AgainstNull(nameof(routingStrategies), routingStrategies);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new OutgoingLogicalMessageContext(
                sourceContext.MessageId,
                sourceContext.Headers,
                outgoingMessage,
                routingStrategies,
                sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IOutgoingLogicalMessageContext" /> based on the current context.
        /// </summary>
        public static IOutgoingLogicalMessageContext CreateOutgoingLogicalMessageContext(this StageConnector<IOutgoingSendContext, IOutgoingLogicalMessageContext> stageConnector, OutgoingLogicalMessage outgoingMessage, IReadOnlyCollection<RoutingStrategy> routingStrategies, IOutgoingSendContext sourceContext)
        {
            Guard.AgainstNull(nameof(outgoingMessage), outgoingMessage);
            Guard.AgainstNull(nameof(routingStrategies), routingStrategies);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new OutgoingLogicalMessageContext(
                sourceContext.MessageId,
                sourceContext.Headers,
                outgoingMessage,
                routingStrategies,
                sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IOutgoingPhysicalMessageContext" /> based on the current context.
        /// </summary>
        public static IOutgoingPhysicalMessageContext CreateOutgoingPhysicalMessageContext(this StageConnector<IOutgoingLogicalMessageContext, IOutgoingPhysicalMessageContext> stageConnector, ReadOnlyMemory<byte> messageBody, IReadOnlyCollection<RoutingStrategy> routingStrategies, IOutgoingLogicalMessageContext sourceContext)
        {
            Guard.AgainstNull(nameof(messageBody), messageBody);
            Guard.AgainstNull(nameof(routingStrategies), routingStrategies);
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            return new OutgoingPhysicalMessageContext(messageBody, routingStrategies, sourceContext);
        }

        /// <summary>
        /// Creates a <see cref="IAuditContext" /> based on the current context.
        /// </summary>
        public static IAuditContext CreateAuditContext(this ForkConnector<IIncomingPhysicalMessageContext, IAuditContext> forkConnector, OutgoingMessage message, string auditAddress, IIncomingPhysicalMessageContext sourceContext)
        {
            Guard.AgainstNull(nameof(sourceContext), sourceContext);

            var connector = (IForkConnector<IIncomingPhysicalMessageContext, IIncomingPhysicalMessageContext, IAuditContext>)forkConnector;
            return connector.CreateAuditContext(message, auditAddress, sourceContext);
        }

        internal static IAuditContext CreateAuditContext(this IForkConnector<IIncomingPhysicalMessageContext, IIncomingPhysicalMessageContext, IAuditContext> forkConnector, OutgoingMessage message, string auditAddress, IIncomingPhysicalMessageContext sourceContext)
        {
            _ = forkConnector;

            return new AuditContext(message, auditAddress, sourceContext);
        }
    }
}