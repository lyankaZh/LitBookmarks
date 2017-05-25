﻿
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;

namespace LitBookmarks.Hubs
{
  [Authorize]
  public class ChatHub : Hub
  {
    public void Send(string message)
    {
      // Call the addNewMessageToPage method to update clients.
      if (message != string.Empty)
      {
        Clients.All.addNewMessageToPage(Context.User.Identity.Name, message);
      }
    }

  }
}