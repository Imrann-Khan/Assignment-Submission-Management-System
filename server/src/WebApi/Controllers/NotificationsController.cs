using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;
using Application.Notifications.List;
using Application.Notifications.MarkAllAsRead;
using Application.Notifications.MarkAsRead;
using Application.Notifications.UnreadCount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/notifications")]
[Authorize]
public class NotificationsController : ApiControllerBase
{
    public NotificationsController(ISender sender) : base(sender){}

    [HttpGet]
    public async Task<ActionResult<PagedResult<NotificationDto>>> GetAll([FromQuery] GetMyNotificationQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetUnreadNotificationCountQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new MarkNotificationAsReadCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        await Sender.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken);
        return NoContent();
    }

}