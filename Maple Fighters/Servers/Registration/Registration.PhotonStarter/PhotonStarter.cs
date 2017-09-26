﻿using Registration.Application;
using ServerApplication.Common.PhotonStarter;
using ServerCommunicationInterfaces;

namespace Registration.PhotonStarter
{
    public class PhotonStarter : PhotonStarterBase<RegistrationApplication>
    {
        protected override RegistrationApplication CreateApplication(IFiberProvider fiberProvider)
        {
            return new RegistrationApplication(fiberProvider);
        }
    }
}