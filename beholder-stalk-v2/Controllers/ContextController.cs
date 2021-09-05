﻿namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.Models;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Concurrent;
  using System.Threading.Tasks;

  [MqttController]
  public class ContextController : IObservable<ContextEvent>
  {
    private readonly BeholderContext _context;
    private readonly ConcurrentDictionary<IObserver<ContextEvent>, ContextEventUnsubscriber> _observers = new ConcurrentDictionary<IObserver<ContextEvent>, ContextEventUnsubscriber>();
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

        OnContextEvent(new PointerPositionChangedEvent()
        {
          Source = "eye",
          OldPointerPosition = currentContextData.PsionixCurrentPointerPosition,
          NewPointerPosition = pointerPosition.Data,
        });
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

        OnContextEvent(new PointerPositionChangedEvent()
        {
          Source = "psionix",
          OldPointerPosition = currentContextData.PsionixCurrentPointerPosition,
          NewPointerPosition = pointerPosition.Data,
        });
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

    public IDisposable Subscribe(IObserver<ContextEvent> observer)
    {
      return _observers.GetOrAdd(observer, new ContextEventUnsubscriber(this, observer));
    }

    /// <summary>
    /// Produces Context Events
    /// </summary>
    /// <param name="contextEvent"></param>
    private void OnContextEvent(ContextEvent contextEvent)
    {
      Parallel.ForEach(_observers.Keys, (observer) =>
      {
        try
        {
          observer.OnNext(contextEvent);
        }
        catch (Exception)
        {
          // Do Nothing.
        }
      });
    }

    #region Nested Classes
    private sealed class ContextEventUnsubscriber : IDisposable
    {
      private readonly ContextController _parent;
      private readonly IObserver<ContextEvent> _observer;

      public ContextEventUnsubscriber(ContextController parent, IObserver<ContextEvent> observer)
      {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        _observer = observer ?? throw new ArgumentNullException(nameof(observer));
      }

      public void Dispose()
      {
        if (_observer != null && _parent._observers.ContainsKey(_observer))
        {
          _parent._observers.TryRemove(_observer, out _);
        }
      }
    }
    #endregion
  }
}