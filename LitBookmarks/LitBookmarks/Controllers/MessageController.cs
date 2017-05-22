﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LitBookmarks.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        public ActionResult Send()
        {
            ViewBag.Nickname = User.Identity.Name;
            return View("ChatView");
        }
    }
}