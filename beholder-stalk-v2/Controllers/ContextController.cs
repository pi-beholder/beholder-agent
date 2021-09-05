﻿namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.Models;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading.Tasks;

  [MqttController]
  public class ContextController
  {
    private readonly BeholderContext _context;
    private readonly ILogger<ContextController> _logger;

    public ContextController(BeholderContext context, ILogger<ContextController> logger)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [EventPattern("beholder/eye/+/pointer_position")]
    public Task UpdatePointerPositionFromEye(ICloudEvent<Point> pointerPosition)
    {
      var currentContextData = _context.Data;

      if (pointerPosition.Data != null &&
          (currentContextData.LastEyePointerUpdate.HasValue == false ||
            currentContextData.LastEyePointerUpdate.Value < pointerPosition.Time
          )
         )
      {
        _context.Data = _context.Data with
        {
          LastEyePointerUpdate = pointerPosition.Time,
          EyeCurrentPointerPosition = pointerPosition.Data
        };
      }
      return Task.CompletedTask;
    }

    [EventPattern("beholder/psionix/+/pointer_position")]
    public Task UpdatePointerPositionFromPsionix(ICloudEvent<Point> pointerPosition)
    {
      var currentContextData = _context.Data;

      if (pointerPosition.Data != null &&
          (currentContextData.LastPsionixPointerUpdate.HasValue == false ||
            currentContextData.LastPsionixPointerUpdate.Value < pointerPosition.Time
          )
         )
      {
        _context.Data = _context.Data with
        {
          LastPsionixPointerUpdate = pointerPosition.Time,
          PsionixCurrentPointerPosition = pointerPosition.Data
        };
        _logger.LogInformation($"Recieved Updated Psionix Pointer Position {pointerPosition.Data} at {pointerPosition.Time}");
      }
      return Task.CompletedTask;
    }

    [EventPattern("beholder/psionix/+/system_information")]
    public Task UpdateSystemInformationFromPsionix(ICloudEvent<SysInfo> sysInfo)
    {
      if (sysInfo.Data != null)
      {
        _context.Data = _context.Data with
        {
          SysInfo = sysInfo.Data
        };
      }
      return Task.CompletedTask;
    }
  }
}