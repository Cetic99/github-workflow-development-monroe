using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetVideoConfigurationQueryValidator : AbstractValidator<GetVideoConfigurationQuery>
    {
        public GetVideoConfigurationQueryValidator()
        {

        }
    }

    public record GetVideoConfigurationQuery : IRequest<VideoConfigurationDto>
    {

    }

    internal sealed class GetVideoConfigurationQueryHandler : IRequestHandler<GetVideoConfigurationQuery, VideoConfigurationDto>
    {
        private readonly ITerminalRepository _db;

        public GetVideoConfigurationQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<VideoConfigurationDto> Handle(GetVideoConfigurationQuery request, CancellationToken cancellationToken)
        {
            var dto = new VideoConfigurationDto();
            var config = await _db.GetVideoConfigurationAsync();

            dto.VideoCaptureDevice = config.VideoCaptureDevice;
            dto.VideoDeviceIp = config.VideoDeviceIp;
            dto.VideoDeviceType = config.VideoDeviceType;
            dto.VideoStreamingServer = config.VideoStreamingServer;

            return await Task.FromResult(dto);
        }
    }
}
