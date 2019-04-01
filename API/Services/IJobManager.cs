// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services
{
    public interface IJobManager
    {
        void StartAsync();

        void StopAsync();
    }
}
