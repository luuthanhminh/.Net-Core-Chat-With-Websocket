﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Web.Common;
using Demo.Web.Hubs;
using Demo.Web.Services;
using Demo.Web.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Web.Controllers
{
    [Route("api/message")]
    public class MessageController : Controller
    {
        private readonly IIdentityService mIdentityService;
        private readonly ChatHub mChatHub;

        #region Constructor

        public MessageController(IIdentityService identityService, ChatHub chatHub)
        {
            mIdentityService = identityService;
            mChatHub = chatHub;
        }

        #endregion


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] MessageBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await this.mIdentityService.GetUser(this.User);

            Message message = new Message()
            {
                FromId = user.Id,
                ToId = model.UserId,
                Data = model.Message
            };


            if (user != null)
            {
                await mChatHub.SendMessageAsync(user.Id, message);
            }
            
            await mChatHub.SendMessageAsync(model.UserId, message);

            return Ok();
        }
    }
}
