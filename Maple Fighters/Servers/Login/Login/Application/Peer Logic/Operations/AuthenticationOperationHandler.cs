﻿using System.Threading.Tasks;
using Authorization.Client.Common;
using Authorization.Server.Common;
using CommonCommunicationInterfaces;
using CommonTools.Coroutines;
using CommonTools.Log;
using Login.Application.Components;
using Login.Common;
using ServerApplication.Common.ApplicationBase;
using ServerCommunicationHelper;

namespace Login.Application.PeerLogic.Operations
{
    internal class AuthenticationOperationHandler : IAsyncOperationRequestHandler<AuthenticateRequestParameters, AuthenticateResponseParameters>
    {
        private readonly IDatabaseUserVerifier databaseUserVerifier;
        private readonly IDatabaseUserPasswordVerifier databaseUserPasswordVerifier;
        private readonly IDatabaseUserIdProvider databaseUserIdProvider;
        private readonly IAuthorizationServiceAPI authorizationServiceApi;

        public AuthenticationOperationHandler()
        {
            databaseUserVerifier = Server.Components.GetComponent<IDatabaseUserVerifier>().AssertNotNull();
            databaseUserPasswordVerifier = Server.Components.GetComponent<IDatabaseUserPasswordVerifier>().AssertNotNull();
            databaseUserIdProvider = Server.Components.GetComponent<IDatabaseUserIdProvider>().AssertNotNull();
            authorizationServiceApi = Server.Components.GetComponent<IAuthorizationServiceAPI>().AssertNotNull();
        }

        public Task<AuthenticateResponseParameters?> Handle(IYield yield, MessageData<AuthenticateRequestParameters> messageData, ref MessageSendOptions sendOptions)
        {
            var email = messageData.Parameters.Email;
            if (!databaseUserVerifier.IsExists(email))
            {
                var responseParameters = new AuthenticateResponseParameters?(new AuthenticateResponseParameters(LoginStatus.UserNotExist));
                return Task.FromResult(responseParameters);
            }

            var password = messageData.Parameters.Password;
            if(!databaseUserPasswordVerifier.Verify(email, password))
            {
                var responseParameters = new AuthenticateResponseParameters?(new AuthenticateResponseParameters(LoginStatus.PasswordIncorrect));
                return Task.FromResult(responseParameters);
            }

            var userId = databaseUserIdProvider.GetUserId(email);
            var parameters = new AuthorizeUserRequestParameters(userId);
            return AccessTokenProvider(yield, parameters);
        }

        private async Task<AuthenticateResponseParameters?> AccessTokenProvider(IYield yield, AuthorizeUserRequestParameters parameters)
        {
            var userAuthorization = await authorizationServiceApi.UserAuthorization(yield, parameters);
            var responseParameters = userAuthorization.Status == AuthorizationStatus.Succeed
                ? new AuthenticateResponseParameters(LoginStatus.Succeed, userAuthorization.AccessToken)
                : new AuthenticateResponseParameters(LoginStatus.NonAuthorized);
            return responseParameters;
        }
    }
}