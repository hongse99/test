using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";


        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //세션에서 카트를 가져온다
            Cart cart = (Cart)controllerContext.HttpContext.Session[sessionKey];
            //세션 데이터에 cart가 하나도 없으면 cart를 하나 생성한다.
            if (cart == null)
            {
                cart = new Cart();
                controllerContext.HttpContext.Session[sessionKey] = cart;
            }
            return cart;
                       
        }
    }
}