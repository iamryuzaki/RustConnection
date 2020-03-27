﻿namespace RustConnection.Base
{
    public interface IWorker
    {
        void Awake();
        void Update(float deltaTime);
    }
}