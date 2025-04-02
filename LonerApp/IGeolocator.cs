using System;

namespace LonerApp;

public interface IGeolocator
{
    Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken);
}
