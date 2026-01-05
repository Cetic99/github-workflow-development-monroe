using FluentValidation;
using MediatR;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateVideoConfigurationCommand : IRequest<Unit>
{
    public string VideoDeviceType { get; set; }
    public string VideoCaptureDevice { get; set; }
    public string VideoDeviceIp { get; set; }
    public bool VideoStreamingServer { get; set; }
}

public class UpdateVideoConfigurationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateVideoConfigurationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateVideoConfigurationCommand command, CancellationToken cancellationToken)
    {
        var config = new VideoConfiguration();

        config.VideoDeviceType = command.VideoDeviceType;
        config.VideoCaptureDevice = command.VideoCaptureDevice;
        config.VideoStreamingServer = command.VideoStreamingServer;
        config.VideoDeviceIp = command.VideoDeviceIp;

        unitOfWork.TerminalRepository.UpdateVideoConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
public class UpdateVideoConfigurationCommandValidator : AbstractValidator<UpdateVideoConfigurationCommand>
{
    public UpdateVideoConfigurationCommandValidator()
    {
        RuleFor(x => x.VideoCaptureDevice).NotEmpty();
        RuleFor(x => x.VideoDeviceIp).NotEmpty();
        RuleFor(x => x.VideoDeviceType).NotEmpty();
        RuleFor(x => x.VideoStreamingServer).NotEmpty();
    }
}
